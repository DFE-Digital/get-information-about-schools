# GIAS Data Stores Catalogue

**Scope:** Production data stores. Non-production environments (Dev, Test, Pre-Prod) follow the same pattern with equivalent resources in their respective subscriptions and resource groups.

**Related resources:** [production deployment architecture](../../deployment-architecture.md), [S158 Data Factory setup](../../data-factory-setup.md), [data architecture overview](overview.md), [GIAS DDL script](../gias-ddl-script.sql), [entity-relationship diagram portfolio](../entity-relationship-diagrams/README.md), [SQL Server database component](../../../service/back-end-component/database/sql-server.md), [Flyway migrations](../../../service/back-end-component/database/flyway-migrations.md), [Azure Blob Storage component](../../../service/back-end-component/storage/azure-blob-storage.md).

---

## Summary — Production Data Stores

| Store | Azure resource name | Type | Subscription | Purpose |
|---|---|---|---|---|
| Primary application database | `ea-edubase-prod` | Azure SQL Database | DFE T1 Production | Authoritative data store for all GIAS application data |
| Read replica database | `ea-edubase-prod` (replica) | Azure SQL Database (replica) | DFE T1 Production | Read replica of the primary application database |
| Archive database | `ea-edubase-prod-archive` | Azure SQL Database | DFE T1 Production | Archived / historic data (paused at time of investigation) |
| Front-end Redis cache | `ea-edubase-prod` | Azure Cache for Redis (Premium) | DFE T1 Production | Session and read cache for the C# web front end |
| API Redis cache | `rg-t1pr-edubase-redis-api` | Azure Cache for Redis (Premium) | DFE T1 Production | Java API and backend application read cache |
| Application storage | `edubasepr` | Azure Storage Account (RA-GRS) | DFE T1 Production | Front-end content, table storage, WebJobs, application logging |
| Extract file store | `strgt1predubase` | Azure Storage Account (LRS) | DFE T1 Production | Scheduled CSV extract output files |
| Diagnostics storage | `strgt1prgiasdiagnostics` | Azure Storage Account (LRS, Cool) | DFE T1 Production | Infrastructure and application diagnostics |
| ADF staging / pipeline | `s158p01-df-gias-01` | Azure Data Factory | s158 Production | Batch data integration: Companies House, DSI sync, archive pipeline |

---

## SQL Databases

GIAS uses Azure SQL Database (PaaS) hosted on two SQL logical servers in the `rg-t1pr-edubase` resource group in the DFE T1 Production subscription.

### Primary Application Database

| Property | Value |
|---|---|
| Database name | `ea-edubase-prod` |
| SQL logical server | `ea-edubase-prod-srv` |
| Hostname | `ea-edubase-prod-srv.database.windows.net` |
| Subscription | DFE T1 Production |
| Resource group | `rg-t1pr-edubase` |
| Access | Private endpoints only. SQL databases are not directly reachable from the public internet. The S158 Data Factory connects via a managed private endpoint (`GiasConnection-Managedvnetprod`). |

**Contents.** This is the single authoritative data store for the GIAS application. It holds all domains described in the data architecture overview:

- Establishment and group registry data (`dbo` schema)
- Change history, audit and workflow tables (`dbo` schema)
- Users, user groups and permissions
- Reference and classification data (establishment types, LA codes, geography, code lists)
- Import and staging tables (`DataOpsJobs` schema)
- Sharing and publication cache (`gias_sharing` schema)
- Front-end content and operational tables (`FrontEnd` schema)
- Provider extract projection (`MasterProvider` schema)
- Scheduler runtime tables (Quartz `QRTZ_*`, Spring Batch `BATCH_*`)

See the [GIAS DDL script](../gias-ddl-script.sql) for the published schema extract and the [entity-relationship diagram portfolio](../entity-relationship-diagrams/README.md) for the published model views.

### Read Replica Database

| Property | Value |
|---|---|
| Database name | `ea-edubase-prod` (replica) |
| SQL logical server | `ea-edubase-prod-rep-srv` |
| Hostname | `ea-edubase-prod-rep-srv.database.windows.net` |
| Subscription | DFE T1 Production |
| Resource group | `rg-t1pr-edubase` |

The same database name appears under both the primary and replica SQL logical servers in the Resource Graph export. The replica server (`ea-edubase-prod-rep-srv`) is a secondary logical server, likely supporting geo-replication or read scale-out. Full replication and failover configuration is not yet captured.

### Archive Database

| Property | Value |
|---|---|
| Database name | `ea-edubase-prod-archive` |
| SQL logical server | `ea-edubase-prod-srv` |
| Hostname | `ea-edubase-prod-srv.database.windows.net` |
| Subscription | DFE T1 Production |
| Resource group | `rg-t1pr-edubase` |
| Status at investigation | Paused |

The archive database is a separate Azure SQL database on the same logical server as the primary. It receives data via an ADF pipeline (`s158p01-df-gias-01`), which connects to it via a second linked service (`GIAS_ArchiveSQL_AsAzureDB`). The archive database was in a paused state at the time of investigation; its paused status may affect whether archive-linked ADF pipelines can run successfully in production.

The archive database is covered in the [S158 Data Factory setup](../../data-factory-setup.md) and the [production deployment architecture](../../deployment-architecture.md).

---

## Redis Cache

Two Azure Cache for Redis instances are in production, both Premium tier in `rg-t1pr-edubase`. They serve as read caches for the Java application tier, reducing database load for frequently accessed establishment and group data.

### Front-End Redis Cache

| Property | Value |
|---|---|
| Resource name | `ea-edubase-prod` |
| Hostname | `ea-edubase-prod.redis.cache.windows.net` |
| Tier | Premium |
| Subscription | DFE T1 Production |
| Resource group | `rg-t1pr-edubase` |
| Confirmed usage | Assumed C# front-end cache (inferred from name and separation from the API cache) |

The C# front-end codebase references a Redis connection string, but the checked-in configuration value is blank. `ea-edubase-prod` is recorded as the likely front-end Redis instance by naming convention and its separation from the Java API Redis cache below. This should be confirmed against deployed application configuration.

### API Redis Cache

| Property | Value |
|---|---|
| Resource name | `rg-t1pr-edubase-redis-api` |
| Hostname | `rg-t1pr-edubase-redis-api.redis.cache.windows.net` |
| Tier | Premium |
| Subscription | DFE T1 Production |
| Resource group | `rg-t1pr-edubase` |
| Confirmed usage | Java API application and Java admin/backend application cache (confirmed from Java application configuration) |

The Java API application explicitly references `rg-t1pr-edubase-redis-api.redis.cache.windows.net` in its production configuration. This instance caches establishment and group data read by the API tier, supporting search results and frequently accessed establishment details. See the [production deployment architecture](../../deployment-architecture.md) for the wider production hosting view.

The `gias_sharing` schema in the SQL database provides denormalised read projections; the Redis cache provides a faster in-memory layer above this.

---

## Storage Accounts

Three storage accounts are in the `rg-t1pr-edubase` production resource group. They serve distinct purposes.

### Application Storage

| Property | Value |
|---|---|
| Resource name | `edubasepr` |
| Subscription | DFE T1 Production |
| Resource group | `rg-t1pr-edubase` |
| Replication | Read-access geo-redundant storage (RA-GRS) |
| Access tier | Standard |
| Public network access | Enabled from selected virtual networks and IP addresses (`vnet2-t1pr` plus six `208.127.*` IP ranges). No private endpoint connections. |

**Contents.**

| Storage type | What it holds |
|---|---|
| Blob containers (8, all private) | `content`, `guidance` — application content (documents, reports); Azure WebJobs containers; logging containers |
| Table storage | API recorder sessions, front-end log messages, FAQ items, glossary items, local authority sets, news articles, notification banners, notification templates, user preference tokens |
| File shares | None |
| Queues | None |

This storage account is the runtime data store for the `FrontEnd.*` schema equivalents served to the C# application: content management, notifications, FAQs and user preferences. Its RA-GRS replication provides geographic redundancy for this application data. See the [Azure Blob Storage component](../../../service/back-end-component/storage/azure-blob-storage.md) for how blob-backed files are used by the application.

**Data protection.** Blob soft delete: 7 days. Container soft delete: 7 days. No point-in-time restore, blob versioning, or blob change feed enabled.

### Extract File Store

| Property | Value |
|---|---|
| Resource name | `strgt1predubase` |
| Subscription | DFE T1 Production |
| Resource group | `rg-t1pr-edubase` |
| Replication | Locally-redundant storage (LRS) |
| Access tier | Standard / Hot |
| Public network access | Enabled from selected virtual networks and IP addresses (`vnet2-t1pr` plus IP rules). No private endpoint connections. |
| Defender for Storage | Classic plan enabled. Recommendations outstanding: private link, stricter VNet access, disallow public access, prevent shared key access. |

**Contents.**

| Storage type | What it holds |
|---|---|
| Blob containers (2) | `extracts` — scheduled CSV extract output files; `$logs` — storage operation logs |
| File shares | None |
| Queues | None |
| Tables | None |

This storage account holds the scheduled extract output files. Scheduled extract jobs write CSV files to the `extracts` container; these are then available for download by public users and downstream consumers. The extract files include establishment extracts (`edubaseall*`), group data (`allgroupsdata`), governance data (`governance*`), links (`alllinksdata`) and group links (`allgroupslinksdata`).

**Data protection.** Blob soft delete: 7 days. Container soft delete: 7 days. No point-in-time restore or blob versioning.

### Diagnostics Storage

| Property | Value |
|---|---|
| Resource name | `strgt1prgiasdiagnostics` |
| Subscription | DFE T1 Production |
| Resource group | `rg-t1pr-edubase` |
| Replication | Locally-redundant storage (LRS) |
| Access tier | Standard / Cool |

Diagnostics storage for infrastructure and application diagnostics. Not an application data store.

---

## Azure Data Factory

| Property | Value |
|---|---|
| Resource name | `s158p01-df-gias-01` |
| Subscription | s158-getinformationaboutschools-production |
| Resource group | `s158p01-rg-dd-adf` |
| Tenant | DfE Platform Identity |
| Connectivity | Managed Virtual Network with private endpoints to the DFE T1 SQL server |

**Purpose.** ADF provides the batch data integration layer for GIAS. It does not hold application data itself; it reads from and writes to the SQL databases.

**Confirmed pipelines.** Five pipelines were visible at the time of investigation. See the [S158 Data Factory setup](../../data-factory-setup.md) for the published ADF detail.

| Pipeline | Direction | What it processes |
|---|---|---|
| Archive pipeline | Primary DB → Archive DB | Moves aged data from `ea-edubase-prod` to `ea-edubase-prod-archive` |
| DSI disabled-account sync | DfE Sign-in → Primary DB | Synchronises disabled user account status from DfE Sign-in into GIAS system user records |
| Companies House | Companies House → Primary DB | Imports Companies House company data for group/trust matching |
| (Further pipelines) | Inbound | See the [S158 Data Factory setup](../../data-factory-setup.md) for the full pipeline inventory |

**Connectivity.** ADF connects to the SQL databases via a single managed private endpoint (`GiasConnection-Managedvnetprod`) using two linked services: `GIAS_ProdSQL_AsAzureDB` (primary database) and `GIAS_ArchiveSQL_AsAzureDB` (archive database). It does not use public internet connectivity for SQL access.

---

## Non-Production Environments

Each non-production environment has equivalent data stores following the same pattern. The table below shows the production Azure resource name and its non-production equivalents.

| Store type | Production | Pre-Prod | Test | Dev |
|---|---|---|---|---|
| SQL primary DB | `ea-edubase-prod` on `ea-edubase-prod-srv` | `ea-edubase-pp` on `ea-edubase-pp-sqlsrv` | `t1te-edubase` on `sqlsrv-t1te-edubase` | `t1dv-edubase` on `sqlsrv-t1dv-edubase` |
| Redis cache (FE) | `ea-edubase-prod` | `ea-edubase-pp` | `gias-stage` | `gias-dev` |
| Redis cache (API) | `rg-t1pr-edubase-redis-api` | `rg-t1pp-edubase-redis-api` | `rc-t1te-edubase` | `rc-t1dv-edubase` |

S158 subscription resources (Dev and Test) also have equivalent SQL databases and Redis caches in the DfE Platform Identity tenant. See the [production deployment architecture](../../deployment-architecture.md) for the published infrastructure view.


