# Acceptance tests

These tests use Reqnroll and xUnit on .NET 10 to call a running GIAS website over HTTP. They do not launch a browser or start the website/backend.

[Establishments.feature](Establishments/Establishments.feature) covers retrieving an establishment as a back-office user, retrieval without signing in, and a URN that does not exist.

## Prepare a local run

You need the .NET 10 SDK, Node/npm, a populated local SQL database, and the local Tomcat backend.

1. Put the shared config files and `devsecrets.gias.signing.pfx.alwaysignore` in the repository root. These files are ignored by Git.
2. Manually copy the contents of `devsecrets.gias.config.local.alwaysignore` into `devsecrets.gias.config.alwaysignore`. Use the remote variant instead when testing with Azure services.
3. For local sign-in, ensure `SASimulatorUri` points to your checkout's `Web/Edubase.AcceptanceTests/.local-signin` directory as a `file:///` URI, and `SASimulatorGuid` is empty. Check that the website/API addresses and SQL connection match your machine.
4. From the repository root, run:

   ```powershell
   .\Web\LocalSetup.ps1
   ```

   This generates public SAML metadata from the shared PFX and installs/starts Azurite. Leave the terminal open. It does not build a project or change your config.
5. Start the local backend and the Debug website. Restart the website whenever you change the active config.

Local sign-in uses `UseLocalSignIn=true`; the remote variant uses `false` to select the Azure sign-in simulator. The active config must contain the complete selected variant before the website or tests run.

## Run the tests

Use Visual Studio Test Explorer, or run from the repository root:

```powershell
dotnet test Web/Edubase.AcceptanceTests/Edubase.AcceptanceTests.csproj
```

The tests do not seed data. The backend needs the test user `user_20170322155311_3600154` with sign-in ID `3600154` and back-office permissions. Establishment `141491` must have name `Landau Forte Academy Tamworth Sixth Form1` and type `Academy 16 to 19 sponsor led`. URN `0` must not exist.

## Add a scenario

Write the behaviour in a `.feature` file and implement its steps in the matching step definitions. Reuse the domain interfaces and register new implementations in [DependencyInjection.cs](DependencyInjection/DependencyInjection.cs). Do not edit generated `.feature.cs` files.

See [sign-in](SigninAuthorities/README.md) for the authentication flow and common failures.
