# Web Front End Downloads

This document describes how the GIAS web front end handles downloads for users.

It documents the download feature in the ASP.NET web application in `Web/Edubase.Web.UI` and the service clients it uses. The web application is mainly responsible for the user journey. Most file generation is done by the back-end APIs.

## Overview

From a front-end perspective, downloads fall into four main patterns:

- Files listed on the `/Downloads` page
- Generated extracts requested from the `/Downloads` page
- Search-result exports for establishments, groups, governors, and change history
- Direct record/report downloads that already have a ready file URL

The web application does not usually build CSV or XLSX files itself. Instead it:

- Renders the download UI
- Collects user choices such as format, dataset, and selected fields
- Calls the back-end download APIs
- Polls for progress when generation is asynchronous
- Streams or redirects the final file to the browser

## Main web download flows

### Downloads page

`DownloadsController.Index` loads:

- The published download list
- The scheduled extract list

It does this through `IDownloadsService`, which is implemented by `DownloadsApiService`.

The returned `FileDownload` metadata includes:

- `Name`
- `Url`
- `Description`
- `FileSizeInBytes`
- `Tag`
- `RequiresGeneration`
- `AuthenticationRequired`
- `FileGeneratedDate`

The front end groups those files in `DownloadsViewModel` into user-facing sections such as:

- Establishment downloads
- Establishment groups
- Governors
- Miscellaneous

### Immediate file download

If a file already exists, the front end can download it immediately.

Two common patterns are used:

- `DownloadFileAsync` calls the file URL returned in `FileDownload.Url` and streams the response back to the browser
- Some entity/report endpoints return a `DownloadDto` with a ready URL, and the web app redirects straight to that URL

This is used for examples such as:

- Published files from the `/Downloads` page
- Establishment data download
- Group data download
- Single-entity change history download
- Governance change history download
- MAT closure report

### Generated extract flow

Some files are generated on demand.

The front-end flow is:

1. The user chooses a download
2. The controller posts a generate request to the back end
3. The back end returns either a generation `Guid` or an immediate `ProgressDto`
4. The front end redirects to a "preparing file" page
5. The browser polls an AJAX progress endpoint
6. When the file is ready, the UI shows a ready-to-download page

This pattern is used by:

- Generated downloads from the `/Downloads` page
- Scheduled extracts
- Establishment search exports
- Group search exports
- Governor search exports
- Search-based change history exports
- Independent schools significant-dates export



### Final availability check

For generated extracts, `DownloadExtractAsync` checks:

- `download/available?resource=...&id=...`

before redirecting the browser to the final file URL.

This lets the front end show a friendly error page if the generated file is no longer available.

## What data users can download

### Published datasets

The `/Downloads` page exposes published datasets supplied by the back end.

Based on the front-end grouping and labels, these include examples such as:

- All establishment data
- Establishment links
- Establishment additional addresses
- Open academies and free schools data
- Open academies and free schools links
- Open state-funded schools data
- Open state-funded schools links
- Open children's centres data
- Open children's centres links
- All group data
- Group links
- Group data with links
- SAT and MAT membership history
- Academy sponsor and trust links
- All governance records
- MAT governance records
- Academy governance records
- LA maintained governance records

The exact list for a given day comes from the back end. The web app groups and labels it, but does not define the dataset content itself.

### Establishment search exports

The establishment search journey supports the richest download options.

The front end can send:

- Dataset type: `Core`, `Full`, `IEBT`, or `Custom`
- File format: CSV or XLSX
- Whether to include links
- Whether to include email addresses
- Whether to include IEBT fields
- Whether to include bring-up fields
- Whether to include children's centre fields
- Selected custom fields

Some of these options are only shown to users in specific roles.

### Group search exports

Group search downloads are simpler. The front end sends:

- The current search criteria
- The selected file format: CSV or XLSX

### Governor search exports

Governor search downloads support:

- The current search criteria
- The selected file format: CSV or XLSX
- An option to include non-public data

The web front end only offers the non-public data option to these roles:

- `EDUBASE`
- `EDUBASE_CMT`
- `EFADO`
- `edubase_ddce`
- `SFC`

### Change history downloads

The front end supports:

- Direct downloads for one establishment or one group
- Generated downloads for broader filtered change-history result sets

The request can include filters such as:

- File format
- Date range
- Applied / approved / effective date mode
- Suggester group
- Approver group
- Selected establishment fields
- Establishment types
- Group types

### Detail-page and specialist downloads

The web front end also supports direct downloads for:

- A single establishment record
- A single group record
- Establishment change history
- Group change history
- Establishment governance change history
- Group governance change history
- MAT closure report

`ToolsController` also provides an independent schools significant-dates export. In the front end, this is implemented as an establishment search download using the `IEBT` dataset in XLSX format.

## Where downloads come from

### MVC controllers

The main front-end entry points are:

- `DownloadsController`
- `Areas/Establishments/Controllers/EstablishmentsSearchController`
- `Areas/Groups/Controllers/GroupSearchController`
- `Areas/Governors/Controllers/GovernorSearchController`
- `ChangeHistoryController`
- `ToolsController`

These controllers manage the user flow, validation, route state, and error handling.

### Front-end service clients

The web application calls typed services rather than constructing raw HTTP requests in controllers.

Important download-related service clients are:

- `DownloadsApiService`
- `EstablishmentDownloadApiService`
- `GroupDownloadApiService`
- `GovernorDownloadApiService`
- `ChangeHistoryService`

These are registered in `App_Start/IocConfig.cs`.

### Back-end APIs

The front end gets its download data from the configured GIAS back-end API base address.

Examples of endpoints used by the front end include:

- `downloads`
- `scheduled-extracts`
- `download/collate`
- `download/generate/{id}`
- `download/progress/{id}`
- `scheduled-extract/generate/{id}`
- `scheduled-extract/progress/{id}`
- `download/available`
- `establishment/search/download/generate`
- `group/search/download/generate`
- `governor/search/download/generate`

So from the web component's perspective, downloads come mainly from the back end over authenticated HTTP calls.

## How downloads are configured in the web front end

### API connectivity

The main front-end configuration for downloads is the API client setup in `IocConfig`.

Important settings include:

- the back-end API base address setting
- `api:Username`
- `api:Password`
- `HttpClient_Timeout`

These settings control where the web app sends download requests and how it authenticates itself to the back end.

### User identity on API calls

`HttpClientWrapper` adds request headers that matter for download behaviour:

- `sa_user_id`
- `X-Source-IP`

This means the web front end calls the back end as the web application, but also identifies which signed-in user the request is acting on behalf of.

### UI grouping and labels

The front end configures the presentation of downloads by:

- Grouping files by `Tag` in `DownloadsViewModel`
- Looking up friendly labels in `FileDownloadNames.resx`

This affects how downloads are shown to users, but not the actual data content inside the files.

### User-selected download options

For generated exports, the front end configures the output by the payload it sends to the back end.

That payload can include:

- File format
- Selected dataset
- Search filters
- Optional field groups
- Selected custom fields
- Whether non-public data should be included

So in practical terms, the web front end configures downloads by combining:

- App settings for API connectivity
- Role-based UI rules
- The user's current search or selection choices

## Summary

The web front end is the user-facing orchestration layer for downloads.

It:

- Shows users which downloads exist
- Lets them choose formats and field sets
- Starts back-end generation jobs
- Polls progress
- Handles download errors
- Streams or redirects final files

