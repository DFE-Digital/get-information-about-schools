# DataQualityStatus

## What it is

`DataQualityStatus` is a small Azure Table Storage record that tracks the review status of a dataset category.

It does not describe the quality of an individual school record.

Instead, it answers questions like:

- when was this dataset last reviewed
- who owns that dataset
- who should be contacted about it

## What one record represents

Each `DataQualityStatus` row represents one establishment dataset category, for example:

- Academy openers
- Free school openers
- Open academies and free schools
- LA maintained schools
- Independent schools
- Pupil referral units
- Secure academy 16 to 19 openers

These categories are defined in the `DataQualityEstablishmentType` enum in `Web/Edubase.Data/Entity/DataQualityStatus.cs`.

## What data is stored

Each record stores:

- `EstablishmentType`: the dataset category the row is about
- `LastUpdated`: when that dataset was last confirmed or reviewed
- `DataOwner`: the responsible team or owner name
- `Email`: the contact email for that owner

The row key is derived from the enum value for the dataset type, so the enum ordering matters.

## What the application uses it for

The web application uses `DataQualityStatus` to support a data stewardship workflow.

In practice this means:

- showing users the last reviewed date for each dataset
- showing the named data owner for each dataset
- allowing authorised users to confirm that their dataset has been reviewed
- allowing administrators to maintain the owner/contact details
- highlighting when a dataset has gone too long without being updated

## How the roles work

`DataQualityController` maps specific user roles to specific dataset categories.

That means a user is not updating all data quality records. They only update the dataset types their role is responsible for.

Administrators have broader access and can:

- update the review date
- update the owner name
- update the owner email

### Roles that can update `LastUpdated`

The following roles can update data quality review dates:

- `IsAdmin`
- `EFADO`
- `AP_AOS`
- `IEBT`
- `APT`
- `SOU`
- `FST`
- `YCS`

### Role to dataset mapping

The controller maps those roles to dataset categories as follows:

- `EFADO` -> `OpenAcademiesAndFreeSchools`
- `AP_AOS` -> `AcademyOpeners`
- `IEBT` -> `IndependentSchools`
- `APT` -> `PupilReferralUnits`
- `SOU` -> `LaMaintainedSchools`
- `FST` -> `FreeSchoolOpeners`
- `YCS` -> `AcademySecure16to19Openers`

In practice this means:

- a non-admin user can only update the `LastUpdated` value for the dataset mapped to their role
- an admin can update all dataset review dates
- only admins can update `DataOwner` and `Email`

## What “urgent” means

The controller compares `LastUpdated` with a configured update period.

If the last update is older than the allowed number of days, the UI can mark that dataset as urgent.

So “urgent” here means:

- this dataset has not been confirmed recently enough

It does not mean:

- there is definitely bad data in the dataset

## What it is not

`DataQualityStatus` is not:

- a score for a school
- a validation result for a single establishment
- a list of broken records
- a detailed audit trail of every data issue

It is best understood as dataset-level review metadata.

## Initial data

`DataQualityStatusRepository` seeds the table with an initial row for each known dataset category if the table is empty.

That gives the application a stable starting set of data quality records.

## Plain-English summary

A good mental model is:

- one `DataQualityStatus` record = one named dataset
- the record says who owns it and when it was last checked
- the web app uses that to drive reminders, visibility, and simple stewardship workflows

## Key code references

- `Web/Edubase.Data/Entity/DataQualityStatus.cs`
- `Web/Edubase.Data/Repositories/DataQualityStatusRepository.cs`
- `Web/Edubase.Web.UI/Controllers/DataQualityController.cs`
