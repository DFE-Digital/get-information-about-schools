# Service Overview

> Get Information about Schools (GIAS) is the Department for Education’s official register of educational establishments in England. It provides a single, authoritative source of information used by schools, trusts, local authorities, government partners, and the public. It holds information about establishments, establishment groups and governors.
>
> GIAS is also the National Database of Governors, holding governance information for state‑funded schools and academy trusts as required by legislation


## About the C4 System Context Diagram

A C4 system context diagram is the highest-level architectural view of a system. It shows GIAS as a single system, the people and external systems around it, and the most important relationships between them.

This diagram is intended to help readers quickly understand the scope and boundaries of GIAS: who uses it, which external services and organisations it depends on, and which other systems consume its data.

It does not show the internal structure of GIAS, such as applications, containers, components, classes, data models, deployment details, or detailed request flows. Those concerns belong in lower-level C4 diagrams and supporting documentation.

## C4 System Context Diagram


### System context diagram for the Get Information about Schools Service
```mermaid
C4Context
 
  Person(anonUser, "Anonymous User", "Accesses publicly available establishment, <br>groups, governance data and search")
  
  Person(authUser, "Signed-in User", "Accesses additional features and data<br> based on permissions")
  
  Person(backOfficeUser, "DfE Back Office User", "Signed-in user with elevated permissions<br> to maintain and manage GIAS data")

  System_Ext(externalSystems, "External Consumer", "Systems that download<br>GIAS datasets") 

  Enterprise_Boundary(govuk, "GovUK") {

    System_Boundary(otherGov, "UK Government Services") {
      System_Ext(Notify, "Gov UK<br>Notify", "Mailer system for<br>Email communications")

      System_Ext(companiesHouse, "Companies House", "UK register of companies")

      System_Ext(hmrc, "HM Revenue and Customs", "Tax-Free Childcare (TFC)<br>service")
    }

      Enterprise_Boundary(dfe, "Department for Education") {

      System_Ext(DfESignIn, "DfE Sign-in (DSI)", "User authentication system. Consumes GIAS data to <br>provide list of establishments on sign in")

      System(GIAS, "Get Information about Schools", "Single authoritative source for registered,<br> accredited and maintained establishments and establishment groups")
      
      System_Ext(spc,"SPC","Schools, Pupils and their Characteristics")

      System_Ext(slasc,"SLASC","School Level Annual School Census")

      System_Ext(ukrlp, "UK Register of Learning<br>Providers (UKRLP)", "National register of learning providers providing<br>UKPRN identifiers and organisation data")

      System_Ext(internal, "Internal DfE systems","Collection of systems that ingest GIAS data")
    }
  }

  System_Boundary(external, "External Data Services") {
    System_Ext(ordnanceSurvey, "Ordnance Survey", "UK address lookup servce")
    System_Ext(azureMaps, "Microsoft Azure Maps", "Used to render maps and display<br>establishment locations")
    System_Ext(ons, "Office for National Statistics", "Official statistics and geographic<br>reference data")
  }

  UpdateLayoutConfig($c4ShapeInRow="4", $c4BoundaryInRow="2")

  Rel(anonUser, GIAS, "Views and downloads data", "HTTPS/HTML & CSV")
  Rel(authUser, GIAS, "Views, updates and downloads<br>permitted data", "HTTPS/HTML & CSV")
  Rel(backOfficeUser, GIAS, "Administers data", "HTTPS/HTML")
  Rel(externalSystems, GIAS, "Downloads GIAS data", "HTTPS/XML/CSV/XLXS<br>using Basic Auth")
  
  Rel(authUser, DfESignIn, "Signs in using")
  Rel(backOfficeUser, DfESignIn, "Signs in using")

  BiRel(DfESignIn, GIAS, "Authenticate users.<br>Consumes provider data", "HTTPS/JSON")

  Rel(ukrlp, GIAS, "Supplies provider identity data", "HTTPS/SOAP/XML")
  Rel(GIAS, Notify, "Submits email messages<br>for delivery", "HTTPS/JSON")
  Rel(ordnanceSurvey, GIAS, "Search address<br>data by postcode", "HTTPS/JSON")
  Rel(azureMaps,GIAS, "Retrieves map tiles<br>and location search results", "HTTPS/JSON")
  Rel(ons,GIAS,"Supplies ONS codes for LA,<br>Region and Constituency","Manual/CSV")
  Rel(companiesHouse, GIAS,"Company data by<br>Companies House Number", "HTTPS/JSON")
  Rel(GIAS, hmrc, "Extract of establishment data<br>for childcare providers (CCPs)", "Manual/CSV")
  Rel(GIAS, internal, "Ingests GIAS data", "HTTPS/CSV/XLXS")
  Rel(spc, GIAS, "Supplies pupil-level statistics", "Manual/DB script")
  Rel(slasc, GIAS, "Supplies pupil-level statistics", "Manual/DB script")

  UpdateRelStyle(anonUser, GIAS, $offsetX="-320", $offsetY="-190")
  UpdateRelStyle(authUser, GIAS, $offsetX="-230", $offsetY="-190")
  UpdateRelStyle(backOfficeUser, GIAS, $offsetX="70", $offsetY="-190")
  UpdateRelStyle(externalSystems, GIAS, $offsetX="190", $offsetY="-200") 

  UpdateRelStyle(authUser, DfESignIn, $offsetX="-20", $offsetY="-0") 
  UpdateRelStyle(backOfficeUser, DfESignIn, $offsetX="-50", $offsetY="-50") 

  UpdateRelStyle(DfESignIn, GIAS, $offsetX="-80", $offsetY="-20") 
  UpdateRelStyle(schoolCensus, GIAS, $offsetX="10", $offsetY="0") 
  UpdateRelStyle(ukrlp, GIAS, $offsetX="-190", $offsetY="180") 
  UpdateRelStyle(GIAS, Notify, $offsetX="-200", $offsetY="-10") 

  UpdateRelStyle(spc, GIAS, $offsetX="-170", $offsetY="10") 
  UpdateRelStyle(slasc, GIAS, $offsetX="-195", $offsetY="100") 

  UpdateRelStyle(ordnanceSurvey, GIAS, $offsetX="120", $offsetY="-50") 
  UpdateRelStyle(azureMaps,GIAS, $offsetX="-50", $offsetY="-30") 
  UpdateRelStyle(ons, GIAS, $offsetX="-60", $offsetY="30") 

  UpdateRelStyle(GIAS, hmrc, $offsetX="-200", $offsetY="10") 

  UpdateRelStyle(companiesHouse, GIAS, $offsetX="-75", $offsetY="-30") 

  UpdateRelStyle(GIAS, internal, $offsetX="-20", $offsetY="290") 


```

**Notes**

1. The `DfE Back Office User` is a signed-in user with elevated permissions. This role can do everything available to anonymous and standard signed-in users, plus additional administrative and data-management actions.
2. To keep the main context diagram easy to read, all DfE internal systems have been grouped into a single external system called `Internal DfE systems`.
3. The `External Consumer` is an external system, not a person. It consumes GIAS data either by downloading extract files or by calling the SOAP interface.
4. For more detail on how front-end authentication works between GIAS and DfE Sign-in, see [GIAS front-end authentication flow](../front-end-component/front-end-authentication-flow/).


The detailed internal DfE systems diagram has been replaced with the tables below. They separate internal DfE systems from external systems referenced in this overview, and give a short description of each service or system's primary purpose.

This list will evolve over time as more internal systems and use cases are identified.

### Internal DfE systems consuming GIAS

| Service name | Description |
| --- | --- |
| DfE Sign-in (DSI) | Department for Education identity and access management service used to sign in to DfE online services. |
| Provider Profile (for DSI) | Service and data source used to supply provider and organisation information to DSI for access management. |
| Collections Online - Learners, Education, Children and Teachers (COLLECT) | DfE's centralised data collection and management system for education returns. |
| School census | DfE statutory data collection covering school, pupil and characteristics data. |
| Submit Learner Data | Service for providers to submit, validate and review learner and funding-related data returns. |
| Individual Learner Record (ILR) | Standard learner-data return used by publicly funded providers to report learner and learning aim data. |
| School to School (S2S) | Secure service for transferring pupil data and files between schools, local authorities and DfE. |
| Document Exchange | Secure file and document exchange service used to share funding and operational documents with providers. |
| Enterprise API Management Platform (EAPIM) | Enterprise platform for publishing, securing, monitoring and managing APIs across DfE services. |
| PIMs | Internal provider information management services used to maintain provider reference data and attributes. |
| Enterprise Data and Analytics Platform (EDAP) | Enterprise platform for storing, transforming and analysing DfE data. |
| SIP | Internal information service used to bring together school profile and operational information for DfE users. |
| Provider CRM | Customer relationship management system for managing provider contacts, enquiries and casework. |
| School and College Database (SCDB) | Reference database of schools and colleges used across DfE services and reporting. |
| Schools Checking Exercise | Checking exercise that allows schools to review and confirm key data held by DfE. |
| Compare the Performance of Schools and Colleges in England (CSCP) | Public service for comparing school and college performance and related data in England. |
| Analyse School Performance (ASP) | Secure service providing detailed attainment and progress reports to support school improvement. |
| Monitor Your School Attendance | Tool for viewing, comparing and downloading daily attendance and absence data. |
| Get Information about Pupils (GIAP) | Secure service that gives authorised users access to individual pupil-level data. |
| Standard Testing Agency (STA) - Assessment Platform (Manage my Pupils) | STA assessment administration platform used to manage pupil and school data for national assessments. |
| Standard Testing Agency (STA) - Digital Platform (in development) | New STA digital platform being developed to deliver and administer assessment services online. |
| Primary Assessment Gateway (PAG) (and replacement) | Secure portal supporting the administration of primary national curriculum assessments. |
| Standard Testing Agency (STA) - Multiplication Tables Checking (MTC) | Online service used to administer the statutory year 4 multiplication tables check. |
| Capital CRM | Customer relationship management system for capital funding, capital programmes and related casework. |
| Financial Benchmarking and Insights Tool | Service that helps schools, trusts and local authorities compare spending and plan finances. |
| Academy Financial Returns | Collection of academy trust financial return submissions, including accounts-return data. |
| Further Education Financial Returns | Collection of financial returns, forecasts and related submissions for further education providers. |
| Calculate Funding Service (CFS) | Internal service used to calculate or adjust funding allocations. |
| Basic Need Allocation | Allocation process and supporting service for school-place capital funding. |
| Manage Your Education and Skills Funding (My ESF) | Service for viewing allocations and payments, signing documents and managing subcontractor information. |
| National Capacity Assessment (NCA) programme | Programme used to assess school-place capacity and need to support capital planning decisions. |
| CRM Land and Transactions | Customer relationship management system for land, property and academy transaction casework. |
| School System Accountability and Improvement Core Brief | Internal briefing product bringing together accountability, performance and improvement information about schools. |
| Trust and Academy Management service (TRAMS) | Service used to manage academy trust and academy operational, governance, land and compliance information. |
| Find Information about Academies and Trusts (FIAT) | Service for finding information about academies and academy trusts. |
| Academies and Free Schools Key Data | Data product summarising key information about academies and free schools. |
| Open Academies with United Kingdom Provider Reference Numbers (UKPRNs) | Data publication linking open academies to their UKPRNs. |
| Open Academies, Free Schools, Studio Schools and University Technology Colleges (UTCs) | Monthly publication listing open academies and academy projects in development. |
| National Foundation for Education Research (NFER) systems | NFER-operated research and assessment systems that use education reference data. |
| Project Titan | Programme to modernise how education data and digital credentials are collected, shared and used across the sector. |
| Edustat | Internal education statistics and analysis tool used by DfE teams. |
| Explore Education Statistics (EES) | Public service for exploring official education statistics, data tables and open datasets. |
| Iris (CRM) | Customer relationship management system used for operational casework and stakeholder management. |
| Database of Qualified Teachers | Teacher Regulation Agency database holding records of qualified teachers and teacher reference numbers. |
| Access Teaching Qualifications | Service that lets teachers view qualifications, induction information and download certificates. |
| Manage Training for Early Career Teachers | Service used by schools to set up and manage early career teacher training and related details. |
| Register for a National Professional Qualification (NPQ) | Service for registering for NPQ courses and checking registration or outcome details. |
| Teaching Vacancy Service | National service for advertising teaching, leadership and education support jobs and managing applications. |
| Funding Transformation Project (FTP) | Funding transformation programme to modernise funding operations and supporting digital services. |
| Funding Data Service | Internal data service that supports allocations, calculations, contracts, statements and payments. |
| Provider Profile  | Internal provider profile view or service used to surface provider reference and operational information. |
| Single Point of Information (SPI) | Internal service intended to provide a single view of key provider and institution information. |
| Complete conversions, transfers and changes | Service used to complete academy conversions, transfers and related change processes. |
| Teacher CPD service | Service family supporting teacher continuing professional development journeys, including early career and NPQ-related services. |
| Analysis and Modelling Platform (AnM) | ESFA data catalogue and analytics environment. |
| Publish Teacher Training | Service for publishing teacher training courses and related provider information. |
| Early Career Framework | Two-year professional development framework and entitlement for early career teachers. |
| Claim Additional Payments for Teaching | Service for teachers to claim eligible additional payments linked to teaching roles and eligibility rules. |
| Find placement schools | Service for schools to record placement capacity and for teacher training providers to find placement schools. |
| Claim funding for mentor training | Service for schools and education organisations to claim funding for ITT mentor training. |


