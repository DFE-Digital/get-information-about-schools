# Self-contained local SAML

All simulator code is in the acceptance-test project. GIAS uses its existing SAML federation loader and normal login, ACS and callback endpoints. No extra web-app code, endpoint, Function app or local HTTP service is required.

## Run locally

Build the acceptance-test project in Debug (a normal local test run also builds it). Its post-build preparation automatically:

1. Generates or reuses `.local-signin/signing.pfx` inside the acceptance-test project.
2. Writes public `Metadata` and `Federation` XML alongside it.
It does not read or modify the website configuration. Set `SASimulatorUri` manually in the repository-root `devsecrets.gias.config.alwaysignore` to the local file URI printed by the build, and set `SASimulatorGuid` to an empty string. Your chosen values are preserved on every build and test run.

Then start the usual Debug website and run the acceptance tests. If the website was already running when its settings changed, restart it once. Existing website/backend setup and localhost HTTPS trust are still required. 

The URI points to a local file, not a server. The existing federation loader reads the public certificate and SSO address from that metadata. `SigninSimulatorOnLocalMachine` intercepts the SAML redirect, signs the response itself, posts it to `/Saml2/Acs` and follows the callback. The placeholder `http://localhost/local-signin` is never contacted. The callback performs normal backend role lookup and issues the application cookie.

Keys and metadata are ignored by Git. The private key is used only by the tests; GIAS reads public metadata. Nothing is installed in the Windows certificate store. Ordinary clean builds retain these files.

## Azure and pipelines

Preparation runs only for local Debug builds, never pipeline/CI builds. Simulator settings are never changed by builds. `/p:PrepareLocalSignIn=false` optionally skips certificate/metadata preparation. For Azure sign-in, also restore the usual Azure simulator entries in the ignored config and select `useLocalSimulator = false` in dependency injection. Restart GIAS after changing its settings. Azure Pipelines selects Azure automatically.

## Checks

Focused tests cover 302/303 redirects, signature validation, tamper rejection, metadata loading through the existing SAML library.