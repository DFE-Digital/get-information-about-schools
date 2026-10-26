# Guidance and Blob Resources

This document describes how the `GIAS Web Front End` delivers guidance content and supporting files.

## Overview

Guidance and content delivery in the web front end combines static MVC guidance pages, direct and proxied Azure Blob Storage file access, and a small amount of runtime transformation for the LA name/code guidance data. Most guidance pages are standard Razor content, while supporting files and downloadable guidance assets are delivered from the `guidance` and `content` blob containers.

The web front end delivers guidance and supporting content in three main ways:

- Server-rendered guidance pages within the application
- Direct file delivery from Azure Blob Storage
- Generated downloads based on guidance data files

The relevant implementation is mainly in:

- `GuidanceController.cs`
- `HomeController.cs`
- `BlobService.cs`

## Guidance Pages

The `GuidanceController` serves the main in-app guidance pages.

These include routes such as:

- `Guidance/General`
- `Guidance/EstablishmentBulkUpdate`
- `Guidance/ChildrensCentre`
- `Guidance/Federation`
- `Guidance/Governance`
- `Guidance/LaNameCodes`

The guidance index page is `Views/Guidance/Index.cshtml`, which links users to these guidance sections.

These guidance pages are normal MVC views rendered by the web application. They are not fetched from blob storage at request time.

## Static Guidance Content in Views

Much of the guidance content is embedded directly in Razor views under `Views/Guidance`.

This means the front end delivers:

- The HTML guidance text
- Page structure and navigation
- Locally hosted images used in guidance pages

Examples of static guidance view files include:

- `Views/Guidance/General.cshtml`
- `Views/Guidance/EstablishmentBulkUpdate.cshtml`
- `Views/Guidance/ChildrensCentre.cshtml`
- `Views/Guidance/Federation.cshtml`
- `Views/Guidance/Governance.cshtml`

Many of these views also reference local static image assets under `public/assets/images/guidance`.

## Blob-Backed Content Delivery

The web front end can also serve files directly from Azure Blob Storage.

This is handled in `HomeController.cs` through:

- `~/content`
- `~/content/guidance`

These routes call a shared helper that:

- Gets a blob reference for the requested file
- Checks whether the blob exists
- Opens the blob stream
- Returns it as a `FileStreamResult`

The two main blob containers used here are:

- `content`
- `guidance`

This gives the application a general mechanism for serving supporting files from blob storage rather than from the local web root.

## Blob Access Layer

Blob access is abstracted behind IBlobService.cs.

The blob service supports:

- Obtaining blob references
- Checking whether blobs exist
- Downloading blobs into memory streams
- Uploading blobs
- Creating ZIP archives in memory
- creating read-only shared access URLs

For guidance and content delivery, the web front end mainly uses:

- `GetBlobReference(...)`
- `ArchiveBlobAsync(...)`

Although `BlobService` supports `UploadAsync(...)`, I could not find any front-end controller or admin workflow that uses those upload methods for guidance files.

That means this subcomponent behaves as a blob-backed content reader and file-delivery mechanism, not as an in-app blob content authoring or upload tool.

## Guidance PDF Delivery

There is also a guidance page in `Views/Home/Guidance.cshtml` that links directly to PDF files stored in the guidance blob container.

These links point to blob URLs such as:

- General guidance PDF
- Establishment bulk update guidance PDF
- Children's centres guidance PDF
- Federation guidance PDF
- Governance guidance PDF

In this case, the browser downloads or opens the guidance PDFs directly from Azure Blob Storage rather than through an MVC file-streaming action.

So the front end supports both:

- Proxied blob delivery through MVC routes
- Direct public blob links from the page

Based on the code in this front end, administrators do not appear to upload those PDFs through the web interface. The files look to be provisioned outside this MVC application and then served or linked by it.

## Local Authority Name and Codes Guidance

The `LaNameCodes` guidance flow is more dynamic than the other guidance pages.

In `GuidanceController.cs`, the front end reads three CSV files from the `guidance` blob container:

- `EnglishLaNameCodes.csv`
- `WelshLaNameCodes.csv`
- `OtherLaNameCodes.csv`

The controller downloads each blob into memory and parses it with `CsvHelper`, then renders the data into the guidance page view model.

This means the content shown on the page is not hard-coded into the Razor view. It is loaded from blob-hosted CSV data at runtime.

## Guidance Data Download Flow

The `LaNameCodes` feature also lets the user download a selected data file in a chosen format.

The flow is:

1. The user selects which LA dataset to download.
2. The user selects a file format.
3. The controller finds the matching blob in the `guidance` container.
4. The blob is downloaded into a memory stream.
5. The web front end wraps that file inside a ZIP archive using `ArchiveBlobAsync(...)`.
6. The ZIP is stored temporarily in `TempData["ArchivedBlob"]`.
7. The user receives a `Results.zip` download from `LaNameCodesDownload`.

This is a front-end generated archive. The app does not ask the back end to generate this ZIP.

## What This Subcomponent Owns

The guidance and content delivery subcomponent is responsible for:

- Rendering the guidance/help pages
- Linking to downloadable guidance resources
- Reading blob-hosted supporting files
- Serving certain blob files through MVC actions
- transforming guidance data blobs into downloadable ZIP output

It is not responsible for:

- The main school/group/governor data model
- Search or results downloads
- Back-end report generation
- Uploading or authoring blob-hosted guidance files through an admin UI


