# ADR-0001 â€” CoreWCF as the SOAP 1.2 server

**Date:** 2026-10-15 Â· **Status:** Accepted

## Context

The bridge must expose a SOAP 1.2 (document/literal, WSDL) endpoint that legacy ERP clients can consume on .NET 10. Options: **CoreWCF** (WCF's successor, official Microsoft sponsorship), **SoapCore** (community, ASP.NET Core-native), hand-rolled XML endpoints.

## Decision

Use **CoreWCF.Http 1.9.1** with a `CustomBinding` (`TextMessageEncodingBindingElement` with `MessageVersion.Soap12` + `HttpTransportBindingElement`), hosted through `app.UseServiceModel(...)`, WSDL published via `ServiceMetadataBehavior.HttpGetEnabled` at `GET /nfe?wsdl`.

## Consequences

- The WCF programming model (`[ServiceContract]`, `[OperationContract]`, `FaultException`, `ChannelFactory` clients) carries over from .NET Framework â€” the code reads like the code legacy teams expect (CoreWCF attributes live in the `CoreWCF` namespace; data contracts keep `System.Runtime.Serialization`).
- SOAP 1.2 + WSDL come from a battle-tested stack instead of hand-rolled XML (YAGNI for a custom serializer).
- SoapCore was rejected: smaller ecosystem, weaker WSDL fidelity, and interview-wise CoreWCF is the signal that maps to real .NET shops.
- Trade-off: CoreWCF's DI/behavior APIs are less documented than WCF's â€” mitigated by following the official `CoreWCF/samples` recipes verbatim.
