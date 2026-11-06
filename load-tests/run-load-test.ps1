param (
    [string]$Scenario = "peak-ticket-rush.js",
    [string]$Network = ""
)

[Console]::OutputEncoding = [System.Text.UTF8Encoding]::new($false)
[Console]::InputEncoding  = [System.Text.UTF8Encoding]::new($false)
$OutputEncoding           = [System.Text.UTF8Encoding]::new($false)

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path

# Tự động chọn network của Docker Compose (mặc định là cinema_perfo-net theo docker-compose.yml)
$networkName = $Network
if (-not $networkName) {
    try {
        $detectedNet = (docker inspect perfo-api --format '{{range $k, $v := .NetworkSettings.Networks}}{{$k}}{{end}}' 2>$null)
        if ($detectedNet) {
            $networkName = $detectedNet.Trim()
        }
    } catch {}
}
if (-not $networkName) {
    $networks = docker network ls --format "{{.Name}}"
    if ($networks | Select-String "^cinema_perfo-net$") {
        $networkName = "cinema_perfo-net"
    } elseif ($networks | Select-String "^cinema_cinema-net$") {
        $networkName = "cinema_cinema-net"
    } elseif ($networks | Select-String "perfo-net") {
        $networkName = ($networks | Select-String "perfo-net" | Select-Object -First 1).Line.Trim()
    } else {
        $networkName = "cinema_perfo-net"
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
    -e API_BASE_URL="http://perfo-api:8080/api/v1" `
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
