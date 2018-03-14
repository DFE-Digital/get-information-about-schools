# Get Information about Schools

GIAS is predominantly an ASP.NET MVC application which acts as a UI wrapper around the Texuna API.  Authentication is performed via SAML.  In dev-mode, we use a Secure Access Simulator and for stage/production we use Texuna&#39;s Secure Access solution.  The project utilises IoC/DI extensively and there are unit tests that are run with each deployment.  There are also some adhoc integration tests as well, which are used for specific nullipotent scenarios.

Redis cache and MemoryCache are used to cache immutable lookup/dictionary data from the API.

JS assets are minified and combined by grunt; and the same goes for CSS, except we also use SASS.

The UI is largely a state machine atop a transient service layer, which communicates with the Texuna API.

There is a &quot;fake api&quot; project which can be used for stubbing out APIs when creating automated integration tests.  At the moment the Fake API is not used.

## Frameworks

- .NET Framework 4.6.1
- ASP.NET MVC 5.2.3

## Languages

- C# 6.0
- JavaScript
- HTML / Razor
- SASS

## Tools

- Visual Studio 2017
- NodeJS / npm
- Ruby
- Grunt
- Bower
- Nuget
- Git

## Integrations

- Texuna GIAS REST API
- SMTP
- Google Places/Maps APIs
- Google Analytics
- Companies House API
- Microsoft Azure Storage
- Redis Cache
- Secure Access
- Secure Access Simulator

## Notable dependencies

- Autofac – IoC/DI
- FluentValidation – model validation
- HttpAuthModule – basic auth
- Kentor.AuthServices – SAML authentication
- MoreLinq
- Automapper
- StackExchange.Redis

## SCM

The SCM is Git.  There are two main branches, dev and master.  Sprint dev work is usually done in feature branches and then merged into dev. At the end of each sprint we merge dev into master.

## Environments

There are a number of environments:

- Dev
- Stage
- LCS
- Production
- Pre-production
- Dev Tex QA

### Dev

[https://edubase-devtex.azurewebsites.net](https://edubase-devtex.azurewebsites.net)

Whenever the dev branch is updated, VSTS will deploy a new version of the app to this environment. Please note that the local development environments and the dev environment utilise the Secure Access Simulator to perform SAML authentication; the SA Simulator allows us to test SAML authentication without the overhead of integrating with the real SA, which is much more difficult in a test environment because Texuna&#39;s SA dev environment is locked down to specific IP addresses and we cannot access any &quot;admin back-end&quot; for it.

### Stage

[https://edubase-stage.azurewebsites.net](https://edubase-stage.azurewebsites.net)

Stage is CD&#39;d off master and it uses the real SA UAT environment for authentication.  It&#39;s normally deployed at the end of each sprint as you&#39;d expect.

### LCS

[https://ea-edubase-prod-lcs.azurewebsites.net](https://ea-edubase-prod-lcs.azurewebsites.net)

LCS is a slot off production.  LCS stands for Last Chance Saloon.  When a release is requested, we manually trigger a build and deploy to LCS in VSTS.  Unfortunately, LCS cannot authentication any users, so LCS really provides for an environment to do some very rudimentary tests prior to the swap with the prod slot.  E.g., just check there are no silly Razor compilation / JS /CSS issues, and that the main public search works properly.

### Production

[https://get-information-schools.service.gov.uk/](https://get-information-schools.service.gov.uk/)

Production is never deployed to directly and is only ever swapped into with LCS, enabling a very useful roll-back strategy if needed.

### Pre-production

There is a pre-production environment; though this is rarely used.

### Dev Text QA

[https://edubase-devtexqa.azurewebsites.net/](https://edubase-devtexqa.azurewebsites.net/)

Stands for Dev - Texuna – Quality Assurance.

This environment is mainly for use by a QA dev; the front-end is a state machine with some validation bits and pieces, so with this environment we set the base API URL to a mock API that has been &#39;injected into&#39; by the QA dev&#39;s test setups.   This allows for automated integration testing across the entire front-end.  The Fake API is located at http://fake-api.azurewebsites.net and the project is called Edubase.TexunaApi.Fake.

## API specification

Olive Jar wrote the API specification and Texuna conformed to it. Should a change be required to the API, the first step is to update the API YAML file located at: Architecture/Texuna Edubase API (functional).yaml in the Edubase repository.  Once Texuna have delivered the API change to their dev environment, we can they write the front-end side.  It&#39;s a good idea to try to make any API changes are backward compatible as possible; e.g., make any new parameters on existing APIs optional.

## Dev set-up / first time

Running the project for the first time is straightforward provided you have installed all the tools listed in the first page. Remember to compile the SASS/JS before running the project.

- The basic auth password can be found in app settings under &#39;Credentials&#39;
- The setting &#39;win:appStartup&#39; is set to &#39;SASimulatorConfiguration&#39; for dev; this configures the  app to use the SA Simulator for authentication.  It&#39;s set to &#39;SecureAccessConfiguration&#39; for non-dev environments, so it&#39;ll use the real SA.  See C# classes &#39;StartupSASimulator&#39; and &#39;StartupSecureAccess&#39; for more details.
- When attempting to login with the simulator, use the &#39;Edubase backoffice&#39; user, as this is the admin/root user so you&#39;ll have full permissions
- The API URL setting name is &#39;TexunaApiBaseAddress&#39; and set to &#39;https://edubase-dev.azurewebsites.net/edubase/rest/&#39; for testing
- The best way to view the API interaction is with Fiddler.  To stop all the noise of other network interactions, turn off capturing in Fiddler and add the following to config under system.net:

    &lt;defaultProxy&gt;

&lt;proxybypassonlocal=&quot;False&quot;usesystemdefault=&quot;True&quot;

proxyaddress=&quot;http://127.0.0.1:8888&quot; /&gt;

 &lt;/defaultProxy&gt;

## Performance issues

The API has often had issues with performance.  It takes a few seconds for some API calls to complete; such as when the user saves an establishment record.

The production database has been upgraded as of Feb 2018, which seems to have raised the performance somewhat.  It should be noted that the API also has a relatively high error rate – and when it does error, it&#39;ll sometimes return one of the following:

- A HTML error message with a 500 status code
- A JSON envelope that does not conform to any schema in the specification
- A JSON envelope that does conform to the error specification, but the error code is not recognised

Errors are recorded in Azure Table Storage and the partition key is the date in the format yyyyMMdd and the row key is a random error code.