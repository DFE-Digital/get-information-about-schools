# Row-Level Access And Organisation Scope

This page explains how a user is associated with an establishment, local authority or education provider group, and how that scope contributes to row-level access decisions.

## Scope

This model covers:

- user group and broad role context;
- direct establishment scope;
- local authority scope;
- direct group scope;
- indirect establishment-to-group scope through group links.

## How To Read This Model

- `SystemUser` can carry one establishment, local authority or group scope.
- User group and broad role decide how those scope values are interpreted.
- Establishment type and status can further restrict visibility.
- The database relationships provide organisation context; they are not the whole access-control policy.

## Application-Derived Insights

- Row-level access is a combination of user group, broad role, organisation scope, establishment type and establishment status.
- Local authority and group scope are not just foreign keys; they must be interpreted by policy.
- Direct establishment scope and indirect group membership are different access paths.
- Future design should make scope claims and policy evaluation explicit.

## Organisation Scope Model

```mermaid
erDiagram
    UserRole {
        nvarchar authority PK
        nvarchar caption
    }
    UserGroup {
        nvarchar code PK
        nvarchar name
        nvarchar role FK
    }
    SystemUser {
        nvarchar username PK
        numeric saUserId
        nvarchar UserGroupCode FK
        numeric URN FK
        nvarchar LocalAuthority FK
        numeric UID FK
        tinyint enabled
    }
    LocalAuthority {
        nvarchar code PK
        nvarchar name
    }
    Establishment {
        numeric URN PK
        int EstablishmentNumber
        nvarchar EstablishmentName
        nvarchar LA_code FK
        nvarchar type_code FK
        nvarchar status_code FK
    }
    EstablishmentType {
        nvarchar code PK
        nvarchar name
    }
    EstablishmentStatus {
        nvarchar code PK
        nvarchar name
    }
    EstablishmentGroup {
        numeric id PK
        nvarchar groupId
        nvarchar name
        nvarchar localAuthority_code FK
        nvarchar type_code FK
    }
    GroupLink {
        numeric id PK
        numeric urn FK
        numeric group_id FK
        nvarchar linkType
        tinyint archived
    }

    UserRole ||--o{ UserGroup : broad_role
    UserGroup ||--o{ SystemUser : has_users
    Establishment ||--o{ SystemUser : direct_user_scope
    LocalAuthority ||--o{ SystemUser : local_authority_scope
    EstablishmentGroup ||--o{ SystemUser : direct_group_scope
    LocalAuthority ||--o{ Establishment : contains_establishments
    LocalAuthority ||--o{ EstablishmentGroup : classifies_group
    EstablishmentType ||--o{ Establishment : classifies
    EstablishmentStatus ||--o{ Establishment : describes_status
    Establishment ||--o{ GroupLink : linked_establishment
    EstablishmentGroup ||--o{ GroupLink : linked_group
```

### SystemUser

Business-friendly pattern:

```text
For this signed-in user,
what organisation do they represent?
```

### UserGroup And UserRole

Business-friendly pattern:

```text
For this signed-in user,
which user group and broad role describe the policy context?
```

### Establishment, LocalAuthority And EstablishmentGroup

Business-friendly pattern:

```text
For this user scope,
which establishment, local authority or group records may be in scope?
```

## Reading This Diagram

Use this model to understand scope inputs for access-control decisions. The diagram shows the relationships that supply context; the final access decision still needs policy rules over role, group, status, type and scope.
