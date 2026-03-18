# Service Overview

> Get Information About Schools (GIAS) is the Department for Education’s official register of educational establishments in England. It provides a single, authoritative source of information used by schools, trusts, local authorities, government partners, and the public
>
> GIAS is also the National Database of Governors, holding governance information for state‑funded schools and academy trusts as required by legislation

## C4 System Context Diagram


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

      System_Ext(DfESignIn, "DfE Sign In", "User authentication system.Consumes GIAS data to <br>provide list of establishments on sign in")
      System_Ext(profileService, "DfE Profile service", "DfE service providing user profiles,<br>organisation relationships, and role-based access data")
      
      System(GIAS, "Get Information About Schools", "Provides establishment and governance roles data")
      
      System_Ext(schoolCensus,"School census","DfE statutory data set.<br>SPC (Schools, Pupils and their Characteristics).<br>SLASC (School Level Annual School Census) ")

      System_Ext(ukrlp, "UKRLP (UK Register of<br>Learning Providers)", "National register of learning providers providing<br>UKPRN identifiers and organisation data")
    }

  }

  System_Boundary(external, "External Data Services") {
    System_Ext(ordnanceSurvey, "Ordnance Survey", "UK address lookup servce")
    System_Ext(azureMaps, "Azure Maps", "Used to render maps and display<br>establishment locations")
    System_Ext(ons, "Office for National Statistics", "Provides ONS codes for LA,<br>Region and Constituency")
  }

  UpdateLayoutConfig($c4ShapeInRow="4", $c4BoundaryInRow="2")

  Rel(authUser, GIAS, "Views and updates<br>permitted data")
  Rel(backOfficeUser, GIAS, "Administers data")
  Rel(anonUser, GIAS, "Searches and<br>views data")
  Rel(externalSystems, GIAS, "Downloads data via APIs<br>using Basic Auth")
  
  Rel(authUser, DfESignIn, "Signs in using")
  Rel(backOfficeUser, DfESignIn, "Signs in using")

  BiRel(DfESignIn, GIAS, "Authenticate users.<br>Consumes establishment data")
  Rel(profileService, GIAS, "Provides user profile data")
  Rel(schoolCensus, GIAS, "Provides pupil-level and<br>school-level statistics")
  Rel(ukrlp, GIAS, "Provides provider identity data")

  UpdateRelStyle(anonUser, GIAS, $offsetX="-300", $offsetY="-280")
  UpdateRelStyle(authUser, GIAS, $offsetX="-200", $offsetY="-280")
  UpdateRelStyle(backOfficeUser, GIAS, $offsetX="50", $offsetY="-280")
  UpdateRelStyle(externalSystems, GIAS, $offsetX="190", $offsetY="-280") 

  UpdateRelStyle(authUser, DfESignIn, $offsetX="0", $offsetY="100") 
  UpdateRelStyle(backOfficeUser, DfESignIn, $offsetX="-70", $offsetY="100") 

  UpdateRelStyle(DfESignIn, GIAS, $offsetX="-80", $offsetY="-120") 

  UpdateRelStyle(ukrlp, GIAS, $offsetX="-80", $offsetY="120") 



```



