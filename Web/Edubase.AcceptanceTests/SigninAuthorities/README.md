# Signing in for acceptance tests

> Local sign-in is self-contained and enabled through the ignored development config. See [local sign-in](LocalSamlSetup.md).

The purpose of sign-in is to give one acceptance-test scenario an authenticated GIAS session for a selected user. The scenario calls [`IGiasFrontEnd.SignIn(user)`](../GiasFrontEnd/IGiasFrontEnd.cs); it does not need to know how that identity reaches the website. [`GiasFrontEndViaHttp`](../GiasFrontEnd/GiasFrontEndViaHttp.cs) coordinates the exchange with an [`ISignInAuthority`](ISignInAuthority.cs).

These are HTTP exchanges against a separately running website and backend. No browser is launched, and the tests do not execute browser JavaScript. Signing in does not create or seed the backend user.

## Shared sign-in model

The following steps describe responsibilities, rather than a particular protocol or number of HTTP requests. An implementation may combine several steps in one request.

### 1. Select the user

The scenario obtains a [`User`](../Users/User.cs) through [`IUsers`](../Users/IUsers.cs) and passes it to the front end. User selection is separate from authentication transport. The supplied identity must correspond to the intended backend user so that GIAS can resolve its permissions.

### 2. Establish a trusted identity

The sign-in mechanism supplies identity information through a route the website is configured to accept. Supplying a user ID to the test alone does not authenticate subsequent requests: the website must accept that identity using the chosen mechanism's trust checks.

Any request correlation or temporary authentication state belongs to this exchange. It must remain associated with the scenario that started it.

### 3. Build the application identity and load roles

The website converts the accepted identity into its application claims, asks the backend security service for the user's roles, and adds those role claims. The simulator does not determine permissions merely by returning an identity. Backend availability and the selected user's backend data remain relevant to sign-in.

### 4. Issue the application session

The website calls the OWIN authentication manager to sign in the application identity. Cookie middleware issues the normal application authentication cookie with the configured lifetime. This cookie is the credential used on subsequent GIAS requests; it is distinct from temporary identity-provider state or a local endpoint access key.

### 5. Retain the session for the scenario

The test's GIAS HTTP client retains the website cookies and uses them for later requests. The [composition root](../DependencyInjection/DependencyInjection.cs) gives each scenario its own client, handler and cookie container, preventing authentication state from leaking between scenarios. Automatic redirects are disabled so the coordinator can explicitly process the sign-in exchange.

An unsigned-in scenario does not call `SignIn`. Sign-in completion is also separate from assertions that a particular user can access a particular resource; those assertions belong to the scenario.

## How the interface expresses the model

`ISignInAuthority` exposes only `Task SignIn(User user)`. `GiasFrontEndViaHttp` delegates to the selected implementation. Each implementation completes its own authentication exchange using the scenario-scoped GIAS client; dependency injection chooses which one to use.
## SigninSimulatorInAzure

[`SigninSimulatorInAzure`](SigninSimulatorInAzure.cs) supplies a SAML assertion through the Azure-hosted sign-in simulator. GIAS still owns the application session. It performs the complete exchange below within its implementation.

### Start website login — shared step 2

For the user selected in [step 1](#1-select-the-user), the coordinator clears explicit default `Cookie` headers and sends `GET /Account/Login?returnUrl=%2F` to GIAS. It retains the returned cookies and redirect location. The website's authentication challenge directs the client to the configured simulator with the SAML request context.

### Obtain the assertion — shared step 2

Using a separate HTTP client, the authority fetches that redirect location and parses the simulator form. It reads `AssertionModel_InResponseTo` and `AssertionModel_RelayState`, then posts a form to its environment-specific simulator URL containing:

- The request's `InResponseTo` correlation value and `RelayState`.
- The website's `/Saml2/Acs` assertion consumer service URL, audience `http://edubase.gov`, and `HttpPost` response binding.
- `User.NameId` as the SAML name ID and the fixed session index `42`.
- `User.AttributeStatementValue` for both `http://www.edubase.gov.uk/SAUserId` and `urn:oid:2.5.4.45` attributes.
- The simulator description selected for the environment.

`sandbox1`, `sandbox2` and `dev` use the Dev / Exp simulator configuration; `test` uses Stage / Test. Other environment names throw. The authority extracts the generated `SAMLResponse` hidden input and returns it with the relay state. It does not issue the GIAS application cookie.

### Submit to GIAS and build the session — shared steps 2–4

The coordinator posts `SAMLResponse` and `RelayState` to `/Saml2/Acs`, carrying the initial login cookies. The configured SAML middleware processes the response, establishes temporary external authentication, and redirects to the external login callback. This completes the external identity exchange in [step 2](#2-establish-a-trusted-identity).

The coordinator follows the callback location. In simulator mode, [`AccountController.ExternalLoginCallback`](../../Edubase.Web.UI/Controllers/AccountController.cs) converts the external identity using [`StubClaimsIdConverter`](../../Edubase.Services/Security/ClaimsIdentityConverters/StubClaimsIdConverter.cs), loads roles through `ISecurityService.GetRolesAsync`, and adds role claims ([step 3](#3-build-the-application-identity-and-load-roles)). It then calls `AuthenticationManager.SignIn`, issuing the application cookie ([step 4](#4-issue-the-application-session)), and calculates the user's landing-page redirect.

### Retain cookies — shared step 5

The coordinator reads cookies from the ACS and callback responses, then explicitly fetches `/` with the application cookie rather than following the callback's selected landing page. It parses the home page's `__RequestVerificationToken` input and adds explicit default cookie headers containing the external cookie, application cookie and that value. The scenario's cookie container also retains response cookies, implementing [step 5](#5-retain-the-session-for-the-scenario).

This describes the current adapter: it assumes particular response headers, cookie ordering and HTML fields rather than checking every sign-in response explicitly. The home-page fetch is not itself an assertion of the user's roles or correct landing-page navigation.

The website must have compatible simulator/SAML configuration and reachable metadata, and the simulator environment must agree with that configuration. Restore the Azure SASimulatorUri and SASimulatorGuid values in the ignored development config when exercising Azure. See [`StartupSASimulator`](../../Edubase.Web.UI/App_Start/StartupSASimulator.cs).

## SigninSimulatorOnLocalMachine

[`SigninSimulatorOnLocalMachine`](SigninSimulatorOnLocalMachine.cs) completes the same shared steps without a separate identity-provider process.

For [step 2](#2-establish-a-trusted-identity), it requests normal GIAS login and retains the correlation cookie. It reads the SAML request and relay state from the redirect, checks that the request targets this local website, and generates a signed SAML response in the test process using the selected user's name ID and backend user ID. It never follows the placeholder identity-provider URL.

The class posts the response and relay state to `/Saml2/Acs`, then follows the same-origin external callback. GIAS validates the signature, converts the claims, loads backend roles and issues the application cookie: shared steps 3 and 4. The scenario's cookie container retains the cookies for step 5.

The acceptance-test project's local Debug build generates its signing certificate and public federation metadata, without modifying the ignored development config. Set the existing simulator settings manually to select local files or Azure. GIAS's existing federation loader establishes trust; the web app has no new bootstrap or trust code. See [local setup](LocalSamlSetup.md).

## Selecting an implementation

The [composition root](../DependencyInjection/DependencyInjection.cs) selects local sign-in outside Azure Pipelines. A local Debug build prepares metadata automatically; simulator settings remain under your control; restart a website that was already running when those settings changed.

To exercise Azure locally, set the Azure simulator settings and set `useLocalSimulator` to false. Pipeline builds do not prepare local metadata or change the ignored config.