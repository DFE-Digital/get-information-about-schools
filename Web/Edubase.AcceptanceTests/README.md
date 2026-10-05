# Acceptance tests

Start with [Establishments.feature](Establishments/Establishments.feature). It describes behaviour in Gherkin, using the language of users and establishments. The main example is:

```gherkin
Scenario: Get an Establishment by URN for a back office user
    Given a back office user is signed in
    And an Establishment with URN "141491" exists
    When the user requests the Establishment with URN "141491"
    Then the Establishment URN is "141491"
    And the Establishment Name is "Landau Forte Academy Tamworth Sixth Form1"
    And the Establishment Type is "Academy 16 to 19 sponsor led"
```

These tests make acceptance criteria executable against a running GIAS application. They connect a readable description of a user's intent to the application's observable behaviour, giving us regression checks across authentication and establishment retrieval.

**The initial implementation primarily establishes a pattern to roll out across acceptance scenarios.** The two existing Get Establishment scenarios are the first examples of that pattern, rather than a comprehensive specification of establishment retrieval or access control. The main deliverable is a reusable structure in which more behaviours can be expressed without putting HTTP, authentication or response-parsing details into feature files.

## From the feature to executable steps

The project targets .NET 8 and uses Reqnroll with xUnit. Reqnroll generates the executable test glue in `Establishments.feature.cs` and connects the Gherkin steps to methods marked with `[Given]`, `[When]` and `[Then]` in [EstablishmentsStepDefinitions](Establishments/EstablishmentsStepDefinitions.cs). Edit the feature and bindings, rather than the generated file.

For the main scenario, the bindings perform these operations:

1. Ask `IUsers` for a back office user and call `IGiasFrontEnd.SignIn(user)`.
2. Record the URN from the establishment precondition. Currently, this step neither creates the establishment nor checks that it exists; the environment must already contain it.
3. Call `IEstablishments.GetEstablishment(urn)` and retain the returned establishment for the assertions.
4. Use xUnit assertions to compare its URN, name and type with the feature's expected values.

The bindings depend on interfaces supplied through constructor injection. They coordinate the scenario and assert outcomes; the classes below them handle how those operations reach GIAS.

## Domain operations and response mapping

[IEstablishments](Establishments/IEstablishments.cs) exposes the domain operation `GetEstablishment(int urn)`. Its current implementation, [EstablishmentsFromGiasFrontEnd](Establishments/EstablishmentsFromGiasFrontEnd.cs), calls:

```text
GET /api/establishment/{urn}
```

It uses `IGiasFrontEnd.GetAsync<GetEstablishmentResponse>()` to obtain the JSON response, then maps `returnValue.name`, `returnValue.typeName` and `returnValue.urn` into the small [Establishment](Establishments/Establishment.cs) model used by the steps. The URN is converted from a string to an integer.

This keeps the endpoint and JSON representation out of the scenario bindings. Future scenarios can reuse domain operations, while changes to the application's response shape can be handled in the adapter and [response model](GiasFrontEnd/GetEstablishmentResponse.cs). The response's `status` field is currently not asserted.

## The GIAS session and sign-in

[IGiasFrontEnd](GiasFrontEnd/IGiasFrontEnd.cs) combines the HTTP operations of `IApi` with `SignIn(User)`. [GiasFrontEndViaHttp](GiasFrontEnd/GiasFrontEndViaHttp.cs) owns the GIAS session and coordinates the sign-in exchange:

1. GET the GIAS login route, retaining its cookies and redirect to the sign-in authority.
2. Delegate to `ISignInAuthority` to obtain a SAML response and relay state.
3. POST those values to GIAS at `/Saml2/Acs`.
4. Follow the external login callback and retrieve the home page.
5. Read the request verification token and retain authentication cookies for subsequent requests.

The current authority is [SigninSimulatorInAzure](SigninAuthorities/SigninSimulatorInAzure.cs). It retrieves the Azure sign-in simulator's form, reads the request correlation values, and posts a form containing the selected user's identity and SAML attributes. It returns the generated SAML response to the GIAS adapter. Environment-specific simulator URLs and descriptions are selected inside this class.

[IUsers](Users/IUsers.cs) separates user selection from sign-in mechanics. [UsersFromHardCodedValues](Users/UsersFromHardCodedValues.cs) currently supplies a fixed back office test identity. These interfaces provide places to add other user sources or sign-in implementations as the pattern expands.

## HTTP and HTML infrastructure

[HttpApi](Apis/HttpApi.cs) implements the shared `IApi` operations using `HttpClient`: JSON GET, HTML GET and form POST. It checks for successful HTTP status codes before deserialising JSON with Newtonsoft.Json or parsing HTML.

[HttpResponseMessageExtensions](Apis/HttpResponseMessageExtensions.cs) turns an HTTP response into an AngleSharp HTML document, retaining its address, status and headers. The sign-in code uses selectors against these documents to extract form values and tokens.

These are HTTP-based acceptance tests against a separately running application. They do not launch a browser or exercise browser rendering and JavaScript. HTML parsing supports the sign-in flow; establishment assertions use the front end's JSON endpoint.

## Wiring and scenario isolation

[DependencyInjection.CreateServices](DependencyInjection/DependencyInjection.cs) is marked with Reqnroll's `[ScenarioDependencies]` attribute and assembles the implementations for each scenario:

| Interface | Current implementation |
| --- | --- |
| `IUsers` | `UsersFromHardCodedValues` |
| `IEstablishments` | `EstablishmentsFromGiasFrontEnd` |
| `IGiasFrontEnd` | `GiasFrontEndViaHttp` |
| `ISignInAuthority` | `SigninSimulatorInAzure` |
| `IApi` for simulator requests | `HttpApi` using the named `AzureSignInSimulator` client |

GIAS receives a scenario-scoped `HttpClient`, handler and cookie container. This prevents a signed-in scenario's GIAS session from leaking into another scenario through a pooled handler. Automatic redirects are disabled so the sign-in implementation can explicitly process each redirect. Simulator requests use a separate named client, also with automatic redirects disabled.

The unsigned-in scenario relies on this fresh GIAS session: its `Given a user is not signed in` binding is empty. Its request step catches any exception and records a generic retrieval error. Consequently, `Then an error occurs` currently proves only that retrieval failed, not that GIAS returned a particular authentication status or message. Network, parsing and other failures can also satisfy that assertion.

## Running the examples

The current composition root hard-codes `https://localhost:44309` as the GIAS base address and `dev` as the simulator environment. Before running, start GIAS with its required backing services and compatible simulator authentication configuration, ensure its HTTPS certificate is trusted, and ensure the configured test identity and establishment data are available. The runner does not start or seed the application.

From the repository root, with the .NET 8 SDK installed:

```shell
dotnet test Web/Edubase.AcceptanceTests/Edubase.AcceptanceTests.csproj
```

The main scenario compares exact data, including the trailing `1` in the establishment name shown above. Changes to that environment's data can therefore cause failures independently of application changes. To target another environment, update the composition root and ensure the user, data and simulator configuration agree; there is currently no environment-variable or command-line configuration layer in this project.

## Applying the pattern to more scenarios

Add new acceptance behaviours in Gherkin first, then implement thin bindings that call domain operations and assert business outcomes. Reuse existing operations where appropriate; add domain interfaces and adapters where a new capability needs them. Keep routes, payload mapping, HTML selectors and authentication mechanics below the bindings, and register new implementations in the scenario composition root.

As coverage grows, make data preconditions reliable and expected failure assertions specific. Preserve scenario isolation and keep environment choices behind the infrastructure interfaces. The aim of this first slice is to make that wider rollout consistent and maintainable; the existing scenarios demonstrate the structure, while fuller behavioural coverage is still to be added.
