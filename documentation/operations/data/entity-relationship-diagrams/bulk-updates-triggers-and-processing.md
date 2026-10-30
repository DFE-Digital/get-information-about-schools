# Bulk Updates, Triggers And Processing

This page explains operational processing tables that record bulk updates, data-quality triggers, reminders and small background process logs.

## Scope

This model covers:

- bulk update job state, progress and messages;
- data-quality trigger settings and reminder state;
- governance reminder state;
- process logs for small background update jobs.

Scheduler runtime tables are covered separately in `scheduler-and-batch-runtime.md`.

## How To Read This Model

- These tables record process evidence rather than canonical provider facts.
- Some tables are initiated by users, such as bulk update files.
- Some tables are produced by automated checks, such as reminders and trigger state.
- Some relationships are inferred from identifiers rather than enforced by foreign keys.

## Application-Derived Insights

- Bulk updates can bypass or interact with change-request behaviour, so they are not just file uploads.
- Data-quality trigger state can lead to user-facing prompts or reminders.
- Governance reminder tables are derived from governance freshness and role-expiration rules.
- Future design should separate commands, process progress, derived reminders, integration holding rows and domain changes.

## Bulk Update Processing

```mermaid
erDiagram
    SystemUser {
        nvarchar username PK
        nvarchar UserGroupCode FK
        tinyint enabled
    }
    BulkUpdate {
        numeric id PK
        datetime createTime
        datetime startTime
        datetime endTime
        datetime effectiveDate
        int status
        nvarchar fileName
        nvarchar originalFileName
        tinyint overrideCR
        int fileFormat
        tinyint createFreeSchools
        tinyint createAcademyLinks
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

    SystemUser ||--o{ BulkUpdate : requested_by
    SystemUser ||--o{ BulkUpdate : run_as_user
    BulkUpdate ||--|| BulkUpdateStatus : progress_status
    BulkUpdate ||--o{ BulkUpdateMessage : messages
```

### BulkUpdate, BulkUpdateStatus And BulkUpdateMessage

Business-friendly pattern:

```text
For this uploaded bulk update file,
who submitted it,
which identity should it run as,
what kind of update is it,
and what was the outcome?
```

The model separates the parent request, progress counts and detailed messages.

## Data-Quality Trigger And Reminder State

```mermaid
erDiagram
    Establishment {
        numeric URN PK
        nvarchar EstablishmentName
        nvarchar eduBaseTrigger1_code FK
        tinyint checkedSixtyDays
    }
    EduBaseTrigger {
        nvarchar code PK
        nvarchar name
        tinyint archived
    }
    EdubaseTriggerSettings {
        numeric id PK
        numeric lowerThreshold
        numeric higherThreshold
        numeric governanceThreshold
        nvarchar saAccessRestrictions_code FK
        tinyint saSyncInitial
    }
    SAAccessRestrictions {
        nvarchar code PK
        nvarchar name
    }
    SixtyDayNotUpdatedReminder {
        numeric id PK
        numeric urn
        datetime sending_date
        varchar addressee
    }
    SixtyDayNotUpdatedReminderFlag {
        numeric id PK
        bit enabled
        datetime update_date
        varchar updated_by
    }

    EduBaseTrigger ||--o{ Establishment : trigger_state
    SAAccessRestrictions ||--o{ EdubaseTriggerSettings : restriction_policy
    Establishment ||..o{ SixtyDayNotUpdatedReminder : inferred_establishment_reminder
```

### EduBaseTrigger And EdubaseTriggerSettings

Business-friendly pattern:

```text
For this establishment,
what data-quality trigger state applies,
and what thresholds control the warning or issue?
```

### SixtyDayNotUpdatedReminder

Business-friendly pattern:

```text
For this establishment,
has a reminder been produced because the record has not been checked recently?
```

## Governance Reminder State

```mermaid
erDiagram
    UserGroup {
        nvarchar code PK
        nvarchar name
    }
    SystemUser {
        nvarchar username PK
    }
    Establishment {
        numeric URN PK
        nvarchar EstablishmentName
    }
    EstablishmentGroup {
        numeric id PK
        nvarchar name
    }
    StaffRole {
        nvarchar code PK
        nvarchar name
        tinyint isOnePersonRole
    }
    Reminder {
        numeric id PK
        nvarchar creatorUsername
        nvarchar creatorUserGroup FK
        nvarchar title
        datetime dueDate
        datetime notificationDate
    }
    ReminderInstance {
        numeric id PK
        numeric reminder_id FK
        numeric URN FK
        nvarchar username FK
        tinyint complete
    }
    StaffRecordsOutdated {
        numeric id PK
        numeric announcementId
        datetime created
        datetime resolved
        numeric saOrgId
        numeric uid
        numeric urn
        datetime governanceLastCheck
    }
    StaffRoleExpiration {
        numeric id PK
        numeric announcementId
        datetime created
        datetime resolved
        numeric saOrgId
        numeric uid
        numeric urn
        numeric gid
        datetime stepdownDate
        nvarchar staffRole FK
    }

    UserGroup ||--o{ Reminder : creator_group
    Reminder ||--o{ ReminderInstance : recipient_instance
    Establishment ||--o{ ReminderInstance : establishment_instance
    SystemUser ||--o{ ReminderInstance : user_instance
    Establishment ||..o{ StaffRecordsOutdated : inferred_establishment_scope
    EstablishmentGroup ||..o{ StaffRecordsOutdated : inferred_group_scope
    StaffRole ||--o{ StaffRoleExpiration : expired_role
    Establishment ||..o{ StaffRoleExpiration : inferred_establishment_scope
    EstablishmentGroup ||..o{ StaffRoleExpiration : inferred_group_scope
```

### Reminder And ReminderInstance

Business-friendly pattern:

```text
For this reminder,
which establishment or user received it,
and has that reminder instance been completed?
```

### StaffRecordsOutdated And StaffRoleExpiration

Business-friendly pattern:

```text
For this establishment or group,
does governance information need attention,
and has the reminder or announcement been created or resolved?
```

## Process Logs

```mermaid
erDiagram
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
    CompaniesHouseDownloadHolder {
        nvarchar companyNumber PK
        numeric edubaseId PK
        nvarchar edubaseName
        tinyint edubaseOpen
        nvarchar companyStatus
        nvarchar status
    }
    SanityCheckLog {
        numeric id PK
        datetime started
        datetime finished
        nvarchar status
    }
    FrontEnd_DataQualityStatus {
        nvarchar PartitionKey PK
        nvarchar RowKey PK
        datetime Timestamp
    }
```

### Process Log Tables

Business-friendly pattern:

```text
For this background process,
when did it run,
what outcome was recorded,
and what operational evidence was kept?
```

`InspectionUpdates` has been omitted because it is marked as having no observed production read or write activity in the 30-day table-usage evidence.

## Reading This Diagram

Use this model to understand operational evidence around changes and reminders. It should not be treated as a single generic jobs model, because each process has different ownership, retention and restart behaviour.
