# Service Overview

> Get Information About Schools (GIAS) is the Department for Education’s official register of educational establishments in England. It provides a single, authoritative source of information used by schools, trusts, local authorities, government partners, and the public
>
> GIAS is also the National Database of Governors, holding governance information for state‑funded schools and academy trusts as required by legislation

## C4 System Context Diagram

**Questions**

- Do we have a catalogue of all the Edubase backend exports, we have about 30, who are the consumers for these, can we get a list from the ws authentication table
- What is Schools, Pupils and their Characteristics
- Do we use Azure Maps, or OS maps
- Do we have a profile service 

```mermaid
C4Context
  title System Context diagram for Get Information About Schools Service
 

  Person(authUser, "Signed-in User", "Accesses additional features<br>and data based on permissions")
  Person(backOfficeUser, "DfE Back Office User", "Maintains and manages GIAS data")
  Person(anonUser, "Anonymous User", "Accesses publicly available school<br>and establishment data")
  System_Ext(externalSystems, "External Consumer", "Systems that download<br>GIAS datasets") 

  Enterprise_Boundary(govuk, "GovUK") {

    System_Boundary(otherGov, "UK Government Services") {
      System_Ext(Notify, "Gov UK<br>Notify", "Mailer system for<br>Email communications")

      System_Ext(companiesHouse, "Companies House", "Provides lookp data via<br>Companies House number")
      System_Ext(ofsted, "Ofsted", "Provides school inspection ratings<br>and links to inspection reports")
      System_Ext(hmrc, "HM Revenue and Customs", "UK tax authority")
    }

      Enterprise_Boundary(dfe, "Department for Education") {

      System_Ext(DfESignIn, "DfE Sign In", "User authentication system.<br>Consumes GIAS data to provide <br>list of establishments on sign in")
      System(GIAS, "Get Information About Schools", "Provides establishment and<br>governance roles data")
      System_Ext(spc, "Schools, Pupils and <br>their Characteristics", "DfE statistical dataset<br>derived from the School Census")

    }

  }

  System_Boundary(external, "External data services") {
    System_Ext(ordnanceSurvey, "Ordnance Survey", "Provides map tiles<br>and address data")
    System_Ext(ons, "Office for national Statistics", "Provides ONS codes for LA,<br>Region and Constituency")
  }


  Rel(DfESignIn, GIAS, "Authenticate users.<br>Consumes establishment data")
  Rel(spc,GIAS,"Provides aggregated pupil<p>characteristic metrics")

  Rel(GIAS, Notify, "Sends emails<br>to users", "API" )
  Rel(ordnanceSurvey, GIAS, "Retrieves map tiles to display school locations")
  Rel(ons, GIAS, "Imports administrative geography<br>and statistical reference datasets")
  Rel(companiesHouse, GIAS, "Company lookup")
  Rel(GIAS, hmrc, "Retrieves establishment information")

  BiRel(ofsted, GIAS, "In : Inspection outcomes and<br>Out : establishment reference data")


  Rel(externalSystems, GIAS, "Downloads data via APIs<br>using Basic Auth")
  Rel(anonUser, GIAS, "Searches and<br>views data")
  Rel(authUser, GIAS, "Views and updates<br>permitted data")
  Rel(backOfficeUser, GIAS, "Administers data")
  Rel(authUser, DfESignIn, "Signs in using")
  Rel(backOfficeUser, DfESignIn, "Signs in using")


  UpdateLayoutConfig($c4ShapeInRow="4", $c4BoundaryInRow="2")

  UpdateRelStyle(externalSystems, GIAS, $offsetX="-110", $offsetY="-100")
  UpdateRelStyle(anonUser, GIAS, $offsetX="30", $offsetY="-100")
  UpdateRelStyle(authUser, GIAS, $offsetX="90", $offsetY="-100")
  UpdateRelStyle(authUser, DfESignIn, $offsetX="20", $offsetY="-60")
  UpdateRelStyle(backOfficeUser, GIAS, $offsetX="200", $offsetY="-100")


```



