# Security and Permissions

This document describes how the `GIAS Web Front End` authenticates users and applies permissions.

## Overview

The web application uses a layered security model:

- Authentication is handled in the web front end using SAML.
- The signed-in user is represented by a claims-based principal stored in an auth cookie.
- The front end asks the back-end security API for the user's GIAS roles.
- Controllers, views and helper classes use those roles to decide what the user can see and do.
- The back end remains the final enforcement point for API access and business permissions.

## Authentication

Authentication is configured in StartupSecureAccess.cs.

The startup pipeline configures:

- The application auth cookie
- A short-lived external sign-in cookie
- The Sustainsys SAML integration
- Secure cookie settings
- Anti-forgery identity configuration
- A Content Security Policy nonce for page responses

The SAML identity provider and callback behaviour are driven by app settings such as:

- `ApplicationIdpEntityId`
- `ExternalAuthDefaultCallbackUrl`
- `MetadataLocation`
- `PublicOrigin`
- `SessionExpireTimeSpan`

## Sign-in Flow

The main sign-in logic is in AccountController.cs**.

The flow is:

1. The user is redirected to the SAML identity provider from `Login`.
2. The SAML callback returns an external identity to `ExternalLoginCallback`.
3. The external identity is converted into the application's internal claim format.
4. The front end calls the security API to retrieve the user's GIAS role names.
5. Those roles are added as `ClaimTypes.Role` claims.
6. The user is signed in with the application cookie.
7. The app redirects the user to an appropriate landing page.

Landing pages may depend on role and back-end lookups:

- Establishment users may be redirected to their establishment details page
- MAT or SSAT users may be redirected to their group details page
- Users with a saved search token may be redirected back to that saved search

## Claims Conversion

The SAML assertion is normalised before the app uses it.

In normal operation, `SecureAccessClaimsIdConverter.cs`
extracts:

- Secure Access user ID
- Username
- First name
- Last name

The most important value is the Secure Access user ID, which is written into the app's internal `NameIdentifier` claim using EduClaimTypes.cs.

In simulator mode, `StubClaimsIdConverter.cs` performs the same role using a stub claim source.

The result is a frontend principal with a stable internal user ID claim that the rest of the application can use.

## Roles and Role Resolution

The front end does not hard-code a user's role membership during login. Instead, it asks the back end for the user's role names through SecurityApiService.cs.

That service exposes methods such as:

- `GetRolesAsync`
- `GetCreateGroupPermissionAsync`
- `GetCreateEstablishmentPermissionAsync`
- `GetMyEstablishmentUrn`
- `GetMyMATUId`

This means the back end is the source of truth for:

- Role membership
- Create permissions
- User-to-establishment or user-to-group relationships

Role constants are defined in EdubaseRoles.cs.

## Front-End Authorization

The front end applies authorization in several ways.

### Authentication Gate

`EdubaseAuthorizeAttribute.cs` is the custom "must be signed in" attribute.

Its behaviour is:

- If the user is not authenticated, trigger the SAML challenge
- If the user is authenticated but not allowed, return HTTP 403

### Role-Based Access

`AuthorizeRolesAttribute.cs` provides thin wrappers over the standard MVC and Web API authorize attributes:

- `MvcAuthorizeRolesAttribute`
- `HttpAuthorizeRolesAttribute`

These attributes take one or more role names and convert them into the comma-separated `Roles` string expected by ASP.NET authorization.

### Grouped Permission Sets

The application groups raw roles into reusable permission bundles in AuthorizedRoles.cs.

Examples include:

- `IsAdmin`
- `CanAccessTools`
- `CanBulkUpdateEstablishments`
- `CanBulkUpdateGovernors`
- `CanManageAcademyTrusts`
- `CanBulkCreateFreeSchools`
- `CanBulkAssociateEstabs2Groups`

This allows controller code to express intent more clearly. For example, a controller can require `AuthorizedRoles.CanBulkUpdateEstablishments` instead of repeating a long list of raw roles.

### Role Checks in Code

The app also uses runtime role checks directly in controller and view-model logic.

Examples include:

- `ToolsController.cs`, which decides which tools to show based on role membership and back-end create-permission responses
- `UserRolesController.cs`, which decides whether a user should be prompted about data quality updates
- `DataQualityController.cs`, which combines role membership with dataset ownership rules

Helper methods in `SecurityExtensionMethods.cs` provide convenience checks such as `InRole(...)`.

## Business-Rule Permissions, (ABAC)

Some permissions depend on both role and record state, not just role membership.

`GroupEditorViewModelRulesHandler.cs` is a good example. It decides things like:

- Whether local authority is editable
- Whether the user can close a group or mark it as created in error
- Whether closed date and status can be edited
- Whether UKPRN can be edited
- Whether a review screen must be shown

These checks combine:

- The user's roles
- The type of group
- The current state of the record

This is how the front end implements fine-grained UI permissions for specific workflows.

## Front End to Back End Security Context

When the front end calls the GIAS back-end APIs, it forwards the current user's internal ID in the `sa_user_id` header. This is done in `HttpClientWrapper.cs`

The wrapper also sends:

- The request body
- A source IP header used for rate-limiting and tracing
- Optional API interaction logging data


The back end receives both:

- The front end's service-level credentials
- The current user's Secure Access identifier

The back end can then apply its own authorization rules to the request.

## Key Implementation Pattern

The overall pattern is:

1. SAML authenticates the browser user.
2. The front end converts the SAML identity into its internal claim model.
3. The front end retrieves the user's GIAS roles from the back end.
4. The front end uses role attributes and code-level checks to control pages, actions and UI behaviour.
5. The back end re-checks authorization on API calls.

## Summary

The `GIAS Web Front End` uses SAML for authentication, claims-based identity for session state, grouped role constants for controller-level authorization, and additional code-level checks for workflow-specific permissions. The front end makes many authorization decisions for user experience and page flow, but the back end remains the authoritative enforcement point for business permissions and API access.
