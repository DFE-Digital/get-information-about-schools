# UK Register of Learning Providers (UKRLP) Integration

## Overview

This system integrates with UKRLP to retrieve UK Provider Reference Number (UKPRN) mappings for:

- Establishments
- Establishment groups

The integration is implemented as a SOAP client and is used to pull provider records from UKRLP

It links:

- Estblishments URNs to UKPRNs
- Companies House numbers to UKPRNs


## Main Classes

### SOAP synchronization service

- `UkrlpSinchronizationService`

This is the main integration class. It:

- Connects to the UKRLP SOAP service
- No application level authentication
- Requests provider records updated since the last sync point
- Filters records by verification authority
- Builds internal maps for establishments and establishment groups

### Base SOAP client support

- `BaseWebServiceSynchronizationService`

This provides the shared SOAP client setup used by the synchronization service.

### Sync tracking

- `UkprnUpdateProcessDao`
- `UkprnUpdateProcess`

These are used to determine the last sync point for each sync type.

## Upstream Service

The integration uses the UKRLP SOAP service defined by:

- `${ukrlp.ws.wsdl.address}`

and forces the runtime endpoint to:

- `${ukrlp.ws.endpoint}`

with a default of:

- `https://webservices.ukrlp.co.uk/UkrlpProviderQueryWS6/ProviderQueryServiceV6`

This is configured and used in `UkrlpSinchronizationService`

## What the Service Pulls

The service supports two main pull modes:

- `pullEstablishmentMap()`
  - returns `Map<Long, Integer>`
  - maps `URN -> UKPRN`

- `pullEstablishmentGroupMap()`
  - returns `Map<String, Integer>`
  - maps `CompaniesHouseNumber -> UKPRN`

Both methods call a shared `pullAll(...)` method and then transform the provider records returned by UKRLP.

## How the Filtering Works

The service filters provider records by verification authority:

- Establishments use `DfE (Schools Unique Reference Number)`
- Establishment groups use `Companies House`

It also filters by:

- Active provider status: `A`
- Stakeholder id from `${ukrlp.ws.wsdl.stakeholder}`
- Updated-since date from the last recorded sync

Integration is incremental, does not peform a full historical pull.

## Sync Window Logic

For each sync type:

- `establishment`
- `establishmentGroup`

the service checks the most recent `UkprnUpdateProcess` record.

If one exists:

- It uses that record's `sinceDate` as `providerUpdatedSince`

If one does not exist:

- It uses a very early default date so the query can return the full applicable dataset

The query id is also derived from the last process record.

## Authentication

Implementation `UkrlpSinchronizationService`, does not have any explicit username/password or token handling.

The service:

- Creates the SOAP port from the WSDL
- Overrides the endpoint URL
- Submits the `retrieveAllProviders` request


## Sequence Diagram

```mermaid
sequenceDiagram
    autonumber
    participant Quartz as Quartz Scheduler
    participant Job as UkprnUpdateJob
    participant Mgr as UkprnUpdateManagerImpl
    participant EstMgr as EstablishmentManagerImpl
    participant GroupMgr as EstablishmentGroupManagerImpl
    participant UKRLP as UkrlpSinchronizationService
    participant ProcDao as UkprnUpdateProcessDao
    participant DB as GIAS Database
    participant SOAP as UKRLP SOAP Service

    Quartz->>Job: Trigger scheduled job
    Job->>Mgr: updateUkprns()

    alt Establishment sync
        Mgr->>EstMgr: ukprnUpdate()
        EstMgr->>UKRLP: pullEstablishmentMap()
        UKRLP->>UKRLP: Set type = establishment
    else Establishment group sync
        Mgr->>GroupMgr: ukprnUpdate()
        GroupMgr->>UKRLP: pullEstablishmentGroupMap()
        UKRLP->>UKRLP: Set type = establishmentGroup
    end

    UKRLP->>ProcDao: getLastByType(type)
    ProcDao->>DB: Load last UKPRN sync record
    DB-->>ProcDao: Last sync record or null
    ProcDao-->>UKRLP: Last sync record or null

    UKRLP->>UKRLP: Build ProviderQueryStructure
    UKRLP->>UKRLP: Set stakeholderId, active status, providerUpdatedSince
    UKRLP->>UKRLP: Override endpoint to HTTPS UKRLP service

    UKRLP->>SOAP: retrieveAllProviders(query)
    SOAP-->>UKRLP: ProviderQueryResponse

    UKRLP->>UKRLP: Filter records by verification authority

    alt Establishment sync
        UKRLP->>UKRLP: Extract URN and UKPRN
        UKRLP-->>EstMgr: Map<URN, UKPRN>
        EstMgr->>DB: Update establishment UKPRNs and audit changes
        DB-->>EstMgr: Saved
    else Establishment group sync
        UKRLP->>UKRLP: Extract CompaniesHouseNumber and UKPRN
        UKRLP-->>GroupMgr: Map<CompaniesHouseNumber, UKPRN>
        GroupMgr->>DB: Update group UKPRNs and audit changes
        DB-->>GroupMgr: Saved
    end
```


