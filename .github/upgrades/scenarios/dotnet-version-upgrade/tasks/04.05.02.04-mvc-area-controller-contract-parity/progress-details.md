## Task
04.05.02.04-mvc-area-controller-contract-parity

## Checkpoint summary (2026-11-11)
This parent task has been decomposed and actively executed in slices. Shared base/helper migration is complete, root-controller migration is partially complete, and area-controller/route-contract subtasks are pending.

## Decomposition status
- ✅ 04.05.02.04.01-shared-controller-base-and-helpers
- 🔄 04.05.02.04.02-root-mvc-controller-contracts (partial completion; handover details in child progress file)
- 🔲 04.05.02.04.03-establishments-area-controller-contracts
- 🔲 04.05.02.04.04-groups-governors-area-controller-contracts
- 🔲 04.05.02.04.05-controller-route-contract-check

## Migration execution pattern used so far
- Maintain small compile slices in `Web/Edubase.Web.UI/Edubase.Web.UI.csproj` so each migration cut builds independently.
- Migrate shared infrastructure first (base controller, JSON result helper, filters, model binding/value providers) before larger controller groups.
- Preserve route names/templates and redirect contract behavior first; postpone broad refactors.
- Preserve existing JSON/result semantics via shared helper path rather than introducing per-controller serializer behavior drift.
- Convert low-risk controllers first, then advance to high-complexity controllers with explicit blocker notes.

## Known cross-cutting risks carried forward
- Route-name drift can silently break redirect/link behavior.
- System.Web-specific request/cookie/context helpers in complex controllers need contract-by-contract replacement in ASP.NET Core.
- Logging-related legacy dependencies (notably Admin flows) may require sequencing with broader logging migration decisions.

## Validation cadence
- Each migration slice is validated with:
  - `dotnet build C:\copilot\get-information-about-schools\Web\Edubase.Web.UI\Edubase.Web.UI.csproj -v minimal`
- Known warning posture remains unchanged (user-approved AutoMapper advisory exposure while pinned, plus existing pre-existing warnings).