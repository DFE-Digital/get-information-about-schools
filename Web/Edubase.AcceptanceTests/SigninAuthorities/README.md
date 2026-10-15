# Sign-in for acceptance tests

For setup and configuration, see the Acceptance Tests [README](../README.md).
For local certificate and metadata setup, see [Local SAML setup](LocalSamlSetup.md).

`UseLocalSignIn` in the shared `devsecrets.gias.config.alwaysignore` selects
`SigninSimulatorOnLocalMachine` (`true`) or `SigninSimulatorInAzure` (`false`).

## How it works

1. The test requests the website's login page.
2. With local sign-in, it signs a SAML response using the shared development certificate. With remote sign-in, the Azure simulator provides the response.
3. The website validates the response, asks the backend for the user's roles, and issues a session cookie.
4. The test uses that cookie for subsequent requests.

There is no browser or separate local sign-in server. Sign-in does not create users or test data.

## If sign-in fails

An unexpected login redirect usually means the website and tests are using different sign-in settings. Check the active config and restart the website.

A 500 from `ExternalLoginCallback` can mean SAML succeeded but backend role lookup or database access failed. Read the application's error log for the underlying exception.
