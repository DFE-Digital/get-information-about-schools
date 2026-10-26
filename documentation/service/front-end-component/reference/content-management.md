# Content Management

The `GIAS Web Front End` includes a small set of built-in content management features for website-owned editorial content. This is not a full content management system for general page authoring. Instead, it supports a limited number of static or semi-static content types that administrators can maintain through the web application.



## What content is managed

The front end provides admin-maintained CRUD-style features for:

- Notification banners
- Notification templates
- News articles
- FAQ groups and FAQ items
- Glossary terms

These features are implemented in:

- `NotificationsController`
- `NewsController`
- `FaqController`
- `GlossaryController`

All editing routes are protected with `EdubaseAuthorize(Roles = AuthorizedRoles.IsAdmin)`, so this content management capability is limited to administrators.

## How it works

The public site renders this content as part of the normal server-rendered MVC application:

- Banners are surfaced through the notifications partial used by the site chrome
- News articles are listed and displayed through the news pages
- FAQs are shown as grouped help content
- Glossary entries are displayed in the glossary pages

Administrators can create, edit, delete and in some cases reorder or preview this content through dedicated maintenance screens.

## Where the content is stored

This content is stored directly by the web tier in Azure Table Storage through front-end-owned repositories:

- `NotificationBannerRepository`
- `NotificationTemplateRepository`
- `NewsArticleRepository`
- `FaqItemRepository`
- `FaqGroupRepository`
- `GlossaryRepository`

These repositories are registered directly in the web application's dependency injection setup and inherit from `TableStorageBase<T>`.

## Feature-specific behaviour

Some of these content types include a little more than basic create/edit/delete:

- Notification banners support scheduling, visibility state and audit history
- Notification templates allow reusable banner content to be managed separately from live banners
- News articles support preview/edit flows and audit/archive history
- FAQ groups and items support display ordering

