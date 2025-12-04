#!/bin/sh
set -e

CERTIFICATE_HOME='/certs'

if [ -d "$CERTIFICATE_HOME" ]; then
   echo "Purging contents of $CERTIFICATE_HOME"
   find "$CERTIFICATE_HOME" -mindepth 1 -delete
fi

echo "Creating certificate output directory: $CERTIFICATE_HOME"
mkdir -p "$CERTIFICATE_HOME"

# Generate a local CA (certificate authority)
echo '[req]
distinguished_name = req_distinguished_name
x509_extensions = v3_ca
[req_distinguished_name]
[v3_ca]
basicConstraints = critical, CA:true
keyUsage = keyCertSign, cRLSign' > /tmp/ca.conf

openssl genrsa -out /tmp/ca.key 2048

openssl req -x509 -new -nodes -key /tmp/ca.key -sha256 -days 3650 -out "$CERTIFICATE_HOME/ca.crt" -subj "/CN=MyLocalCA" -extensions v3_ca -config /tmp/ca.conf

openssl genrsa -out /tmp/wiremock-cert.key 2048

# Generate CSR (Certificate signing request) to sign WireMock self-generated certificate with local CA
openssl req -new -key /tmp/wiremock-cert.key -out /tmp/wiremock-cert.csr -subj '/CN=localhost'

echo 'authorityKeyIdentifier=keyid,issuer
basicConstraints=CA:FALSE
keyUsage = digitalSignature, keyEncipherment
extendedKeyUsage = serverAuth
subjectAltName = @alt_names
1.3.6.1.4.1.311.84.1.1 = DER:01

[alt_names]
DNS.1 = localhost
DNS.2 = 127.0.0.1' > /tmp/wiremock-cert.ext

# Generate the signed certificate (PEM .crt) for WireMock
openssl x509 -req -in /tmp/wiremock-cert.csr -CA "$CERTIFICATE_HOME/ca.crt" -CAkey /tmp/ca.key -CAcreateserial -out "$CERTIFICATE_HOME/wiremock-cert.crt" -days 365 -sha256 -extfile /tmp/wiremock-cert.ext

# Convert to a (PKCS12 .pfx) 
openssl pkcs12 -export -out "$CERTIFICATE_HOME/wiremock-cert.pfx" -inkey /tmp/wiremock-cert.key -in "$CERTIFICATE_HOME/wiremock-cert.crt" -certfile "$CERTIFICATE_HOME/ca.crt" -passout pass:yourpassword

# Convert .pfx to a (.jks) which WireMock:java requires
keytool -importkeystore \
   -srckeystore "$CERTIFICATE_HOME/wiremock-cert.pfx" \
   -srcstoretype pkcs12 \
   -srcstorepass yourpassword \
   -destkeystore "$CERTIFICATE_HOME/wiremock-keystore.jks" \
   -deststoretype JKS \
   -deststorepass $WIREMOCK_KEYSTORE_STOREPASSWORD \
   -destkeypass $WIREMOCK_KEYSTORE_KEYPASSWORD \
   -noprompt