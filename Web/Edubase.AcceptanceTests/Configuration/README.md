# Acceptance Tests - Configuration

## The Settings File
### [acceptance.tests.config.alwaysignore](acceptance.tests.config.alwaysignore)
This is the file read in by the acceptance tests in order to initialise the settings required.
It should be populated before running any tests. It is not committed to the repo and should
always be ignored by git. It is populated by manually copying the the settings from other local
files or from an automated process in the ADO pipeline.

## Local Configuration Files
### [acceptance.tests.config.dev.alwaysignore](acceptance.tests.config.dev.alwaysignore)
This file must be requested from other developers as it is never committed to git.
It contains all the settings required to run the acceptance tests against the
DEV version of the Web UI app in Azure. To run the acceptance tests against the DEV environment
copy and paste the contents of this file into `acceptance.tests.config.alwaysignore` and run the
tests locally on your machine.

### [acceptance.tests.config.test.alwaysignore](acceptance.tests.config.test.alwaysignore)
This file must be requested from other developers as it is never committed to git.
It contains all the settings required to run the acceptance tests against the
TEST version of the Web UI app in Azure. To run the acceptance tests against the TEST environment
copy and paste the contents of this file into `acceptance.tests.config.alwaysignore` and run the
tests locally on your machine.

## ADO Configuration Files
### [acceptance.tests.config](acceptance.tests.config)
This is a template file used in the ADO pipeline. It should always have empty values locally as it
is committed to the repo. It serves as a template from which the actual settings file is created
in the ADO pipeline post-deployment.

The ADO pipeline will populate the values in this file with the correct values for the environment
being deployed from the group variables defined in a library in ADO. The pipline then copies the
populated version of this file to `acceptance.tests.config.alwaysignore` so that the acceptance
tests can run against the correct environment.   
