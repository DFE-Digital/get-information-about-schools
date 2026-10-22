# Set up everything this repository needs before a developer runs the application for the first time.
# This script is intended to be run after the repository has first been pulled onto a developer's PC.
# It creates the local sign-in files, installs the front-end packages, builds the front-end files and installs Azurite.
# It does not start any long-running services. Use StartLocalServices.ps1 for that.

# Tell PowerShell to stop immediately if any command reports an error, rather than carrying on with a half-finished setup.
$ErrorActionPreference = 'Stop'

# Show the developer what this script is doing so it is clear that this is the one-off setup stage.
Write-Host "Setting up this repository for local development. Please wait..."

# Work out the Web folder by using the folder that this script itself is stored in.
$webRoot = $PSScriptRoot

# Work out the repository root, which is the folder immediately above the Web folder.
$repoRoot = Split-Path $PSScriptRoot -Parent

# Build the path to the folder used by the acceptance tests for local sign-in information.
$signinDirectory = Join-Path $webRoot 'Edubase.AcceptanceTests\.local-signin'

# Build the path to the web user-interface project where package.json and the front-end code live.
$uiDirectory = Join-Path $webRoot 'Edubase.Web.UI'

# Create the local sign-in folder if it does not already exist; -Force makes this safe to run again later.
[void](New-Item -ItemType Directory -Path $signinDirectory -Force)

# Build the filename used to store the local certificate that signs the test SAML sign-in messages.
$certificatePath = Join-Path $repoRoot 'devsecrets.gias.signing.pfx.alwaysignore'

# The local development certificate deliberately has no password, so keep the plain-text password empty.
$pfxPasswordText = ''

# Create the empty secure password object that PowerShell requires when exporting a PFX certificate.
$pfxPassword = New-Object System.Security.SecureString

# Only create a certificate when the repository does not already have the local development certificate.
if (-not (Test-Path $certificatePath)) {

    # Tell the developer that a new local certificate is being created.
    Write-Host "Local signing certificate not found. Creating one..."

    # Create a self-signed certificate for local development only; this certificate is not used in real environments.
    $generatedCertificate = New-SelfSignedCertificate `
        -Subject 'CN=GIAS Local SAML Signing' `
        -CertStoreLocation 'Cert:\CurrentUser\My' `
        -KeyAlgorithm RSA `
        -KeyLength 2048 `
        -HashAlgorithm SHA256 `
        -KeyExportPolicy Exportable `
        -KeyUsage DigitalSignature `
        -NotAfter (Get-Date).AddYears(5)

    # Use try/finally so the temporary certificate is removed from the Windows certificate store even if exporting fails.
    try {

        # Save the new certificate into the repository's local development certificate file.
        Export-PfxCertificate `
            -Cert $generatedCertificate `
            -FilePath $certificatePath `
            -Password $pfxPassword | Out-Null

        # Confirm where the local certificate file was created.
        Write-Host "Created local signing certificate:"
        Write-Host "  $certificatePath"
    }
    finally {

        # Remove the temporary copy from the Windows certificate store because the application uses the exported file instead.
        Remove-Item "Cert:\CurrentUser\My\$($generatedCertificate.Thumbprint)" -ErrorAction SilentlyContinue
    }
}
else {

    # Tell the developer that the existing certificate will be reused instead of creating another one.
    Write-Host "Using existing local signing certificate:"
    Write-Host "  $certificatePath"
}

# Open the local certificate file so its public certificate can be placed into the SAML metadata files.
$certificate = [System.Security.Cryptography.X509Certificates.X509Certificate2]::new(
    $certificatePath,
    $pfxPasswordText,
    [System.Security.Cryptography.X509Certificates.X509KeyStorageFlags]::EphemeralKeySet
)

# Use try/finally so the certificate is always released from memory after the SAML files have been created.
try {

    # Convert the public part of the certificate into the text format expected inside a SAML metadata file.
    $publicCertificate = [Convert]::ToBase64String($certificate.RawData)

    # Convert the local sign-in folder into a file URI because that is used as the local SAML issuer name.
    $baseUri = ([uri]($signinDirectory + [IO.Path]::DirectorySeparatorChar)).AbsoluteUri.TrimEnd('/')

    # Add /Metadata to the local URI and escape any characters that would otherwise be invalid inside XML.
    $issuer = [System.Security.SecurityElement]::Escape($baseUri + '/Metadata')

    # Build the SAML metadata XML that tells the application about the local test sign-in service and its signing certificate.
    $entity = @"
<EntityDescriptor xmlns="urn:oasis:names:tc:SAML:2.0:metadata" entityID="$issuer">
  <IDPSSODescriptor protocolSupportEnumeration="urn:oasis:names:tc:SAML:2.0:protocol">
    <KeyDescriptor use="signing">
      <KeyInfo xmlns="http://www.w3.org/2000/09/xmldsig#">
        <X509Data><X509Certificate>$publicCertificate</X509Certificate></X509Data>
      </KeyInfo>
    </KeyDescriptor>
    <SingleSignOnService Binding="urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect" Location="http://localhost/local-signin" />
  </IDPSSODescriptor>
</EntityDescriptor>
"@

    # Save the single SAML identity-provider description into the Metadata file expected by the local application.
    [IO.File]::WriteAllText((Join-Path $signinDirectory 'Metadata'), $entity)

    # Save the same identity-provider description inside a Federation wrapper because the application also expects that file.
    [IO.File]::WriteAllText(
        (Join-Path $signinDirectory 'Federation'),
        '<EntitiesDescriptor xmlns="urn:oasis:names:tc:SAML:2.0:metadata">' + $entity + '</EntitiesDescriptor>'
    )

    # Confirm where the two local SAML files were written.
    Write-Host "Created local SAML Metadata and Federation files in:"
    Write-Host "  $signinDirectory"
}
finally {

    # Release the certificate object now that it is no longer needed.
    $certificate.Dispose()
}

# Stop with a clear explanation if the web user-interface project cannot be found where this repository expects it.
if (-not (Test-Path $uiDirectory)) {
    throw "Front-end project folder was not found at '$uiDirectory'."
}

# Build the full path to package.json, which tells npm which front-end packages and build commands this project uses.
$packageJson = Join-Path $uiDirectory 'package.json'

# Stop with a clear explanation if package.json is missing because npm setup cannot continue without it.
if (-not (Test-Path $packageJson)) {
    throw "package.json was not found at '$packageJson'."
}

# Check that npm is available before trying to install anything, so a missing Node.js installation gives a useful error.
$npmCommand = Get-Command npm.cmd -ErrorAction SilentlyContinue

# Stop and explain what is missing if Node.js/npm has not been installed on this PC.
if (-not $npmCommand) {
    throw 'npm was not found. Install Node.js, which includes npm, and then run LocalSetup.ps1 again.'
}

# Tell the developer that the front-end packages defined by this repository are about to be installed.
Write-Host "Installing front-end npm dependencies..."

# Temporarily change into the web user-interface project because npm commands need to run beside package.json.
Push-Location $uiDirectory

# Use try/finally so PowerShell always returns to the developer's original folder after the npm work finishes.
try {

    # Install the front-end packages required by the repository.
    npm.cmd install

    # Stop immediately if npm reported that installing the packages failed.
    if ($LASTEXITCODE -ne 0) {
        throw "npm install failed with exit code $LASTEXITCODE."
    }

    # Read package.json into a PowerShell object so we can see whether the repository defines a build command.
    $package = Get-Content $packageJson -Raw | ConvertFrom-Json

    # Only run a front-end build when package.json actually contains a script named build.
    if ($package.scripts -and $package.scripts.build) {

        # Tell the developer that the JavaScript/CSS front-end files are now being built.
        Write-Host "Building front-end assets..."

        # Run the build command defined by the repository rather than hard-coding a particular build tool here.
        npm.cmd run build

        # Stop immediately if the front-end build reports a failure.
        if ($LASTEXITCODE -ne 0) {
            throw "npm run build failed with exit code $LASTEXITCODE."
        }
    }
    else {

        # Warn rather than fail if this repository no longer defines a build script, because the package installation itself succeeded.
        Write-Warning "No 'build' script is defined in $packageJson. Front-end dependencies were installed, but no asset build was run."
    }
}
finally {

    # Return PowerShell to the folder the developer was in before the npm commands started.
    Pop-Location
}

# Build the path to the generated front-end folder that the .NET web application expects to find when it starts.
$expectedBuildDirectory = Join-Path $uiDirectory 'public\assets\scripts\build'

# Warn if the expected generated folder still does not exist after running the repository's build command.
if (-not (Test-Path $expectedBuildDirectory)) {
    Write-Warning "The expected front-end build directory still does not exist:"
    Write-Warning "  $expectedBuildDirectory"
    Write-Warning "Check the scripts section of Edubase.Web.UI\package.json for the correct front-end build command."
}
else {

    # Confirm that the front-end output required by the web application now exists.
    Write-Host "Front-end assets are available at:"
    Write-Host "  $expectedBuildDirectory"
}

# Tell the developer that Azurite, the local replacement for Azure Storage, is about to be installed or updated.
Write-Host "Installing/updating Azurite..."

# Install Azurite globally so it can be started later by StartLocalServices.ps1 from anywhere on this PC.
npm.cmd install -g azurite

# Stop if npm was unable to install Azurite successfully.
if ($LASTEXITCODE -ne 0) {
    throw 'Azurite installation failed.'
}

# Ask npm where globally installed command-line tools are stored on this PC.
$npmPrefix = (& npm.cmd prefix -g).Trim()

# Stop if npm did not return a global tools folder because we would not be able to confirm the Azurite installation.
if (-not $npmPrefix) {
    throw 'Could not determine the npm global installation directory.'
}

# Build the expected full filename of the Azurite command that StartLocalServices.ps1 will run later.
$azuriteCommand = Join-Path $npmPrefix 'azurite.cmd'

# Stop if npm claimed to install Azurite but its command file cannot actually be found.
if (-not (Test-Path $azuriteCommand)) {
    throw "Azurite was installed, but azurite.cmd was not found at '$azuriteCommand'."
}

# Add an empty line before the final summary so it is easy to spot in the PowerShell window.
Write-Host ""

# Confirm that all one-off repository setup work has completed successfully.
Write-Host "Local setup completed successfully."

# Tell the developer which script should be used from now on when they want to run the application locally.
Write-Host "Run .\StartLocalServices.ps1 before starting Edubase.Web.UI."
