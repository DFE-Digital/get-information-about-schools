# API Session Recorder and Logging

The `GIAS Web Front End` contains a small set of operational diagnostics features that help support teams investigate failures and integration issues.

The front end includes a limited diagnostics layer for support and investigation:

- API session recording captures detailed outbound API traffic when explicitly enabled
- web logging captures unexpected application errors and operational log messages

Because API session recording can capture sensitive request and response data, it is disabled by default and is used as a temporary troubleshooting aid rather than normal runtime behaviour.

## API session recorder

The web application calls the main GIAS back-end through `HttpClientWrapper`.

When API logging is enabled, `HttpClientWrapper` records details of each outbound API call into Azure Table Storage using `ApiRecorderSessionItemRepository`.

Each `ApiRecorderSessionItem` stores:

- HTTP method
- Request path
- Request headers
- Response headers
- Raw request body
- Raw response body
- Elapsed duration as text and milliseconds

The partition key is based on the current user ID where available. If no user ID is available, the record is stored under `global`.

This makes the feature useful for tracing exactly what the front end sent to the back-end APIs, what came back, and how long the call took.

## Configuration and sensitivity

API session recording is controlled by the `EnableApiLogging` app setting.

In the checked-in configuration it is disabled by default.

This is intentional because the recorded data is highly detailed and can contain sensitive information, including:

- Request and response headers
- Request bodies
- Response bodies
- Identifiers associated with the current user

The configuration comments explicitly warn that API logging should not be routinely enabled in production and that any logged data should be tightly controlled and removed as soon as practical.

## Web and exception logging

Separate from API call recording, the web application also writes operational log messages and unexpected exception details to Azure Table Storage using the Azure Table Logger integration.

This is wired through:

- `ILoggingService`
- `IAzLogger`
- the `AZTLoggerMessages` storage table

Unexpected MVC exceptions are captured by the custom exception filter and logged with information such as:

- Environment
- Exception text
- Message
- URL
- HTTP method
- User ID and user name
- Client IP address
- Referrer URL
- Request body where available

This is application error logging rather than detailed API tracing.

## Admin access

Administrators can view stored web log messages through the admin log viewer.

The admin log screen supports:

- Searching by log ID
- Searching within a date range
- Filtering across key text fields
- Excluding routine purge messages

This allows for operational troubleshooting.

## Storage

Both of these diagnostics features use Azure Table Storage directly from the web tier:

- API session recordings are stored through `ApiRecorderSessionItemRepository`
- web and exception logs are stored through the `AZTLoggerMessages` table and accessed through `WebLogItemRepository`



