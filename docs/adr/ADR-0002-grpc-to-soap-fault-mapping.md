# ADR-0002 — gRPC status → SOAP Fault mapping with stable codes

**Date:** 2026-10-15 · **Status:** Accepted

## Context

The backend speaks gRPC; the ERP consumes SOAP faults. Operators and tests need deterministic identifiers to branch on, and the real SEFAZ sends business rejections (`cStat`≠100) as *normal responses*, not faults.

## Decision

All gRPC failures funnel through one class, `FaultTranslator`, and one factory, `SefazFault`:

| Condition | SOAP Fault (`Code.SubCode`) | Level |
|---|---|---|
| `RpcException` `Unavailable` (dead port) | `SEFAZ:BackendUnavailable` | Server |
| `RpcException` `DeadlineExceeded` (deadline config) | `SEFAZ:Timeout` | Server |
| Any other `RpcException` status | `SEFAZ:BackendUnavailable` | Server |
| Backend reply missing required fields (`cStat 100` without `nRec`/`nProt`) | `SEFAZ:IntegrityError` | Server |
| Bridge validation failure (empty lote, invalid `cUF`) | `SEFAZ:InvalidRequest` | Client |

The fault **reason string always contains the code** (`"SEFAZ:..."`), so tests assert a substring and operators can grep logs. Business rejections are never faults.

## Consequences

- Tests and runbooks depend only on the `SEFAZ:*` strings — transport/message details can change freely.
- One catch-all for "other gRPC statuses" keeps the contract closed (YAGNI: no `SEFAZ:Cancelled` etc. until a real need appears).
- Known deviation: CoreWCF returns HTTP 500 for every SOAP fault (a `Client` fault would ideally be 400); the stable fault code carries the client/server distinction instead.