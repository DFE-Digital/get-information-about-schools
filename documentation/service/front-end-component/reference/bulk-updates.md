# Web Front End Bulk Updates

This document describes how bulk updates work in the GIAS web front end.


## Overview

In the front end, "bulk updates" are spreadsheet-driven batch operations exposed through the tools and management areas.

There are three main bulk-update style flows:

- Establishment bulk update
- Governor bulk update
- Bulk association of establishments to groups

All three follow the same broad pattern:

1. The user opens a protected bulk-update page
2. The browser uploads a spreadsheet or file
3. The MVC controller validates the basic input
4. The file is saved temporarily on the web server
5. The web app sends the file to the back-end API as a multipart request
6. The web app shows either progress, success, validation errors, or an error log

## Access and role control

Bulk update pages are protected by MVC role checks in `AuthorizedRoles`.

The main role groupings are:

- `CanBulkUpdateEstablishments`
- `CanBulkUpdateGovernors`
- `CanBulkAssociateEstabs2Groups`

These are defined in `AuthorizedRoles.cs`

## Common front-end behaviour

Across the different bulk-update flows, the web front end does the following:

- Validates that a file was uploaded
- Checks the file extension
- In some flows, checks that the file is under 1 MB
- Writes the file to a temporary location
- Calls the back-end API through a typed service client
- Deletes the temporary file after the API request
- Renders either:
  - An in-progress page
  - A completed page
  - Inline validation errors
  - A link to an error log file

The web app does not parse the uploaded spreadsheet content in detail. That deeper validation is delegated to the back end.

## Establishment bulk update

The establishment bulk update journey is implemented in `BulkUpdateController.cs`
### What the user provides

The front end asks for:

- A bulk update file
- A file type
- An optional effective date

Admins can also use an override flag for the CR process.

The web-tier validation is defined in BulkUpdateViewModelValidator.cs.

It checks:

- A file type has been selected
- The effective date is either empty or valid
- A file is present and non-empty
- The file extension is `csv`, `xlsx`, or `xls`

The controller also enforces a maximum file size of 1 MB.

### What the front end sends to the back end

The view model is mapped to `BulkUpdateDto` in BulkUpdateViewModelToDtoMapper.cs.

That payload includes:

- `BulkFileType`
- `EffectiveDate`
- `OverrideCRProcess`
- A temporary file path used for the multipart upload

The DTO shape is defined in BulkUpdateDto.cs.

### Processing flow

The front end:

1. Saves the uploaded file to a temp location
2. Calls `IEstablishmentWriteService.BulkUpdateAsync(...)`
3. Redirects to a result page if the API accepts the request
4. Polls progress through `BulkUpdateAsync_GetProgressAsync(...)`

Those API calls are implemented in EstablishmentWriteApiService.cs.

This is an asynchronous batch-task flow. The user sees an "in progress" page until the back end marks the task complete.

## Governor bulk update

The governor bulk update journey is implemented in GovernorsBulkUpdateController.cs.

### What makes it different

Governor bulk update uses a two-step backend process:

1. Validate the uploaded file
2. If validation succeeds, submit the validated batch for processing

This is different from establishment bulk update, which goes more directly into an async batch task.

### Front-end validation

The validator is GovernorsBulkUpdateViewModelValidator.cs.

It requires:

- A file is present
- The file is non-empty
- The extension is `xlsx`

The controller also applies the same 1 MB size limit before sending the file to the API.

### Template download

This journey also has a template download route:

- `DownloadTemplate`

The web app gets the template URI from the backend service and redirects the user to it.

### Processing flow

The front end:

1. Saves the uploaded XLSX file to a temp location
2. Calls `IGovernorsWriteService.BulkUpdateValidateAsync(...)`
3. If validation succeeds, calls `BulkUpdateProcessRequestAsync(...)`
4. If validation fails, shows inline errors or an error-log download

The API client methods are implemented in GovernorsWriteApiService.cs.

The validation response model is GovernorBulkUpdateValidationResult.cs.

That response can contain:

- A backend-generated batch ID
- Success/failure
- Calidation errors
- An error-log file download

## Bulk associate establishments to groups

This journey is implemented in BulkAssociateEstabs2GroupsController.cs.

This is effectively another bulk batch operation, but its purpose is association rather than field update.

### Front-end validation

The validator is BulkAssociateEstabs2GroupsViewModelValidator.cs.

It requires:

- A file is present
- The file is non-empty
- The extension is `csv` or `xls`

### Processing flow

The front end:

1. Saves the uploaded file to a temp location
2. Calls `IEstablishmentWriteService.BulkAssociateEstabs2GroupsAsync(...)`
3. Redirects to a result route if the request is accepted
4. Polls progress with `BulkAssociateEstabs2GroupsGetProgressAsync(...)`

Those service calls are implemented in EstablishmentWriteApiService.cs.

If the batch completes:

- A full success shows a completed view
- Row-level problems can produce an error-log link
- API errors are added to model state and shown back on the form

## Progress and completion model

The shared status model used by establishment bulk update and bulk association is BulkUpdateProgressModel.cs.

It includes:

- `Id`
- `IsComplete`
- `Status`
- `Errors`
- `RowErrors`
- `ErrorLogFile`

Helper methods on that model let the front end distinguish between:

- Still running
- Failed
- Completed successfully
- Completed with row-level errors / partial success

## How the web front end talks to the back end

These flows all use typed API service clients rather than direct HTTP code in the controllers.

The main services are:

- `IEstablishmentWriteService`
- `IGovernorsWriteService`

Their Texuna-backed implementations send multipart uploads to endpoints such as:

- `establishment/bulk-update`
- `bulk-update/progress/{id}`
- `establishment/bulk-associate-to-groups`
- `establishment/bulk-associate-to-groups/progress/{id}`
- `governor/bulk-update`
- `governor/bulk-update/{id}`

The shared `HttpClientWrapper` adds the usual application credentials and user identity headers, so the back end knows both:

- Which client application is calling
- Which signed-in user the request is acting for

## Summary

From the front-end component's perspective, bulk updates are controlled upload-and-progress workflows.

The web application is responsible for:

- Access control
- Upload validation
- Temporary file handling
- Calling the correct back-end endpoint
- Polling status where needed
- Presenting errors and error-log downloads to the user

The back end is responsible for:

- Validating spreadsheet content
- Running the batch process
- Tracking task progress
- Producing row-level error logs
