# Lookup and Caching

This document describes the internal lookup and caching layer used by the `GIAS Web Front End`.

## Overview

The `lookup-and-caching` subcomponent is the web application's internal reference-data and cache layer. `LookupApiService` fetches lookup lists from the back-end APIs, `CachedLookupService` caches and resolves them for the UI, and `CacheAccessor` provides the broader memory-plus-optional-Redis caching infrastructure used by the site.

This subcomponent is distinct from `external-lookup-links`.

- `external-lookup-links` is about outbound links to external services such as FSCPD, Financial Benchmarking and Ofsted
- `lookup-and-caching` is about internal reference data and the cache infrastructure used by the web application itself

The main responsibilities here are:

- Retrieving reference/lookup data from the GIAS back-end APIs
- Caching that data so it does not need to be fetched repeatedly
- Resolving lookup IDs into display names for pages and view models
- Providing a shared cache abstraction that can use in-memory cache and Redis

The main implementation is in:

- `CachedLookupService.cs`
- `LookupApiService.cs`
- `CacheAccessor.cs`

## Lookup Data

Lookup data is the application's reference data: the relatively stable lists used to populate filters, dropdowns and display labels.

Examples include:

- Local authorities
- Establishment types
- Establishment statuses
- Education phases
- Group types
- Group statuses
- Governor roles
- Titles
- Dioceses
- SEN provision types
- Parliamentary constituencies
- Many other code tables used across the application

The source-of-truth lookup service is LookupApiService.cs.

That service calls back-end endpoints under `lookup/` such as:

- `lookup/local-authorities`
- `lookup/governor-roles`
- `lookup/group-types`
- `lookup/establishment-statuses`
- `lookup/establishment-types`

## Cached Lookup Layer

The main front-end abstraction is `ICachedLookupService.cs`, implemented by `CachedLookupService.cs`

`CachedLookupService` wraps the normal lookup service and adds caching behaviour.

It does two main things:

- Caches whole lookup lists such as all local authorities or all establishment types
- Maps lookup field names and IDs to user-facing names

This is why controllers and view models can ask for:

- All values in a lookup list
- A display label for an ID field like `LocalAuthorityId` or `StatusId`

without repeatedly calling the API directly.

## Name Resolution

One important feature of `CachedLookupService` is its internal mapping dictionary that connects model field names to lookup resolvers.

Examples include:

- `LocalAuthorityId`
- `TypeId`
- `StatusId`
- `GroupTypeId`
- `Section41ApprovedId`
- `OfstedRatingId`
- `CountyId`
- `CountryId`

This allows the front end to resolve IDs into readable names through methods such as:

- `GetNameAsync(string lookupName, int? id, string domain = null)`
- `GetNameAsync(Expression<Func<int?>> expression, string domain = null)`
- `IsLookupField(string name)`

This mapping behaviour is widely used when building detail pages and result views.

## How Caching Is Applied

`CachedLookupService` uses `ICacheAccessor` through helper methods like:

- `AutoAsync(...)`
- `Auto(...)`

The cache keys are built automatically from:

- The caller type
- The caller method name
- Optional parameter content

That means each lookup method such as `LocalAuthorityGetAllAsync()` or `EstablishmentTypesGetAllAsync()` gets its own stable cache key.

The effect is:

- The first request fetches the lookup data from the back end
- Subsequent requests reuse the cached data

## Cache Infrastructure

The shared cache abstraction is `ICacheAccessor.cs`, implemented by CacheAccessor.cs.

This is broader than lookup caching alone. It is a general-purpose cache layer used by the website.

Important characteristics:

- It has two in-memory caches
- It can also connect to Redis
- It prefers in-memory cache first
- It can fall back to Redis when available
- It can repopulate in-memory cache from Redis
- It can broadcast cache updates and invalidations across nodes

The code path in `GetWithMetaDataAsync(...)` is effectively:

1. Try in-memory cache first
2. If not found and Redis is available, try Redis
3. If found in Redis, hydrate the local in-memory cache

## Memory Cache vs Redis

`CacheAccessor` supports both:

- Local in-memory cache
- Central Redis cache

The configuration object is `CacheConfig.cs`, which reads the `Redis` connection string.

`Web.config` explicitly says the website makes limited use of Redis caches in combination with in-memory caches.

So the general cache design is:

- memory-first for speed
- optional Redis as a central/shared cache

This is different from the `external-lookup-links` services, which cache their availability checks directly in `MemoryCache.Default` and do not use `ICacheAccessor`.

## Dependency Injection

The cache and lookup services are wired in IocConfig.cs.

The important registrations are:

- `CacheAccessor` as `ICacheAccessor`
- `CachedLookupService` as `ICachedLookupService`

This makes the cached lookup layer available throughout the web application.

## Where The Front End Uses It

The cached lookup layer is used all over the MVC application.

Representative usage includes:

- Search controllers populating filter dropdowns and checkbox lists
- Detail pages resolving IDs into human-readable names
- Group, establishment and governor flows building display models
- Sitemap and home-related pages that need lookup-backed labels

Because lookup IDs are everywhere in the domain models, this caching layer is a foundational part of rendering the UI.

## Cache Invalidation and Clearing

The admin tooling includes a cache clear route in AdminController.cs.

That action calls `ICacheAccessor.ClearAsync()` and reports:

- `Redis cache and MemoryCache cleared successfully.`

`CacheAccessor.ClearAsync()`:

- Recreates the local in-memory caches
- Publishes a clear-cache message to other nodes
- Flushes the Redis databases

This gives the application a way to invalidate cached lookup/reference data when needed.

## What This Subcomponent Owns

The lookup and caching subcomponent is responsible for:

- Retrieving internal reference data from the GIAS back end
- Caching reference lists for reuse
- Resolving lookup IDs into names
- Providing a general cache abstraction for the site
- Coordinating in-memory and Redis-based cache behaviour

It is not responsible for:

- External service link generation
- Azure Table Storage persistence
- Business-entity editing or approval workflows




