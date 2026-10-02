# Local sign-in per scenario

The signed-in Given step calls the local website once per scenario. The website
converts the supplied user ID using its existing simulator claims converter,
loads roles through its existing backend security service, and issues its normal
application cookie with the normal lifetime. Each scenario has its own cookie
container. Unsigned-in scenarios do not call the endpoint.

## Local setup

1. Configure the website to use your local backend as usual.
2. In the website's ignored local appSettings configuration, add:

   ```xml
   <add key="LocalTestSignInKey" value="YOUR-RANDOM-LOCAL-KEY" />
   ```

   Keep `owin:appStartup` set to `SASimulatorConfiguration`. Build in Debug and
   restart IIS Express. Local mode skips SAML middleware and remote metadata.
3. Give the acceptance-test process the same key:

   ```powershell
   $env:GIAS_LOCAL_SIGN_IN_KEY = 'YOUR-RANDOM-LOCAL-KEY'
   dotnet test Web/Edubase.AcceptanceTests/Edubase.AcceptanceTests.csproj
   ```

   For Test Explorer, start Visual Studio with that environment variable set.
   No browser login, cached cookie, SAML certificate or expiry override is needed.
   HTTPS still uses your existing localhost HTTPS certificate.

The local endpoint is only compiled in Debug, only accepts local requests, and
requires the configured key in a custom header. This prevents an arbitrary browser
page from signing in a user through the development endpoint. Keep the key local
and out of Git; it is unrelated to SAML signing or website cookie encryption.

Azure Pipelines (`TF_BUILD=True`) always selects the Azure simulator. Other runs
without the environment variable also use Azure. Remove the website setting and
restart IIS Express to restore SAML login locally. Do not configure this setting
in pipeline deployments.

This reproduces the simulator-mode application identity and role lookup. It does
not test SAML validation or the external callback's landing-page selection; those
remain covered by the Azure flow. Backend failures still fail the sign-in.
