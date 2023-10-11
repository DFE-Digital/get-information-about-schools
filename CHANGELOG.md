# GIAS Web UI Change Log

These changes specifically relate to the C# Web UI and changes made via this GitHub repository.

These is a mostly technical change log, and many changes may not result in any visible changes to the UI or behaviour.

Additionally, changes to the UI / behaviour may be caused by changes applied to the internal/private GIAS backend API (not necessarily listed here).

For user-facing release notes, see the release notes published via MS Teams, Slack, and system update posts on the [GIAS website news section](https://get-information-schools.service.gov.uk/News).


<!-- 
TEMPLATE:

## [Unreleased] vNEXT (v)

https://github.com/DFE-Digital/get-information-about-schools/releases/tag/v


### 🌶️ Hot-Fixes (applied after release)

-  (#)

### 🆕 Added

-  (#)

### ✏️ Changed

-  (#)

### 🟡 Deprecated

-  (#)

### ❌ Removed

-  (#)

### 🔨 Fixed

-  (#)

### 🔑 Security

-  (#)

### 🔍 Internal (e.g., tooling or docs or release related)

-  (#)

### 📜 Full release notes

Note that these notes may include changes made outside of this GitHub repository.

Quoted from: https://get-information-schools.service.gov.uk/News?year=2023

> ...



-->

## [Unreleased] vNEXT (v4.55)

https://github.com/DFE-Digital/get-information-about-schools/releases/tag/v4.55

### 🌶️ Hot-Fixes (applied after release)

-  (#)

### 🆕 Added

- Bulk-added "release 3" Secure Academy changes from the epic/feature branch ([#439])
  - See pull request for list of changes made
- Allow users linked to the Youth Custody Service (YCS) and Secure Single-Academy Trusts (SSAT) to access bulk governance functionality ([#445] [AB#171959])
- Add the option for governance individuals, in limited circumstances, to be flagged as an "original signatory member" ([#444] [AB#172669] [#446] [AB#174529] [#452] [#453] [AB#177803] [#459])
- Add the option for governance individuals, in limited circumstances, to be flagged as an "original chair of trustees" ([#447] [AB#174527])
- Add additional validation to prevent an establishment group both being marked as "created in error" and also having a closure date defined (can't close if it never opened!) ([#448] [#454] [#457] [AB#174545] [#459])

### ✏️ Changed

- Additional changes for GIAS to function correctly when behind a proxy (e.g., Azure Front Door) ([#438] [AB#157607])

### 🟡 Deprecated

-  (#)

### ❌ Removed

-  (#)

### 🔨 Fixed

- Previous HOTFIX (2023-09-08) also applied to dev: Additional header to requests made to FSCP/CSCP ([#440] [#441])
- Make the governance tab visible, when editing a Secure Single-Academy-Trust (SSAT) ([#467] [AB#179004])
- Update the notice when closing a Secure Single Academy Trust (SSAT) / Single Academy Trust (SAT) to remove references to shared governors ([#450] [#455] [#456] [AB#174420])

### 🔑 Security

-  (#)

### 🔍 Internal (e.g., tooling or docs or release related)

- Migrate deployments to now use YAML syntax Azure Pipelines ([#460] [#468] [AB#178569])
- Add SonarCloud analysis as a PR check ([#464])

### 📜 Full release notes

Note that these notes may include changes made outside of this GitHub repository.

Quoted from: https://get-information-schools.service.gov.uk/News?year=2023

> ...



## [v4.54] - 2023-08-22

https://github.com/DFE-Digital/get-information-about-schools/releases/tag/v4.54

### 🌶️ Hot-Fixes (applied after release)

- HOTFIX (2023-09-08): Additional header to requests made to FSCP/CSCP ([#440] [AB#175627])

### 🆕 Added

- Allow for the API to provide customised list of establishment statuses, based on the currently-logged-in user ([#408], [AB#158754])

### ✏️ Changed

- Update description text and filenames to show that Secure Single-Academy Trusts (SSATs) now appear within the trust closures report ([#407] [AB#146409])
- Update the position of the Regional Schools Commissioner (RSC) region field within the establishment location tab ([#418] [AB#164244])
- Minor refactor to only call an API if the value is needed / is going to be used ([#421])
- Update help text to reflect a recent data update, where LSOA/MSOA data now reflects the 2021 census (previously the 2011 census) ([#423] [AB#167633])
- Stop showing a link to general academy guidance documentation/support when viewing a Secure Single-Academy Trust (SSAT) as it is not applicable to SSATs ([#425] [AB#168854])
- Miscellaneous edits to improve ability to run selenium tests ([#419] [#420] [#426] [#428])

### 🔨 Fixed

- Update the logic behind whether the edit button is visible or not for the establishment location tab ([#414] [#417] [AB#163004] [AB#167518])

### 🔍 Internal (e.g., tooling or docs or release related)

- Add build and test checks (GitHub Actions) to the PR process ([#415])
- Add a new configuration option to allow overriding of the "public origin" (used for login redirect URLs) in preparation for moving GIAS behind a proxy/WAF ([#430] [AB#157607])

### 📜 Full release notes

Note that these notes may include changes made outside of this GitHub repository.

Quoted from: https://get-information-schools.service.gov.uk/News?year=2023

> An updated version of the Get Information about Schools (GIAS) service has been released to all users. You do not need to take any specific action unless otherwise indicated below.
> 
> This release includes the following improvements:
> 
> - Bug fixes and fine-tuning to improve the overall user experience, including:
>   - Issue with changing establishment types – internal only
>   - Update guidance: local authority name and associated codes for Cumberland and Westmorland and Furness
>   - Update guidance: local authority name and associated codes for North Yorkshire and Somerset
> 
> Further information
> 
> 1. Office for National Statistics (ONS) updates
>    
> The update to the geo spatial fields that are updated from the Office for National Statistics (ONS) has now been completed.
> 
> The relevant fields are:
> 
> - Government Statistical Service (GSS) local authority (LA) code (also known as the Office for National Statistics (ONS) local authority (LA) number) name
> - Government Statistical Service (GSS) local authority (LA) code (also known as the Office for National Statistics (ONS) local authority (LA) number) code
> - administrative district name
> - administrative district code
> - ward name
> - ward code
> - parliamentary constituency name
> - parliamentary constituency code
> - urban/rural description name
> - urban/rural description code
> - middle super output area (MSOA) name
> - middle super output area (MSOA) code
> - lower super output area (LSOA) name
> - lower super output area (LSOA) code
> 
> This also updated those fields that had a temporary code in place with their new ONS data following the local government reorganisations in April 2023 (see previous system updates for more information on this).
> 
> - Government Statistical Service (GSS) local authority (LA) code (also known as the Office for National Statistics (ONS) local authority (LA) number) name
> - Government Statistical Service (GSS) local authority (LA) code (also known as the Office for National Statistics (ONS) local authority (LA) number) code
> - administrative district name
> - administrative district code
> 
> 2. Yearly update of data from the underlying data of the schools, pupils and their characteristics statistical release
> 
> The yearly update of data from the underlying data of the schools, pupils and their characteristics statistical publication will take place shortly over several days. A notice will be added to the service whilst this work is undertaken and then updated once all work is completed.
> 
> The fields that will be updated are the:
> 
> - total number of pupils
> - total number of boys
> - total number of girls
> - number of pupils eligible for free school meals (FSM)
> - percentage of children eligible for free school meals (FSM)
> 
> 3. Yearly update of data from the school level annual school census (SLASC) for independent schools only
> 
> The yearly update of data from the school level annual school census (SLASC) data collection will take place shortly over several days for independent schools only. A notice will be added to the service whilst this work is undertaken and then updated once all work is completed.
> 
> The fields that will be updated (where applicable) are:
> 
> - street
> - locality
> - address3
> - town
> - county
> - postcode
> - establishment name
> - head first name
> - head last name
> - head title
> - number of pupils
> - number of special pupils under a special educational needs (SEN) statement/education, health and care (EHC) plan
> - number of special pupils not under a special educational needs (SEN) statement/education, health and care (EHC) plan
> 
> Kind regards
> 
> GIAS support team


## [v4.53] - 2023-06-28

https://github.com/DFE-Digital/get-information-about-schools/releases/tag/v4.53

<!--
### 🌶️ Hot-Fixes (applied after release)

-  (#)

### 🆕 Added

-  (#)

### ✏️ Changed

-  (#)

### 🟡 Deprecated

-  (#)

### ❌ Removed

-  (#)

### 🔨 Fixed

-  (#)

### 🔑 Security

-  (#)

### 🔍 Internal (e.g., tooling or docs or release related)

-  (#)

-->

### 📜 Full release notes

Note that these notes may include changes made outside of this GitHub repository.

Quoted from: https://get-information-schools.service.gov.uk/News?year=2023

> An updated version of the Get Information about Schools (GIAS) service has been released to all users. You do not need to take any specific action unless otherwise indicated below.
> 
> This release includes the following improvements:
> 
> - Bug fixes and fine-tuning to improve the overall user experience, including:
>   - countries should not be listed in the county field
>   - remove link to any single-academy trust (SAT) pages on Find and Check the Performance of Schools and Colleges in England in the "Data from other services" section to reflect changes made on this service
>   - fix to stop non-children's centres establishment types being added to children’s centre groups
>   - error message is now displayed when there is no change made in the edit federation group page and save is selected
>   - remove button is now only needed to be selected once to remove a pre-defined local authority set
>   - create a federation process pop up to remind users to update establishment governance when creating a federation
>   - GIAS support team can now edit data owner contact details on service for status page
>   - add a new field for the online providers establishment type only called accreditation expiry date
>   - status of pending approval is now default on creation of an online provider establishment type record and can now be selected for online providers when editing by DfE team
> 
> Further information
> 
> 1. Office for National Statistics (ONS) updates
> 
> An update to the geo spatial fields that are updated from the Office for National Statistics (ONS) will be undertaken soon.
> 
> The relevant fields are:
> 
> - Government Statistical Service (GSS) local authority (LA) code (also known as the Office for National Statistics (ONS) local authority (LA) number) name
> - Government Statistical Service (GSS) local authority (LA) code (also known as the Office for National Statistics (ONS) local authority (LA) number) code
> - administrative district name
> - administrative district code
> - ward name
> - ward code
> - parliamentary constituency name
> - parliamentary constituency code
> - urban/rural description name
> - urban/rural description code
> - middle super output area (MSOA) name
> - middle super output area (MSOA) code
> - lower super output area (LSOA) name
> - lower super output area (LSOA) code
> 
> This will also update those fields that have had a temporary code in place with their new ONS data following the local government reorganisations in April 2023 (see previous system updates for more information on this).
> 
> - Government Statistical Service (GSS) local authority (LA) code (also known as the Office for National Statistics (ONS) local authority (LA) number) name
> - Government Statistical Service (GSS) local authority (LA) code (also known as the Office for National Statistics (ONS) local authority (LA) number) code
> - administrative district name
> - administrative district code
> 
> We will let users know once this work has been completed.
> 
> 2. Yearly update of data from the underlying data of the schools, pupils and their characteristics statistical release
>    
> The yearly update of data from the underlying data of the schools, pupils and their characteristics statistical publication will take place shortly over several days. A notice will be added to the service whilst this work is undertaken and then updated once all work is completed.
> 
> The fields that will be updated are the:
> 
> - total number of pupils
> - total number of boys
> - total number of girls
> - number of pupils eligible for free school meals (FSM)
> - percentage of children eligible for free school meals (FSM)
> 
> Kind regards
> 
> 
> GIAS support team


## [v4.52] - 2023-03-21

https://github.com/DFE-Digital/get-information-about-schools/releases/tag/v4.52

### 📜 Full release notes

Note that these notes may include changes made outside of this GitHub repository.

Quoted from: https://get-information-schools.service.gov.uk/News?year=2023

> An updated version of the Get Information about Schools (GIAS) service has been released to all users. You do not need to take any specific action unless otherwise indicated below.
> 
> This release includes the following improvements:
> 
> - Bug fixes and fine-tuning to improve the overall user experience, including:
> 
>   - pagination issue when navigating to last page
>   - search term text box behaviour when using the browser back button
>   - download LA name code data in ZIP format
>   - glossary formatting
>   - no effective date set from bulk update establishments tool
>   - bulk update instructions - LA field name is incorrect
>   - rename DfE service reference of the School Performance service to Find and Compare School and College Data in England (FSCP)
>   - add MSOA & LSOA pop-up message on the tool tip
> 
> Further information
> 
> Office for National Statistics (ONS) update
> 
> The following fields have been updated this month (where required) following the March 2023 update of the Office for National Statistics (ONS) data:
> 
> - District
> - Ward
> - Parliamentary constituency
> - Urban/rural description
> - Middle super output area (MSOA)
> - Lower super output area (LSOA)
> 
> Cumbria, North Yorkshire, and Somerset local government reorganisations (LGRs) 01/04/2023
> 
> We posted a note on this news page on 03/03/2023 around the local government reorganisations so if you need to know any more information, please review that post.
> 
> Kind regards
> 
> GIAS support team


## [v4.51] - 2022-11-15

https://github.com/DFE-Digital/get-information-about-schools/releases/tag/v4.51

Note that these notes may include changes made outside of this GitHub repository.

Quoted from: https://get-information-schools.service.gov.uk/News?year=2022

### 📜 Full release notes

> An updated version of the Get Information about Schools (GIAS) service has been released to all users. You do not need to take any specific action unless otherwise indicated below.
>
> This release includes the following improvements:
>
> - Bug fixes and fine-tuning to improve the overall user experience, including:
>
>   - fix establishment breadcrumb which is incorrect in edit mode on IEBT tab
>   - fix establishment edit view local authority code
>   - fix establishment change history missing pagination chevrons
>   - fix pagination functionality in GIAS with missing chevrons
>   - fix spelling error in the academy joined date error message
>   - fix news and sign out links where overlapping
>
> Further information
> 
> Office for National Statistics (ONS) frequency update
> 
> We mentioned in our last release note that we had paused the frequency update from yearly to quarterly of the ONS data as we needed to resolve some technical issues and advised that we would keep users updated. This work is still paused, and we will provide a further update in our next release.
> 
> For reference the fields updated are:
> 
> - District
> - Ward
> - Parliamentary constituency
> - Urban/rural description
> - Middle super output area (MSOA)
> - Lower super output area (LSOA)
>
> Kind regards
>
> GIAS support team



## [v4.50] - 2022-09-16

https://github.com/DFE-Digital/get-information-about-schools/releases/tag/v4.50

### 📜 Full release notes

Note that these notes may include changes made outside of this GitHub repository.

Quoted from: https://get-information-schools.service.gov.uk/News?year=2022

> An updated version of the Get Information about Schools (GIAS) service has been released to all users. You do not need to take any specific action unless otherwise indicated below.
> 
> This release includes the following improvements:
> 
> - Bug fixes and fine-tuning to improve the overall user experience, including:
>   - Corrected incorrect establishment breadcrumb in edit mode.
>   - Added more padding between 'Search' and 'Clear search' on the download no results page.
>   - Added breadcrumbs to the establishment governance tab for "Edit Person" and "Replace Person" pages in edit mode.
>   - Amended the alignment and increased the font of 'Clear search' on the individual search - choose specific download no results page.
>   - Update styling for 'Closed' label.
> 
> Further information
> 
> Office for National Statistics (ONS) frequency update
> 
> We mentioned in our last release note that we would be increasing the frequency from yearly to quarterly. We still plan on doing this, but we need to resolve some technical issues before we can complete the next update. We will keep users updated with progress on this work.
> 
> For reference the fields updated are:
> 
> - District
> - Ward
> - Parliamentary constituency
> - Urban/rural description
> - Middle super output area (MSOA)
> - Lower super output area (LSOA)
> - Further information
> 
> The yearly update of data from the underlying data of the schools, pupils and their characteristics statistical publication has now been completed.
> 1. The fields that were updated were the:
> 
>   - total number of pupils
>   - total number of boys
>   - total number of girls
>   - number of pupils eligible for free school meals (FSM)
>   - percentage of children eligible for free school meals (FSM)
> 
> 2. The yearly update of data from the school level annual school census (SLASC) data collection for independent schools has now been completed.
>
> Kind regards
>
> GIAS support team



[unreleased]: https://github.com/DFE-Digital/get-information-about-schools/compare/v4.54...dev
[v4.54]: https://github.com/DFE-Digital/get-information-about-schools/compare/v4.53...v4.54
[v4.53]: https://github.com/DFE-Digital/get-information-about-schools/compare/v4.52...v4.53
[v4.52]: https://github.com/DFE-Digital/get-information-about-schools/compare/v4.51...v4.52
[v4.51]: https://github.com/DFE-Digital/get-information-about-schools/compare/v4.50...v4.51
[v4.50]: https://github.com/DFE-Digital/get-information-about-schools/compare/v4.49...v4.50


[#407]: https://github.com/DFE-Digital/get-information-about-schools/pull/407
[#408]: https://github.com/DFE-Digital/get-information-about-schools/pull/408
[#414]: https://github.com/DFE-Digital/get-information-about-schools/pull/414
[#415]: https://github.com/DFE-Digital/get-information-about-schools/pull/415
[#417]: https://github.com/DFE-Digital/get-information-about-schools/pull/417
[#418]: https://github.com/DFE-Digital/get-information-about-schools/pull/418
[#419]: https://github.com/DFE-Digital/get-information-about-schools/pull/419
[#420]: https://github.com/DFE-Digital/get-information-about-schools/pull/420
[#421]: https://github.com/DFE-Digital/get-information-about-schools/pull/421
[#423]: https://github.com/DFE-Digital/get-information-about-schools/pull/423
[#425]: https://github.com/DFE-Digital/get-information-about-schools/pull/425
[#426]: https://github.com/DFE-Digital/get-information-about-schools/pull/426
[#428]: https://github.com/DFE-Digital/get-information-about-schools/pull/428
[#430]: https://github.com/DFE-Digital/get-information-about-schools/pull/430
[#438]: https://github.com/DFE-Digital/get-information-about-schools/pull/438
[#439]: https://github.com/DFE-Digital/get-information-about-schools/pull/439
[#440]: https://github.com/DFE-Digital/get-information-about-schools/pull/440
[#440]: https://github.com/DFE-Digital/get-information-about-schools/pull/440
[#441]: https://github.com/DFE-Digital/get-information-about-schools/pull/441
[#444]: https://github.com/DFE-Digital/get-information-about-schools/pull/444
[#445]: https://github.com/DFE-Digital/get-information-about-schools/pull/445
[#446]: https://github.com/DFE-Digital/get-information-about-schools/pull/446
[#447]: https://github.com/DFE-Digital/get-information-about-schools/pull/447
[#448]: https://github.com/DFE-Digital/get-information-about-schools/pull/448
[#450]: https://github.com/DFE-Digital/get-information-about-schools/pull/450
[#452]: https://github.com/DFE-Digital/get-information-about-schools/pull/452
[#453]: https://github.com/DFE-Digital/get-information-about-schools/pull/453
[#454]: https://github.com/DFE-Digital/get-information-about-schools/pull/454
[#455]: https://github.com/DFE-Digital/get-information-about-schools/pull/455
[#456]: https://github.com/DFE-Digital/get-information-about-schools/pull/456
[#457]: https://github.com/DFE-Digital/get-information-about-schools/pull/457
[#459]: https://github.com/DFE-Digital/get-information-about-schools/pull/459
[#459]: https://github.com/DFE-Digital/get-information-about-schools/pull/459
[#460]: https://github.com/DFE-Digital/get-information-about-schools/pull/460
[#464]: https://github.com/DFE-Digital/get-information-about-schools/pull/464
[#467]: https://github.com/DFE-Digital/get-information-about-schools/pull/467
[#468]: https://github.com/DFE-Digital/get-information-about-schools/pull/468


[#]: https://github.com/DFE-Digital/get-information-about-schools/pull/
[#]: https://github.com/DFE-Digital/get-information-about-schools/pull/
[#]: https://github.com/DFE-Digital/get-information-about-schools/pull/

[AB#146409]: https://dfe-ssp.visualstudio.com/s158-Get-Information-About-Schools/_workitems/edit/146409
[AB#157607]: https://dfe-ssp.visualstudio.com/s158-Get-Information-About-Schools/_workitems/edit/157607
[AB#158754]: https://dfe-ssp.visualstudio.com/s158-Get-Information-About-Schools/_workitems/edit/158754
[AB#163004]: https://dfe-ssp.visualstudio.com/s158-Get-Information-About-Schools/_workitems/edit/163004
[AB#164244]: https://dfe-ssp.visualstudio.com/s158-Get-Information-About-Schools/_workitems/edit/164244
[AB#167518]: https://dfe-ssp.visualstudio.com/s158-Get-Information-About-Schools/_workitems/edit/167518
[AB#167633]: https://dfe-ssp.visualstudio.com/s158-Get-Information-About-Schools/_workitems/edit/167633
[AB#168854]: https://dfe-ssp.visualstudio.com/s158-Get-Information-About-Schools/_workitems/edit/168854
[AB#171959]: https://dfe-ssp.visualstudio.com/s158-Get-Information-About-Schools/_workitems/edit/171959
[AB#172669]: https://dfe-ssp.visualstudio.com/s158-Get-Information-About-Schools/_workitems/edit/172669
[AB#174420]: https://dfe-ssp.visualstudio.com/s158-Get-Information-About-Schools/_workitems/edit/174420
[AB#174527]: https://dfe-ssp.visualstudio.com/s158-Get-Information-About-Schools/_workitems/edit/174527
[AB#174529]: https://dfe-ssp.visualstudio.com/s158-Get-Information-About-Schools/_workitems/edit/174529
[AB#174545]: https://dfe-ssp.visualstudio.com/s158-Get-Information-About-Schools/_workitems/edit/174545
[AB#175627]: https://dfe-ssp.visualstudio.com/s158-Get-Information-About-Schools/_workitems/edit/175627
[AB#177803]: https://dfe-ssp.visualstudio.com/s158-Get-Information-About-Schools/_workitems/edit/177803
[AB#178569]: https://dfe-ssp.visualstudio.com/s158-Get-Information-About-Schools/_workitems/edit/178569
[AB#179004]: https://dfe-ssp.visualstudio.com/s158-Get-Information-About-Schools/_workitems/edit/179004

[AB#]: https://dfe-ssp.visualstudio.com/s158-Get-Information-About-Schools/_workitems/edit/
[AB#]: https://dfe-ssp.visualstudio.com/s158-Get-Information-About-Schools/_workitems/edit/
[AB#]: https://dfe-ssp.visualstudio.com/s158-Get-Information-About-Schools/_workitems/edit/

