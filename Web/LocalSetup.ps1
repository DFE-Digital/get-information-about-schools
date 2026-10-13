# Prepare SAML metadata using the shared development signing certificate.
# The application configuration is shared separately; this script does not edit it.
$ErrorActionPreference = 'Stop'
$signinDirectory = Join-Path $PSScriptRoot 'Edubase.AcceptanceTests\.local-signin'
[void](New-Item -ItemType Directory -Path $signinDirectory -Force)
# Share this development-only PFX alongside the config file. No certificate-store installation.
$certificatePath = Join-Path (Split-Path $PSScriptRoot -Parent) 'devsecrets.gias.signing.pfx.alwaysignore'
$certificate = [System.Security.Cryptography.X509Certificates.X509Certificate2]::new(
    $certificatePath, '', [System.Security.Cryptography.X509Certificates.X509KeyStorageFlags]::EphemeralKeySet)
try {
    $publicCertificate = [Convert]::ToBase64String($certificate.RawData)
    $baseUri = ([uri]($signinDirectory + [IO.Path]::DirectorySeparatorChar)).AbsoluteUri.TrimEnd('/')
    $issuer = [System.Security.SecurityElement]::Escape($baseUri + '/Metadata')
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
    [IO.File]::WriteAllText((Join-Path $signinDirectory 'Metadata'), $entity)
    [IO.File]::WriteAllText((Join-Path $signinDirectory 'Federation'), '<EntitiesDescriptor xmlns="urn:oasis:names:tc:SAML:2.0:metadata">' + $entity + '</EntitiesDescriptor>')
} finally { $certificate.Dispose() }

# Install and run local Table/Blob/Queue storage with persistent data.
# Leave this terminal open while running the application.
npm.cmd install -g azurite
if ($LASTEXITCODE -ne 0) { throw 'Azurite installation failed.' }
azurite.cmd --location (Join-Path $env:USERPROFILE 'azurite-data')
