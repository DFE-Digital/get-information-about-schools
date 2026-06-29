# Scheduled Extracts And Callbacks

This page explains scheduled extracts, selected extract fields, generated output artefacts, callback history and execution logs.

## Scope

This model covers:

- reusable scheduled extract definitions;
- saved filters and selected output fields;
- callback/output artefact state;
- callback history and post-run verification;
- execution logs for scheduled and generated extracts.

## How To Read This Model

- A scheduled extract is more than a file; it combines schedule, filter, fields, ownership and output state.
- A callback represents generated output state and storage information.
- Some history and verifier relationships are inferred by identifier columns rather than enforced by foreign keys.
- Access-control semantics for scheduled extracts sit alongside this model, but are not repeated here.

## Application-Derived Insights

- Scheduled extracts combine record selection, field selection, schedule, ownership and run-as context.
- Output state is split between current callback, callback history, generation logs and verification attempts.
- Future modelling should preserve the difference between extract definition, run, artefact, history and verification.

## Scheduled Extract Definition

```mermaid
erDiagram
    ScheduledExtract {
        numeric id PK
        nvarchar name
        nvarchar type
        nvarchar frequency
        datetime startDate
        datetime endDate
        datetime lastExtractionDate
        numeric filter_id FK
        numeric callback_id FK
        nvarchar creatorUsername
        nvarchar creatorGroup FK
        nvarchar asUserUsername
        nvarchar asGroup FK
        tinyint publicAccessible
    }
    EstablishmentFilter {
        numeric id PK
        nvarchar name
        nvarchar creatorGroup_code FK
        nvarchar creatorUsername
    }
    Callback {
        numeric id PK
        nvarchar uid
        nvarchar name
        nvarchar type
        nvarchar format
        nvarchar status
        datetime created
        datetime started
        datetime finished
        nvarchar storageType
        nvarchar storageKey
        nvarchar storageUri
    }
    UserGroup {
        nvarchar code PK
        nvarchar name
        nvarchar role
    }
    SystemUser {
        nvarchar username PK
        nvarchar UserGroupCode FK
        tinyint enabled
    }

    UserGroup ||--o{ ScheduledExtract : creator_group
    UserGroup ||--o{ ScheduledExtract : run_as_group
    EstablishmentFilter ||--o{ ScheduledExtract : extract_filter
    Callback ||--o{ ScheduledExtract : current_callback
    SystemUser ||..o{ ScheduledExtract : inferred_creator_user
    SystemUser ||..o{ ScheduledExtract : inferred_run_as_user
```

### ScheduledExtract

Business-friendly pattern:

```text
For this reusable extract,
what should be generated,
when should it run,
which filter and fields should be used,
and under which ownership or run-as context?
```

### Callback

Business-friendly pattern:

```text
For this generated extract artefact,
what type and format is it,
where is it stored,
what state is it in,
and who created it?
```

## Extract Fields And Output History

```mermaid
erDiagram
    ScheduledExtract {
        numeric id PK
        nvarchar name
        numeric callback_id FK
        numeric filter_id FK
    }
    ExtractFieldWithParams {
        numeric id PK
        numeric scheduledExtract_id FK
        nvarchar field_shortName FK
        tinyint showLookupName
    }
    EstablishmentField {
        nvarchar shortName PK
        nvarchar displayName
        nvarchar fieldtype
        int orderInExtracts
    }
    Callback {
        numeric id PK
        nvarchar uid
        nvarchar status
        nvarchar storageType
        nvarchar storageKey
        nvarchar storageUri
    }
    CallbackHistory {
        numeric id PK
        nvarchar uid
        numeric scheduled_extract_id
        nvarchar status
        datetime created
        datetime started
        datetime finished
        int total
        int processed
        numeric dataSize
        nvarchar storageType
        nvarchar storageKey
        nvarchar storageUri
        datetime lastSuccess
    }
    ExtractsPostRunVerifierAudit {
        numeric id PK
        numeric callbackId
        numeric scheduledExtractId
        nvarchar extractName
        int retry_attempt
        datetime retryed_at
        datetime finishedAt
        nvarchar status
        nvarchar correlationId
    }

    ScheduledExtract ||--o{ ExtractFieldWithParams : selected_fields
    EstablishmentField ||--o{ ExtractFieldWithParams : selected_field
    Callback ||--o{ CallbackHistory : inferred_callback_runs
    ScheduledExtract ||--o{ CallbackHistory : generated_history
    ScheduledExtract ||--o{ ExtractsPostRunVerifierAudit : post_run_verification
    Callback ||--o{ ExtractsPostRunVerifierAudit : verified_callback
```

### ExtractFieldWithParams

Business-friendly pattern:

```text
For this scheduled extract,
which establishment field should be included,
and should lookup names be shown?
```

### CallbackHistory

Business-friendly pattern:

```text
For this generated extract run,
what happened to the callback,
where was the output stored,
and did it complete successfully?
```

### ExtractsPostRunVerifierAudit

Business-friendly pattern:

```text
For this generated extract callback,
what verification attempt ran,
when did it run,
what was the outcome,
and how is it correlated with other processing evidence?
```

## Execution Logs

```mermaid
erDiagram
    SystemUser {
        nvarchar username PK
    }
    ScheduledExtract {
        numeric id PK
        nvarchar name
    }
    EstablishmentFilter {
        numeric id PK
        nvarchar name
    }
    ScheduledExtractLog {
        numeric id PK
        datetime timestamp
        nvarchar systemUser
        numeric scheduledExtract
        tinyint isGeneratedOnFly
        numeric establishmentFilter
        tinyint success
        datetime timeStarted
        datetime timeFinished
    }
    GroupExtractLog {
        numeric id PK
        tinyint isWebService
        nvarchar systemUser
        datetime timestamp
        nvarchar type
    }

    SystemUser ||..o{ ScheduledExtractLog : inferred_system_user
    ScheduledExtract ||..o{ ScheduledExtractLog : inferred_scheduled_extract
    EstablishmentFilter ||..o{ ScheduledExtractLog : inferred_filter
    SystemUser ||..o{ GroupExtractLog : inferred_system_user
```

### ScheduledExtractLog

Business-friendly pattern:

```text
For this scheduled or generated-on-the-fly extract run,
who ran it,
which scheduled extract or filter was used,
when did it start and finish,
and did it succeed?
```

### GroupExtractLog

Business-friendly pattern:

```text
For this group extract download or web-service request,
who requested it,
when did it run,
and what kind of extract was produced?
```

`ExtractionLog` has been omitted because it is marked as having no observed production read or write activity in the 30-day table-usage evidence.

## Reading This Diagram

Use this model to distinguish scheduled extract definitions from generated extract outputs. The definition, selected fields, callback, history, execution log and verification evidence are separate concepts.
