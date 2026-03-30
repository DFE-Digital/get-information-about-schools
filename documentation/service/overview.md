# Service Overview

> Get Information About Schools (GIAS) is the Department for Education’s official register of educational establishments in England. It provides a single, authoritative source of information used by schools, trusts, local authorities, government partners, and the public
>
> GIAS is also the National Database of Governors, holding governance information for state‑funded schools and academy trusts as required by legislation

## C4 System Context Diagram


### System context diagram for the Get Information About Schools Service
```mermaid
C4Context
 
  Person(anonUser, "Anonymous User", "Accesses publicly available establishment<br> and governor data")
  Person(authUser, "Signed-in User", "Accesses additional features<br>and data based on permissions")
  Person(backOfficeUser, "DfE Back Office User", "Maintains and manages GIAS data")

  System_Ext(externalSystems, "External Consumer", "Systems that download<br>GIAS datasets") 

  Enterprise_Boundary(govuk, "GovUK") {

    System_Boundary(otherGov, "UK Government Services") {
      System_Ext(Notify, "Gov UK<br>Notify", "Mailer system for<br>Email communications")

      System_Ext(companiesHouse, "Companies House", "UK register of companies")
      System_Ext(ofsted, "Ofsted", "Inspection ratings and reports<br>for education providers")
      System_Ext(hmrc, "HM Revenue and Customs", "UK tax authority")
    }

      Enterprise_Boundary(dfe, "Department for Education") {

      System_Ext(DfESignIn, "DfE Sign In (DSI)", "User authentication system.Consumes GIAS data to <br>provide list of establishments on sign in")


      System(GIAS, "Get Information About Schools", "Master information system for Establishment and<br>Governors data")
      
      System_Ext(schoolCensus,"School census","DfE statutory data set.<br>SPC (Schools, Pupils and their Characteristics).<br>SLASC (School Level Annual School Census) ")

      System_Ext(ukrlp, "UK Register of Learning<br>Providers (UKRLP)", "National register of learning providers providing<br>UKPRN identifiers and organisation data")

      System_Ext(internal, "Internal DfE systems","Collection of systems that ingest GIAS data")

    }

  }

  System_Boundary(external, "External Data Services") {
    System_Ext(ordnanceSurvey, "Ordnance Survey", "UK address lookup servce")
    System_Ext(azureMaps, "Azure Maps", "Used to render maps and display<br>establishment locations")
    System_Ext(ons, "Office for National Statistics", "Official statistics and geographic<br>reference data")
  }

  UpdateLayoutConfig($c4ShapeInRow="4", $c4BoundaryInRow="2")

  Rel(anonUser, GIAS, "Views and downloads data", "HTTPS/HTML & CSV")
  BiRel(authUser, GIAS, "Views, updates and downloads<br>permitted data", "HTTPS/HTML & CSV")
  Rel(backOfficeUser, GIAS, "Administers data", "HTTPS/HTML")
  Rel(externalSystems, GIAS, "Downloads establishment and governor data", "HTTPS/XML/CSV<br>using Basic Auth")
  
  Rel(authUser, DfESignIn, "Signs in using")
  Rel(backOfficeUser, DfESignIn, "Signs in using")

  BiRel(DfESignIn, GIAS, "Authenticate users.<br>Consumes provider data", "HTTPS/JSON")

  Rel(schoolCensus, GIAS, "Supplies pupil-level and<br>school-level statistics", "Manual/DB script")
  Rel(ukrlp, GIAS, "Supplies provider identity data", "HTTPS/SOAP/XML")

  Rel(GIAS, Notify, "Submits email messages<br>for delivery", "HTTPS/JSON")

  Rel(ordnanceSurvey, GIAS, "Search address<br>data by postcode", "HTTPS/JSON")
  Rel(azureMaps,GIAS, "Retrieves map tiles<br>and location search results", "HTTPS/JSON")
  Rel(ons,GIAS,"Supplies ONS codes for LA,<br>Region and Constituency","Manual/CSV")


  Rel(companiesHouse, GIAS,"Company data by<br>Companies House Number", "HTTPS/JSON")
  Rel(ofsted,GIAS,"Supplies school inspection ratings<br>and links to inspection reports","Manual/XLSX")
  Rel(GIAS, hmrc, "Extract of establishment data<br>for childcare providers (CCPs)", "Manual/CSV")

  Rel(GIAS, internal, "Ingests GIAS data", "HTTPS/CSV")

  UpdateRelStyle(anonUser, GIAS, $offsetX="-320", $offsetY="-190")
  UpdateRelStyle(authUser, GIAS, $offsetX="-230", $offsetY="-190")
  UpdateRelStyle(backOfficeUser, GIAS, $offsetX="70", $offsetY="-190")
  UpdateRelStyle(externalSystems, GIAS, $offsetX="190", $offsetY="-200") 

  UpdateRelStyle(authUser, DfESignIn, $offsetX="-40", $offsetY="-50") 
  UpdateRelStyle(backOfficeUser, DfESignIn, $offsetX="-50", $offsetY="-50") 

  UpdateRelStyle(DfESignIn, GIAS, $offsetX="-80", $offsetY="-20") 
  UpdateRelStyle(schoolCensus, GIAS, $offsetX="10", $offsetY="0") 
  UpdateRelStyle(ukrlp, GIAS, $offsetX="-80", $offsetY="100") 
  UpdateRelStyle(GIAS, Notify, $offsetX="-200", $offsetY="-10") 

  UpdateRelStyle(ordnanceSurvey, GIAS, $offsetX="120", $offsetY="-50") 
  UpdateRelStyle(azureMaps,GIAS, $offsetX="-50", $offsetY="-30") 
  UpdateRelStyle(ons, GIAS, $offsetX="-60", $offsetY="30") 

  UpdateRelStyle(GIAS, hmrc, $offsetX="-300", $offsetY="120") 

  UpdateRelStyle(companiesHouse, GIAS, $offsetX="-75", $offsetY="-30") 
  UpdateRelStyle(ofsted, GIAS, $offsetX="-150", $offsetY="10") 

  UpdateRelStyle(GIAS, internal, $offsetX="0", $offsetY="230") 


```

**Note** To keep the main context diagram easy to read, all DfE internal systems have been grouped into a single external system called "Internal DfE systems"


A second diagram has been created below to show these internal systems in more detail and how they relate to each other.

We do not yet know all the internal systems that interact with GIAS. This list will grow over time as more systems and use cases are identified.

3 mechanisms
 - edubase/downloads/File.xhtm?id=?
 - /edubase/downloads/public (direct file)
 - /edubase/service (SOAP)


CRM clients
 - capital
 - provider
 - IEBT

### System context diagram showing the internal DfE systems
```mermaid
C4Context 

Enterprise_Boundary(dfe, "Department for Education") {

    UpdateLayoutConfig($c4ShapeInRow="3", $c4BoundaryInRow="2")

    System_Ext(anm, "Analysis and Modelling Platform (AnM)", "Education & Skills Funding Agency (ESFA)<br>Data Catalogue")
      
    System_Ext(tv, "Teaching Vacancies", "Publishes and manages school job vacancies <br>and applications")

    System_Ext("ptt", "Publish Teacher Training","Publishes teacher training course informatio<br>for providers")

    System_Ext(ecf, "Early Career Framework", "Teacher training and development programme")

    System(GIAS, "Get Information About Schools", "Master information system for Establishment and<br>Governors data")

    System_Ext(capt, "Claim Additional Payments for Teaching", "Manages teacher payment<br>claim and eligibility")

    System_Ext(fbit,"Financial Benchmarking and Insights Tool<br>(FBIT)","Compares and analyses school financial data to<br>support  planning and accountability") 
  }

  Rel(GIAS,anm,"Ingests establishment and<br>governor data","HTTPS/CSV")
  Rel(GIAS, tv, "Ingests establishment data", "HTTPS/CSV")
  Rel(GIAS, ptt, "Ingests establishment data", "HTTPS/CSV")
  Rel(GIAS, ecf, "Ingests establishment data", "HTTPS/CSV")
  Rel(GIAS, capt, "Ingests establishment data", "HTTPS/CSV")
  Rel(GIAS, fbit, "Ingests establishment data", "Manual/CSV")

 
  UpdateRelStyle(GIAS, anm, $offsetX="-190", $offsetY="-10") 
  UpdateRelStyle(GIAS, tv, $offsetX="0", $offsetY="-20") 
  UpdateRelStyle(GIAS, ptt, $offsetX="0", $offsetY="-10") 
  UpdateRelStyle(GIAS, ecf, $offsetX="-70", $offsetY="40") 
  UpdateRelStyle(GIAS, capt, $offsetX="0", $offsetY="40") 
  UpdateRelStyle(GIAS, fbit, $offsetX="-30", $offsetY="40") 
```
