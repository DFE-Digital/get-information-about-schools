# Inbox Recipients And Email

This page explains legacy inbox messages, recipients, email attachments and automated email templates.

## Scope

This model covers:

- inbox messages;
- recipient scope;
- email attachments;
- automated email templates and template audit.

## How To Read This Model

- The inbox message table stores multiple communication types.
- A recipient can be linked to a user, establishment, local authority or free-form address information.
- Email attachments belong to inbox email messages, not to the document library.
- Automated email templates have approval and sign-off state.

## Application-Derived Insights

- Inbox messages mix email and phone-call concepts in one table.
- Recipient rows are both addressing data and scope data.
- Template approval is separate from message delivery evidence.
- Future design should model communication record, recipient, attachment, template and delivery outcome separately.

## Inbox Messages

```mermaid
erDiagram
    InboxMessage {
        numeric id PK
        nvarchar type
        nvarchar subject
        nvarchar body
        datetime created
        nvarchar messageId
        tinyint imported
        tinyint hasAttachments
        nvarchar loggedBy_username FK
    }
    Recipient {
        numeric id PK
        nvarchar type
        nvarchar info
        numeric messageId FK
        nvarchar user_username FK
        numeric establishment_URN FK
        nvarchar localAuthority_code FK
    }
    EmailAttachment {
        numeric id PK
        nvarchar fileName
        numeric fileSize
        numeric email_id FK
    }
    SystemUser {
        nvarchar username PK
        nvarchar UserGroupCode FK
    }
    Establishment {
        numeric URN PK
        nvarchar EstablishmentName
    }
    LocalAuthority {
        nvarchar code PK
        nvarchar name
    }

    InboxMessage ||--o{ Recipient : recipients
    InboxMessage ||--o{ EmailAttachment : email_attachments
    SystemUser ||--o{ InboxMessage : logged_by
    SystemUser ||--o{ Recipient : recipient_user
    Establishment ||--o{ Recipient : recipient_establishment
    LocalAuthority ||--o{ Recipient : recipient_local_authority
```

### InboxMessage

Business-friendly pattern:

```text
For this communication record,
what message was recorded,
when was it created,
and was it imported or logged manually?
```

### Recipient

Business-friendly pattern:

```text
For this message,
who is it for, copied to or from,
and what user, establishment or local authority scope applies?
```

### EmailAttachment

Business-friendly pattern:

```text
For this email message,
which attachment filename and size were recorded?
```

## Automated Email Templates

```mermaid
erDiagram
    AutomatedEmailTemplate {
        numeric id PK
        nvarchar code UK
        nvarchar subject
        nvarchar body
        nvarchar description
        tinyint approved
        int version
        nvarchar signOffUser_username FK
    }
    AutomatedEmailTemplateAud {
        numeric id PK
        numeric ver_rev PK
        tinyint REVTYPE
        nvarchar code
        nvarchar subject
        tinyint approved
        nvarchar signOffUser_username
    }
    RevisionInfo {
        numeric id PK
        numeric timestamp
    }
    SystemUser {
        nvarchar username PK
    }

    SystemUser ||--o{ AutomatedEmailTemplate : sign_off_user
    AutomatedEmailTemplate ||--o{ AutomatedEmailTemplateAud : audited_versions
    RevisionInfo ||--o{ AutomatedEmailTemplateAud : revision
```

### AutomatedEmailTemplate

Business-friendly pattern:

```text
For this automated email,
what subject and body template should be used,
and has it been approved for use?
```

## Reading This Diagram

Use this model to understand legacy communications storage. It should not be treated as the same concern as provider data, document publishing or change-request audit.
