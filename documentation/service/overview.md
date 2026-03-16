# Service Overview

> Get Information About Schools (GIAS) is the Department for Education’s official register of educational establishments in England. It provides a single, authoritative source of information used by schools, trusts, local authorities, government partners, and the public
>
> GIAS is also the National Database of Governors, holding governance information for state‑funded schools and academy trusts as required by legislation

## C4 System Context Diagram


```mermaid
C4Context
  title System Context diagram for Get Information About Schools Service
 
  System_Ext(externalSystems, "External Consumer", "Systems that downloa<br>GIAS datasets")
  Person(anonUser, "Anonymous User", "Accesses publicly available school<br>and establishment data")
  Person(authUser, "Signed-in User", "Accesses additional features<br>and data based on permissions")
  Person(backOfficeUser, "DfE Back Office User", "Maintains and manages GIAS data")




  Enterprise_Boundary(govuk, "GovUK") {



    Enterprise_Boundary(dfe, "Department for Education") {
      System(GIAS, "Get Informatio About Schools", "Provides establishment and<br>governance roles data")
      System_Ext(DfESignIn, "DfE Sign In", "User authentication system")

    }


  }

  Rel(anonUser, GIAS, "Searches and<br>views data")
  Rel(authUser, GIAS, "Views and updates<br>permitted data")
  Rel(backOfficeUser, GIAS, "Administers data")
  Rel(authUser, DfESignIn, "Signs in using")
  Rel(backOfficeUser, DfESignIn, "Signs in using")


  Rel(DfESignIn, GIAS, "")





  UpdateLayoutConfig($c4ShapeInRow="4", $c4BoundaryInRow="1")
  UpdateRelStyle(anonUser, GIAS, $offsetX="-110", $offsetY="-100")
  UpdateRelStyle(authUser, GIAS, $offsetX="0", $offsetY="-100")
  UpdateRelStyle(authUser, DfESignIn, $offsetX="0", $offsetY="0")
  UpdateRelStyle(backOfficeUser, GIAS, $offsetX="80", $offsetY="-100")





```



