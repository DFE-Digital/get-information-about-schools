# 06-solution-validation: Validate full solution, parity and handover

Build the complete solution and run all unit, integration and acceptance suites against the completed application, including Edubase.AcceptanceTests. Verify representative existing client requests and file fixtures, SOAP/WSDL integration boundaries, and browser/visual/accessibility journeys rather than treating compilation as compatibility proof. Compare results with baseline expectations; unexplained differences, missing external access and unexecuted required checks prevent a parity claim. Check published output and dependency/security results, remove migration-only assets, and confirm no temporary adapters, Framework targets, unsupported package fallbacks or placeholder code remain.

Record unresolved limitations honestly and update directly related operational/developer documentation. Register CPM as a deferred recommendation, not an authorised implementation: 'All projects are now SDK-style and on a single TFM - CPM can be added cleanly without VersionOverride friction.' Hand over evidence and release-readiness decisions for user review; executing this plan does not authorise production cutover.

**Done when**: Full solution builds warning-free, required tests and agreed client/file/UI checks pass, verified dependency findings are addressed, publication/rehearsal evidence and documentation are available, and no required compatibility gate is unverified. Any unavailable verification is reported as blocked, not complete. User approval remains required for production release.

## Research findings (2026-11-12)
- Full solution build currently fails at restore/build gate for `Web/Edubase.Web.UIUnitTests/Edubase.Web.UIUnitTests.csproj`.
- Root cause: `Edubase.Web.UIUnitTests` still targets `net48` and references `Edubase.Web.UI` now targeting `net10.0`, producing `NU1201` compatibility failure.
- This is a prerequisite migration gap for full-solution validation and test execution; until addressed, Task 06 cannot satisfy its "full solution builds/tests" completion criteria.
