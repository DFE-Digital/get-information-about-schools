## C4 Component Diagram

Internal component view of the `GIAS Web Front End` container (`Web/Edubase.Web.UI`), based on the code under `/Web`.

```mermaid
C4Component

    title Component diagram for the GIAS Web Front End

    Person(user, "GIAS User", "Browses, searches and manages school information")
    System_Ext(dsi, "DfE Sign-in", "External identity provider used via SAML")
    System_Ext(texuna, "GIAS Backend / Texuna APIs", "Primary back-end APIs for establishments, groups, governors, approvals, downloads and security")
    System_Ext(extData, "External Data Services", "Azure Maps, OS Places, Companies House, Ofsted, Financial Benchmarking and FSCPD")
    ContainerDb_Ext(tableStorage, "Azure Table Storage", "Azure Storage Tables", "Stores tokens, user preferences, FAQ/glossary/news/notifications, data quality and web log records used directly by the web tier")

    Container_Boundary(web, "GIAS Web Front End") {
        Component(auth, "Authentication and Security Pipeline", "OWIN + Sustainsys.Saml2 + ISecurityService", "Handles login, external callback, cookie session, CSP nonce generation and role resolution")
        Component(mvc, "MVC Pages and Area Controllers", "ASP.NET MVC 5 Controllers", "Server-rendered flows for home, search, downloads, approvals, notifications, admin, establishments, groups and governors")
        Component(api, "AJAX and Workflow API", "ASP.NET Web API Controllers", "Supports tokenisation, approvals, academy openings, bulk creation, establishment utilities and other async UI actions")
        Component(views, "Razor Views and View Models", "CSHTML + ViewModel classes", "Builds HTML responses, edit journeys, partials and form state")
        Component(client, "Client-side Assets", "webpack + JavaScript + Vue + Sass", "Enhances search, downloads, maps, polling, form behaviour and rich UI interactions")
        Component(validation, "Validation, Filters and Model Binding", "FluentValidation + MVC filters + custom binders", "Applies authorisation, anti-forgery, exception handling, model validation and token/model binding conventions")
        Component(services, "Back-end Service Client Layer", "Edubase.Services + Edubase.Services.Texuna", "Typed clients for establishments, groups, governors, downloads, approvals, lookups and change history")
        Component(external, "External Lookup Integration Layer", "Geo and reference-data services", "Calls Azure Maps, OS Places, Companies House, Ofsted, Financial Benchmarking and FSCPD services")
        Component(data, "Web Data Access and Supporting Services", "Repositories + cache + blob/logging helpers", "Persists tokens, user preferences, glossary/FAQ/news/notifications, data quality, logs, cache and file references")
    }

    Rel(user, mvc, "Uses", "HTTPS")
    Rel(user, api, "Invokes async actions via browser", "HTTPS/JSON")
    Rel(user, auth, "Authenticates", "Browser redirect / cookie session")

    Rel(auth, dsi, "Authenticates with", "SAML2")
    Rel(auth, services, "Resolves roles and user access", "In-process service calls")

    Rel(mvc, views, "Renders")
    Rel(mvc, validation, "Uses")
    Rel(api, validation, "Uses")
    Rel(views, client, "Bootstraps page-specific assets")

    Rel(mvc, services, "Calls")
    Rel(api, services, "Calls")
    Rel(mvc, external, "Uses for selected lookup journeys")
    Rel(client, api, "Calls", "HTTPS/JSON")

    Rel(services, texuna, "Calls", "HTTPS/JSON")
    Rel(external, extData, "Calls", "HTTPS")
    Rel(data, tableStorage, "Reads and writes", "Azure Storage Tables")

    Rel(mvc, data, "Reads and writes web-owned state/content")
    Rel(api, data, "Stores tokens and workflow state")
    Rel(services, data, "Uses cache, blob and repository-backed helpers")
```

### Notes

- The main runtime centre of gravity is the MVC controller layer, especially the `Areas/Establishments`, `Areas/Groups` and `Areas/Governors` flows.
- `App_Start/IocConfig.cs` wires the web app to typed service clients for the main GIAS back-end APIs and also registers direct repositories used by the web tier.
- Those direct repositories are Azure Table Storage-based via `Edubase.Data.Repositories.TableStorage.TableStorageBase<T>`, rather than direct SQL Server access from the web app.
- `Controllers/Api` provides lightweight endpoints used by the client-side bundles for AJAX and long-running workflow support.
- `Assets/Scripts/Entry` and `Assets/Scripts/GiasVueComponents` show that the UI is mostly server-rendered, with targeted JavaScript/Vue enhancement rather than a separate SPA.

Related notes in this repository:

- [`address-lookups.md`](./address-lookups.md)
- [`azure-table-storage.md`](./azure-table-storage.md)
- [`bulk-updates.md`](./bulk-updates.md)
- [`companies-house-number.md`](./companies-house-number.md)
- [`data-quality-status.md`](./data-quality-status.md)
- [`downloads.md`](./downloads.md)
- [`security-and-permissions.md`](./security-and-permissions.md)
- [`tokens.md`](./tokens.md)
