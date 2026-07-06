# Change Request Approval And Workflow Access

This page explains how proposed changes record proposal, ownership, trusted-source checks and approval decisions.

## Scope

This model covers:

- establishment change approval policy;
- group change actor and trusted-source context;
- staff change actor context;
- proposer and decision attribution.

## How To Read This Model

- Establishment changes have the richest configurable ownership and trusted-source model.
- Group changes use group-field permissions, authorities and organisation scope.
- Staff changes record proposer and decision provenance, but may be applied immediately.
- A change-request row does not always mean that a separate human approval step occurred.

## Application-Derived Insights

- Establishment, group and staff changes do not share one consistent approval model.
- Establishment workflow preserves proposer group, data owner and approver group more explicitly than group and staff workflows.
- Group and staff actor history may depend on a user's current group unless the request row snapshots that context.
- Future design should define a common change-decision vocabulary and then allow deliberate domain-specific differences.

## Establishment Change Approval

```mermaid
erDiagram
    Establishment {
        numeric URN PK
        nvarchar type_code FK
        nvarchar status_code FK
        nvarchar EstablishmentName
    }
    EstablishmentField {
        nvarchar shortName PK
        nvarchar displayName
        nvarchar fieldtype
    }
    UserGroup {
        nvarchar code PK
        nvarchar name
        nvarchar role
    }
    SystemUser {
        nvarchar username PK
        nvarchar UserGroupCode FK
        numeric URN FK
        nvarchar LocalAuthority FK
        numeric UID FK
    }
    OwningRule {
        numeric id PK
        nvarchar field FK
        nvarchar type FK
        nvarchar status FK
        nvarchar userGroup_code FK
    }
    TrustedSource {
        numeric id PK
        nvarchar field FK
        nvarchar type FK
        nvarchar userGroup_code FK
    }
    EstablishmentChangeHistory {
        numeric id PK
        nvarchar status
        numeric establishment_URN FK
        nvarchar field_shortName FK
        nvarchar proposedByUser_username FK
        nvarchar proposedByUserGroup_code FK
        nvarchar dataOwner_code FK
        nvarchar approvedOrRejectedBy_username FK
        nvarchar approvedOrRejectedByUserGroup_code FK
        tinyint sameUser
    }

    EstablishmentField ||--o{ OwningRule : owned_field
    UserGroup ||--o{ OwningRule : data_owner
    EstablishmentField ||--o{ TrustedSource : trusted_field
    UserGroup ||--o{ TrustedSource : trusted_group
    UserGroup ||--o{ SystemUser : has_users
    Establishment ||--o{ EstablishmentChangeHistory : proposed_change
    EstablishmentField ||--o{ EstablishmentChangeHistory : changed_field
    SystemUser ||--o{ EstablishmentChangeHistory : proposed_or_decided_by
    UserGroup ||--o{ EstablishmentChangeHistory : proposed_owned_or_decided_by
```

### OwningRule

Business-friendly pattern:

```text
For this establishment field, type and status,
which user group owns the data?
```

### TrustedSource

Business-friendly pattern:

```text
For this establishment field and type,
is the proposing user group trusted to make the change?
```

### EstablishmentChangeHistory

Business-friendly pattern:

```text
For this proposed establishment change,
who proposed it,
who owned the data,
who approved or rejected it,
and what decision state was recorded?
```

## Group And Staff Change Actors

```mermaid
erDiagram
    UserGroup {
        nvarchar code PK
        nvarchar name
        nvarchar role
    }
    Authority {
        nvarchar code PK
        nvarchar description
    }
    UserGroup_Authority {
        nvarchar UserGroup_code PK
        nvarchar authorities_code PK
    }
    SystemUser {
        nvarchar username PK
        nvarchar UserGroupCode FK
    }
    EstablishmentGroup {
        numeric id PK
        nvarchar name
        nvarchar type_code
    }
    GroupField {
        nvarchar shortName PK
        nvarchar displayName
        nvarchar fieldtype
    }
    GroupFieldPermission {
        nvarchar groupCode PK
        nvarchar shortName PK
        nvarchar permission
    }
    GroupChangeRequest {
        numeric id PK
        nvarchar requestType
        nvarchar status
        nvarchar proposedBy_username FK
        nvarchar approvedOrRejectedBy_username FK
        nvarchar field_shortName FK
        numeric group_id FK
    }
    StaffRecord {
        numeric uid PK
        numeric establishment_URN FK
        numeric group_id FK
    }
    StaffField {
        nvarchar shortName PK
        nvarchar displayName
        nvarchar fieldtype
    }
    StaffChangeRequest {
        numeric id PK
        nvarchar type
        nvarchar status
        nvarchar proposedBy_username FK
        nvarchar approvedOrRejectedBy_username FK
        nvarchar field_shortName FK
        numeric staffRecord_uid FK
    }

    UserGroup ||--o{ SystemUser : has_users
    UserGroup ||--o{ UserGroup_Authority : granted_to
    Authority ||--o{ UserGroup_Authority : grants
    UserGroup ||--o{ GroupFieldPermission : group_permission
    GroupField ||--o{ GroupFieldPermission : field_permission
    SystemUser ||--o{ GroupChangeRequest : proposed_or_decided_by
    EstablishmentGroup ||--o{ GroupChangeRequest : target_group
    GroupField ||--o{ GroupChangeRequest : changed_field
    SystemUser ||--o{ StaffChangeRequest : proposed_or_decided_by
    StaffRecord ||--o{ StaffChangeRequest : target_staff_record
    StaffField ||--o{ StaffChangeRequest : changed_field
```

### GroupChangeRequest

Business-friendly pattern:

```text
For this proposed change to an education provider group or one of its links,
what is changing,
who proposed it,
can it be applied immediately,
and who approved or rejected it?
```

### StaffChangeRequest

Business-friendly pattern:

```text
For this governance or staff-field change,
who performed the change,
and what change-request record preserves that action?
```

`FyiStakeholder` has been omitted because it is marked as having no observed production read or write activity in the 30-day table-usage evidence.

## Reading This Diagram

Use this model to understand change-decision responsibility. The future design should make proposer, owner, trusted source, approver, decision, effective date and applied date explicit.
