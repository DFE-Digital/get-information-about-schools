# Create the files that tell the app how to trust the local test sign-in service.
# SAML is the format used to share these sign-in details.
# The application configuration is shared separately; this script does not edit it.

# Stop if a PowerShell step fails, so we do not carry on with an incomplete setup.
$ErrorActionPreference = 'Stop'

# Build the folder path relative to this script, regardless of where it is run from.
$signinDirectory = Join-Path $PSScriptRoot 'Edubase.AcceptanceTests\.local-signin'

# Create the folder if needed. Keep an existing folder and hide the usual creation message.
[void](New-Item -ItemType Directory -Path $signinDirectory -Force)

# Find the shared development-only signing certificate in the repository's top-level folder.
# Share this PFX file alongside the config file; it is not installed into Windows.
$certificatePath = Join-Path (Split-Path $PSScriptRoot -Parent) 'devsecrets.gias.signing.pfx.alwaysignore'

# Open the certificate so we can read the details needed to check sign-in messages.
# Use an empty password and keep the private signing key in memory rather than saving it.
$certificate = [System.Security.Cryptography.X509Certificates.X509Certificate2]::
    new($certificatePath, '', [System.Security.Cryptography.X509Certificates.X509KeyStorageFlags]::EphemeralKeySet)

# Always release the certificate when this section finishes, even if something fails.
try {
    # Turn the public certificate into text that can be included in the sign-in details.
    # This shares the public part only, not the private key used to sign messages.
    $publicCertificate = [Convert]::ToBase64String($certificate.RawData)

    # Turn the folder path into a file address and remove any trailing slash.
    $baseUri = ([uri]($signinDirectory + [IO.Path]::DirectorySeparatorChar)).AbsoluteUri.TrimEnd('/')

    # Use the Metadata file address to identify the sign-in service, making it safe for XML.
    $issuer = [System.Security.SecurityElement]::Escape($baseUri + '/Metadata')

    # Build the XML document below, inserting the values we prepared above.
    # The lines inside it mean:
    # - EntityDescriptor: start the service details and give the service its identifier.
    # - IDPSSODescriptor: describe a service that signs users in using SAML.
    # - KeyDescriptor: start the certificate details used to check signed messages.
    # - KeyInfo: mark the enclosed information as certificate/key details.
    # - X509Data / X509Certificate: include the public certificate as text.
    # - /KeyInfo: finish the certificate/key details.
    # - /KeyDescriptor: finish the section about checking signed messages.
    # - SingleSignOnService: give the local sign-in address, reached by a browser redirect.
    # - /IDPSSODescriptor: finish the sign-in service description.
    # - /EntityDescriptor: finish the overall service details.
    # The @" and "@ markers keep this whole block together as one piece of text.
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

    # Save the service details as Metadata, replacing the previous file if it exists.
    [IO.File]::WriteAllText((Join-Path $signinDirectory 'Metadata'), $entity)

    # Save the same details inside a list of services, as expected by the Federation file.
    [IO.File]::WriteAllText((Join-Path $signinDirectory 'Federation'), '<EntitiesDescriptor xmlns="urn:oasis:names:tc:SAML:2.0:metadata">' + $entity + '</EntitiesDescriptor>')

# Release the certificate's resources whether the files were saved successfully or not.
} finally { $certificate.Dispose() }

# Install Azurite, which provides local versions of Azure's table, file (blob), and queue storage.
# The global install makes its command available outside this repository too.
npm.cmd install -g azurite

# npm is a separate program, so check its result explicitly and stop if installation failed.
if ($LASTEXITCODE -ne 0) { throw 'Azurite installation failed.' }

# Start Azurite and save its data in your user folder so it survives restarts.
# Leave this terminal open while running the application; closing it stops local storage.
azurite.cmd --location (Join-Path $env:USERPROFILE 'azurite-data')
