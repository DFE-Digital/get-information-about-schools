# External Lookup Links

This document describes how the `GIAS Web Front End` builds and shows links to external information services.

## Overview

The `ExternalLookupService` is a front-end helper layer for outbound links to Find School and College Performance Data (FSCPD), Financial Benchmarking and Ofsted. It centralises URL generation, existence checks and caching so that establishment and group detail pages can show relevant external links without each page having to know the integration details of those services.

The web front end has a small integration layer whose job is to:

- Build public URLs for external services
- Check whether those external pages or records exist
- Let detail pages show or hide those links appropriately

This is implemented through ExternalLookupService.cs.

The main external services covered are:

- FSCPD
- Financial Benchmarking
- Ofsted report pages

This subcomponent is about outbound reference links. It does not import or persist core GIAS data from those systems.

## Abstraction Layer

The front-end abstraction is IExternalLookupService.cs.

It exposes two kinds of behaviour for each external system:

- Build the public URL
- Check whether the destination exists

The interface includes:

- `FscpdURL(...)`
- `FscpdCheckExists(...)`
- `SfbURL(...)`
- `SfbCheckExists(...)`
- `OfstedReportUrl(...)`
- `OfstedReportPageCheckExists(...)`

The concrete implementation delegates to service-specific adapters.

## FSCPD Links

FSCPD integration is implemented in FSCPService.cs.

Its responsibilities are:

- Build the public FSCPD URL for a school or MAT
- Check whether the corresponding page exists
- Cache the existence result in memory

The service:

- Uses configured base URL settings
- Generates a URL-safe school or trust name segment
- Sends an HTTP `HEAD` request to test page existence
- Caches the result in `MemoryCache`

It distinguishes between:

- School pages
- Multi-academy trust pages

## Financial Benchmarking Links

Financial Benchmarking integration is implemented in FBService.cs.

It supports three lookup types:

- `School`
- `Federation`
- `Trust`

The service builds:

- The public URL used in the browser
- The API URL used to check whether the target exists

The check is done by calling a Financial Benchmarking status endpoint and testing for HTTP `200 OK`. The result is cached in memory.

The lookup key depends on the entity type:

- School uses URN
- Federation uses group UID
- Trust uses Companies House number

## Ofsted Links

Ofsted integration is implemented in OfstedService.cs.

Its responsibilities are:

- Build the public Ofsted report URL from the configured base URL and URN
- Optionally check whether the report page exists
- Cache the existence result in memory

The service performs existence checking with an HTTP `HEAD` request.

## How The Front End Uses These Links

The main consumers are the establishment and group detail view models.

### Establishment Detail

`EstablishmentDetailViewModel.cs` uses the external lookup service for:

- FSCPD links
- Financial Benchmarking links
- Ofsted report links

The detail model exposes:

- `FscpdURL`
- `FinancialBenchmarkingURL`
- `OfstedReportUrl`

It also contains logic to determine whether these links should be shown.

For FSCPD:

- Only certain establishment types are eligible
- The model can call `SetFscpdAsync()` to perform an existence check

For Financial Benchmarking:

- The model can call `SetShowFinancialBenchmarkingAsync()` to perform an existence check

For Ofsted:

- The detail flow currently uses establishment type rules to decide whether an Ofsted link is relevant
- The controller sets `ShowOfstedReportLink` and `OfstedReportUrl` directly in `EstablishmentController.cs`

So although the external lookup layer supports an Ofsted existence check, the current establishment detail path mainly uses:

- Type-based eligibility
- Direct URL generation

### Group Detail

`GroupDetailViewModel.cs` uses the external lookup service for:

- FSCPD links
- Financial Benchmarking links

For groups:

- FSCPD is mainly relevant to MATs
- Financial Benchmarking uses different lookup IDs depending on group type

The group detail model works out whether Financial Benchmarking should use:

- Group UID for federations
- Companies House number for MATs and SATs

## Configuration

The external lookup services are wired in IocConfig.cs.

Relevant configuration includes:

- `FscpdURL`
- `FscpdUsername`
- `FinancialBenchmarkingApiURL`
- `FinancialBenchmarkingURL`
- `FinancialBenchmarkingUsername`
- `FinancialBenchmarkingPassword`
- `OfstedService_BaseAddress`
- Retry and cache settings for each service

`ExternalLookupService` itself is registered as a singleton and auto-activated in the container.

## Existence Checking and Caching

A common pattern across these services is:

1. Build the external URL or status endpoint URL.
2. Check whether the external resource exists.
3. Cache the result in `MemoryCache`.
4. Reuse the cached status for subsequent page requests.

This reduces repeated external calls and avoids showing links that are unlikely to work.

The checks also use retry policies configured through `PollyUtil`.

## What This Subcomponent Owns

The external lookup links subcomponent is responsible for:

- Generating outbound external URLs
- Testing whether external resources appear to exist
- Caching those availability checks
- Giving detail pages a single front-end abstraction for external links

It is not responsible for:

- Importing external source data into GIAS
- Persisting external data locally
- Performing core search or data matching




