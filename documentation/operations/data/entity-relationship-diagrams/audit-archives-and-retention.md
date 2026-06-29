# Audit Archives And Retention Entity Relationship Diagram

This page explains the data model used for retained audit, archive, activity and operational history records.

## Scope

This view focuses on:

- establishment change-history archive concepts;
- multi-value old/new value archive concepts;
- change-request mailing and inspection archive concepts;
- activity and operational history;
- retention classification for audit and archive families.

It does not show the full audit catalogue or every audited table.

## How To Read This Model

The audit and archive model contains several different kinds of history:

- active business workflow history;
- technical audit snapshots;
- archived copies of historic workflow data;
- user activity and operational history;
- maintenance and extract logs;
- classification evidence used to decide what should be retained, migrated, archived or retired.

Archive tables should be treated as retained historical evidence. They may have meaningful relationships even where the schema does not enforce those relationships with physical foreign keys.

## Historic Audit And Archive Pattern

```mermaid
erDiagram
    EstablishmentChangeHistory {
        numeric id
        nvarchar type
        numeric establishment_URN
        nvarchar field_shortName
        nvarchar status
        datetime createdDate
        datetime effectiveDate
    }

    EstablishmentChangeHistoryAudit {
        numeric id
        numeric ver_rev
        tinyint REVTYPE
        nvarchar type
        nvarchar status
        numeric establishment_URN
        nvarchar field_shortName
    }

    EstablishmentChangeHistoryArchive {
        numeric id
        nvarchar type
        numeric establishment_URN
        nvarchar field_shortName
        nvarchar status
        datetime createdDate
        datetime effectiveDate
    }

    ChangeHistoryOldValuesArchive {
        numeric EstablishmentChangeHistory_id
        nvarchar value
    }

    ChangeHistoryNewValuesArchive {
        numeric EstablishmentChangeHistory_id
        nvarchar value
    }

    ChangeRequestMailingHistoryArchive {
        numeric id
        nvarchar addresseeType
        datetime date
        nvarchar mailType
        numeric changeRequest_id
    }

    InspectionArchive {
        numeric id
        datetime inspectionDate
        numeric changeRequest_id
    }

    RevisionInfo {
        numeric id
        numeric timestamp
        nvarchar modifiedBy_username
    }

    EstablishmentChangeHistory ||--o{ EstablishmentChangeHistoryAudit : active_audit_versions
    RevisionInfo ||--o{ EstablishmentChangeHistoryAudit : revision
    EstablishmentChangeHistoryArchive ||--o{ ChangeHistoryOldValuesArchive : archived_old_values
    EstablishmentChangeHistoryArchive ||--o{ ChangeHistoryNewValuesArchive : archived_new_values
    EstablishmentChangeHistoryArchive ||--o{ ChangeRequestMailingHistoryArchive : archived_mailings
    EstablishmentChangeHistoryArchive ||--o{ InspectionArchive : archived_findings
```

### EstablishmentChangeHistory

`EstablishmentChangeHistory` records business workflow and change-history rows for establishment changes.

Business-friendly pattern:

```text
For this establishment,
which field was changed or proposed for change,
what happened to that change,
and what evidence should be retained?
```

### EstablishmentChangeHistoryAudit

The audit form of establishment change history stores revisioned snapshots of change-history rows.

Business-friendly pattern:

```text
For this change-history row,
what did it look like at this audit revision?
```

### EstablishmentChangeHistoryArchive

The archive form of establishment change history represents retained historic copies of change-history workflow rows.

Business-friendly pattern:

```text
For this archived establishment change-history row,
what historic workflow evidence has been retained?
```

### ChangeHistoryOldValuesArchive

`ChangeHistoryOldValuesArchive` stores archived old values for multi-value changes.

Business-friendly pattern:

```text
For this archived multi-value change,
which previous values were retained?
```

### ChangeHistoryNewValuesArchive

`ChangeHistoryNewValuesArchive` stores archived new values for multi-value changes.

Business-friendly pattern:

```text
For this archived multi-value change,
which replacement values were retained?
```

### ChangeRequestMailingHistoryArchive

`ChangeRequestMailingHistoryArchive` stores retained evidence of mailings or notifications associated with historic change requests.

Business-friendly pattern:

```text
For this archived change request,
which mailing or notification evidence was retained?
```

### InspectionArchive

`InspectionArchive` stores retained inspection or sanity-check findings associated with historic change requests.

Business-friendly pattern:

```text
For this archived change request,
which inspection or sanity-check evidence was retained?
```

## Activity And Operational History

```mermaid
erDiagram
    SystemUser {
        nvarchar username
        nvarchar UserGroupCode
        tinyint enabled
    }

    UserActivity {
        numeric id
        datetime date
        nvarchar type
        nvarchar user_username
        nvarchar details
    }

    MaintenanceLog {
        bigint id
        datetime2 OperationTime
        datetime2 StartTime
        datetime2 EndTime
        varchar StatusMessage
    }

    SanityCheckLog {
        numeric id
        datetime started
        datetime finished
        nvarchar status
    }

    ScheduledExtractLog {
        numeric id
        datetime started
        datetime finished
        nvarchar status
    }

    SystemUser ||--o{ UserActivity : user_activity
```

### UserActivity

`UserActivity` records user activity events.

Business-friendly pattern:

```text
For this user activity event,
who performed it,
when did it happen,
and what activity was recorded?
```

### MaintenanceLog

Maintenance logs record operational database maintenance activity.

Business-friendly pattern:

```text
For this maintenance operation,
when did it run,
what status was recorded,
and what operational evidence should be retained?
```

### SanityCheckLog

`SanityCheckLog` records sanity-check processing activity.

Business-friendly pattern:

```text
For this sanity check,
when did it run,
what status was recorded,
and what evidence should be retained?
```

### ScheduledExtractLog

`ScheduledExtractLog` records scheduled extract processing activity.

Business-friendly pattern:

```text
For this scheduled extract run,
when did it run,
what status was recorded,
and what evidence should be retained?
```

## Retention Classification

```mermaid
erDiagram
    AuditTableCatalogue {
        nvarchar audit_table
        nvarchar audited_subject
        nvarchar current_use_assessment
        nvarchar evidence_and_rationale
    }

    ActiveAuditFamily {
        nvarchar family_name
        nvarchar example_tables
    }

    LowActivityAuditFamily {
        nvarchar family_name
        nvarchar example_tables
    }

    LegacyCandidateAuditFamily {
        nvarchar family_name
        nvarchar example_tables
    }

    ArchiveHistoryFamily {
        nvarchar family_name
        nvarchar example_tables
    }

    AuditTableCatalogue ||--o{ ActiveAuditFamily : classifies
    AuditTableCatalogue ||--o{ LowActivityAuditFamily : classifies
    AuditTableCatalogue ||--o{ LegacyCandidateAuditFamily : classifies
    AuditTableCatalogue ||--o{ ArchiveHistoryFamily : classifies
```

### AuditTableCatalogue

`AuditTableCatalogue` is the control view for classifying audit and archive table families for retention and rationalisation decisions.

Business-friendly pattern:

```text
For this audit, archive or history table,
what subject does it record,
how active is it,
and what retention or migration decision is needed?
```

## Reading This Diagram

These ERDs are explanatory views, not a deletion or retention schedule. Any archive or audit retirement decision still needs retention, legal, operational and business-owner confirmation.

