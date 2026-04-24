# Change History

This document describes how the `GIAS Web Front End` implements change history search and download.



## Overview

The change history feature in the web front end is a filtered audit-search interface for establishment and group changes. It supports both single-record and cross-entity history searches, uses AJAX to refresh filtered results, relies on typed back-end services for search and download, and switches between direct-download and asynchronous generation depending on whether the user is downloading history for one record or for a broader filtered search.


The change history feature lets users search and download historical changes for:

- Establishments
- Groups

The front end supports two main modes:

- Search for the change history of a specific establishment or group
- Browse change history across all establishments or all groups

The feature is implemented in ChangeHistoryController.cs.

## Entry Flow

The entry route is `ChangeHistory/`.

From there, the front end:

- Captures whether the user is searching establishments or groups
- Validates the initial search request
- Redirects to the correct specialist route

The main routing split is:

- `ChangeHistory/Search/Establishments`
- `ChangeHistory/Search/Groups`

This is handled by `SearchChangeHistory(...)` in the controller.

## Search Modes

### Establishments

Establishment change history supports:

- A specific establishment search using text, URN, LAESTAB or UKPRN
- An all-establishments browse mode

For specific-establishment search, the front end first resolves the establishment identity using the same general establishment search services used elsewhere in the application. Once the establishment is identified, it calls `GetChangeHistoryAsync(...)` on `IEstablishmentReadService`.

For all-establishments browsing, the front end builds a shared change-history browse payload and calls the generic change-history service instead.

### Groups

Group change history supports:

- A specific group search
- An all-groups browse mode

For specific-group search, the front end resolves the group UID from auto-suggest or search text, then calls `GetChangeHistoryAsync(...)` on `IGroupReadService`.

For all-groups browsing, the front end uses the generic change-history service in the same way as the establishment browse mode.

## Search Payload Model

The generic browse and download flows use the shared payload classes in SearchChangeHistoryPayload.cs.

These payloads include:

- `EntityName`
- `SuggesterUserGroupCode`
- `ApproverUserGroupCode`
- Establishment type filters
- Group type filters
- Establishment field filters
- Applied date range
- Effective date range
- Approved date range
- Sort order

The browse payload is `SearchChangeHistoryBrowsePayload.cs`, which adds:

- `Skip`
- `Take`

The download payload is `SearchChangeHistoryDownloadPayload.cs`, which adds file format.

## View Model and Filter Options

The front-end filter state is carried in ChangeHistoryViewModel.cs.

Important filter areas include:

- Establishment fields
- Establishment types
- Group types
- Suggester group
- Approver group
- Date range
- Date filter mode

The date filter mode supports:

- effective date
- Applied date
- approved date

For establishment history, the front end can also filter by changed field IDs through `SelectedEstablishmentFields`.

## Services Used

The shared browse/download service is ChangeHistoryService.cs.

It calls the back-end endpoints for:

- Generic change-history search
- Generic change-history download generation
- Change-history download progress
- Suggester groups
- Establishment field metadata

For single-record history, the front end uses more specific services:

- `IEstablishmentReadService` for establishment-specific history and downloads
- `IGroupReadService` for group-specific history
- `IGroupDownloadService` for group-specific history downloads

## Result Shapes

The front end renders a unified result shape using ChangeHistorySearchItem.cs.

That model includes:

- Establishment or group name
- Suggester name
- Approver name
- Effective date
- Requested date
- Changed date
- Field name
- Old value
- New value

For group change history it can also include:

- Group UID
- Change request type
- Link type
- Linked establishment details

The controller converts establishment-specific and group-specific DTOs into this shared shape before rendering.

## Lookup and Filter List Population

Before rendering the results page, the controller loads supporting filter lists using:

- `IChangeHistoryService.GetSuggesterGroupsAsync(...)`
- `IChangeHistoryService.GetEstablishmentFieldsAsync(...)`
- `ICachedLookupService.EstablishmentTypesGetAllAsync(...)`
- `ICachedLookupService.GroupTypesGetAllAsync(...)`

This is how the results page gets the selectable:

- Suggester groups
- Approver groups
- Establishment fields
- Establishment types
- Group types

## AJAX Result Refresh

The results page supports AJAX filter refresh.

The browser-side logic is implemented in `GiasSearchChangeHistory.js`.

That script:

- Watches filter changes
- Validates date filters
- Serialises the current filter form
- Updates browser history
- Calls the correct partial endpoint
- Updates the results container
- Updates the download link
- Updates the result count notification

The partial endpoints are:

- `/ChangeHistory/Search/Establishments/results-js`
- `/ChangeHistory/Search/Groups/results-js`

The controller returns `x-count` headers so the client can update the visible result count.

## Download Behaviour

The download flow is handled by `ChangeHistory/Search/Download`.

The exact path depends on whether the user is looking at:

- A single establishment
- A single group
- all establishments
- All groups

### Single Establishment

For a specific establishment, the front end builds `EstablishmentChangeHistoryDownloadFilters.cs` and calls the establishment read service directly.

These filters can include:

- File format
- Approved by
- Suggested by
- Fields updated
- eEffective/applied/approved date range

The back end returns a ready file location, so the front end can show a ready-to-download page immediately.

### Single Group

For a specific group, the front end uses `IGroupDownloadService.DownloadGroupHistory(...)`.

This is also a direct download-style flow, using:

- Format
- Date from/to
- Suggested by

The back end returns a ready file location and size.

### All Establishments or All Groups

For browse-all searches, the front end starts an asynchronous download-generation task through `IChangeHistoryService.SearchWithDownloadGenerationAsync(...)`.

The progress page then polls:

- `change-history/download/progress/{id}`

through `GetDownloadGenerationProgressAsync(...)` until the file is ready.

## Download Size Warning

The client-side change-history script shows a modal warning when the current result set exceeds 19,999 records.

In that case, the user is told:

- To filter the search to fewer than 20,000 changes

This is a front-end safeguard applied before download.



