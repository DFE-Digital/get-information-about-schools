
## C4 Container Diagram

Major components forming GIAS service, and how they interact with each other and external actors.

This is the container-level view of the system. It shows the major deployable/application building blocks and the main relationships between them, but it does not break the Java back end down into its internal Spring, persistence, extract, and integration components.



```mermaid
C4Container


    UpdateLayoutConfig($c4ShapeInRow="3", $c4BoundaryInRow="1")


    System_Ext(dsi, "DfE Sign-in (DSI", "Authentication and identity service")

    Person(user, "GIAS User", "Accesses GIAS through the web interface")
    
    System_Ext(externalSystems, "External Consumer", "Systems that download<br>GIAS datasets") 
    

    System_Boundary(gias, "Get Information About Schools (GIAS)") {
        
        Container(profileAPI, "Profile API", "C# API", "Supplies list of education providers")

        Container(web, "GIAS Web Front End", "C# ASP.NET Web Application", "Provides the user interface for <br>searching, viewing and managing GIAS data")
        
        Container(fileAPI, "File API", "Azure App Service", "Serves ZIP files from the extract file storage")

        ContainerDb(db, "GIAS Database", "MS SQL Server", "Stores establishment, governance,<br>user and reference data")

        Container(backend, "GIAS Backend", "Java Rest API Application + SOAP", "Executes business rules, validation and<br>data access logic")

        ContainerDb(fileStorage, "File Storage", "Azure File Storage", "Stores generated ZIP<br>extracts available for download")
    }

    Rel(user,web, "Browse GIAS data", "HTTPS/HTML")
    Rel(user, fileAPI, "Downloads files using", "HTTPS/CSV/ZIP")
    Rel(dsi,profileAPI , "Retrieves provider<br>information", "HTTPS")
    Rel(user, dsi, "Authenticates via", "OIDC / OAuth2")
    Rel(dsi, web, "Authenticates via", "OIDC / OAuth2")
    Rel(web, backend, "Calls", "HTTPS")
    Rel(profileAPI,db,  "Reads from", "SQL")
    Rel(backend, db, "Reads from and writes to", "TCP/SQL")
    Rel(backend, fileStorage, "Writes ZIP files to", "HTTPS/CSV/ZIP")
    Rel(fileAPI,fileStorage, "Reads ZIP files from", "HTTPS/CSV/ZIP")
    Rel(externalSystems, fileAPI,"Downloads GIAS data","HTTPS/CSV/ZIP")
    Rel(externalSystems, backend,"Retrieves GIAS data","HTTPS/SOAP")

    UpdateRelStyle(user, dsi, $offsetX="-48", $offsetY="-60") 
    UpdateRelStyle(dsi,profileAPI, $offsetX="-120", $offsetY="-100") 
    UpdateRelStyle(user,web, $offsetX="-90", $offsetY="-70") 
    UpdateRelStyle(user, fileAPI, $offsetX="-160", $offsetY="-60") 
    UpdateRelStyle(dsi, web, $offsetX="-140", $offsetY="-80") 
    UpdateRelStyle(api, db, $offsetX="-50", $offsetY="50") 
    UpdateRelStyle(externalSystems, fileAPI, $offsetX="-20", $offsetY="-60") 
    UpdateRelStyle(externalSystems, backend, $offsetX="-5", $offsetY="-200") 
    UpdateRelStyle(backend, db, $offsetX="-50", $offsetY="50") 
    UpdateRelStyle(backend, fileStorage, $offsetX="-50", $offsetY="50") 
    UpdateRelStyle(web, backend, $offsetX="-50", $offsetY="-20")

```

For that lower-level view, see [`back-end-component.md`](./back-end-component.md). This document explains how the `GIAS Backend` container is structured internally, including client-facing entry points, scheduled and batch processing, reference-data provider integrations, and the authentication flow used by the front end when it calls the back-end APIs.
