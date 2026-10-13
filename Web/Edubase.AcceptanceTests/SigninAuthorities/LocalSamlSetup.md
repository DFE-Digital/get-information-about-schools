# Self-contained local SAML

All simulator code is in the acceptance-test project. GIAS uses its existing SAML federation loader and normal login, ACS and callback endpoints. No extra web-app code, endpoint, Function app or local HTTP service is required.

## Run locally

Run `Web/LocalSetup.ps1` from the repository root (or `./LocalSetup.ps1` from
`Web`). It generates the signing certificate and metadata directly and installs/starts
Azurite. Application configuration is shared manually; the script does not edit
it. Leave its terminal open, then start or restart the Debug website.

The script and acceptance tests read the shared repository-root `devsecrets.gias.signing.pfx.alwaysignore` directly. The script writes only public `Metadata` and `Federation` XML into `Web/Edubase.AcceptanceTests/.local-signin`. No project build is required. The shared configuration must point `SASimulatorUri` to this directory's file URI and have an empty `SASimulatorGuid`. Nothing is installed in the Windows certificate store.

Then start the usual Debug website and run the acceptance tests. If the website was already running when its settings changed, restart it once. Existing website/backend setup and localhost HTTPS trust are still required. 

The URI points to a local file, not a server. The existing federation loader reads the public certificate and SSO address from that metadata. `SigninSimulatorOnLocalMachine` intercepts the SAML redirect, signs the response itself, posts it to `/Saml2/Acs` and follows the callback. The placeholder `http://localhost/local-signin` is never contacted. The callback performs normal backend role lookup and issues the application cookie.

Keys and metadata are ignored by Git. The private key is used only by the tests; GIAS reads public metadata. Nothing is installed in the Windows certificate store. Ordinary clean builds retain these files.

## Azure and pipelines

Preparation is explicit through LocalSetup.ps1 and is no longer a post-build step. The script does not modify simulator settings. For Azure sign-in, also restore the usual Azure simulator entries in the ignored config and select `useLocalSimulator = false` in dependency injection. Restart GIAS after changing its settings. Azure Pipelines selects Azure automatically.

## Checks

Focused tests cover 302/303 redirects, signature validation, tamper rejection, metadata loading through the existing SAML library.
