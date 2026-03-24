# GIAS Backend Component Diagram



- the WAR-based Edubase application declared in [pom.xml](/C:/code/gias-dd-backend-from-zip/pom.xml)
- public and lookup APIs described in [Texuna Edubase API (functional).yaml](/C:/code/gias-dd-backend-from-zip/Texuna%20Edubase%20API%20(functional).yaml) and [Texuna Edubase Dictionary Lookups API.yaml](/C:/code/gias-dd-backend-from-zip/Texuna%20Edubase%20Dictionary%20Lookups%20API.yaml)
- OLAP analysis servlets in [src/main/java/com/texunatech/edubase/analysis/servlet/AnalysisEngineSchemaServlet.java](/C:/code/gias-dd-backend-from-zip/src/main/java/com/texunatech/edubase/analysis/servlet/AnalysisEngineSchemaServlet.java)
- persistence and SQL Server integration in [src/main/java/com/texunatech/edubase/dao](/C:/code/gias-dd-backend-from-zip/src/main/java/com/texunatech/edubase/dao) and [src/main/java/com/texunatech/edubase/dao/hibernate/EdubaseSQLServerDialect.java](/C:/code/gias-dd-backend-from-zip/src/main/java/com/texunatech/edubase/dao/hibernate/EdubaseSQLServerDialect.java)
- batch/import assets in [jobs](/C:/code/gias-dd-backend-from-zip/jobs), [addressLayer](/C:/code/gias-dd-backend-from-zip/addressLayer), and [sql](/C:/code/gias-dd-backend-from-zip/sql)


```mermaid
C4Component
title GIAS Backend - C4 Component Diagram

Person(api_user, "API Consumer", "Public or partner system using JSON APIs")
Person(ops_user, "Operations User", "Runs deployments, DB patches, and batch processes")

System_Ext(ref_data, "Reference Data Providers", "Companies House, Ofsted, SAFE/SA, Address Layer, other upstream feeds")
System_Ext(object_store, "Object Storage", "Azure Blob storage or Amazon S3 for callback/export files")
SystemDb_Ext(sql_server, "Edubase SQL Server", "Primary operational database")

Container_Boundary(edubase, "Edubase WAR Application") {
  Component(api_surface, "API Surface", "Servlet/JSP + JSON endpoints", "Functional and dictionary lookup endpoints exposed under /v1")
  Component(analysis_engine, "Analysis / OLAP Engine", "Servlets + MDX builders", "Builds report queries and serves OLAP schema/data")
  Component(registry_domain, "Registry Domain Model", "Domain layer", "Establishments, groups, users, documents, filters, dictionaries, change requests")
  Component(workflow_jobs, "Workflow and Scheduled Processing", "Domain + DAO services", "Callbacks, scheduled extracts, reminders, inspection and Companies House update processing")
  Component(persistence, "Persistence Layer", "DAO / Hibernate / JDBC", "Repository access, SQL construction, and database-specific dialect")
  Component(storage_adapter, "Export and Callback Storage Adapter", "Storage integration", "Stores generated callback/export payloads using local disk or object storage")
}

Container_Boundary(batch_tools, "Batch and Import Tooling") {
  Component(address_import, "Address Layer Import", "Standalone Java + SQL", "Imports and normalizes address-layer source files")
  Component(db_patch_jobs, "DB Patch and Bootstrap Jobs", "Ant / SQL / shell", "Applies schema/data patches, bootstraps WAR deployments, precompiles JSPs")
}

Rel(api_user, api_surface, "Queries APIs and lookup endpoints", "HTTPS/JSON")
Rel(api_surface, registry_domain, "Invokes business rules and search/filter logic")
Rel(api_surface, workflow_jobs, "Starts exports, callbacks, and scheduled processes")
Rel(api_surface, persistence, "Reads and writes registry data")

Rel(analysis_engine, registry_domain, "Uses dataset metadata and report criteria")
Rel(analysis_engine, persistence, "Loads OLAP/report data")

Rel(workflow_jobs, registry_domain, "Uses callback, extract, reminder, and update models")
Rel(workflow_jobs, persistence, "Persists job state and operational records")
Rel(workflow_jobs, storage_adapter, "Writes generated files")
Rel(workflow_jobs, ref_data, "Imports or synchronizes external updates")

Rel(storage_adapter, object_store, "Stores callback/export artifacts")
Rel(persistence, sql_server, "Reads and writes", "Hibernate/JDBC")

Rel(ops_user, db_patch_jobs, "Runs deployment and maintenance tooling")
Rel(db_patch_jobs, sql_server, "Applies DB patches and restores")
Rel(db_patch_jobs, api_surface, "Deploys packaged WAR")
Rel(address_import, ref_data, "Consumes address-layer source files")
Rel(address_import, sql_server, "Loads normalized address data")
```
