# News Announcements And Notifications

This page explains news, announcement and frontend notification tables.

## Scope

This model covers:

- legacy news stories and targeted user groups;
- legacy announcement templates and publication logs;
- frontend news articles;
- frontend notification templates and banners.

## How To Read This Model

- Legacy news stories combine content, approval and audience targeting.
- Announcement templates are used to create publish or remove attempts.
- Frontend notification tables use a content-store style key shape.
- Announcement publication identifiers are integration references, not physical foreign keys.

## Application-Derived Insights

- News and announcements are communication models, not provider facts.
- Legacy and frontend news models are separate shapes and should not be merged without confirming the source of truth.
- Announcement templates mix content, audience targeting, trigger/action classification and publication behaviour.
- Future design should separate message template, target audience, publication event and delivery state.

## Legacy News

```mermaid
erDiagram
    NewsStory {
        numeric id PK
        nvarchar title
        nvarchar summary
        nvarchar text
        bit approved
        datetime effectiveDate
        datetime createdAt
        datetime updatedAt
        nvarchar createdBy_username FK
        nvarchar signOffUser_username FK
        nvarchar signedOffBy_username FK
    }
    NewsStoryUserGroup {
        numeric NewsStory_id PK
        nvarchar groups_code PK
    }
    UserGroup {
        nvarchar code PK
        nvarchar name
        nvarchar role
    }
    SystemUser {
        nvarchar username PK
        nvarchar UserGroupCode FK
    }

    NewsStory ||--o{ NewsStoryUserGroup : targeted_groups
    UserGroup ||--o{ NewsStoryUserGroup : audience_group
    SystemUser ||--o{ NewsStory : created_by
    SystemUser ||--o{ NewsStory : sign_off_user
    SystemUser ||--o{ NewsStory : signed_off_by
```

### NewsStory

Business-friendly pattern:

```text
For this legacy news story,
what content should be shown,
from what effective date,
and which user groups should see it after approval?
```

### NewsStoryUserGroup

Business-friendly pattern:

```text
For this news story,
which user groups should see it?
```

## Legacy Announcements

```mermaid
erDiagram
    AnnouncementTemplate {
        numeric id PK
        nvarchar announcementType
        nvarchar applicationId
        varchar announcementTitle
        varchar announcementSummary
        int expirationAfterDays
        bit enabled
        bit canBeSentManually
        numeric organisationId
        nvarchar announcementAction_code FK
    }
    AnnouncementTemplateUserGroup {
        numeric AnnouncementTemplate_id PK
        nvarchar groups_code PK
    }
    AnnouncementCollection {
        numeric id PK
        nvarchar announcementType
        numeric template_id FK
        numeric saOrgId
        numeric urn
        numeric uid
        bit publish
        bit published
        bit remove
        bit removed
        datetime ts
        datetime datePublished
        datetime dateRemoved
    }
    AnnouncementCollectionValues {
        numeric announcementId PK
        nvarchar fieldKey PK
        nvarchar fieldVal
    }
    EduBaseTrigger {
        nvarchar code PK
        nvarchar name
    }
    UserGroup {
        nvarchar code PK
        nvarchar name
        nvarchar saUserGroupCode
    }

    EduBaseTrigger ||--o{ AnnouncementTemplate : action
    AnnouncementTemplate ||--o{ AnnouncementTemplateUserGroup : selected_groups
    UserGroup ||--o{ AnnouncementTemplateUserGroup : audience_group
    AnnouncementTemplate ||--o{ AnnouncementCollection : creates_publish_log
    AnnouncementCollection ||--o{ AnnouncementCollectionValues : template_values
```

### AnnouncementTemplate

Business-friendly pattern:

```text
For this announcement template,
what message should be prepared,
which trigger or action describes it,
which groups are targeted,
and can it be sent manually?
```

### AnnouncementCollection

Business-friendly pattern:

```text
For this generated announcement,
was it queued to publish or remove,
and what publication outcome was recorded?
```

## Frontend News And Notifications

```mermaid
erDiagram
    FrontEndNewsArticles {
        varchar PartitionKey PK
        varchar RowKey PK
        datetime2 Timestamp
        varchar Title
        datetime2 ArticleDate
        bit ShowDate
        varchar Content
        tinyint Version
    }
    FrontEndNotificationTemplates {
        varchar PartitionKey PK
        varchar RowKey PK
        datetime2 Timestamp
        varchar Content
    }
    FrontEndNotificationBanners {
        varchar PartitionKey PK
        varchar RowKey PK
        datetime2 Timestamp
        tinyint Importance
        varchar Content
        datetime2 Start
        datetime2 End
        tinyint Version
    }

    FrontEndNotificationTemplates ||..o{ FrontEndNotificationBanners : optional_template_source
```

### FrontEndNewsArticles

Business-friendly pattern:

```text
For this frontend news article,
what article content, date and version should be displayed?
```

### FrontEndNotificationBanners

Business-friendly pattern:

```text
For this frontend notification banner,
what content should be shown,
what importance does it have,
and when should it start and end?
```

## Reading This Diagram

Use this model to separate legacy portal communications from frontend content. News, announcements and banners are all communications, but they have different lifecycle and audience rules.
