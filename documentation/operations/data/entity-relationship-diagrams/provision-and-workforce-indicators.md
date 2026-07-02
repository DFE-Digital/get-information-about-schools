# Provision And Workforce Indicators

This page explains provision, early-years and workforce-related reference data held against establishment records.

## Scope

This view focuses on:

- Burnham Report classification;
- EYFS exemption classification;
- learning support unit classification;
- private finance initiative classification;
- registered early-years classification;
- superannuation category;
- studio-school indicator.

It does not show the wider establishment record, audit tables, permissions tables or detailed policy history for these values.

## How To Read This Model

- These tables are reference data attached to an establishment by code.
- The values describe provision, workforce or legacy programme characteristics.
- Some values are used for export, reporting or historic compatibility even where they are not prominent in the public website.
- The establishment record stores the code. Extract and cache projections can expose both code and name.

## Application-Derived Insights

- These indicators are part of the current establishment data export surface as code/name pairs.
- Some values have defaults or type-specific behaviour in the application.
- `StudioSchoolIndicator` is not the same as the establishment type for Studio Schools; it is a separate coded indicator.
- A future model should test whether each value belongs in public provider data, internal stewardship data or archive-only migration scope.

## Provision And Workforce Indicator Model

```mermaid
erDiagram
    Establishment {
        numeric URN
        nvarchar EstablishmentName
        nvarchar burnhamReport_code
        nvarchar eyfsExemption_code
        nvarchar learningSupportUnit_code
        nvarchar privateFinanceInitiative_code
        nvarchar registeredEarlyYears_code
        nvarchar superannuationCategory_code
        nvarchar studioSchoolIndicator_code
    }

    BurnhamReport {
        nvarchar code
        nvarchar name
        tinyint archived
    }

    EYFSExemption {
        nvarchar code
        nvarchar name
        tinyint archived
    }

    LearningSupportUnit {
        nvarchar code
        nvarchar name
        tinyint archived
    }

    PrivateFinanceInitiative {
        nvarchar code
        nvarchar name
        tinyint archived
    }

    RegisteredEarlyYears {
        nvarchar code
        nvarchar name
        tinyint archived
    }

    SuperannuationCategory {
        nvarchar code
        nvarchar name
        tinyint archived
    }

    StudioSchoolIndicator {
        nvarchar code
        nvarchar name
        tinyint archived
        int orderBy
    }

    BurnhamReport ||--o{ Establishment : burnham_report
    EYFSExemption ||--o{ Establishment : eyfs_exemption
    LearningSupportUnit ||--o{ Establishment : learning_support_unit
    PrivateFinanceInitiative ||--o{ Establishment : private_finance_initiative
    RegisteredEarlyYears ||--o{ Establishment : registered_early_years
    SuperannuationCategory ||--o{ Establishment : superannuation_category
    StudioSchoolIndicator ||--o{ Establishment : studio_school_indicator
```

### BurnhamReport

`BurnhamReport` classifies the Burnham Report value held on an establishment.

Business-friendly pattern:

```text
For this establishment,
what Burnham Report classification applies?
```

### EYFSExemption

`EYFSExemption` classifies whether an establishment has an early-years foundation stage exemption.

Business-friendly pattern:

```text
For this establishment,
what EYFS exemption classification applies?
```

### LearningSupportUnit

`LearningSupportUnit` classifies the learning-support-unit value held on an establishment.

Business-friendly pattern:

```text
For this establishment,
what learning support unit classification applies?
```

### PrivateFinanceInitiative

`PrivateFinanceInitiative` classifies the private finance initiative status held on an establishment.

Business-friendly pattern:

```text
For this establishment,
what private finance initiative classification applies?
```

### RegisteredEarlyYears

`RegisteredEarlyYears` classifies the registered early-years value held on an establishment.

Business-friendly pattern:

```text
For this establishment,
what registered early-years classification applies?
```

### SuperannuationCategory

`SuperannuationCategory` classifies the superannuation category held on an establishment.

Business-friendly pattern:

```text
For this establishment,
what superannuation category applies?
```

### StudioSchoolIndicator

`StudioSchoolIndicator` classifies the studio-school indicator value held on an establishment.

Business-friendly pattern:

```text
For this establishment,
what studio-school indicator applies?
```
