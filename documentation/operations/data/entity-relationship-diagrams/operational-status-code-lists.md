# Operational Status Code Lists

This page explains status, flag and operational code-list tables that support establishment records, independent-school details, user groups and background processing.

## Scope

This model covers:

- establishment operational support classifications;
- independent-school operational classifications;
- user-group record status;
- bulk-update status and progress;
- feature flags and small process-run state.

It does not cover core establishment lifecycle status, geography classifications or organisation group classifications.

## How To Read This Model

- Some statuses are normalised lookup values with a code, name and archived flag.
- Some process statuses are stored directly as text or integer fields.
- Feature flags are operational configuration, not business reference data.
- Process-run statuses describe operational execution, not provider facts.

## Application-Derived Insights

- The model mixes governed business-style code lists with technical process states.
- Establishment operational statuses support communication, login and inspection-related behaviour.
- Bulk update status is split between parent-run state, progress totals and detailed messages.
- Future modelling should separate business reference data from operational runtime state.

## Establishment Operational Code Lists

```mermaid
erDiagram
    Establishment {
        numeric URN PK
        nvarchar EstablishmentName
        nvarchar changeReason_code FK
        nvarchar contactPreference_code FK
        nvarchar MarketingOptInOut_code FK
        nvarchar movedToEdubaseLogin_code FK
        nvarchar s2sLogin_code FK
        nvarchar ofstedSpecialMeasures_code FK
    }
    ChangeReason {
        nvarchar code PK
        nvarchar name
        tinyint archived
        int id
    }
    ContactPreference {
        nvarchar code PK
        nvarchar name
        tinyint archived
        int id
    }
    MarketingOptOut {
        nvarchar code PK
        nvarchar name
        tinyint archived
        int id
    }
    MovedToEdubaseLogin {
        nvarchar code PK
        nvarchar name
        tinyint archived
        int id
    }
    S2SLogin {
        nvarchar code PK
        nvarchar name
        tinyint archived
        int id
    }
    OfstedSpecialMeasures {
        nvarchar code PK
        nvarchar name
        tinyint archived
        int id
    }

    ChangeReason ||--o{ Establishment : change_reason
    ContactPreference ||--o{ Establishment : contact_preference
    MarketingOptOut ||--o{ Establishment : marketing_choice
    MovedToEdubaseLogin ||--o{ Establishment : migrated_login_status
    S2SLogin ||--o{ Establishment : s2s_login_status
    OfstedSpecialMeasures ||--o{ Establishment : ofsted_special_measures
```

### Establishment Operational Code Lists

Business-friendly pattern:

```text
For this establishment record,
which support status, contact preference, login state or inspection-related flag applies?
```

These values sit alongside the provider record. They are not the same as the provider's open, closed or proposed lifecycle status.

## Independent-School And User Status

```mermaid
erDiagram
    IndependentSchools {
        numeric URN PK
        nvarchar accomChange_code FK
        varchar registrationSuspended_code FK
    }
    AccommodationChanged {
        nvarchar code PK
        nvarchar name
        tinyint archived
        int id
    }
    RegistrationSuspended {
        varchar code PK
        varchar name
        int orderBy
        bit archived
        int id
    }
    UserGroup {
        nvarchar code PK
        nvarchar name
        nvarchar recordStatus_code FK
    }
    RecordStatus {
        nvarchar code PK
        nvarchar name
        tinyint archived
        int id
    }

    AccommodationChanged ||--o{ IndependentSchools : accommodation_changed
    RegistrationSuspended ||--o{ IndependentSchools : registration_suspended
    RecordStatus ||--o{ UserGroup : user_group_record_status
```

### Independent-School Operational Status

Business-friendly pattern:

```text
For this independent-school detail record,
has accommodation changed,
and is registration suspended?
```

### RecordStatus

Business-friendly pattern:

```text
For this user group,
what record status applies to the group itself?
```

`RecordStatus` is not the same concept as establishment status.

## Operational Process State

```mermaid
erDiagram
    BulkUpdate {
        numeric id PK
        datetime createTime
        datetime startTime
        datetime endTime
        int status
        int fileFormat
        nvarchar user_username FK
        nvarchar asUser_username FK
    }
    BulkUpdateStatus {
        numeric id PK, FK
        nvarchar type
        numeric total
        numeric processed
        int errorsCount
        int rowsCount
        nvarchar status
    }
    BulkUpdateMessage {
        numeric id PK
        numeric bulkUpdate_id FK
        datetime time
        nvarchar message
        tinyint skipped
    }
    FeatureFlags {
        varchar flagName PK
        varchar description
        bit enabled
        datetime2 updatedAt
    }
    TelephoneUpdateProcess {
        numeric id PK
        numeric updated_items_count
        varchar process_status
        datetime date_start
        datetime date_end
    }
    WebAddressUpdateProcess {
        numeric id PK
        numeric updated_items_count
        varchar process_status
        datetime date_start
        datetime date_end
    }
    UkprnUpdateProcess {
        numeric id PK
        datetime since_date
        varchar item_type
        numeric updated_items_count
        varchar process_status
        datetime date_start
        datetime date_end
    }

    BulkUpdate ||--o| BulkUpdateStatus : progress_status
    BulkUpdate ||--o{ BulkUpdateMessage : process_messages
```

### BulkUpdate, BulkUpdateStatus And BulkUpdateMessage

Business-friendly pattern:

```text
For this bulk update run,
who ran it,
what progress was made,
and what messages or skipped rows were recorded?
```

### FeatureFlags

Business-friendly pattern:

```text
For this runtime feature,
is the feature enabled,
and when was the toggle last updated?
```

### Update Process Tables

Business-friendly pattern:

```text
For this operational update process,
when did it run,
how many items were updated,
and what process status was recorded?
```

`InspectionUpdates` has been omitted because it is marked as having no observed production read or write activity in the 30-day table-usage evidence.

## Reading This Diagram

Use this model to distinguish provider-facing operational classifications from process-state evidence. The same database area contains business-style code lists, feature toggles and technical process tracking, so those concepts should not be collapsed into one future reference-data model.
