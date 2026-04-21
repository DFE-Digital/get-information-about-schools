# Azure Table Storage Usage

## Overview

The GIAS web front end uses Azure Table Storage as its main directly-managed persistence store.

This storage is used for web-owned data rather than the core establishment, group, governor, approval, and extract domain model. Core GIAS business data is retrieved and updated through the back-end APIs. Azure Table Storage is used by the web application for supporting content, UI state, user preferences, operational diagnostics, and a small number of administration features.

The integration is implemented through `Edubase.Data.Repositories.TableStorage.TableStorageBase<T>`, which:

- reads the `DataConnectionString`
- creates an Azure `CloudTableClient`
- resolves a table per entity type
- ensures the table exists
- applies an exponential retry policy

Most repository classes in `Web/Edubase.Data/Repositories` inherit from this base class.

## Main usage pattern

The web application uses Azure Table Storage in three broad ways:

1. Persisted UI and user state
2. CMS-style and operational content managed in the web tier
3. Diagnostics and support data for the web application itself

## Data types stored

### 1. Search tokens

Entity:

- `Token`

Stored data:

- serialized search and filter state in the `Data` field

Used for:

- persisting search/filter form state
- loading results and downloads using `?tok=<token>`
- restoring saved searches after login

Main usage points:

- `SearchApiController`
- `TokenValueProviderFactory`
- `AccountController`

This is documented in more detail in [`tokens.md`](../tokens/).

### 2. User preferences

Entity:

- `UserPreference`

Stored data:

- `SavedSearchToken`

Used for:

- remembering a signed-in user's chosen saved search token
- redirecting the user back to their saved search after sign-in

Main usage points:

- `UtilApiController`
- `AccountController`
- `EstablishmentsSearchController`

### 3. Local authority sets

Entity:

- `LocalAuthoritySet`

Stored data:

- a title
- a BSON-serialized list of local authority IDs

Used for:

- predefined local authority sets in the tools area
- reusing saved authority selections in independent school search workflows

Main usage points:

- `ToolsController`

### 4. FAQ groups

Entity:

- `FaqGroup`

Stored data:

- group name
- display order

Used for:

- grouping FAQ content for display
- maintaining editable ordering in the admin UI

Main usage points:

- `FaqController`

### 5. FAQ items

Entity:

- `FaqItem`

Stored data:

- title
- content
- display order
- owning FAQ group ID

Used for:

- rendering public FAQ content
- allowing administrators to create, edit, delete, regroup, and reorder FAQ entries

Main usage points:

- `FaqController`

### 6. Glossary items

Entity:

- `GlossaryItem`

Stored data:

- title
- content

Used for:

- rendering glossary content in the public web UI
- allowing administrators to create, edit, and delete glossary terms

Main usage points:

- `GlossaryController`

### 7. Notification templates

Entity:

- `NotificationTemplate`

Stored data:

- reusable template content

Used for:

- providing pre-authored content when administrators create notification banners
- reducing repeated manual editing of common banner messages

Main usage points:

- `NotificationsController`

### 8. Notification banners

Entity:

- `NotificationBanner`

Stored data:

- banner content
- importance
- start and end times
- optional links
- version and audit metadata
- tracker ID
- current/archive partition state

Used for:

- showing time-bound notification banners across the site
- supporting admin-managed banner creation and editing
- retaining audit/history snapshots of previous versions and deletions

Important behaviour:

- active banners are cached in memory for the public template path
- updates first archive the existing version, then replace the live version
- deletions also create archive/audit records

Main usage points:

- `NotificationsController`
- `_NotificationsBannersPartial`

### 9. News articles

Entity:

- `NewsArticle`

Stored data:

- title
- content
- article date
- show/hide date flag
- version and audit metadata
- tracker ID
- current/archive partition state

Used for:

- rendering public news content
- allowing administrators to manage future and live news items
- preserving historical versions and delete audit records

Important behaviour:

- live filtering is based on `ArticleDate`
- updates and deletes create archive snapshots

Main usage points:

- `NewsController`
- `HomeController`

### 10. Data quality status records

Entity:

- `DataQualityStatus`

Stored data:

- establishment-type dataset identifier
- last updated date
- data owner name
- data owner email

Used for:

- showing freshness/status information for key establishment datasets
- allowing authorised users to confirm their dataset has been reviewed
- allowing administrators to maintain data owner details

Important behaviour:

- records are seeded if the table is empty
- row keys are tied to specific enum values, so the enum ordering matters

Main usage points:

- `DataQualityController`
- `DataQualityReadService`
- `DataQualityWriteService`

### 11. API recorder session items

Entity:

- `ApiRecorderSessionItem`

Stored data:

- HTTP method
- path
- request headers
- response headers
- raw request body
- raw response body
- elapsed time
- session ID / user-scoped partition key

Used for:

- optional recording of front-end to back-end API calls
- diagnostics for troubleshooting REST interactions made by the web front end

Important behaviour:

- records are only written when API logging is enabled
- the logging is performed inside `HttpClientWrapper`
- entries are partitioned by session or user identifier

Main usage points:

- `HttpClientWrapper`
- `ToolsController` (`ApiSessionRecorder`)

### 12. Web log records

Entity:

- `AZTLoggerMessages`

Stored data:

- timestamp
- environment
- log level
- message
- exception text
- URL and request details
- client IP address
- user ID and user name
- user agent and referrer

Used for:

- storing application and operational log messages in Azure Table Storage
- supporting the admin log viewer
- querying logs by ID or date range

Important behaviour:

- logs are written by the Azure table logging service configured in `IocConfig`
- the web application reads them back via `WebLogItemRepository`

Main usage points:

- `IocConfig`
- `AdminController`
- `WebLogItemRepository`

## Summary by purpose

### UI state and user-specific state

- `Token`
- `UserPreference`
- `LocalAuthoritySet`

These support saved searches, reusable selections, and browser/UI workflows.

### Managed site content

- `FaqGroup`
- `FaqItem`
- `GlossaryItem`
- `NotificationTemplate`
- `NotificationBanner`
- `NewsArticle`

These allow the web tier to own and manage content without routing that content through the Java back end.

### Operational and governance support

- `DataQualityStatus`

This supports data stewardship and review workflows presented in the web UI.

### Diagnostics and support

- `ApiRecorderSessionItem`
- `AZTLoggerMessages`

These support troubleshooting and operational visibility for the web application.
