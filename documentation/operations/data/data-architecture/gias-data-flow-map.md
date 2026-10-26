# Where GIAS data goes

A  map of every discovered live system we found that uses GIAS data, and how the data reaches it. Some systems may be undiscovered at time of writing.


## Key

- **Solid line.** We can see the code that moves the data.
- **Dashed line.** The data moves, but the loader is private, so we can't see how.
- **Cylinder.** A database that holds a copy of GIAS data.

## The whole picture

```mermaid
%%{init: {"flowchart": {"htmlLabels": false, "padding": 30, "wrappingWidth": 260, "nodeSpacing": 30, "useMaxWidth": false}, "themeVariables": {"fontFamily": "Arial, Helvetica, sans-serif", "fontSize": "14px"}}}%%
flowchart LR
    GIAS[(GIAS)]

    GIAS --> PUB[Public daily CSV]
    GIAS --> SOAP[SOAP web service]
    GIAS --> HAND[Downloaded by hand]
    GIAS -.-> ADB[(Academies DB)]
    GIAS -.-> DSI[(DfE Sign-in)]
    GIAS -.-> FDZ[(Funding Data Zone)]

    subgraph P1 [12 services]
        p01[Buy for your school]
        p02[Claim additional payments]
        p03[NPQ registration]
        p04[Teacher training entitlement]
        p05[Publish teacher training]
        p06[Register trainee teachers]
        p07[ITT mentor services]
        p08[Find placement schools]
        p09[Get school experience]
        p10[Teaching Vacancies]
        p11[Teaching record system]
        p12[Plan technology]
    end
    PUB --> P1

    SOAP --> s01[Register early career teachers]

    subgraph H1 [6 services]
        h01[Complete]
        h02[Education benchmarking]
        h03[Get school improvement insights]
        h04[Check your eligibility]
        h05[Childcare support platform]
        h06[Teacher CPD dataform]
    end
    HAND --> H1

    ADB --> a01[11 academies and trusts services, see next diagram]

    DSI --> d01[Every service that uses DfE Sign-in organisations]

    FDZ --> f01[Calculate funding service]
```

## Through the Academies DB

```mermaid
%%{init: {"flowchart": {"htmlLabels": false, "padding": 30, "wrappingWidth": 260, "nodeSpacing": 30, "useMaxWidth": false}, "themeVariables": {"fontFamily": "Arial, Helvetica, sans-serif", "fontSize": "14px"}}}%%
flowchart LR
    GIAS[(GIAS)]

    subgraph ADB [Academies DB]
        G[(gias)]
        M[(mstr)]
        G -.-> M
    end

    GIAS -.-> G
    GIAS -. weekly spreadsheet by hand .-> FIAT

    subgraph API [Academies API]
        V3[v1 to v3]
        V4[v4]
    end

    G --> V3
    M --> V4
    G --> FIAT[Find information about schools and trusts]
    M --> PAPI[Persons API]

    V3 --> RC[Record concerns]

    V4 --> PC[Prepare conversions]
    V4 --> AAPI[Academisation API]
    V4 --> A2B[Apply to become an academy]
    V4 --> CMP[Complete]
    V4 --> MSI[Manage school improvement]
    V4 --> FIAT
    V4 --> CAA[Case aggregation API]
    V4 --> SA[School Account]
    V4 --> EA[External applications]
    V4 --> FF[FlexForms]

    PAPI --> FIAT
    PAPI --> PC
    PAPI --> CMP
    PAPI --> MSI
```

Notes:

- **`gias` and `mstr`** are schemas in the Academies DB. `gias` is a straight copy of the GIAS tables. `mstr` holds master tables built from it.
- **Most services here never touch GIAS.** They see it through the Academies API, which reads the `mstr` master tables. Private pipelines build those tables.
- **Persons API** serves governance from `mstr`. It also serves MPs, which don't come from GIAS.
- **Complete** appears on both diagrams. It gets open academies from the Academies API, and academies that haven't opened yet by hand from GIAS.
- **Manage free school projects** isn't shown. It reads trusts from its own `dbo.Trust` table in the Academies DB, and we couldn't see where that data comes from.

## Counts

| Route | Live systems |
| --- | --- |
| Public daily CSV | 12 |
| SOAP web service | 1 |
| Downloaded by hand | 6 |
| Academies DB, through FIAT, the Academies API or the Persons API | 11 |
| DfE Sign-in | Every service that uses its organisations |
| Funding Data Zone | 1 |

Complete is counted twice, once by hand and once through the Academies DB.

## Left off the map

- **Archived or dormant.** Early careers framework, Get an identity, Find a tuition partner, Get help with tech, SPI GIAS adapter and SPI tools, Prepare transfers, Identifiers API, `gias-data`, `tslr`, `twd-workforce-data-api` and `support-for-early-career-teachers-legacy`.
- **Tools.** `gias-query-tool`, which loads the public CSV into a local database for ad hoc queries.
- **Further copies.** Several services stream their GIAS tables on into BigQuery, and reports read the Academies DB through private views. We can't see who uses those copies.

## Repositories

The live systems on the map, in the order they appear. Some services have more than one repository. These are the ones that handle GIAS data.

| Service | Repository |
| --- | --- |
| Buy for your school | [buy-for-your-school](https://github.com/DFE-Digital/buy-for-your-school) |
| Claim additional payments | [claim-additional-payments-for-teaching](https://github.com/DFE-Digital/claim-additional-payments-for-teaching) |
| NPQ registration | [npq-registration](https://github.com/DFE-Digital/npq-registration) |
| Teacher training entitlement | [teacher-training-entitlement](https://github.com/DFE-Digital/teacher-training-entitlement) |
| Publish teacher training | [publish-teacher-training](https://github.com/DFE-Digital/publish-teacher-training) |
| Register trainee teachers | [register-trainee-teachers](https://github.com/DFE-Digital/register-trainee-teachers) |
| ITT mentor services | [itt-mentor-services](https://github.com/DFE-Digital/itt-mentor-services) |
| Find placement schools | [find-placement-schools](https://github.com/DFE-Digital/find-placement-schools) |
| Get school experience | [schools-experience](https://github.com/DFE-Digital/schools-experience) |
| Teaching Vacancies | [teaching-vacancies](https://github.com/DFE-Digital/teaching-vacancies) |
| Teaching record system | [teaching-record-system](https://github.com/DFE-Digital/teaching-record-system) |
| Plan technology | [sts-plan-technology-for-your-school](https://github.com/DFE-Digital/sts-plan-technology-for-your-school) |
| Register early career teachers | [register-early-career-teachers-public](https://github.com/DFE-Digital/register-early-career-teachers-public) |
| Complete, Ruby | [dfe-complete-conversions-transfers-and-changes](https://github.com/DFE-Digital/dfe-complete-conversions-transfers-and-changes) |
| Complete, .NET | [complete-conversions-transfers-changes](https://github.com/DFE-Digital/complete-conversions-transfers-changes) |
| Education benchmarking | [education-benchmarking-and-insights](https://github.com/DFE-Digital/education-benchmarking-and-insights) |
| Get school improvement insights, sector | [sap-sector](https://github.com/DFE-Digital/sap-sector) |
| Get school improvement insights, public | [sap-public](https://github.com/DFE-Digital/sap-public) |
| Check your eligibility | [eligibility-checking-engine](https://github.com/DFE-Digital/eligibility-checking-engine) |
| Childcare support platform | [childcare-support-platform](https://github.com/DFE-Digital/childcare-support-platform) |
| Teacher CPD dataform | [teacher-cpd-dataform](https://github.com/DFE-Digital/teacher-cpd-dataform) |
| Find information about schools and trusts | [find-information-about-schools-and-trusts](https://github.com/DFE-Digital/find-information-about-schools-and-trusts) |
| Academies API | [academies-api](https://github.com/DFE-Digital/academies-api) |
| Persons API | [persons-api](https://github.com/DFE-Digital/persons-api) |
| Record concerns | [record-concerns-support-trusts](https://github.com/DFE-Digital/record-concerns-support-trusts) |
| Prepare conversions | [prepare-academy-conversions](https://github.com/DFE-Digital/prepare-academy-conversions) |
| Academisation API | [academies-academisation-api](https://github.com/DFE-Digital/academies-academisation-api) |
| Apply to become an academy | [Dfe.Academies.External](https://github.com/DFE-Digital/Dfe.Academies.External) |
| Manage school improvement | [manage-school-improvement](https://github.com/DFE-Digital/manage-school-improvement) |
| Case aggregation API | [case-aggregation-api](https://github.com/DFE-Digital/case-aggregation-api) |
| School Account | [SchoolAccount-Web](https://github.com/DFE-Digital/SchoolAccount-Web) |
| External applications | [external-applications-web](https://github.com/DFE-Digital/external-applications-web) |
| FlexForms | [flexforms-web](https://github.com/DFE-Digital/flexforms-web) |
| DfE Sign-in | [login.dfe.organisations](https://github.com/DFE-Digital/login.dfe.organisations) |
| Calculate funding service | [funding-service-cfs-backend](https://github.com/DFE-Digital/funding-service-cfs-backend) |
