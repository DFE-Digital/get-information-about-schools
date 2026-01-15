# Summary of Changes for Migration (t1-cip-migration)

This document summarizes the changes made to the Frontend Web App (`Edubase.Web.UI`) pipeline and configuration. These changes need to be replicated for the API and Backend Web App.

## 1. Pipeline Template Changes (`pipelines/template-deployment-stage.yml`)

The deployment template was updated to support injecting App Settings from Key Vault references.

*   **Added `appSettings` parameter:**
    ```yaml
    parameters:
      # ... existing parameters ...
      - name: appSettings
        default: ''
    ```

*   **Added `DeploymentKeyVaultName` variable:**
    This is required for the Key Vault references (e.g., `@Microsoft.KeyVault(VaultName=$(DeploymentKeyVaultName)...)`) to resolve correctly during deployment.
    ```yaml
    jobs:
      - deployment: ${{ parameters.thisJobName }}
        # ...
        variables:
          DeploymentKeyVaultName: ${{ parameters.keyVaultName }}
          # ...
    ```

*   **Passed `AppSettings` to `AzureRmWebAppDeployment` task:**
    ```yaml
              # ... inside AzureRmWebAppDeployment task ...
              enableCustomDeployment: true
              DeploymentType: 'webDeploy'

              ## App Settings (Key Vault References)
              AppSettings: ${{ parameters.appSettings }}
    ```

## 2. Pipeline Configuration Changes (`azure-pipelines.yml`)

The main pipeline file was updated to define the new environment variables, app settings, and updated infrastructure targets.

### Variables

*   **Added `isMigration` flag:**
    ```yaml
    isMigration: $[eq(variables['Build.SourceBranch'], 'refs/heads/am/t1-cip-migration')]
    ```

*   **Defined App Settings Variables:**
    Added `commonAppSettings`, `sandbox1AppSettings`, and `sandbox2AppSettings`. These contain newline-separated lists of Key Vault references.
    *   *Note: Ensure the Secret Names match the new Key Vault.*
    *   Example format: `-SettingName "@Microsoft.KeyVault(VaultName=$(DeploymentKeyVaultName);SecretName=Secret-Name)"`

### Stages & Jobs

*   **Updated Deployment Conditions:**
    Added `eq(variables.isMigration, 'true')` to the `condition` for Dev and Sandbox stages.
    ```yaml
    condition: and(succeeded(), or(., eq(variables.isMigration, 'true')))
    ```

*   **Updated Service Connections & Resource Names:**
    Updated the following parameters in `DeployDevSlotMain`, `DeployDevSandbox1`, and `DeployDevSandbox2` (and their smoke tests):
    *   `keyVaultServiceConnectionName`: Updated to `s158d.bsvc.cip.azdo`
    *   `keyVaultName`: Updated to `s158d01-gias-kv-01`
    *   `deploymentServiceConnectionName`: Updated to `s158d.bsvc.cip.azdo`
    *   `destinationResourceGroupName`: Updated to `s158d01-rg-gias-dev`
    *   `destinationWebAppName`: Updated to `s158d01-gias`

*   **Updated Slot Names:**
    *   Sandbox1: `dev-sandbox1` -> `sandbox1`
    *   Sandbox2: `dev-sandbox2` -> `sandbox2`

*   **Passed App Settings:**
    Passed the relevant variable to the template:
    ```yaml
    appSettings: $(commonAppSettings) # or $(sandbox1AppSettings), etc.
    ```

## 3. Web Configuration Changes (`Web/Edubase.Web.UI/Web.config`)

*   **Updated `SASimulatorUri`:**
    Pointed to the new simulator instance.
    ```xml
    <add key="SASimulatorUri" value="https://s158d01-gias-dsi-simulator.azurewebsites.net/" />
    ```
