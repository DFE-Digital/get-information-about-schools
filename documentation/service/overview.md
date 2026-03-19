# Service Overview

> Get Information About Schools (GIAS) is the Department for Education’s official register of educational establishments in England. It provides a single, authoritative source of information used by schools, trusts, local authorities, government partners, and the public
>
> GIAS is also the National Database of Governors, holding governance information for state‑funded schools and academy trusts as required by legislation

## C4 System Context Diagram

**Questions**

- How does GIAS ingest the DfE profile service data
- How does GIAS ingest the UK Register of Learning Providers 




```mermaid
C4Context
  title System Context diagram for Get Information About Schools Service
 
  Person(anonUser, "Anonymous User", "Accesses publicly available school<br>and establishment data")
  Person(authUser, "Signed-in User", "Accesses additional features<br>and data based on permissions")
  Person(backOfficeUser, "DfE Back Office User", "Maintains and manages GIAS data")

  System_Ext(externalSystems, "External Consumer", "Systems that download<br>GIAS datasets") 

  Enterprise_Boundary(govuk, "GovUK") {

    System_Boundary(otherGov, "UK Government Services") {
      System_Ext(Notify, "Gov UK<br>Notify", "Mailer system for<br>Email communications")

      System_Ext(companiesHouse, "Companies House", "Provides lookp data via<br>Companies House number")
      System_Ext(ofsted, "Ofsted", "Provides school inspection ratings<br>and links to inspection reports")
      System_Ext(hmrc, "HM Revenue and Customs", "UK tax authority")
    }

      Enterprise_Boundary(dfe, "Department for Education") {

      System_Ext(DfESignIn, "DfE Sign In (DSI)", "User authentication system.Consumes GIAS data to <br>provide list of establishments on sign in")


      
      System(GIAS, "Get Information About Schools", "Supplies establishment and governance roles data")
      
      System_Ext(schoolCensus,"School census","DfE statutory data set.<br>SPC (Schools, Pupils and their Characteristics).<br>SLASC (School Level Annual School Census) ")

      System_Ext(ukrlp, "UKRLP (UK Register of<br>Learning Providers)", "National register of learning providers providing<br>UKPRN identifiers and organisation data")
    }

  }

  System_Boundary(external, "External Data Services") {
    System_Ext(ordnanceSurvey, "Ordnance Survey", "UK address lookup servce")
    System_Ext(azureMaps, "Azure Maps", "Used to render maps and display<br>establishment locations")
    System_Ext(ons, "Office for National Statistics", "Supplies ONS codes for LA,<br>Region and Constituency")
  }

  UpdateLayoutConfig($c4ShapeInRow="4", $c4BoundaryInRow="2")

  Rel(anonUser, GIAS, "Views and downloads data", "HTML/CSV")
  BiRel(authUser, GIAS, "Views, updates and downloads<br>permitted data", "HTML/CSV")
  Rel(backOfficeUser, GIAS, "Administers data", "HTML")
  Rel(externalSystems, GIAS, "Downloads establishment and governor data", "SOAP/XML and CSV<br>using Basic Auth")
  
  Rel(authUser, DfESignIn, "Signs in using")
  Rel(backOfficeUser, DfESignIn, "Signs in using")

  BiRel(DfESignIn, GIAS, "Authenticate users.<br>Consumes provider data", "API")

  Rel(schoolCensus, GIAS, "Supplies pupil-level and<br>school-level statistics", "Manual DB script")
  Rel(ukrlp, GIAS, "Supplies provider identity data")

  Rel(GIAS, Notify, "Submits email messages<br>for delivery", "API")

  Rel(ordnanceSurvey, GIAS, "Search address<br>data by postcode", "API")
  Rel(azureMaps,GIAS, "Retrieves map tiles<br>and location search results", "API")

  UpdateRelStyle(anonUser, GIAS, $offsetX="-320", $offsetY="-190")
  UpdateRelStyle(authUser, GIAS, $offsetX="-250", $offsetY="-190")
  UpdateRelStyle(backOfficeUser, GIAS, $offsetX="50", $offsetY="-190")
  UpdateRelStyle(externalSystems, GIAS, $offsetX="180", $offsetY="-200") 

  UpdateRelStyle(authUser, DfESignIn, $offsetX="0", $offsetY="0") 
  UpdateRelStyle(backOfficeUser, DfESignIn, $offsetX="-70", $offsetY="100") 

  UpdateRelStyle(DfESignIn, GIAS, $offsetX="-80", $offsetY="-20") 

  UpdateRelStyle(ukrlp, GIAS, $offsetX="-80", $offsetY="120") 

  UpdateRelStyle(GIAS, Notify, $offsetX="-200", $offsetY="-10") 

  UpdateRelStyle(ordnanceSurvey, GIAS, $offsetX="120", $offsetY="-50") 
  UpdateRelStyle(azureMaps,GIAS, $offsetX="20", $offsetY="20") 



```



