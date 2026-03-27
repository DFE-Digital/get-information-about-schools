# Flyway Migration Categories

## Overview

The GIAS Back End services uses Flyway for much more than code-coupled schema evolution.

Flyway is actively configured and maintained as part of the operational lifecycle of the system.


The migration tree under `src/main/resources/db/migration`contains a large body of SQL that supports:

- Application schema changes
- Eeference and lookup maintenance
- Permissions and rules management
- Reporting and extract infrastructure
- Operational job state
- Performance tuning
- Monitoring and diagnostics
- Data correction and backfill

The structure of the migration folders, including release, sprint, and specialist subfolders such as `functions`, `indexes`, `views`, `monitoring`, and `olap`, shows that Flyway is being used as an operational database delivery mechanism as well as a schema migration tool.


The Flyway estate makes the database an active operational platform component.

Flyway is being used to manage:

- Schema
- Data
- Security configuration
- Reference content
- Runtime rules
- Reporting support
- Integration support
- Database tuning
- Observability and monitoring artefacts



## Conclusion

The Flyway scripts in this project should be understood as a combined mechanism for:

- database schema evolution
- production configuration rollout
- operational support deployment
- data maintenance and correction



## Migration Scope

The migration supports several broad types of database change.

Flyway migration scripts are located in `/src/main/resources/db/migration/`

### 1. Domain schema and field model changes

These migrations introduce or alter the core database model used by the application, including:

- Tables
- Columns
- Entity relationships
- Field metadata
- Archived or reactivated fields
- Schema support for new functional areas

This is the classic Flyway use case, but it is only one part of the overall migration set.

### 2. Permissions, roles, owning rules, and trusted source configuration

A significant portion of the migrations modifies the security and governance model stored in the database.

These changes include:

- User group permissions
- Authorities
- Owning rules
- Trusted source rules
- By-type access control
- Feature-specific read or edit access


### 3. Reference data, lookups, and controlled vocabularies

Flyway is  used to deploy evolving business configuration and classification data into the live system.

Many migrations modify the application’s reference and lookup data.

These changes include:

- New lookup values
- Renamed values
- Corrected values
- Reference model extensions
- Controlled dictionary updates


### 4. Geo, postcode, address, and territorial data maintenance

Flyway is used to manage changes related to geography and addressing.

These include:

- Postcode data changes
- Geographical code updates
- Administrative boundaries
- Territorial mappings
- Geo lookup maintenance
- UPRN and address-related updates


### 5. Extracts, reports, downloads, callbacks, and query support

Flyway is being used to roll out operational data-delivery capabilities. It is used to manage changes around data extraction and reporting workflows.

These migrations cover:

- Callback infrastructure
- Report support
- Scheduled extract support
- Historical extract support
- Single query and export support
- Tools and reporting tables



### 6. Job, batch, and integration process state

Some migrations create or evolve tables used by background jobs and integrations.

These include support for:

- UKPRN update tracking
- Inspection update tracking
- Companies House processing
- Bulk update status
- Lock handling
- Cache availability
- Other operational process state


### 7. Validation and business-rule configuration

A substantial part of the migration set is used to manage rule-driven behaviour in the database.

This includes:

- Validation rules
- Display policies
- Field mappings
- Field-type relationships
- Default field behaviour
- By field type permissions and policy adjustments


### 8. Performance tuning and physical database optimisation

The inclusionof  `indexes` migrations shows Flyway is being used for operational tuning.

These changes include:

- Adding or changing indexes
- Improving query performance
- Supporting reporting and search workloads
- Adapting the physical database design over time


### 9. Functions, views, and database-side utility logic

Flyway is also used to deploy reusable SQL logic, via the dedicated `functions` and `views` folders.

These changes include:

- Database functions
- Utility functions
- Encryption-related functions
- Reporting views
- Export views
- Operational helper views

### 10. Monitoring, diagnostics, and operational observability

One of the key operational uses of Flyway is Monitoring, these scripts live in the  `monitoring` folder.

These migrations support:

- Process monitoring
- Lock monitoring
- Slow query inspection
- Index analysis
- Waiting statistics
- Memory and performance metrics
- Table size and database health visibility


### 11. Data fixes, corrections, backfills, and cleanup

Flyway is used to make un-structured changes. Instead, they mutate existing data or metadata in place.

These include:

- Populating newly added fields
- Correcting bad values
- Clearing invalid data
- Resolving missing configuration
- Deleting inconsistent rows
- Normalising or reclassifying stored values

Flyway is used as a controlled production data-maintenance mechanism.

## Folder Patterns


High-level patterns include:

- Release and sprint folders, showing ongoing incremental operational rollout
- Version folders that combine schema, data, and policy change
- Dedicated subfolders for:
  - Functions
  - Indexes
  - Views
  - Monitoring
  - Olap
  - Repeatable updates



