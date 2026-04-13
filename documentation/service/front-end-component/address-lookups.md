# Web Front End Address Lookups

This document describes how the GIAS web front end performs address and place lookups.

The front end uses two different mechanisms depending on the user journey:

- Place lookup for search and location disambiguation
- Postcode-to-address lookup for establishment address editing

## Overview

There is not a single shared "address lookup" implementation in the web app.

Instead, the front end uses:

- A local places lookup service built from Azure Maps and OS Places
- The main GIAS back-end API for establishment postcode address lookup

 How the front end looks up addresses depends on which screen the user is using.

## 1. Place lookup for search and location disambiguation

The main search journey uses `IPlacesLookupService`.

This is injected into **SearchController.cs** and used in two key places:

- `SuggestPlace`, for place typeahead suggestions
- `ProcessLocationDisambiguation`, for the full location search flow

### Suggest place

`SuggestPlace` is the front-end endpoint used for place suggestions:

- **SearchController.cs**

It:

- Validates the query
- Calls `_placesService.SearchAsync(text, true)`
- Returns the results as JSON

The returned items are `PlaceDto` objects, which contain:

- `Name`
- `Coords`

See **PlaceDto.cs**.

### Location disambiguation

When a user searches by location and the location token cannot be parsed directly, the front end uses:

- `ProcessLocationDisambiguation`

in **SearchController.cs**.

That method calls:

- `_placesService.SearchAsync(query, false)`

If matches are found, the user is shown a `LocationDisambiguation` view so they can choose the correct place.

## How `IPlacesLookupService` works

`IPlacesLookupService` is implemented by **PlacesLookupService.cs**.

Its logic is:

1. Search Azure Maps first
2. If Azure Maps returns results, use them
3. If Azure Maps returns nothing and this is not a typeahead request, try OS Places

So the web-tier place lookup strategy is:

- Azure Maps is the primary provider
- OS Places is the fallback provider
- OS Places fallback is only used for non-typeahead lookups

## Azure Maps usage

Azure Maps is used through **AzureMapsService.cs**.

Important details from the implementation:

- It calls `/search/address/json`
- It restricts results to `countrySet=GB`
- It uses the `typeahead` flag supplied by the caller
- It limits results to 10
- It returns simplified `PlaceDto` results with coordinates

The service also filters and normalises some Azure Maps results before returning them to the UI.

One important behaviour is postcode handling:

- If the search text looks like a UK postcode, and none of the Azure Maps results actually contain that postcode, the service returns no results
- That allows the lookup flow to fall back to OS Places for postcode searches

## OS Places usage

OS Places is used through **OSPlacesApiService.cs**.

Important details from the implementation:

- It only runs for postcode-shaped input
- It calls `search/places/v1/postcode`
- It requests `dataset=DPA,LPI`
- It filters for postal addresses only
- It deduplicates by UPRN
- It returns simplified `PlaceDto` results with coordinates

So in the front end, OS Places is mainly a postcode-specific fallback for the broader location search journey.

## 2. Establishment postcode-to-address lookup

The establishment edit flow uses a different mechanism.

When a user adds or replaces an address in the establishment editor, the front end does not call Azure Maps or OS Places directly. Instead, it calls the GIAS back-end API through `IEstablishmentReadService`.

This happens in **EstablishmentController.cs**.

### Add or replace address flow

In `AddOrReplaceEstablishmentAddressPostAsync`:

- If the user action is `find-address`
- And the selected country is the UK

the controller calls:

- `_establishmentReadService.GetAddressesByPostCodeAsync(viewModel.PostCode, User)`

If results are returned:

- The UI moves to the `selectaddress` step

If not:

- The user sees "We could not find any addresses matching that postcode"

### What the back-end call returns

`GetAddressesByPostCodeAsync` is implemented in **EstablishmentReadApiService.cs**.

It calls the backend endpoint:

- `establishment/addressBase/queryByPostcode?postcode=...`

The returned records are mapped into `AddressLookupResult`, defined in **AddressLookupResult.cs**.

That model includes:

- `FullAddress`
- `Street`
- `Town`
- `UPRN`
- `PostCode`
- `Easting`
- `Northing`
- `BusinessName`

After the user picks a result, the front end copies fields such as street, town, postcode, easting, and northing into the address-edit view model.

## Configuration

The external place lookup clients are wired up in **IocConfig.cs**.

The front end registers:

- `AzureMapsService` as `IAzureMapsService`
- `OSPlacesApiService` as `IOSPlacesApiService`
- `PlacesLookupService` as `IPlacesLookupService`

The related app settings include:

- `AzureMapsUrl`
- `AzureMapsApiKey`
- `AzureMapService_Timeout`
- `AzureMapService_RetryIntervals`
- `OSPlacesUrl`
- `OSPlacesApiKey`
- `OSPlacesApiServices_Timeout`
- `OSPlacesApiServices_RetryIntervals`

The establishment postcode lookup does not use those direct clients in the controller. It uses the main backend API via `IEstablishmentReadService`, so it depends on the standard API configuration instead.

## Summary

From the front-end component's perspective:

- Place search and location disambiguation use `IPlacesLookupService`
- `IPlacesLookupService` uses Azure Maps first
- OS Places is used as a postcode-oriented fallback for non-typeahead search
- Establishment postcode address selection uses the GIAS backend API instead of the direct Azure Maps / OS Places integration

