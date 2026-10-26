# GIAS Integration Catalogue

**Scope:** BAU current state. This document catalogues all known integrations between GIAS and external systems — both inbound (data received by GIAS) and outbound (data published by GIAS). It is a reference index, not a detailed technical specification.


## Integration Summary

| # | External system | Direction | Mechanism | Frequency | Status |
|---|---|---|---|---|---|
| 1 | UKRLP | Inbound | Java Quartz job → UKRLP web service | Nightly / monthly | Active |
| 2 | Companies House (batch) | Inbound | Java scheduled job → Companies House API | Scheduled | Active |
| 3 | Companies House (interactive) | Inbound | C# web front end → Companies House API | On demand | Active |
| 4 | ONS Postcode Directory | Inbound | SSIS / SQL stored procedure | Periodic | Active |
| 5 | DfE Sign-in (account sync) | Inbound | ADF pipeline → DfE Sign-in REST API | Daily | Active |
| 6 | Ofsted | Inbound | DataOps import staging | — | **Retired** |
| 7 | DfE School Census | Inbound | DataOps import staging | — | **Retired** |
| 8 | Public users — web | Outbound | HTTPS / HTML via Azure Front Door | On demand | Active |
| 9 | Public users — CSV downloads | Outbound | Scheduled extract → Azure Blob Storage | Scheduled | Active |
| 10 | Partner organisations | Outbound | SOAP / REST API | On demand | Active |
| 11 | DfE Sign-in / downstream provider systems | Outbound | SQL views (MasterProvider schema) | On demand / query | Active |

---

## Inbound Integrations

### 1. UKRLP — UKPRN Sync

| Property | Detail |
|---|---|
| External system | UK Register of Learning Providers (UKRLP) |
| Data exchanged | UKPRN values for establishments (matched by URN) and groups (matched by Companies House number) |
| Direction | Inbound |
| Protocol / mechanism | Java Quartz job (`UkprnUpdateJob`) calls UKRLP SOAP/web service |
| Frequency | Nightly for establishments without a UKPRN; monthly for change-check on existing UKPRNs |
| Trigger | Quartz scheduler in Java application |
| Target tables | `dbo.Establishment.UKPRN`, `dbo.EstablishmentGroup.UKPRN`, `dbo.EstablishmentChangeHistory`, `dbo.GroupChangeRequest`, `dbo.UkprnUpdateProcess` |
| Status | Active |
| Security / auth | Not confirmed in current evidence |
| Key notes | UKRLP is the authoritative issuer of UKPRN; GIAS is a primary verification source for compulsory-school-age providers. The relationship is reciprocal. UKPRN changes are written as applied change-history records (actor `edubase`). Welsh establishments and non-live statuses are excluded. |
| Further detail | `docs/data/ukprn-sync-data-flow.md` |

---

### 2. Companies House — Batch Import

| Property | Detail |
|---|---|
| External system | Companies House |
| Data exchanged | Company profiles for trust/group types: MAT (type_code 06), SAT (type_code 10), Sponsor (type_code 11) |
| Direction | Inbound |
| Protocol / mechanism | Java scheduled job (`CompaniesHouseUpdateJob`) calls Companies House API; populates `CompaniesHouseDownloadHolder`; creates `GroupChangeRequest` rows for differences |
| Frequency | Scheduled (cadence not confirmed in current evidence) |
| Trigger | Java scheduler |
| Target tables | `dbo.CompaniesHouseDownloadHolder`, `dbo.GroupChangeRequest`, `dbo.CompaniesHouseUpdates` |
| Status | Active (54,893 writes to `CompaniesHouseDownloadHolder`, 112 writes to `CompaniesHouseUpdates` in June 2026) |
| Security / auth | Not confirmed in current evidence |
| Key notes | The batch job does **not** directly update `EstablishmentGroup` records. It creates `GroupChangeRequest` rows that enter the governed change-request workflow. |
| Further detail | `docs/data/erd/imports/companies-house-and-master-provider-imports.md` |

---

### 3. Companies House — Interactive Lookup

| Property | Detail |
|---|---|
| External system | Companies House |
| Data exchanged | Company details for a single Companies House number (company name, status, address) |
| Direction | Inbound |
| Protocol / mechanism | C# web front end calls Companies House API on demand during group editing |
| Frequency | On demand (user-initiated) |
| Trigger | User action on group edit screen |
| Target tables | None — read-only lookup displayed in the user interface |
| Status | Active |
| Security / auth | Not confirmed in current evidence |
| Key notes | Read-only. Does not write to the database. Separate from the Java batch import (integration 2). |
| Further detail | `docs/data/erd/imports/companies-house-and-master-provider-imports.md` |

---

### 4. ONS Postcode Directory — Geography Import

| Property | Detail |
|---|---|
| External system | Office for National Statistics (ONS) — Postcode Directory |
| Data exchanged | Postcode-to-geography mappings (ward, LSOA, MSOA, parliamentary constituency, district, urban/rural) and administrative area code/name reference data |
| Direction | Inbound |
| Protocol / mechanism | Load to `DataOpsJobs.GeoData_Import` (SSIS package per procedure evidence); SQL stored procedure `DataOpsJobs.ONSPostcodeDirectory_Load` applies import |
| Frequency | Periodic (schedule not confirmed; operated manually or via SSIS) |
| Trigger | SSIS package / manual execution |
| Target tables | `DataOpsJobs.GeoData_Import` (staging), `dbo.GeoData` (current postcode geography), `dbo.Establishment` (geography fields), `DataOpsJobs.GeoData_AUD` (import audit), `dbo.EstablishmentChangeHistory` (via post-import procedure) |
| Status | Active (`GeoData_Import` reads and writes observed 16 June 2026) |
| Security / auth | Not applicable — file-based import |
| Key notes | The `DataOpsJobs.ONSPDURNExclusionList` table suppresses parliamentary constituency updates for specific establishments. The import can directly mutate establishment transactional geography fields, which has data-stewardship implications for the target architecture. |
| Further detail | `docs/data/erd/imports/geography-and-postcode-imports.md` |

---

### 5. DfE Sign-in — Disabled Account Sync

| Property | Detail |
|---|---|
| External system | DfE Sign-in (DfE identity provider) |
| Data exchanged | Disabled user account records (user identity, email, DSI role, organisation) |
| Direction | Inbound |
| Protocol / mechanism | Azure Data Factory pipeline (`gias_dsi_account_sync_prod`): REST copy activity → SQL staging → stored procedure |
| Frequency | Daily (scheduled trigger `GIAS_DSI_API_DailyExecution` at ~20:00 UK time) |
| Trigger | ADF scheduled trigger |
| Target tables | `DataOpsJobs.gias_dsi_account_sync` (raw JSON snapshot), `DataOpsJobs.gias_dsi_account_sync_Log` (per-user audit), `dbo.SystemUser` (`enabled` column set to `0`) |
| Status | Active (confirmed daily run history in May 2026 ADF investigation) |
| Security / auth | ADF linked service handles authentication to DSI REST API; no credentials in pipeline definition |
| Key notes | Disable-only: never re-enables or creates accounts. Matching is by email address (collation-dependent). Full audit trail at per-user level. REST ingestion retries up to six times. One-year retention enforced by stored procedure. |
| Further detail | `docs/infra/reference/ADF/adf-syncing-disabled-accounts-from-DSI.md` |

---

### 6. Ofsted — Inspection Data Import (Retired)

| Property | Detail |
|---|---|
| External system | Ofsted |
| Data exchanged | Inspection grades, inspection dates for state-funded schools, independent schools and FE providers |
| Direction | Inbound |
| Status | **Retired** — documented in ERD as no longer in use |
| Key notes | Staging tables (`DataOpsJobs.Ofsted_IndependentSchools`, `Ofsted_StateFunded`, `Ofsted_FurtherEducation`, `Ofsted_GIASProdExtract`) remain in the schema as historical evidence. The current mechanism for updating Ofsted fields on establishment records is not confirmed in existing documentation. |
| Further detail | `docs/data/erd/imports/ofsted-and-school-census-imports.md` |

---

### 7. DfE School Census — Pupil Data Import (Retired)

| Property | Detail |
|---|---|
| External system | DfE School Census |
| Data exchanged | Pupil count and census data for establishments |
| Direction | Inbound |
| Status | **Retired** — documented in ERD as no longer in use |
| Key notes | Staging tables (`DataOpsJobs.GIAS_SchoolCensus`, `GIAS_SchoolCensus_Updates`, `GIAS_IEBTSchoolCensus`, `GIAS_IEBTSchoolCensus_Updates`) remain in the schema as historical evidence. |
| Further detail | `docs/data/erd/imports/ofsted-and-school-census-imports.md` |

---

## Outbound Integrations

### 8. Public Users — Web Interface

| Property | Detail |
|---|---|
| External system / consumer | Public users, DfE staff, partner users with web access |
| Data exchanged | Establishment details, group details, governance data, search results, download portal |
| Direction | Outbound |
| Protocol / mechanism | HTTPS / HTML via Azure Front Door → C# web front end (`ea-edubase-prod`) → Java API (`ea-edubase-api-prod`) → SQL / Redis |
| Frequency | On demand |
| Status | Active |
| Security / auth | Azure Front Door with WAF for public ingress; DfE Sign-in OIDC for authenticated users |
| Key notes | The Java API (`ea-edubase-api-prod`) is currently publicly reachable without WAF or perimeter control — a noted security gap. The `gias_sharing` schema provides a denormalised read cache for establishment and group queries. |
| Further detail | `docs/infra/service-access-and-security.md` |

---

### 9. Public Users and Subscribers — Scheduled CSV Extracts

| Property | Detail |
|---|---|
| External system / consumer | Public users, data subscribers, downstream DfE and partner systems that consume CSV files |
| Data exchanged | Establishment, group, governance, links and group-links data as CSV files |
| Direction | Outbound |
| Protocol / mechanism | Quartz scheduler triggers extract jobs in Java application; output written to `extracts` blob container in `strgt1predubase` storage account; users download via the portal |
| Frequency | Scheduled (cadence per `ScheduledExtract.frequency` configuration) |
| Status | Active |
| Security / auth | Storage account access controlled by VNet and IP rules. Download via web portal subject to normal user authentication where applicable. |
| Known extract files | `edubaseall*` (all establishments), `allgroupsdata`, `alllinksdata`, `governance*`, `allgroupslinksdata` |
| Key notes | `ScheduledExtract.publicAccessible` controls whether an extract is publicly available or restricted to owning user group. Both open-data and subscriber-restricted extract types exist. |
| Further detail | `docs/data/erd/operations/scheduled-extracts-and-callbacks.md` |

---

### 10. Partner Organisations — SOAP / REST API

| Property | Detail |
|---|---|
| External system / consumer | Partner organisations and DfE internal systems with an API integration |
| Data exchanged | Establishment and group data via SOAP and REST endpoints |
| Direction | Outbound |
| Protocol / mechanism | SOAP / REST over HTTPS via `ea-edubase-backend-prod` (Java admin application) |
| Frequency | On demand |
| Status | Active |
| Security / auth | No WAF or perimeter control in front of `ea-edubase-backend-prod` — a noted security gap. Authentication mechanism for individual partner access not confirmed in current evidence. |
| Key notes | `ea-edubase-backend-prod` combines the admin/JSP interface and SOAP/REST partner endpoints on the same App Service, creating a mixed trust boundary. The identity of specific current partner consumers is not captured in existing documentation. |
| Further detail | `docs/infra/service-access-and-security.md` |

---

### 11. DfE Sign-in and Downstream Provider Systems — MasterProvider Extract

| Property | Detail |
|---|---|
| External system / consumer | DfE Sign-in; downstream DfE provider-registry consumers |
| Data exchanged | Provider-registry projection: establishment identity, UKPRN, status, and related provider fields from `MasterProvider.DSI_Provider_Extract` and `MasterProvider.DSI_Links_Provider_Extract` |
| Direction | Outbound |
| Protocol / mechanism | SQL views in the `MasterProvider` schema, read directly by downstream consumers |
| Frequency | On demand / query time |
| Status | Active |
| Security / auth | Database-level access control; not confirmed whether consumers read via an API layer or directly from SQL |
| Key notes | The MasterProvider schema is separate from the `gias_sharing` schema. It is shaped for provider-registry and identity-system consumption. Whether consumers read directly from SQL or via a data sync process is not confirmed in current evidence. |
| Further detail | `docs/data/erd/imports/companies-house-and-master-provider-imports.md` |

---

