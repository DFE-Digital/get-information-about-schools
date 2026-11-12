# Migration Progress
**Progress**: 10/24 tasks complete <progress value="42" max="100"></progress> 42%
**Status**: In Progress - Task 04-net10-application-migration

## Tasks

- ✅ 01-readiness-and-compatibility: Establish toolchain, compatibility baselines and decision gates ([Content](tasks/01-readiness-and-compatibility/task.md), [Progress](tasks/01-readiness-and-compatibility/progress-details.md))
  - ✅ 02.02-binding-and-package-conflicts: Review binding redirects and package divergence ([Content](tasks/02.02-binding-and-package-conflicts/task.md), [Progress](tasks/02.02-binding-and-package-conflicts/progress-details.md))
- ✅ 02-configuration-and-dependencies: Map configuration and review assembly/dependency conflicts ([Content](tasks/02-configuration-and-dependencies/task.md), [Progress](tasks/02-configuration-and-dependencies/progress-details.md))
- ❌ 03-sdk-project-conversion: Convert legacy project structure without changing target frameworks ([Content](tasks/03-sdk-project-conversion/task.md), [Progress](tasks/03-sdk-project-conversion/progress-details.md))
- 🔄 04-net10-application-migration: Migrate project TFMs, dependencies and application behaviour ([Content](tasks/04-net10-application-migration/task.md))
  - ✅ 04.01-common-data-net10: Migrate Common/Data closure to net10 with EF6 preserved ([Content](tasks/04.01-common-data-net10/task.md), [Progress](tasks/04.01-common-data-net10/progress-details.md))
  - ✅ 04.02-services-net10: Migrate Services closure to net10 preserving API contracts ([Content](tasks/04.02-services-net10/task.md), [Progress](tasks/04.02-services-net10/progress-details.md))
  - ✅ 04.03-texuna-resources-net10: Migrate Texuna/Resources closure to net10 preserving contracts ([Content](tasks/04.03-texuna-resources-net10/task.md), [Progress](tasks/04.03-texuna-resources-net10/progress-details.md))
  - ✅ 04.04-webui-host-foundation-net10: Retarget Web.UI and migrate host/pipeline/config/DI foundation ([Content](tasks/04.04-webui-host-foundation-net10/task.md), [Progress](tasks/04.04-webui-host-foundation-net10/progress-details.md))
  - 🔄 04.05-webui-feature-parity-net10: Migrate Web.UI controllers/views/contracts to behavior parity ([Content](tasks/04.05-webui-feature-parity-net10/task.md))
    - ✅ 04.05.01-webui-bootstrap-di-routing: Migrate Program/App_Start bootstrap, DI and routing to ASP.NET Core startup flow ([Content](tasks/04.05.01-webui-bootstrap-di-routing/task.md), [Progress](tasks/04.05.01-webui-bootstrap-di-routing/progress-details.md))
    - 🔄 04.05.02-webui-controllers-api-contracts: Migrate API + MVC/Area controller behavior and contract-sensitive surfaces ([Content](tasks/04.05.02-webui-controllers-api-contracts/task.md))
      - ✅ 04.05.02.01-webapi-controller-surface: Migrate Web API controller surface to ASP.NET Core action patterns ([Content](tasks/04.05.02.01-webapi-controller-surface/task.md), [Progress](tasks/04.05.02.01-webapi-controller-surface/progress-details.md))
      - ✅ 04.05.02.02-filter-pipeline-migration: Migrate API/MVC filter and exception handling pipeline components ([Content](tasks/04.05.02.02-filter-pipeline-migration/task.md), [Progress](tasks/04.05.02.02-filter-pipeline-migration/progress-details.md))
      - ✅ 04.05.02.03-model-binding-value-provider-migration: Migrate custom model binder and token value-provider infrastructure ([Content](tasks/04.05.02.03-model-binding-value-provider-migration/task.md), [Progress](tasks/04.05.02.03-model-binding-value-provider-migration/progress-details.md))
      - 🔄 04.05.02.04-mvc-area-controller-contract-parity: Migrate MVC/Area controller contract surfaces and route parity dependencies ([Content](tasks/04.05.02.04-mvc-area-controller-contract-parity/task.md), [Progress](tasks/04.05.02.04-mvc-area-controller-contract-parity/progress-details.md))
        - ✅ 04.05.02.04.01-shared-controller-base-and-helpers: Migrate shared MVC base controller and helper dependencies used by non-API controllers ([Content](tasks/04.05.02.04.01-shared-controller-base-and-helpers/task.md), [Progress](tasks/04.05.02.04.01-shared-controller-base-and-helpers/progress-details.md))
        - 🔄 04.05.02.04.02-root-mvc-controller-contracts: Migrate root (non-area) MVC controllers preserving route/action contracts ([Content](tasks/04.05.02.04.02-root-mvc-controller-contracts/task.md), [Progress](tasks/04.05.02.04.02-root-mvc-controller-contracts/progress-details.md))
        - 🔲 04.05.02.04.03-establishments-area-controller-contracts: Migrate Establishments area controllers and their route/redirect contracts ([Content](tasks/04.05.02.04.03-establishments-area-controller-contracts/task.md))
        - 🔲 04.05.02.04.04-groups-governors-area-controller-contracts: Migrate Groups/Governors area controllers and contract-sensitive flows ([Content](tasks/04.05.02.04.04-groups-governors-area-controller-contracts/task.md))
        - 🔲 04.05.02.04.05-controller-route-contract-check: Validate route/action contract parity for migrated MVC/Area controller surfaces ([Content](tasks/04.05.02.04.05-controller-route-contract-check/task.md))
      - 🔲 04.05.02.05-endpoint-contract-validation: Run targeted endpoint contract validation for migrated controller surfaces ([Content](tasks/04.05.02.05-endpoint-contract-validation/task.md))
    - 🔲 04.05.03-webui-views-assets-parity: Migrate Razor views/static assets to ASP.NET Core-compatible behavior ([Content](tasks/04.05.03-webui-views-assets-parity/task.md))
    - 🔲 04.05.04-webui-parity-validation: Execute focused Web.UI parity validation and document evidence ([Content](tasks/04.05.04-webui-parity-validation/task.md))
  - 🔲 04.06-cross-closure-validation-and-gates: Validate migrated closures and carry forward explicit blockers ([Content](tasks/04.06-cross-closure-validation-and-gates/task.md))
- 🔲 05-release-readiness: Prepare and rehearse the single-cutover release ([Content](tasks/05-release-readiness/task.md))
- 🔲 06-solution-validation: Validate full solution, parity and handover ([Content](tasks/06-solution-validation/task.md))

**Legend**: ✅ Complete | 🔄 In Progress | 🔲 Pending | ⚠️ Blocked | ❌ Failed
