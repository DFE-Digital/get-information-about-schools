# Start the local services that must be running while the application is being used.
# Run this script whenever you want to run the application locally, after LocalSetup.ps1 has been run at least once.
# This script does not install packages, create certificates or rebuild the application.

# Tell PowerShell to stop immediately if starting a required local service fails.
$ErrorActionPreference = 'Stop'

# Show the developer that this script is only starting the services needed while the application is running.
Write-Host "Starting local services required by the application..."

# Check that npm is available because npm tells us where the globally installed Azurite command is stored.
$npmCommand = Get-Command npm.cmd -ErrorAction SilentlyContinue

# Stop with a useful message if the one-off LocalSetup requirements are not available on this PC.
if (-not $npmCommand) {
    throw 'npm was not found. Run LocalSetup.ps1 after installing Node.js before trying to start local services.'
}

# Ask npm where globally installed command-line tools are stored on this PC.
$npmPrefix = (& npm.cmd prefix -g).Trim()

# Stop if npm cannot tell us where global command-line tools are installed.
if (-not $npmPrefix) {
    throw 'Could not determine the npm global installation directory.'
}

# Build the full filename of the Azurite command installed earlier by LocalSetup.ps1.
$azuriteCommand = Join-Path $npmPrefix 'azurite.cmd'

# Stop with a clear instruction if Azurite has not yet been installed on this PC.
if (-not (Test-Path $azuriteCommand)) {
    throw "Azurite is not installed. Run .\LocalSetup.ps1 first. Expected to find '$azuriteCommand'."
}

# Build the folder where Azurite will keep its local development storage files between runs.
$azuriteDataDirectory = Join-Path $env:USERPROFILE 'azurite-data'

# Create the Azurite data folder if this is the first time the local services have been started.
[void](New-Item -ItemType Directory -Path $azuriteDataDirectory -Force)

# Check whether Azurite is already running so we do not accidentally start a second copy on the same ports.
$existingAzurite = Get-CimInstance Win32_Process -Filter "Name = 'node.exe'" -ErrorAction SilentlyContinue |
    Where-Object { $_.CommandLine -match 'azurite' }

# If an Azurite process is already running, leave it alone and simply tell the developer that it is ready.
if ($existingAzurite) {
    Write-Host "Azurite is already running."
    Write-Host "Local services are ready - you can now start Edubase.Web.UI."
    return
}

# Add an empty line before showing the service details so they are easy to see in the PowerShell window.
Write-Host ""

# Tell the developer which Azurite command is about to be run.
Write-Host "Starting Azurite from:"
Write-Host "  $azuriteCommand"

# Tell the developer where Azurite will keep its local storage files.
Write-Host "Using local storage data in:"
Write-Host "  $azuriteDataDirectory"

# Explain that this PowerShell window must remain open because Azurite runs inside it until the developer stops it.
Write-Host "Leave this PowerShell window open while running the application."

# Confirm that the required service is ready to be started and the application can be launched once Azurite is listening.
Write-Host "Local services are starting - you can start Edubase.Web.UI once Azurite reports that it is listening."

# Add another empty line before Azurite starts writing its own messages to the PowerShell window.
Write-Host ""

# Start Azurite and keep it running in this PowerShell window until the developer stops it, normally with Ctrl+C.
& $azuriteCommand --location $azuriteDataDirectory

# If Azurite ever exits by itself with an error code, report that as a failure rather than silently returning to PowerShell.
if ($LASTEXITCODE -ne 0) {
    throw "Azurite exited with code $LASTEXITCODE."
}
