# Acceptance Tests

## Prerequisites
For running these tests Reqnroll must be installed. A running GIAS application is also
required with its backing services and a compatible authenticator simulator. The tests
do not start or seed the application; they assume that the data resources contain
required user and establishment data.

### Reqnroll
Ensure you have the Reqnroll extension installed in Visual Studio.

### Java back end
For the initial rollout the Acceptance Tests assume that the Java back end is running
and accessible in Azure. A local back-end will follow as soon as it can be verified.
Follow the instructions for setting up a local Java back end from the Wiki.

### SQL Server database
For the initial rollout the Acceptance Tests assume that the SQL Server database is
running and accessible in Azure. A local SQL Server instance will be created and
seeded as part of the Java back end installation, again, once it has all been verified.

### GIAS web application - *Edubase.Web.UI*
Ensure the Web UI configuration file `devsecrets.gias.config.alwaysignore` is correct by
copying a known working config file from another developer. This file should be placed
in the root folder - the same level as the `.git` folder. There are two configs available:

- one pointing to remote (Azure) services - `devsecrets.gias.config.remote.alwaysignore`-
- the other pointing to local services `devsecrets.gias.config.local.alwaysignore`.

These files should never be committed to GIT as they may contain secrets.

### Local Execution
For local execution, run `Web/LocalSetup.ps1` to prepare the local machine for local execution
including creating `devsecrets.gias.signing.pfx.alwaysignore` if it is missing and writing
local SAML metadata. The script also installs Azurite. Run `Web/StartLocalServices.ps1` thereafter
to start Azurite and leave that terminal open while the application is running.

Ensure the Web UI app is running and accessible, usually at `https://localhost:44309`.
The acceptance tests will use this URL to interact with the application. This usually
means opening a separate Visual Studio instance and starting up *Edubase.Web.UI*.

## Final Checklist
If runtime issues occur during the execution of acceptance tests check the following:
- Has the correct config file been copied to the root directory?
- Is the SQL Server database stated in the config file available?
- Is the Java back end stated in the config file available?
- Has `Web/LocalSetup.ps1` been run?
- Is Azurite running in a window started by `Web/StartLocalServices.ps1`?
- Is an instance of the Web app running locally?

## Features and scenarios
Start with [Establishments.feature](Establishments/Establishments.feature).
It describes behaviour in Gherkin, using the domain language of Users and Establishments.
The main example is:

```gherkin
Scenario: Get an Establishment by URN for a back office user
    Given a back office user is signed in
    And an Establishment with URN "141491" exists
    When the user requests the Establishment with URN "141491"
    Then the Establishment URN is "141491"
    And the Establishment Name is "Landau Forte Academy Tamworth Sixth Form1"
    And the Establishment Type is "Academy 16 to 19 sponsor led"
```

These tests make acceptance criteria executable against a running GIAS application.
They connect a readable description of a user's intent to the application's observable
behaviour, giving us regression checks across authentication and establishment retrieval.

**The initial implementation primarily establishes a pattern to roll out across
acceptance scenarios.** The three existing Get Establishment scenarios are the first
examples of that pattern, rather than a comprehensive specification of establishment
retrieval or access control. The main deliverable is a reusable structure in which more
behaviours can be expressed without putting HTTP, authentication or response-parsing
details into feature files.

## From the feature to executable steps

The project targets .NET 10 and uses Reqnroll with xUnit. Reqnroll generates the
executable test glue in `Establishments.feature.cs` and connects the Gherkin steps
to methods marked with `[Given]`, `[When]` and `[Then]` in
[EstablishmentsStepDefinitions](Establishments/EstablishmentsStepDefinitions.cs).
Edit the feature and bindings, rather than the generated file.

For the main scenario, the bindings perform these operations:

1. Ask `IUsers` for a back office user and call `IGiasFrontEnd.SignIn(user)`.
2. Declare the establishment precondition. Currently, this step neither creates the establishment nor checks that it exists; the environment must already contain it.
3. Call `IEstablishments.GetEstablishment(urn)` and retain the returned establishment for the assertions.
4. Use xUnit assertions to compare its URN, name and type with the feature's expected values.

The bindings depend on interfaces supplied through constructor injection. They coordinate
the scenario and assert outcomes; the classes below them handle how those operations reach
GIAS.

## Domain operations and response mapping

[IEstablishments](Establishments/IEstablishments.cs) exposes the domain operation
`GetEstablishment(int urn)`.
Its current implementation, [EstablishmentsFromGiasFrontEnd](Establishments/EstablishmentsFromGiasFrontEnd.cs), calls:

```text
GET /api/establishment/{urn}
```

It uses `IGiasFrontEnd.GetAsync<GetEstablishmentResponse>()` to obtain the JSON response,
then maps `returnValue.name`, `returnValue.typeName` and `returnValue.urn` into the small
[Establishment](Establishments/Establishment.cs) model used by the steps. The URN is
converted from a string to an integer.

This keeps the endpoint and JSON representation out of the scenario bindings. Future
scenarios can reuse domain operations, while changes to the application's response
shape can be handled in the adapter and [response model](GiasFrontEnd/GetEstablishmentResponse.cs).
The response's `status` field is currently not asserted.

## The GIAS session and sign-in

[IGiasFrontEnd](GiasFrontEnd/IGiasFrontEnd.cs) combines the HTTP operations of `IApi`
with `SignIn(User)`. [GiasFrontEndViaHttp](GiasFrontEnd/GiasFrontEndViaHttp.cs) delegates
sign-in to `ISignInAuthority`, which uses the shared GIAS session to:

1. GET the GIAS login route, retaining its cookies and redirect to the sign-in authority.
2. Create a SAML response locally or obtain one from the Azure simulator, retaining the relay state.
3. POST those values to GIAS at `/Saml2/Acs`.
4. Follow the external login callback and retain authentication cookies for subsequent requests.

The authority is selected by `UseLocalSignIn` in the shared configuration.
[SigninSimulatorOnLocalMachine](SigninAuthorities/SigninSimulatorOnLocalMachine.cs)
signs the response using the repository's local development certificate without contacting a sign-in server.
[SigninSimulatorInAzure](SigninAuthorities/SigninSimulatorInAzure.cs)
retrieves the Azure sign-in simulator's form, reads the request correlation values,
and posts a form containing the selected user's identity and SAML attributes. It returns
the generated SAML response to GIAS. Environment-specific simulator URLs and
descriptions are selected inside this class.

[IUsers](Users/IUsers.cs) separates user selection from sign-in mechanics.
[UsersFromHardCodedValues](Users/UsersFromHardCodedValues.cs) currently supplies a
fixed back office test identity. These interfaces provide places to add other user
sources or sign-in implementations as the pattern expands.

## HTTP and HTML infrastructure

[HttpApi](Apis/HttpApi.cs) implements the shared `IApi` operations using `HttpClient`:
JSON GET, HTML GET and form POST. It checks for successful HTTP status codes before
deserialising JSON with Newtonsoft.Json or parsing HTML.

[HttpResponseMessageExtensions](Apis/HttpResponseMessageExtensions.cs) turns an HTTP
response into an AngleSharp HTML document, retaining its address, status and headers.
The sign-in code uses selectors against these documents to extract form values and tokens.

These are HTTP-based acceptance tests against a separately running application. They do
not launch a browser or exercise browser rendering and JavaScript. HTML parsing supports
the sign-in flow; establishment assertions use the front end's JSON endpoint.

## Wiring and scenario isolation

[DependencyInjection.CreateServices](DependencyInjection/DependencyInjection.cs) is
marked with Reqnroll's `[ScenarioDependencies]` attribute and assembles the implementations
for each scenario:

| Interface | Current implementation |
| --- | --- |
| `IUsers` | `UsersFromHardCodedValues` |
| `IEstablishments` | `EstablishmentsFromGiasFrontEnd` |
| `IGiasFrontEnd` | `GiasFrontEndViaHttp` |
| `ISignInAuthority` | `SigninSimulatorOnLocalMachine` or `SigninSimulatorInAzure`, selected by `UseLocalSignIn` |
| `IApi` for simulator requests | `HttpApi` using the named `AzureSignInSimulator` client |

GIAS receives a scenario-scoped `HttpClient`, handler and cookie container. This prevents a
signed-in scenario's GIAS session from leaking into another scenario through a pooled handler.
Automatic redirects are disabled so the sign-in implementation can explicitly process each
redirect. Simulator requests use a separate named client, also with automatic redirects
disabled.

The unsigned-in scenario relies on this fresh GIAS session: its `Given a user is not signed in`
binding is empty. Its request step catches any exception and records a generic retrieval error.
Consequently, `Then an error occurs` currently proves only that retrieval failed, not that GIAS
returned a particular authentication status or message. Network, parsing and other failures can
also satisfy that assertion.

The third scenario requests URN `0` and checks that the recorded error contains
`Not Found`. Its non-existence precondition is also an empty binding.

## Running the examples

On start-up the Web app and the Acceptance Tests read `GiasFrontEndBaseAddress`, `Environment` and `UseLocalSignIn`
from the repository-root `devsecrets.gias.config.alwaysignore`. Before running, start GIAS - 
start up from a separate Visual Studio instance is normally the best approach as it can be
debugged when necessary. Ensure its required backing services and compatible simulator
authentication configuration, ensure its HTTPS certificate is trusted, and ensure the
configured test identity and establishment data are available.
The runner does not start or seed the application.

From a separate Visual Studio instance open Test Explorer and execute the Acceptance Tests
as required in the usual way. That tests should issue HTTP requests to the running Web app.

The main scenario compares exact data, including the trailing `1` in the establishment
name shown above. Changes to that environment's data can therefore cause failures
independently of application changes. To target another environment, update the shared
configuration and ensure the user, data and simulator configuration agree.

## Applying the pattern to more scenarios

Add new acceptance behaviours in Gherkin first, then implement thin bindings that call
domain operations and assert business outcomes. Reuse existing operations where appropriate;
add domain interfaces and adapters where a new capability needs them. Keep routes, payload
mapping, HTML selectors and authentication mechanics below the bindings, and register new
implementations in the scenario composition root.

As coverage grows, make data preconditions reliable and expected failure assertions specific.
Preserve scenario isolation and keep environment choices behind the infrastructure interfaces.
The aim of this first slice is to make that wider rollout consistent and maintainable; the
existing scenarios demonstrate the structure, while fuller behavioural coverage is still to
be added.
