# Report And Content Artefacts

This page explains legacy analysis report definitions and generated report-file artefacts.

## Scope

This model covers:

- legacy analysis report definitions;
- report criteria and criterion values;
- analysis measure types;
- generated report files and user-group access;
- report-file audit versions.

## How To Read This Model

- An analysis report is a saved report definition, not a generated file.
- A report file is a generated artefact registered by report key.
- Report-file permission is user-group access to generated reports.
- Some legacy analysis-report tables are candidates for retirement unless the old analysis engine is confirmed as required.

## Application-Derived Insights

- Analysis reports and generated report files are separate concepts.
- Analysis criteria are meaningful only with the legacy analysis engine and dataset metadata.
- The report-file permission bridge uses a misleading column name for the report-file identifier.
- Future reporting should model report definition, generated artefact and audience explicitly.

## Analysis Reports

```mermaid
erDiagram
    AnalysisReport {
        numeric id PK
        nvarchar name
        nvarchar description
        nvarchar datasetName
        tinyint isSimple
        datetime creationDate
        nvarchar creatorUser_username FK
        nvarchar lastModifiedUser_username FK
        nvarchar predefinedReportCategory_code FK
        numeric filter_id FK
    }
    AnalysisCriterion {
        numeric id PK
        nvarchar code
        int criterionType
        int sortOrder
        numeric report_id FK
    }
    AnalysisCriterionValue {
        numeric id PK
        nvarchar value
        numeric analysisCriterion_id FK
    }
    AnalysisReportToMeasureType {
        numeric report_id FK
        nvarchar measure_code FK
    }
    MeasureType {
        nvarchar code PK
        nvarchar name
        tinyint affectsSensitive
        tinyint archived
    }
    PredefinedReportCategory {
        nvarchar code PK
        nvarchar name
        tinyint archived
    }
    EstablishmentFilter {
        numeric id PK
    }
    SystemUser {
        nvarchar username PK
    }

    AnalysisReport ||--o{ AnalysisCriterion : criteria
    AnalysisCriterion ||--o{ AnalysisCriterionValue : values
    AnalysisReport ||--o{ AnalysisReportToMeasureType : measures
    MeasureType ||--o{ AnalysisReportToMeasureType : measure_type
    PredefinedReportCategory ||--o{ AnalysisReport : category
    EstablishmentFilter ||--o{ AnalysisReport : saved_filter
    SystemUser ||--o{ AnalysisReport : creator
    SystemUser ||--o{ AnalysisReport : last_modified_by
```

### AnalysisReport

Business-friendly pattern:

```text
For this saved analysis report,
what dataset, criteria, measures, optional filter and category define the report?
```

### AnalysisCriterion

Business-friendly pattern:

```text
For this analysis report,
which selected report dimension or filter is being applied?
```

### AnalysisCriterionValue

Business-friendly pattern:

```text
For this analysis criterion,
which selected value is included?
```

## Generated Report Files

```mermaid
erDiagram
    ReportFile {
        numeric id PK
        nvarchar reportKey UK
        datetime lastGeneratedDate
    }
    ReportFilePermission {
        numeric reportKey PK
        nvarchar groupCode PK
    }
    UserGroup {
        nvarchar code PK
        nvarchar name
    }
    ReportFileAud {
        numeric id PK
        numeric ver_rev PK
        tinyint REVTYPE
        nvarchar reportKey
        datetime lastGeneratedDate
    }
    RevisionInfo {
        numeric id PK
    }

    ReportFile ||--o{ ReportFilePermission : allowed_groups
    UserGroup ||--o{ ReportFilePermission : user_group
    ReportFile ||--o{ ReportFileAud : audit_versions
    RevisionInfo ||--o{ ReportFileAud : revision
```

### ReportFile

Business-friendly pattern:

```text
For this report key,
which generated report file exists,
and when was it last generated?
```

### ReportFilePermission

Business-friendly pattern:

```text
For this generated report file,
which user groups are allowed to access it?
```

## Reading This Diagram

Use this model to distinguish saved report definitions from generated report artefacts. These are related reporting concepts, but they need different future ownership and retention decisions.
