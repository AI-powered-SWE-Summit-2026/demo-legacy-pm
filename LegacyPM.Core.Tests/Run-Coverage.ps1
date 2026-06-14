param(
    [int]$MinCoverage = 25,
    [int]$MaxCoverage = 30
)

$projectPath = Join-Path $PSScriptRoot "LegacyPM.Core.Tests.csproj"
$coverageDir = Join-Path $PSScriptRoot "TestResults\coverage"
$coveragePrefix = Join-Path $coverageDir "coverage"

if (Test-Path $coverageDir) {
    Remove-Item -Path $coverageDir -Recurse -Force
}

dotnet test $projectPath `
    /p:CollectCoverage=true `
    /p:CoverletOutput="$coveragePrefix" `
    /p:CoverletOutputFormat="cobertura" `
    /p:Include="[LegacyPM.Core]LegacyPM.Core.Services.ProjectService%2c[LegacyPM.Core]LegacyPM.Core.Models.Milestone%2c[LegacyPM.Core]LegacyPM.Core.Models.NotificationLog%2c[LegacyPM.Core]LegacyPM.Core.Models.ProjectMember%2c[LegacyPM.Core]LegacyPM.Core.Models.Report" `
    /p:Threshold=$MinCoverage `
    /p:ThresholdType="line" `
    /p:ThresholdStat="total"

if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}

$coverageFile = "$coveragePrefix.cobertura.xml"
[xml]$coverageXml = Get-Content -Path $coverageFile
$lineCoverage = [math]::Round(([double]$coverageXml.coverage.'line-rate' * 100), 2)

if ($lineCoverage -gt $MaxCoverage) {
    Write-Error "Line coverage $lineCoverage% exceeds cap of $MaxCoverage%."
    exit 1
}

Write-Host "Line coverage is $lineCoverage% (target range: $MinCoverage%-$MaxCoverage%)."
