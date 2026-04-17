# Web Front End Tokens

This document describes the tokens stored by the GIAS web front end in Azure Table Storage and how they are used.

## What These Tokens Are

The `TokenRepository` stores application state tokens, not authentication tokens.

Each token is represented by `Edubase.Data.Entity.Token` and contains:

- `Id`: a generated identifier formed from the Azure Table `PartitionKey` and `RowKey`
- `Data`: the serialized form/query state captured from the browser request

The token ID is designed to be short enough to place on URLs as a `tok` query parameter.

## What They Are Used For

These tokens are primarily used to persist search and filter state for the web UI.

Main uses:

- store the current search filter state produced by the browser
- reload search results using `?tok=<token>`
- generate download links tied to the same search/filter definition
- save a user's preferred search token for later reuse after sign-in

## End-to-End Flow

1. The browser serializes the active search/filter form and posts it to `/api/tokenize`.
2. `SearchApiController` creates `new Token(formstate)` and stores it through `ITokenRepository`.
3. The API returns the token ID to the browser.
4. The browser uses that token in URLs such as search results and download requests.
5. `TokenValueProviderFactory` reads the `tok` query-string parameter, loads the token, parses `token.Data`, and injects the values into MVC model binding.

## Saved Search Token

For authenticated users, the currently selected token can also be stored in `UserPreference.SavedSearchToken`.

That value is updated by `/api/save-search-token` and is used by `AccountController` after login to redirect the user back to a previously saved search.

## What These Tokens Are Not

These tokens are not:

- SAML authentication assertions
- Session cookies
- Bearer tokens
- API credentials
- Anti-forgery tokens

They are best understood as persisted UI state tokens for search, results, and download workflows.

## Key Code References

- `Web/Edubase.Data/Entity/Token.cs`
- `Web/Edubase.Data/Repositories/TokenRepository.cs`
- `Web/Edubase.Web.UI/Controllers/Api/SearchApiController.cs`
- `Web/Edubase.Web.UI/Helpers/ValueProviders/TokenValueProviderFactory.cs`
- `Web/Edubase.Web.UI/Controllers/Api/UtilApiController.cs`
- `Web/Edubase.Data/Entity/UserPreference.cs`
