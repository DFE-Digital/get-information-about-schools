# Web Front End Companies House Number

This document describes how the GIAS web front end uses the Companies House number.

In the front end, the Companies House number is mainly used as the identifier that links academy trust and related group records in GIAS to Companies House records.

## Overview

The Companies House number is used in several ways in the web application:

- As a search key
- As an input when creating academy trust records
- As a stored field on group records
- As a link back to the Companies House record
- As a key used by other external and back-end integrations

The most important point is that, for academy trust creation journeys, the number is sourced from Companies House itself and then stored in GIAS.

## Where the Companies House number comes from

### Source for new academy trust creation

When a user creates an academy trust from the front end, the Companies House number comes from the Companies House API.

The relevant flow is in **GroupController.cs**.

The user can search Companies House by:

- Company name
- Company number

If the search text is numeric, the front end calls:

- `ICompaniesHouseService.SearchByCompaniesHouseNumber(...)`

Otherwise it calls:

- `ICompaniesHouseService.SearchByName(...)`

The implementation is in **CompaniesHouseService.cs**.

That service uses the Companies House API and maps the result into a `CompanyProfile`, which contains:

- `Name`
- `Number`
- `IncorporationDate`
- `Address`

See **CompanyProfile.cs**.

So in this creation flow, the Companies House number is not manually invented inside GIAS. It is pulled from the external Companies House record selected by the user.

### Source for existing group records

After a trust or other group record has been created, the number is stored in the GIAS group model as:

- `GroupModel.CompaniesHouseNumber`

See **GroupModel.cs**.

From that point on, the web app usually reads it from the GIAS backend record rather than going back to Companies House every time.

### Establishment field

There is also a `CompaniesHouseNumber` field on establishments:

- **EstablishmentModel.cs**

In the front-end code, this field is mainly displayed or edited on establishment detail screens. I did not find a direct front-end Companies House lookup flow for establishments equivalent to the trust creation flow.

## How the front end uses it

### 1. Search key for groups

The Companies House number can be used as a group search identifier.

The group read service supports searching by:

- `groupId`
- `groupUId`
- `companiesHouseNumber`
- `ukprn`

This is implemented in **GroupReadApiService.cs**

The user-facing search screens also mention Companies House number as a valid search input.

### 2. Duplicate/existence checks

The front end and backend use the Companies House number when checking whether a group already exists.

`GroupReadApiService.ExistsAsync(...)` accepts an optional `CompaniesHouseNumber` value:

- **GroupReadApiService.cs**

This helps prevent duplicate trust/group records being created for the same company number.

### 3. Pre-populating academy trust creation

When a Companies House record is selected, the front end uses the returned company profile to pre-populate a trust creation model.

In **GroupController.cs**:

- The Companies House number
- Company name
- Registered office address
- Incorporation date

are loaded into `CreateAcademyTrustViewModel`.

When the user saves the new academy trust, the Companies House number is written into the new `GroupModel`:

- **GroupController.cs**

### 4. Displaying a link back to Companies House

Group pages expose a direct link back to the Companies House record.

`GroupDetailViewModel` builds:

- `CompaniesHouseUrl`

from:

- `CompaniesHouseBaseUrl`
- `Group.CompaniesHouseNumber`

See **GroupDetailViewModel.cs**.

This is then used in group detail and search views to render clickable Companies House links.

### 5. External lookup usage

For some group types, the Companies House number is also used as the lookup identifier for external financial benchmarking links.

In `GroupDetailViewModel`, for MATs and SATs:

- `Group.CompaniesHouseNumber.ToInteger()`

is used as the lookup ID for the financial benchmarking URL.

See **GroupDetailViewModel.cs**.

### 6. Back-end Companies House sync

The front end is not responsible for the scheduled Companies House sync, but it depends on the same stored field.

The back-end documentation shows that the scheduled Companies House job reads stored Companies House numbers for relevant group records and uses them to fetch current company data and create change requests.

See [companies-house-integration.md](/documentation/service/back-end-component/integrations/companies-house-integration.md).

So the stored Companies House number is the join key between:

- The GIAS group record
- The external Companies House record
- The back-end sync process

## Supporting types

There is also a small wrapper struct:

- **CompaniesHouseNumber.cs**

This is used in some service method signatures to represent the number as a typed value rather than a plain string.

## Configuration

The direct Companies House lookup service in the web app is registered in:

- **IocConfig.cs**

It is configured using:

- `CompaniesHouseApiKey`

The user-facing outbound link to Companies House uses:

- `CompaniesHouseBaseUrl`

## Summary

From the front-end component's point of view, the Companies House number is mainly the external company identifier used to create, search, display, and cross-reference trust/group records.

In practical terms:

- It is sourced from the Companies House API during trust creation
- It is then stored on the GIAS group record
- It is used for search and duplicate detection
- It is shown back to users with a link to Companies House
- It is reused by other integrations and by the back-end Companies House sync
