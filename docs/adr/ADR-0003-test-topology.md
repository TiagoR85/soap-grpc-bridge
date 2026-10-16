# ADR-0003 — Test topology: WebApplicationFactory + Kestrel + WireMock.Net HTTP/2

**Date:** 2026-10-15 · **Status:** Accepted

## Context

The spec requires a *real* `System.ServiceModel.ChannelFactory` client talking to the *real* host endpoint (acceptance criterion 4), plus a gRPC backend stub — while staying fully offline and in a single test process.

## Decision

- **Host**: `WebApplicationFactory<Program>` with `WithWebHostBuilder(builder => builder.UseUrls($"http://127.0.0.1:{SoapGrpcBridge.Tests.Infrastructure.FreePort.Get()}"))` + `factory.UseKestrel()` — the host runs on a real Kestrel listener on a dynamic port. `CreateClient()` starts the host and yields the actual base address. Config (`Sefaz:GrpcUrl`, `Sefaz:DeadlineSeconds`) is injected via `ConfigureAppConfiguration` + in-memory collection.
- **SOAP client**: a mirror `System.ServiceModel` contract (`SoapClientContract.cs`) with identical `Namespace`/`Name`/`Action`/`DataMember` names, called through `ChannelFactory<INfeAuthorizationChannel>` over a SOAP 1.2 `CustomBinding`, with `Abort()` cleanup in `finally`.
- **gRPC backend**: WireMock.Net 2.19.0 with `UseHttp2 = true` on a free port; stub mappings build the gRPC wire frame manually (`GrpcFrame`: 5-byte header + protobuf payload), served with `Content-Type: application/grpc` + trailing `grpc-status: 0`; timeouts via `WithDelay(3000)`; "backend down" is a `DeadBackend` — a `TcpListener` that accepts and immediately disposes connections (→ gRPC `Unavailable`).
- Port allocation uses a bind-and-release `TcpListener` trick (`FreePort`); each test class owns its own stub fixture (`IClassFixture`) and resets mappings per test.

## Consequences

- One process, no Docker, no network — the whole 4+2 matrix runs in seconds and behaves identically locally and on the ubuntu CI runner.
- The mirror client contract doubles as documentation of the wire contract an ERP would implement.
- Rejected alternatives: plain `TestServer` (no sockets → `ChannelFactory` cannot connect), spawned self-host process (slow, lifecycle flakiness), in-process gRPC server instead of WireMock (spec chose WireMock for stub fidelity and reuse across the portfolio).