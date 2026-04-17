# Change Requests and Approvals

This document describes how the `GIAS Web Front End` handles change requests and approvals.

## Overview

The web application uses a mixed save model for establishment edits:

- Some changes are applied immediately
- Some changes require approval
- Approvers process pending items from a dedicated approvals queue

The front end is responsible for:

- Collecting the user's edits
- Calculating which changes require approval
- Showing the user a review summary
- Submitting the save request to the back end
- Exposing the approvals queue to users who can approve changes

The back end is responsible for:

- Applying immediate changes
- Creating pending change requests
- Enforcing approval rules
- Applying approvals or rejections
- Notifying the original editor

## Where Approval Rules Come From

Before saving an establishment, the front end retrieves the edit policy for that establishment from the back end using `EstablishmentReadApiService.cs`.

That API returns an `EstablishmentEditPolicyEnvelope.cs`, which contains:

- `EditPolicy`
- `ApprovalsPolicy`

The `ApprovalsPolicy` says:

- Which fields require approval
- Which approver group is responsible for each field

The helper methods on `EstablishmentApprovalsPolicy` include:

- `GetFieldsRequiringApproval()`
- `GetApproverName(fieldName)`

## How the Front End Detects Change Requests

When the user edits an establishment, the front end compares the edited model with the current persisted model using `GetModelChangesAsync(...)` in `EstablishmentReadApiService.cs`.

Each detected change becomes a `ChangeDescriptorDto.cs` containing:

- `Name`
- `OldValue`
- `NewValue`
- `RequiresApproval`
- `ApproverName`

This logic also handles:

- Standard establishment fields
- additional addresses
- SEN changes
- IEBT-related nested fields

The front end uses the approval policy to decide whether each detected change is:

- An immediate change
- A pending approval change request

## User Review Before Save

The establishment edit flow is handled in `EstablishmentController.cs`.

Before the final save, the front end builds a summary of the detected changes and stores it on the edit view model in `EditEstablishmentModel.cs`:

- `ChangesSummary`
- `ChangesRequireApprovalCount`
- `ChangesInstantCount`
- `ApprovalFields`
- `ChangeEffectiveDate`
- `OverrideCRProcess`

This lets the UI show the user:

- What will change
- How many changes are immediate
- How many changes will become pending approvals
- Who is expected to approve certain fields

If there are no actual changes, the controller returns the empty-change view instead of saving.

## Saving Changes

Once the user confirms the edit, the front end sends the establishment to the back end using `SaveAsync(...)` in `EstablishmentWriteApiService.cs`.

The save request includes:

- The edited establishment model
- An optional `effectiveDate`
- An `overrideCR` flag when allowed

The front end does not itself split the save into separate API calls for approved and non-approved fields. Instead, it submits the edited model and relies on the back end to:

- Apply the fields that can be updated immediately
- Create pending change requests for the fields that require approval

After save, the front end redirects back to the establishment details page and passes counts showing how many changes were:

- Approved immediately
- Left pending approval

## Override of the Change Request Process

The edit model includes `CanOverrideCRProcess` and `OverrideCRProcess`.

In the front end, the override option is only enabled for admins in `EstablishmentController.cs`.

At the service layer, `EstablishmentWriteApiService.cs` only sends `overrideCR=true` when the current user is in `ROLE_BACKOFFICE`.

This means:

- Normal users cannot bypass the approval workflow
- Back office/admin users can submit changes without creating pending approval items when that override is used

## Approvals Queue

Pending approvals are managed through the Approvals area in `ApprovalsController.cs`.

Access to this area is restricted to users in `AuthorizedRoles.CanApprove` from `AuthorizedRoles.cs`.

The Tools page exposes this as the "Manage change requests" link in `ToolsViewModel.cs`.

The queue is loaded through `ApprovalService.cs`, which calls the back-end endpoint:

- `approvals/pending`

The queue result is `PendingApprovalsResult.cs`, which contains a count and an array of `PendingApprovalItem`.

Each `PendingApprovalItem.cs` includes:

- The pending item ID
- Originator username and full name
- Approver username and full name
- Requested date
- Effective date
- Field name
- Old value
- New value
- Establishment URN, name and LAESTAB

## Approving and Rejecting Changes

Approvers can act on one or more pending items at once.

The action payload is `PendingChangeRequestAction.cs`, which contains:

- `Ids`
- `Action`
- `ActionSpecifier`
- `RejectionReason`

The main actions are:

- approve
- reject

Rejections require a reason. The MVC controller enforces that before posting the action to the back end in `ApprovalsController.cs`.

The front end sends approval actions to the back end through `ApprovalService.cs`, which posts to:

- `approvals/pending`

Success messages in the controller indicate that the original editor is notified by email after approval or rejection.

## API Endpoints Used by the Front End

The front end uses both MVC and Web API surfaces for approvals.

MVC routes:

- `ApprovalsController.Index` for the approvals page
- POST back to `ApprovalsController.Index` for approve/reject actions

AJAX/API routes:

- `ApprovalsApiController.cs`
- `GET /api/approvals/change-requests`
- `POST /api/approvals/change-request`

Both use the same `IApprovalService` underneath.

## Summary

In the web front end, a change request is created when the user edits establishment fields that the back-end approval policy marks as requiring approval. The front end detects those changes, shows the user a mixed summary of immediate and approval-controlled edits, submits the full save to the back end, and then exposes the resulting pending items through the Approvals queue for authorized approvers to approve or reject.
