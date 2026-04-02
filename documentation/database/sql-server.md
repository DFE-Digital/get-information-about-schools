# SQL Server Integration

## Overview

The GIAS back end service uses Microsoft SQL Server as its primary relational data store.

SQL Server is used for:

- Core application data
- Lookup and reference data
- Users and security data
- Establishments, groups, staff, and change requests
- Callback and extract metadata
- Scheduled job state
- Schema migrations

The integration is implemented through:

- Spring-managed JDBC data sources
- Hibernate ORM
- DAO classes
- Spring Batch job repository access
- Flyway-based schema migrations

## Main Configuration

### Hibernate and data source wiring

- `applicationContext-hibernate.xml`

This file defines:

- `dataSource`
- `standbyDataSource`
- `routingDataSource`
- `sessionFactory`
- `txManager`
- `masterDataSource`
- `flyway`

### JDBC driver

The project uses the Microsoft SQL Server JDBC driver:

- `com.microsoft.sqlserver:mssql-jdbc`

It also uses:

- Hibernate
- Envers
- a Flyway variant (`sv5-flyway`)

## Data Sources

The main runtime data source is:

- `dataSource`

There is also a standby/read alternative:

- `standbyDataSource`

Both are wrapped by:

- `routingDataSource`

using `EdubaseRoutingDataSource`

This means the application can route database access between:

- `MAIN`
- `STANDBY`

depending on runtime database selection logic.

## Connection Properties

The SQL Server connection is configured with properties such as:

- `db.driver`
- `db.url`
- `db.username`
- `db.password`
- pool sizing and validation settings

The database pool implementation is:

- `org.apache.tomcat.jdbc.pool.DataSource`

## Encryption

The service runs: `EXEC dbo.initEncryptor`

as `initSQL` when connections are initialized. 

Most of the schema is unencrypted, however staff record data is encrypted using the `StaffRecordEnc` class


## ORM and Persistence Layer

Hibernate is configured through:

- `sessionFactory`
- `HibernateTransactionManager`

The session factory scans domain entities under:

- `com.texunatech.edubase.domain`

The persistence layer is then exposed through many DAO classes under:

- `src/main/java/com/texunatech/edubase/dao`

Example DAO classes:

- `EstablishmentDao`
- `EstablishmentGroupDao`
- `UserDao`
- `CallbackDao`
- `InspectionUpdatesDao`

These DAOs are used throughout the service layer to read and write SQL Server data.

## Transactions

Transactions are managed with:

- Spring transaction annotations
- `HibernateTransactionManager`

This means service-layer methods can run as:

- read-only transactions
- read-write transactions
- explicitly propagated transactional workflows

## Auditing

The SQL Server integration also includes auditing through Hibernate Envers.

In `applicationContext-hibernate.xml` Envers listeners are registered for:

- post-insert
- post-update
- post-delete
- collection changes

This means certain entity changes are automatically audited into database-backed history.

## Batch and Job Repository

Spring Batch also uses SQL Server.

In `applicationContext-jobs.xml` the job repository is created with:

- `dataSource`
- `txManager`

SQL Server stores:

- Job execution metadata
- Batch state
- Job repository records

## Schema Migration and Data Changes

Schema migration is handled through Flyway in `applicationContext-hibernate.xml`.

Flyway:

- Runs automatically through `init-method="migrate"`
- Uses `masterDataSource`
- Stores migration state in the `ApplicationSchema` table
- Loads migrations from configured migration locations

Migration scripts live under:

- `src/main/resources/db/migration`

This directory contains versioned and repeatable SQL scripts across multiple release and sprint folders.

## Operational and Administrative Access

There is a separate: `masterDataSource`

Schema migration and certain administrative operations use different credentials from normal runtime application access.

