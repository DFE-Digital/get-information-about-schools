# C4 Container Diagram

Major components forming GIAS service, and how they interact with each other and external actors.

This is the container-level view of the system. It shows the major deployable/application building blocks and the main relationships between them, but it does not break the Java back end down into its internal Spring, persistence, extract, and integration components.



```mermaid
C4Container

    UpdateLayoutConfig($c4ShapeInRow="2", $c4BoundaryInRow="2")

    Person(user, "GIAS User", "Accesses GIAS through the web interface")

    Person(admin, "GIAS Admin", "Uses the JSP administration interface")
        
    System_Ext(externalSystems, "External Consumer", "Systems that download GIAS datasets") 


    System_Boundary(gias, "Get Information about Schools (GIAS)") {
        
        Container(web, "GIAS Web Front End", "C# ASP.NET Web Application", "Provides the user interface for searching, viewing and<br> managing GIAS data")

        Container(adminSoapBackend, "GIAS Admin and SOAP Application", "Java Application with JSP and SOAP", "Back-end deployment with admin JSP and SOAP capabilities.")

        Container(apiBackend, "GIAS API Application", "Java REST API Application", "Back-end deployment without the JSP layer.<br>Used by the C# front end.")
        
        ContainerDb(fileStorage, "File Store", "Azure Storage", "Stores generated ZIP<br>extracts available for download")
        
        Container(providerProfileApi, "Provider Profile API", "Azure Function App", "S158 provider API exposing provider lookup functions.")

        ContainerDb(db, "GIAS Database", "MS SQL Server", "Stores establishment, governance, user<br> and reference data")
        
        
        Container(dataFactory, "GIAS Data Factory", "Azure Data Factory", "S158 data integration workload for GIAS SQL<br> processing and archive activity.")
        
        Container(redundantGiasApi, "GIAS API", "Azure Function App", "S158 GIAS API Function App.<br>Not currently used.")
    }

    Rel(user,web, "Browse GIAS data", "HTTPS/HTML")
    Rel(admin, adminSoapBackend, "Administers GIAS data using JSP interface", "HTTPS/HTML")

    Rel(web, apiBackend, "Calls", "HTTPS")
    Rel(web, fileStorage, "Reads generated extracts from", "HTTPS/CSV/ZIP")
    Rel(apiBackend, db, "Reads from and writes to", "TCP/SQL")
    Rel(apiBackend, fileStorage, "Uses extract storage", "HTTPS/CSV/ZIP")
    Rel(adminSoapBackend, db, "Reads from and writes to", "TCP/SQL")
    Rel(adminSoapBackend, fileStorage, "Uses extract storage", "HTTPS/CSV/ZIP")
    Rel(providerProfileApi, db, "Reads provider data from", "SQL over private endpoint")
    Rel(redundantGiasApi, db, "Associated SQL path; active use not evidenced", "SQL over private endpoint")
    Rel(dataFactory, db, "Reads from and writes to", "SQL over private endpoint")

    Rel(externalSystems, web,"Downloads GIAS data","HTTPS/CSV/ZIP")
    Rel(externalSystems, adminSoapBackend,"Retrieves GIAS data","HTTPS/SOAP")

    UpdateRelStyle(user, dsi, $offsetX="-48", $offsetY="-60") 
    UpdateRelStyle(user,web, $offsetX="-90", $offsetY="-70") 
    UpdateRelStyle(admin, adminSoapBackend, $offsetX="-60", $offsetY="-80") 
    UpdateRelStyle(dsi, web, $offsetX="-140", $offsetY="-80") 
    UpdateRelStyle(externalSystems, web, $offsetX="-20", $offsetY="-60") 
    UpdateRelStyle(externalSystems, adminSoapBackend, $offsetX="-5", $offsetY="-200") 
    UpdateRelStyle(apiBackend, db, $offsetX="-50", $offsetY="50") 
    UpdateRelStyle(adminSoapBackend, db, $offsetX="-50", $offsetY="50") 
    UpdateRelStyle(apiBackend, fileStorage, $offsetX="-50", $offsetY="50") 
    UpdateRelStyle(adminSoapBackend, fileStorage, $offsetX="-50", $offsetY="50") 
    UpdateRelStyle(web, apiBackend, $offsetX="-50", $offsetY="-20")

```

For lower-level Java back-end detail, see [the back-end component diagrams](../back-end-component/component/). That documentation explains the internal structure behind the Java API and admin/SOAP application capabilities, including client-facing entry points, scheduled and batch processing, reference-data provider integrations, and the authentication flow used by the front end when it calls the back-end APIs.
