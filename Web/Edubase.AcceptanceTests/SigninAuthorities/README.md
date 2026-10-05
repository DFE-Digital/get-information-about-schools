# Signing in for acceptance tests

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

`GiasFrontEndViaHttp.SignIn(user)` first calls `ISignInAuthority.TrySignInLocally(user)`:

- `true` means the local mechanism has completed sign-in using the shared GIAS client. The coordinator returns immediately.
- `false` means the coordinator must start the external exchange. This is the interface's default implementation.
- An exception means sign-in failed. It does not trigger a fallback to the other implementation.

For the external exchange, the coordinator starts website login, then calls `ISignInAuthority.SignIn(user, authorityLocation, assertionConsumerServiceUrl)`. That method returns a SAML response and relay state; the coordinator submits them to GIAS and completes the website session. Thus the current interface has a local shortcut and a SAML-specific external contract, while the shared responsibilities above apply to both.

## SigninSimulatorInAzure

[`SigninSimulatorInAzure`](SigninSimulatorInAzure.cs) supplies a SAML assertion through the Azure-hosted sign-in simulator. GIAS still owns the application session. Its `TrySignInLocally` uses the default `false`, so the coordinator performs the full exchange below.

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

The website must have compatible simulator/SAML configuration and reachable metadata, and the simulator environment must agree with that configuration. In Debug, a non-empty website `LocalTestSignInKey` disables SAML middleware registration, so clear that setting and restart the website when using this route. See [`StartupSASimulator`](../../Edubase.Web.UI/App_Start/StartupSASimulator.cs).

## SigninSimulatorOnLocalMachine

[`SigninSimulatorOnLocalMachine`](SigninSimulatorOnLocalMachine.cs) combines [steps 2–4](#2-establish-a-trusted-identity) in one request to the local website. It uses the same scenario-scoped HTTP client as `GiasFrontEndViaHttp`, so the resulting session is immediately available to the scenario.

### Submit the user to the local endpoint — shared steps 1–2

The constructor requires a non-empty local sign-in key and an HTTP(S) loopback base address. `TrySignInLocally` requires a user with a non-empty `AttributeStatementValue`, then sends:

```text
POST /Account/LocalSignIn
X-Gias-Local-Sign-In-Key: <matching website key>
Content-Type: application/x-www-form-urlencoded

userId=<URL-encoded User.AttributeStatementValue>
```

`User.NameId` is not used by this mechanism. The endpoint in [`AccountController.LocalSignIn`](../../Edubase.Web.UI/Controllers/AccountController.cs) is compiled only in Debug. It returns 404 unless the request is local, `owin:appStartup` is `SASimulatorConfiguration`, and the header exactly matches a non-empty website `LocalTestSignInKey`. A blank user ID returns 400. These checks provide this development mechanism's trust boundary for [step 2](#2-establish-a-trusted-identity).

### Build the identity and issue the cookie — shared steps 3–4

The endpoint constructs an external identity containing an `SAUserId` claim from `userId`. It uses `StubClaimsIdConverter`, loads backend roles and adds them to the application identity, reproducing [step 3](#3-build-the-application-identity-and-load-roles) as performed by the simulator-mode external callback.

It calls `AuthenticationManager.SignIn` and returns HTTP 204. The normal application cookie middleware implements [step 4](#4-issue-the-application-session); the local key is neither the application cookie nor a SAML signing key. Backend failures still fail sign-in.

### Reuse the session — shared step 5

The shared HTTP handler retains the application cookie from that response ([step 5](#5-retain-the-session-for-the-scenario)). The authority requires a successful response with status exactly 204 before returning `true`. The coordinator then returns without a SAML exchange, external callback or home-page token fetch. Calling this authority's SAML `SignIn` method directly throws `NotSupportedException`.

This path exercises application identity conversion, backend role lookup and application cookie issuance. It does not exercise SAML validation, remote metadata, temporary external authentication, or the callback's landing-page selection. It does not fetch an anti-forgery token; a later operation needing one must obtain it separately.

## Selecting and configuring an implementation

The [composition root](../DependencyInjection/DependencyInjection.cs) selects the local authority when `localSignInKey` is non-blank; otherwise it selects Azure. At present, `CreateServices` hard-codes the website as `https://localhost:44309`, environment as `dev`, `runningInAzurePipeline` as `false`, and a non-empty local key. Consequently, the checked-in code selects local sign-in. It currently reads neither `TF_BUILD` nor `GIAS_LOCAL_SIGN_IN_KEY`.

For local sign-in, run the website in Debug with `SASimulatorConfiguration` and configure its ignored local appSettings file with a `LocalTestSignInKey` matching the key passed by the composition root. Restart the website after configuration changes. Normal localhost HTTPS certificate trust still applies. In this mode, startup retains cookie middleware but skips remote SAML middleware and metadata.

For Azure sign-in, pass a null or blank `localSignInKey`, select the matching simulator environment, and ensure the website registers SAML middleware as described above. Both implementations require the running website, backend and appropriate test-user data.

The separate [local setup guide](SigninSimulatorOnLocalMachine.md) still describes environment-variable and pipeline detection. Those selection instructions do not match the current composition root; use the actual selection rules in this section until that configuration is reconciled.
