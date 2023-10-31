param (
## The URL to create an HTTP connection to
    [Parameter(Mandatory = $true)]
    [ValidateNotNullOrEmpty()]
    [string]$url,

## A piece of text which is required to be present on the requested page
    [Parameter(Mandatory = $false)]
    [ValidateNotNullOrEmpty()]
    [string]$needleString,

## If the URL requires basic auth, the username to use
    [Parameter(Mandatory = $false)]
    [ValidateNotNullOrEmpty()]
    [string]$basicAuthUsername,

## If the URL requires basic auth, the password to use
    [Parameter(Mandatory = $false)]
    [ValidateNotNullOrEmpty()]
    [string]$basicAuthPassword,

## The number of seconds to wait between requests
    [Parameter(Mandatory = $false)]
    [ValidateNotNullOrEmpty()]
    [ValidateRange(1, 600)]
    [int]$durationBetweenRequestsSeconds = 5,

## The maximum number of minutes to wait before aborting
    [Parameter(Mandatory = $false)]
    [ValidateNotNullOrEmpty()]
    [ValidateRange(1, 60)]
    [int]$maximumDurationMinutes = 20
)


if ($url -eq '')
{
    Write-Host 'Smoke test URL not provided. Skipping smoke test.'
    exit(0)
}

## If either the username or the password is supplied, require both
if (($basicAuthUsername -eq '' -and $basicAuthPassword -ne '') -or ($basicAuthUsername -ne '' -and $basicAuthPassword -eq ''))
{
    Write-Warning 'Basic auth username and password must both be supplied, or neither.'
    exit(1)
}

$maximumDurationSeconds = $maximumDurationMinutes * 60

$attemptNumber = 0
$response = ''

$sequentialSuccessCount = 0
$minimumSequentialSuccessCountRequired = 5

$overallTimer = [Diagnostics.Stopwatch]::StartNew()

Write-Host "CONFIGURATION:"
Write-Host "- URL: $url"
Write-Host "- Needle string: $needleString"
#Write-Host "- Username: $basicAuthUsername"
#Write-Host "- Password: $basicAuthPassword"
Write-Host "- Duration between requests: $durationBetweenRequestsSeconds seconds"
Write-Host "- Maximum duration: $maximumDurationMinutes minutes"
Write-Host "- Minimum sequential success count required: $minimumSequentialSuccessCountRequired"

do
{
    $attemptNumber++
    Write-Host ''
    Write-Host "Attempt $attemptNumber @ $( Get-Date )"

    if ($overallTimer.Elapsed.TotalSeconds -gt $maximumDurationSeconds)
    {
        Write-Warning "- Timeout reached, aborting and not continuing to retry"
        Write-Warning "- Elapsed time: $( $overallTimer.Elapsed.TotalSeconds ) seconds"
        Write-Warning "- Timeout threshold: $maximumDurationSeconds seconds"

        throw "Timeout reached"
    }

    Write-Host "- Fetching $url"

    $requestTimer = [Diagnostics.Stopwatch]::StartNew()
    $response = try
    {
        if ($username -eq '')
        {
            Write-Host 'Username not provided. Doing smoke test without basic auth.'
            Invoke-WebRequest -Uri $url
        }
        else
        {
            Write-Host 'Username provided. Doing smoke test with basic auth.'
            $base64AuthInfo = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes(("{0}:{1}" -f $basicAuthUsername, $basicAuthPassword)))
            Invoke-WebRequest -Uri $url -Headers @{ Authorization = ("Basic {0}" -f $base64AuthInfo) }
        }
    }
    catch [System.Net.WebException]
    {
        Write-Host "A web exception was caught: $( $_.Exception.Message )"
        $_.Exception.Response
    }
    catch [System.Exception]
    {
        Write-Host "A generic exception was caught: $( $_.Exception.Message )"
        $_.Exception.Response
    }
    finally
    {
        $requestTimer.Stop()
        Write-Host "- Request duration: $( $requestTimer.Elapsed.TotalSeconds ) seconds"
    }

    Write-Host "- Status code: $( [Int]$response.StatusCode ) - $( $response.StatusCode ) - $( $response.StatusDescription )"
    Write-Host "- Response headers:"
    $response.Headers | Format-Table -AutoSize
    Write-Host "- Response body:"
    $response.Content


    if ($response.StatusCode -ne 200)
    {
        Write-Host "- Page request unsuccessful for $url"

        $sequentialSuccessCount = 0

        Write-Host "- Pausing for $durationBetweenRequestsSeconds seconds before next attempt"
        Start-Sleep -Milliseconds ($durationBetweenRequestsSeconds * 1000)

        continue
    }

    ## If a needle string is provided, check that it is present within the response body
    if ($needleString -ne '')
    {
        if ($response.Content.Contains($needleString) -ne 'True')
        {
            Write-Warning "- HTTP response code suggests a successful request, but response body does not contain value '$needleString' - cannot verify success."

            Write-Host "- Pausing for $durationBetweenRequestsSeconds seconds before next attempt"
            Start-Sleep -Milliseconds ($durationBetweenRequestsSeconds * 1000)

            continue
        }
        else
        {
            Write-Host "- Value '$needleString' found within response body."
        }
    }

    $sequentialSuccessCount++
    Write-Host "- Success count: $sequentialSuccessCount / $minimumSequentialSuccessCountRequired"

    if ($sequentialSuccessCount -lt $minimumSequentialSuccessCountRequired)
    {
        Write-Host "- Minimum of $minimumSequentialSuccessCountRequired sequential successes required before accepting site is running satisfactorily"
        Write-Host "- Pausing for $durationBetweenRequestsSeconds seconds before next attempt"
        Start-Sleep -Milliseconds ($durationBetweenRequestsSeconds * 1000)

        continue
    }

    Write-Host "- Successfully fetched $url"

    break
} while ($true)

$overallTimer.Stop()
Write-Host "Finished in $( $overallTimer.Elapsed.TotalSeconds ) seconds"
Write-Host "Finished at $( Get-Date )"