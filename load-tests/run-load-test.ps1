param (
    [string]$Scenario = "peak-ticket-rush.js",
    [string]$Network = ""
)

[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path

# Tự động chọn network của Docker Compose (mặc định là cinema_cinema-net theo docker-compose.yml)
$networkName = $Network
if (-not $networkName) {
    if (docker network ls --format "{{.Name}}" | Select-String "^cinema_cinema-net$") {
        $networkName = "cinema_cinema-net"
    } else {
        $networkName = "cinema_cinema-network"
    }
}

# Đảm bảo thư mục lưu kết quả kiểm thử tồn tại
$resultsDir = Join-Path $scriptDir "results"
if (-not (Test-Path $resultsDir)) {
    New-Item -ItemType Directory -Force -Path $resultsDir | Out-Null
}

$scenarioBase = [System.IO.Path]::GetFileNameWithoutExtension($Scenario)
$jsonSummaryContainerPath = "/scripts/results/${scenarioBase}-summary.json"

Write-Host "===> Running Load Test Scenario: $Scenario using grafana/k6 on network $networkName <===" -ForegroundColor Cyan

docker run --rm `
    --network $networkName `
    -v "${scriptDir}:/scripts" `
    -e API_BASE_URL="http://cinema-api:8080/api/v1" `
    -e RESULTS_DIR="/scripts/results" `
    grafana/k6 run --summary-export $jsonSummaryContainerPath "/scripts/scenarios/$Scenario"

$exitCode = $LASTEXITCODE

if ($exitCode -eq 0) {
    Write-Host "===> Load Test Passed All Thresholds Successfully! <===" -ForegroundColor Green
} else {
    Write-Host "===> Load Test Exceeded Thresholds (Exit Code: $exitCode) <===" -ForegroundColor Red
}

$htmlReportLocal = Join-Path $resultsDir "${scenarioBase}-report.html"
$jsonReportLocal = Join-Path $resultsDir "${scenarioBase}-summary.json"

Write-Host ""
Write-Host "===> Kết quả kiểm thử đã được export: <===" -ForegroundColor Yellow
if (Test-Path $htmlReportLocal) {
    Write-Host "  [HTML Report]: $htmlReportLocal" -ForegroundColor Cyan
}
if (Test-Path $jsonReportLocal) {
    Write-Host "  [JSON Summary]: $jsonReportLocal" -ForegroundColor Cyan
}
Write-Host ""

exit $exitCode
