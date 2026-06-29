# Sharing And Public Cache

This page explains the public sharing cache model in the `gias_sharing` schema.

## Working Assumption

The strong working assumption is that the cache tables provide a stable public/read model for a REST API or downstream data-sharing consumer.

Evidence:

- Public views exist for establishment, group, group-link and governance cache data.
- The group-link public view description says it supports a REST API consumer and decouples that API from changes to underlying core and cache table structures.
- The cache refresh procedure is documented as being called by an Azure Function.
- Table-usage evidence shows refresh activity for the main cache tables and cache update logs.
- The cache tables are denormalised around published establishment, group, group-link and governance views rather than edit workflows.

The exact named consumer has not yet been identified. Treat the consumer as an assumed public/read API or downstream sharing contract until the refresh function and view readers are traced.

## Scope

This model covers:

- public establishment cache;
- public group cache;
- public group-link cache;
- public governance/staff cache;
- public change-history cache;
- cache refresh logging and public-column selection.

## How To Read This Model

- Cache tables are read-model projections, not editable source records.
- Public views expose the cache shape to consumers.
- `public_columns` controls which change-history fields can be included in public change history.
- Cache update logs record refresh runs and per-table refresh outcomes.

## Application-Derived Insights

- The sharing cache is a publication/read model boundary.
- It decouples downstream consumers from transactional table structure.
- Change-history publication is filtered separately from source change history.
- Future design should decide whether this remains SQL cache projection, API read model, event projection or another publication contract.

## Public Cache Projection

```mermaid
erDiagram
    Establishment {
        numeric URN PK
        nvarchar EstablishmentName
        nvarchar type_code FK
        int version
    }
    establishment_cache {
        numeric urn PK,FK
        int version
        nvarchar establishment_name
        nvarchar establishment_type_code
        nvarchar status_code
        numeric ukprn
        int establishment_number
    }
    EstablishmentGroup {
        numeric id PK
        nvarchar name
        nvarchar type_code FK
        nvarchar companiesHouseNumber
    }
    establishment_group_cache {
        numeric gid PK,FK
        nvarchar name
        nvarchar group_type_code
        nvarchar group_status_code
        nvarchar companies_house_number
        numeric ukprn
    }
    GroupLink {
        numeric id PK
        numeric urn FK
        numeric group_id FK
        int version
    }
    group_link_cache {
        numeric id PK,FK
        numeric urn FK
        numeric gid FK
        int urn_version
        int gid_version
        nvarchar link_type
        datetime effective_date
    }
    StaffRecord {
        numeric uid PK
        numeric establishment_URN FK
        numeric group_id FK
        int version
    }
    staff_record_cache {
        numeric uid PK,FK
        int version PK
        numeric establishment_urn
        numeric gid
        nvarchar staff_role_code
        nvarchar staff_role_name
        nvarchar forename_1
        nvarchar surname
    }

    Establishment ||--o| establishment_cache : public_projection
    EstablishmentGroup ||--o| establishment_group_cache : public_projection
    GroupLink ||--o| group_link_cache : public_projection
    StaffRecord ||--o{ staff_record_cache : public_projection
    establishment_cache ||--o{ group_link_cache : cached_establishment
    establishment_group_cache ||--o{ group_link_cache : cached_group
```

### EstablishmentCache

Business-friendly pattern:

```text
For this establishment,
what public establishment details should be made available without joining the operational establishment model?
```

### EstablishmentGroupCache

Business-friendly pattern:

```text
For this education provider group,
what public group details should be made available without joining the operational group model?
```

### GroupLinkCache

Business-friendly pattern:

```text
For this establishment and group relationship,
what public relationship details should be shared?
```

### StaffRecordCache

Business-friendly pattern:

```text
For this governance role record,
what public governance details should be shared?
```

## Public Change History And Refresh

```mermaid
erDiagram
    EstablishmentChangeHistory {
        numeric id PK
        numeric establishment_urn FK
        nvarchar field_short_name
    }
    establishment_change_history_cache {
        numeric id PK,FK
        numeric establishment_urn
        nvarchar field_short_name
        nvarchar request_type
        nvarchar status
    }
    GroupChangeRequest {
        numeric id PK
        numeric groupId FK
        nvarchar status
    }
    establishment_group_change_history_cache {
        numeric id PK,FK
        int version PK
        numeric gid
        numeric urn
        nvarchar field_short_name
        nvarchar request_type
        nvarchar status
    }
    StaffChangeRequest {
        numeric id PK
        numeric staffRecord_uid FK
        nvarchar status
    }
    staff_record_change_history_cache {
        numeric id PK,FK
        int version PK
        numeric staff_record_uid
        numeric urn
        numeric gid
        nvarchar field_short_name
        nvarchar request_type
        nvarchar status
    }
    public_columns {
        int public_columns_record_id PK
        nvarchar source_table_name
        nvarchar target_table_name
        nvarchar source_column_name
        nvarchar target_column_name
    }
    cache_update_log {
        bigint cache_update_log_id PK
        datetime start_time
        datetime end_time
        tinyint success_flag
        nvarchar log_error_code
    }
    cache_update_table_log {
        bigint cache_update_log_id PK,FK
        nvarchar table_name PK
        int number_of_rows_inserted
        int number_of_rows_updated
    }

    EstablishmentChangeHistory ||--o| establishment_change_history_cache : public_change_projection
    GroupChangeRequest ||--o{ establishment_group_change_history_cache : public_change_projection
    StaffChangeRequest ||--o{ staff_record_change_history_cache : public_change_projection
    cache_update_log ||--o{ cache_update_table_log : table_refresh_results
    public_columns }o..o{ establishment_change_history_cache : filters_public_fields
    public_columns }o..o{ establishment_group_change_history_cache : filters_public_fields
    public_columns }o..o{ staff_record_change_history_cache : filters_public_fields
```

### Public Change History Cache

Business-friendly pattern:

```text
For this source change-history record,
is the changed field allowed to appear in public change history,
and what public change details should be shared?
```

### PublicColumns

Business-friendly pattern:

```text
For this source table and column,
which target cache table and column is allowed for public projection?
```

### CacheUpdateLog

Business-friendly pattern:

```text
For this cache refresh run,
when did it start and finish,
did it succeed,
and what per-table refresh results were recorded?
```

## Reading This Diagram

Use this model as the publication boundary for public/shareable data. The cache is intentionally shaped for stable read access and should not be confused with the transactional establishment, group or governance source model.
