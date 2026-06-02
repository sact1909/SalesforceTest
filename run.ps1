$api = Start-Process -FilePath "dotnet" `
    -ArgumentList "run --project src/SalesforceTest.Api --launch-profile https" `
    -PassThru -NoNewWindow

$web = Start-Process -FilePath "dotnet" `
    -ArgumentList "run --project src/SalesforceTest.Web --launch-profile https" `
    -PassThru -NoNewWindow

Write-Host "API  -> https://localhost:7257"
Write-Host "Web  -> https://localhost:7286"
Write-Host ""
Write-Host "Press Ctrl+C to stop both..."

try {
    Wait-Process -Id $api.Id, $web.Id
} finally {
    if (!$api.HasExited)  { $api.Kill($true) }
    if (!$web.HasExited)  { $web.Kill($true) }
}
