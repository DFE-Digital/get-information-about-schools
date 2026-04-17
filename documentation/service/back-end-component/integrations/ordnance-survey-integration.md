# Ordnance Survay Integration

## Overview



Two related address integration paths exist:

1. **Offline import pipeline that converts source XML into CSV and then into database address structures**
   - Batch-style local data import process under `addressLayer`
   - Builds a local address dataset from source XML and supporting files
   - Imports and transforms that data into database tables

2. **Runtime postcode lookup against the OS Places AddressBase API**
   - Live API integration in `AddressBaseApiController`
   - Exposes a REST endpoint for postcode-based address lookup
   - Calls the Ordnance Survey AddressBase Places API at runtime


## Main Classes and Files

### Runtime API integration

This is the application-facing integration used to query addresses by postcode.

Main classes :

- `AddressBaseApi`
- `AddressBaseApiController`
- `AddressBaseResultsDeserializer`


### Offline import module

This module is used to turn source address XML into a CSV and then into database structures used by the wider application.

Main classes :
- `addressLayer/Import Sequence.txt`
- `ImportXmlMapFiles`
- `XmlMapSaxHandler`
- `LocationNameFormatter`
- `Import.sql`
- `Cleaning AddressLayerRaw.sql`
- `Analysis.sql`
- `EdubaseAddressLayer.sql`


## Runtime Address Lookup Flow

The live API endpoint is:

- `GET /establishment/addressBase/queryByPostcode`

defined in `AddressBaseApi`

The controller then calls:

- `https://api.os.uk/search/places/v1/postcode`

from [`AddressBaseApiController`].

It sends these request parameters:

- `postcode`
- `dataset=DPA,LPI`
- `key=<address.base.api.key>`
- `offset=100` on the second request if more than 100 results are returned

The controller:

- Rejects any uses in the `PUBLIC` UserGroup
- Calls the external API
- Handles error and fault responses
- Deserializes successful responses
- Merges up to two pages of results
- Sorts the results by match score and address
- Returns JSON to the caller

## Authentication

The runtime address lookup authenticates using an API key passed as a query parameter:

- `key=<address.base.api.key>`

This is loaded in `AddressBaseApiController` via:

- `messageManager.getServerProperty("address.base.api.key")`

The runtime integration uses:

- API key authentication
- passed as a request parameter, not an HTTP header

## Runtime Sequence Diagram

```mermaid
sequenceDiagram
    autonumber
    participant Client as GIAS Caller
    participant API as AddressBaseApiController
    participant Config as Server Properties
    participant OS as OS Places AddressBase API

    Client->>API: GET /establishment/addressBase/queryByPostcode?postcode=...
    API->>API: Check user is not in userGroup.PUBLIC

    alt Access denied
        API-->>Client: 400 error response
    else Authorised
        API->>Config: Read address.base.api.key
        Config-->>API: API key

        API->>OS: GET /search/places/v1/postcode?postcode=...&dataset=DPA,LPI&key=...
        OS-->>API: Results page 1 or error

        alt More than 100 results
            API->>OS: GET /search/places/v1/postcode?...&offset=100
            OS-->>API: Results page 2
        end

        alt Upstream error or fault
            API-->>Client: Error/fault response
        else Success
            API->>API: Deserialize, merge and sort results
            API-->>Client: JSON address results
        end
    end
```

## Offline Import Flow


Process steps

1. Run `ImportXmlMapFiles.main()` to create `addressLayerFull.csv`
2. Run [`Import.sql`]to load the CSV into a temporary table
3. Run [`Cleaning AddressLayerRaw.sql`] to fix data
4. Optionally run [`Analysis.sql`] for checks
5. Run [`EdubaseAddressLayer.sql`] to build the final database structures

The import step also requires supporting source files, including:

- `addressLayerFull.csv`
- `Scottish_Postcodes.dat`
- `townToCounty.csv`

## Offline Import Sequence Diagram

```mermaid
sequenceDiagram
    autonumber
    participant Ops as Operations User
    participant Importer as ImportXmlMapFiles
    participant XML as Address Layer Source XML
    participant CSV as addressLayerFull.csv
    participant SQL as SQL Import Scripts
    participant DB as GIAS Database

    Ops->>Importer: Run import utility
    Importer->>XML: Parse source XML files via SAX
    XML-->>Importer: Address records
    Importer->>CSV: Write flattened addressLayerFull.csv

    Ops->>SQL: Run Import.sql
    SQL->>DB: Load CSV into temp/raw table
    DB-->>SQL: Imported

    Ops->>SQL: Run Cleaning AddressLayerRaw.sql
    SQL->>DB: Clean imported data
    DB-->>SQL: Cleaned

    opt Optional analysis
        Ops->>SQL: Run Analysis.sql
        SQL->>DB: Execute checks
        DB-->>SQL: Results
    end

    Ops->>SQL: Run EdubaseAddressLayer.sql
    SQL->>DB: Build final Address Layer structures
    DB-->>SQL: Ready
```


