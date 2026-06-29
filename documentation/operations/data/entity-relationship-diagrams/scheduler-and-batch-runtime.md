# Scheduler And Batch Runtime

This page explains the runtime tables used by Quartz Scheduler and Spring Batch.

These tables support scheduled and batch processing. They are operational infrastructure, not education provider business-domain entities.

## Scope

This model covers:

- Quartz scheduler job and trigger runtime tables;
- Spring Batch job, execution, step and parameter runtime tables;
- the boundary between framework runtime state and provider-facing operational records.

## How To Read This Model

- Quartz tables hold scheduler jobs, triggers, fire times, locks and scheduler heartbeat state.
- Spring Batch tables hold job instances, executions, parameters, step executions and restart context.
- Runtime tables do not physically own provider facts.
- Application jobs connect runtime state to business work such as extracts, callbacks, imports and reminders.

## Application-Derived Insights

- Scheduler and batch tables are high-volume operational state, not business history.
- Future design should decide whether this runtime state remains in the application database or moves to platform orchestration.
- Scheduled extracts, staff imports, cache refreshes and reminders may need different scheduling patterns.
- Business history should be held by domain records or events, not by framework runtime tables.

## Quartz Scheduler Runtime

```mermaid
erDiagram
    QRTZ_JOB_DETAILS {
        varchar SCHED_NAME PK
        varchar JOB_NAME PK
        varchar JOB_GROUP PK
        varchar JOB_CLASS_NAME
        varchar DESCRIPTION
        varchar IS_DURABLE
        varchar IS_NONCONCURRENT
        varchar REQUESTS_RECOVERY
    }
    QRTZ_TRIGGERS {
        varchar SCHED_NAME PK, FK
        varchar TRIGGER_NAME PK
        varchar TRIGGER_GROUP PK
        varchar JOB_NAME FK
        varchar JOB_GROUP FK
        bigint NEXT_FIRE_TIME
        bigint PREV_FIRE_TIME
        varchar TRIGGER_STATE
        varchar TRIGGER_TYPE
        bigint START_TIME
        bigint END_TIME
    }
    QRTZ_CRON_TRIGGERS {
        varchar SCHED_NAME PK, FK
        varchar TRIGGER_NAME PK, FK
        varchar TRIGGER_GROUP PK, FK
        varchar CRON_EXPRESSION
        varchar TIME_ZONE_ID
    }
    QRTZ_SIMPLE_TRIGGERS {
        varchar SCHED_NAME PK, FK
        varchar TRIGGER_NAME PK, FK
        varchar TRIGGER_GROUP PK, FK
        bigint REPEAT_COUNT
        bigint REPEAT_INTERVAL
        bigint TIMES_TRIGGERED
    }
    QRTZ_SIMPROP_TRIGGERS {
        varchar SCHED_NAME PK, FK
        varchar TRIGGER_NAME PK, FK
        varchar TRIGGER_GROUP PK, FK
        varchar STR_PROP_1
        int INT_PROP_1
        bigint LONG_PROP_1
    }
    QRTZ_FIRED_TRIGGERS {
        varchar SCHED_NAME PK
        varchar ENTRY_ID PK
        varchar TRIGGER_NAME
        varchar TRIGGER_GROUP
        varchar INSTANCE_NAME
        bigint FIRED_TIME
        bigint SCHED_TIME
        varchar STATE
    }
    QRTZ_SCHEDULER_STATE {
        varchar SCHED_NAME PK
        varchar INSTANCE_NAME PK
        bigint LAST_CHECKIN_TIME
        bigint CHECKIN_INTERVAL
    }
    QRTZ_LOCKS {
        varchar SCHED_NAME PK
        varchar LOCK_NAME PK
    }
    QRTZ_PAUSED_TRIGGER_GRPS {
        varchar SCHED_NAME PK
        varchar TRIGGER_GROUP PK
    }

    QRTZ_JOB_DETAILS ||--o{ QRTZ_TRIGGERS : job_has_triggers
    QRTZ_TRIGGERS ||--o| QRTZ_CRON_TRIGGERS : cron_detail
    QRTZ_TRIGGERS ||--o| QRTZ_SIMPLE_TRIGGERS : simple_detail
    QRTZ_TRIGGERS ||--o| QRTZ_SIMPROP_TRIGGERS : property_detail
    QRTZ_TRIGGERS ||..o{ QRTZ_FIRED_TRIGGERS : fired_instances
```

### Quartz Runtime Tables

Business-friendly pattern:

```text
For this scheduled job,
what job should run,
which trigger fires it,
when is it next due,
and which scheduler instance is managing it?
```

`QRTZ_BLOB_TRIGGERS` and `QRTZ_CALENDARS` have been omitted because they are marked as having no observed production read or write activity in the 30-day table-usage evidence.

## Spring Batch Runtime

```mermaid
erDiagram
    BATCH_JOB_INSTANCE {
        bigint JOB_INSTANCE_ID PK
        bigint VERSION
        varchar JOB_NAME
        varchar JOB_KEY
    }
    BATCH_JOB_EXECUTION {
        bigint JOB_EXECUTION_ID PK
        bigint JOB_INSTANCE_ID FK
        datetime CREATE_TIME
        datetime START_TIME
        datetime END_TIME
        varchar STATUS
        varchar EXIT_CODE
        varchar EXIT_MESSAGE
        datetime LAST_UPDATED
    }
    BATCH_JOB_EXECUTION_PARAMS {
        bigint JOB_EXECUTION_ID FK
        varchar TYPE_CD
        varchar KEY_NAME
        varchar STRING_VAL
        datetime DATE_VAL
        bigint LONG_VAL
        float DOUBLE_VAL
        char IDENTIFYING
    }
    BATCH_JOB_EXECUTION_CONTEXT {
        bigint JOB_EXECUTION_ID PK, FK
        varchar SHORT_CONTEXT
        text SERIALIZED_CONTEXT
    }
    BATCH_STEP_EXECUTION {
        bigint STEP_EXECUTION_ID PK
        bigint JOB_EXECUTION_ID FK
        varchar STEP_NAME
        datetime START_TIME
        datetime END_TIME
        varchar STATUS
        bigint READ_COUNT
        bigint WRITE_COUNT
        bigint COMMIT_COUNT
        varchar EXIT_CODE
    }
    BATCH_STEP_EXECUTION_CONTEXT {
        bigint STEP_EXECUTION_ID PK, FK
        varchar SHORT_CONTEXT
        text SERIALIZED_CONTEXT
    }

    BATCH_JOB_INSTANCE ||--o{ BATCH_JOB_EXECUTION : job_runs
    BATCH_JOB_EXECUTION ||--o{ BATCH_JOB_EXECUTION_PARAMS : parameters
    BATCH_JOB_EXECUTION ||--o| BATCH_JOB_EXECUTION_CONTEXT : job_context
    BATCH_JOB_EXECUTION ||--o{ BATCH_STEP_EXECUTION : step_runs
    BATCH_STEP_EXECUTION ||--o| BATCH_STEP_EXECUTION_CONTEXT : step_context
```

### Spring Batch Runtime Tables

Business-friendly pattern:

```text
For this batch job run,
which job instance ran,
which parameters identified it,
which steps ran,
how many records were read or written,
and what status or exit message was recorded?
```

## Runtime Boundary

```mermaid
erDiagram
    QRTZ_JOB_DETAILS {
        varchar JOB_NAME PK
        varchar JOB_GROUP PK
        varchar JOB_CLASS_NAME
    }
    QRTZ_TRIGGERS {
        varchar TRIGGER_NAME PK
        varchar TRIGGER_GROUP PK
        varchar TRIGGER_STATE
        bigint NEXT_FIRE_TIME
    }
    BATCH_JOB_INSTANCE {
        bigint JOB_INSTANCE_ID PK
        varchar JOB_NAME
        varchar JOB_KEY
    }
    BATCH_JOB_EXECUTION {
        bigint JOB_EXECUTION_ID PK
        varchar STATUS
        datetime START_TIME
        datetime END_TIME
    }
    ScheduledExtract {
        numeric id PK
        nvarchar name
        nvarchar frequency
    }
    Callback {
        numeric id PK
        nvarchar status
        nvarchar storageKey
    }
    StaffImportRecord {
        numeric id PK
        nvarchar importUUID
        nvarchar status
    }

    QRTZ_JOB_DETAILS ||--o{ QRTZ_TRIGGERS : schedules_runtime_job
    BATCH_JOB_INSTANCE ||--o{ BATCH_JOB_EXECUTION : executes_batch_job
    QRTZ_JOB_DETAILS }o..o{ ScheduledExtract : application_logic
    BATCH_JOB_EXECUTION }o..o{ StaffImportRecord : application_logic
    QRTZ_TRIGGERS }o..o{ Callback : application_logic
```

### Runtime Boundary

Business-friendly pattern:

```text
For this scheduled or batch process,
which runtime state supports the execution,
and which provider-facing record records the business outcome?
```

## Reading This Diagram

Use this model to avoid treating scheduler rows as business history. Scheduler and batch tables explain how work runs, but the meaningful business result belongs in provider records, extract records, import records, reminders or audit events.
