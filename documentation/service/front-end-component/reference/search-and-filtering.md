# Search and Filtering

This document describes how the `GIAS Web Front End` implements search and filtering.

## Overview

Search in the web front end is split into two layers:

- A common search entry layer that captures the user's initial search intent
- Apecialist search controllers for establishments, groups and governors

The front end is responsible for:

- Capturing the search type and initial query
- Validating the request and showing no-results or error states
- Redirecting to the correct specialist search area
- Building typed search payloads from query-string and filter selections
- Loading lookup data for the filter UI
- Supporting AJAX result refresh
- Tokenising filter state for saved searches, downloads and shareable URLs

The back end is responsible for:

- Executing the actual searches
- Enforcing record visibility and security rules
- Generating downloadable search extracts

## Search Entry Layer

The search landing page is handled by `SearchController.cs`

This controller handles:

- The main search page
- The selected search type
- Validation messages
- No-results handling
- Redirects into specialist search areas
- Auto-suggest endpoints
- Local authority and location disambiguation

Supported search paths include:

- Establishment text search
- Location search
- Local authority search
- Group search
- Governor search
- Governor reference search

Once the input is valid, `IndexResults` redirects to:

- `Establishments/Search`
- `Groups/Search`
- `Governors/Search`

Depending on the selected search type.

## Auto-Suggest and Disambiguation

The shared search layer provides lightweight suggestion endpoints:

- `Suggest` for establishments
- `SuggestGroup` for groups
- `SuggestPlace` for places

These use:

- `IEstablishmentReadService`
- `IGroupReadService`
- `IPlacesLookupService`

The search entry layer also performs disambiguation when:

- A local authority name is ambiguous
- A location search is text that has not yet been resolved to coordinates

In those cases, the user is shown a disambiguation view before reaching the final results page.

## Specialist Search Controllers

### Establishments

`EstablishmentsSearchController.cs` is the richest search implementation.

It supports:

- Text search
- URN search
- UKPRN search
- LAESTAB search
- Location/radius search
- Local authority search
- Large numbers of additional filters
- Paged HTML results
- JSON results for the search map
- Search-driven downloads

It also has direct-to-detail behaviour:

- If the user effectively searched by URN and has access, the controller redirects directly to the establishment details page
- If the result set contains exactly one result and the view model allows it, the controller redirects directly to details

### Groups

Group search is handled by GroupSearchController.cs.

It supports:

- Text search
- UID and identifier-based lookup
- Group type filters
- Group status filters
- Direct redirect to group details when the auto-suggest value identifies a group

If the result set contains only one group, the controller redirects straight to the group details page.

### Governors

Governor search is handled by GovernorSearchController.cs.

It supports:

- Name-based search
- Governor ID search
- Role filters
- Local authority filters
- Governor type flag filters
- Optional inclusion of historic governors

## Search Payloads

Each specialist controller maps the incoming view model into a typed search payload:

- `EstablishmentSearchPayload`
- `GroupSearchPayload`
- `GovernorSearchPayload`

These payloads include combinations of:

- Text terms
- Paging
- Sort order
- Selected filter IDs
- Date ranges
- Role/type/status filters
- Location/radius data where relevant

The establishment payload is the most extensive and includes filters such as:

- Education phase
- Establishment status
- Establishment type
- Local authority
- Religious character
- Admissions policy
- Diocese
- District
- Region
- Section 41 designation
- SEN provision
- Ofsted rating
- Open/close date ranges
- Statutory age ranges

## Filter Lookups

The search pages depend heavily on lookup/reference data loaded from `ICachedLookupService`.

This lookup layer provides the values used to render filter controls such as:

- Local authorities
- Statuses
- Establishment types
- Sducation phases
- Governor roles
- Group types
- Group statuses
- Many search-specific filter lists

This is what allows the front end to build stable filter UIs with consistent labels and IDs.

## Tokenised Filter State

The front end uses tokenised filter state so that search state can be:

- Kept out of long query strings
- Reused across AJAX refreshes
- Saved as a user's saved filter set
- Reused by download links

The mechanism works like this:

1. The browser serialises the current filter form.
2. It posts the form state to `POST /api/tokenize`.
3. `SearchApiController.cs` stores that serialised form in `TokenRepository`.
4. The API returns a token ID.
5. The browser updates the URL to `?tok=...`.
6. On later requests, `TokenValueProviderFactory.cs` loads the token and injects the saved values back into MVC model binding.

This is the same token model used by saved search filters and by the search download flow.

## AJAX Result Refresh

The search result pages support asynchronous filtering without a full page reload.

The browser-side orchestration for this lives in `GiasFiltering.js`

That script:

- Watches filter changes
- Serialises current form state
- Requests a token if one does not already exist
- Calls the relevant `results-js` endpoint with `tok=...`
- Updates the results container
- Updates result counts
- Updates the current download link
- Supports saved filter set behaviour

The main HTML partial refresh endpoints are:

- `/Establishments/Search/results-js`
- `/Groups/Search/results-js`
- `/Governors/Search/results-js`

Establishment search also exposes:

- `/Establishments/Search/results-json`

for the map-based result view.

## Saved Search Filters

Establishment search has an extra saved-filter behaviour.

When an authenticated user returns to establishment search, `EstablishmentsSearchController.cs` checks `UserPreferenceRepository` for a previously saved `SavedSearchToken`.

The client-side filtering script can also:

- Save the current token through `/api/save-search-token`
- Restore the saved token
- Delete the saved token

This gives users a reusable saved filter set on top of the normal tokenised search state.

## Result Rendering Behaviour

The specialist search controllers all follow the same general pattern:

- Validate the model
- Build the payload
- Call the appropriate search service
- Populate lookup-backed display values
- Set the total count for paging
- rReturn either a full page or a partial result view

The front end also applies user-experience behaviour such as:

- Redirecting back to the shared search page with `NoResults=true`
- Redirecting straight to detail pages for exact or single-result matches
- Rxposing counts via response headers such as `x-count`

## Relationship to Downloads

Search and filtering is tightly connected to downloads.

Once the current search state has been tokenised or mapped into a search payload, the corresponding search controller can pass that payload into the relevant download service to generate:

- Establishment search downloads
- Group search downloads
- Governor search downloads

This is why the download links on the search results pages can follow the currently selected filters.


