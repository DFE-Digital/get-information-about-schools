# C4 Component Diagram for the GIAS backend Java component



- the WAR-based Edubase application declared in [pom.xml](/C:/code/gias-dd-backend-from-zip/pom.xml)
- public and lookup APIs described in [Texuna Edubase API (functional).yaml](/C:/code/gias-dd-backend-from-zip/Texuna%20Edubase%20API%20(functional).yaml) and [Texuna Edubase Dictionary Lookups API.yaml](/C:/code/gias-dd-backend-from-zip/Texuna%20Edubase%20Dictionary%20Lookups%20API.yaml)
- OLAP analysis servlets in [src/main/java/com/texunatech/edubase/analysis/servlet/AnalysisEngineSchemaServlet.java](/C:/code/gias-dd-backend-from-zip/src/main/java/com/texunatech/edubase/analysis/servlet/AnalysisEngineSchemaServlet.java)
- persistence and SQL Server integration in [src/main/java/com/texunatech/edubase/dao](/C:/code/gias-dd-backend-from-zip/src/main/java/com/texunatech/edubase/dao) and [src/main/java/com/texunatech/edubase/dao/hibernate/EdubaseSQLServerDialect.java](/C:/code/gias-dd-backend-from-zip/src/main/java/com/texunatech/edubase/dao/hibernate/EdubaseSQLServerDialect.java)
- batch/import assets in [jobs](/C:/code/gias-dd-backend-from-zip/jobs), [addressLayer](/C:/code/gias-dd-backend-from-zip/addressLayer), and [sql](/C:/code/gias-dd-backend-from-zip/sql)


Questions

- What are th Spring Batch jobs, is this still used ? ie `/jobs`
- How/When do we run the .php scripts in /scripts
- How do operational users authenticate with the Web/MV layer 
- Where is the permissions engine, ABAC and RBAC
- What is the address layer import
- Why do we have 2 APIs, Texuna Edubase API (functional).yaml and Texuna Edubase Dictionary Lookups API.yaml. Do we have 2 distinct use cases ? eg internal, external ?
- What is the Analysis / OLAP engine, src/main/java/com/texunatech/edubase/analysis/mdx/MDXQueryBuilder.java
- When are all the jobs ran under src/main/java/com/texunatech/edubase/service/quartz/
- What is the role of flyway, its seems to have an operational role
  - are the old flyway scripts in sql_archive, what is the process to running flyway
  - what is done through flyway, what is done through the front end support section of the tools area
- When do we run sql/Db maintenance sql scripts
- When do we run sql/snapshot_tests 



```mermaid
C4Component
title GIAS Backend - C4 Component Diagram

UpdateLayoutConfig($c4ShapeInRow="4", $c4BoundaryInRow="1")

System_Ext(dsi, "DfE Sign-in (DSI", "Authentication and identity service")


Person(ops_user, "Operational User", "Runs deployments, DB patches,<br>and batch processes")


System_Ext(ref_data, "Reference Data Providers", "Companies House, Ofsted, SAFE/SA,<br>Address Layer, other upstream feeds")

Container_Ext(giasFE, "GIAS Front End Web Application", "C#/ASP.Net")

Container_Boundary(edubase, "Edubase Java Application") {


    Component(auth, "Authentication & Security", "Spring Security + SAML", "SAFE/SA SSO, role-based access control, REST basic auth")

    Component(web_mvc, "Web MVC Controllers", "Spring MVC", "Back-office UI, establishment/group/staff workflows search,<br> content and admin screens")

   Component(flyway, "Flyway DB Migration Scripts", "Flyway + T-SQL", "Versioned SQL migrations used by operations through the<br> deployment/startup process to<br> maintain schema, database configuration, rules, reference data,<br> and corrective data updates")

    Component(rest_api, "REST API Controllers", "Spring MVC REST", "REST endpoints for establishments, groups, users,<br> downloads, lookups and approvals")

    Component(domain_services, "Domain Services", "Spring Services", "Core business logic for establishments, groups, staff, users, validation, approvals and reporting")

    Component(search_lookup, "Search & Lookup Services", "Search/Dictionary Services", "Filtering, lookup dictionaries, data dictionary, geo and query support")

    Component(extracts, "Extract & Download Services", "Extract Managers/Renderers", "Generates CSV/XLS/XLSX/XML extracts, scheduled downloads and callbacks")

    Component(batch_jobs, "Batch & Scheduled Jobs", "Spring Batch + Quartz", "Bulk update/create, scheduled extracts, sync and maintenance jobs")

    Component(integrations, "Integration Adapters", "Integration/WS Clients", "Adapters for Companies House, Ofsted/SAFE sync, CRM and reference-data ingestion")

    Component(persistence, "Persistence Layer", "DAO + Hibernate/JDBC", "Reads and writes application, audit, lookup, callback and job state data")
}



Container_Boundary(managedServices, "Managed Services") {
  ContainerDb_Ext(sql_server, "GIAS Data database", "MS SQL Server")
  Container_Ext(object_store, "Object Storage, stores data extracts", "Azure Blob storage")
}





Rel(giasFE, rest_api, "Uses<br>", "HTTPS/JSON/<br>Basic Auth")




Rel(ops_user, dsi, "Authenticates via", "SAML")
Rel(ops_user, web_mvc, "Operates through admin and back-office screens", "HTTPS")
Rel(ops_user, flyway, "Operates via deploymet<br>pipeline")
Rel(dsi, auth, "Authenticates via", "SAML")

Rel(web_mvc, auth, "Authenticates and authorises via")
Rel(rest_api, auth, "Authorises via")
Rel(web_mvc, domain_services, "Invokes")
Rel(rest_api, domain_services, "Invokes")

```

## GIAS front end authentication flow
```mermaid
sequenceDiagram
    autonumber
    actor authUser as Authenticated User
    participant FE as GIAS CSharp Web App
    participant API as Edubase REST API
    participant Auth as Auth/Security Layer
    participant UM as UserManager
    participant RBAC as Domain Services / RBAC
    participant DB as GIAS DB

    authUser->>FE: Use UI action
    FE->>API: HTTPS request + Basic Auth + sa_user_id header
    API->>Auth: Validate API client credentials
    Auth-->>API: API client authenticated

    API->>API: RESTLoginFilter reads sa_user_id
    API->>UM: loadUserBySAUserId(sa_user_id, true)
    UM->>DB: Lookup internal user and group/authorities
    DB-->>UM: User, roles, authorities
    UM-->>API: Internal user

    API->>Auth: setAuthentication(user)
    Auth-->>API: Current request security context established

    API->>RBAC: Execute endpoint/business operation
    RBAC->>Auth: getCurrentUser()/hasAuthority(...)
    Auth-->>RBAC: Current user and authorities
    RBAC->>DB: Read/write permitted data
    DB-->>RBAC: Result
    RBAC-->>API: Response payload
    API-->>FE: HTTPS/JSON response
    FE-->>Ops: Render result
```