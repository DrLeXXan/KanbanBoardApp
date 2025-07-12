param(
    [Parameter(Mandatory = $false)]
    [string]$MarkdownPath = "TESTPROTOKOLL.md"
)

# Automatically find the newest .trx file in the current directory or subdirectories
$trxFiles = Get-ChildItem -Path . -Filter *.trx -Recurse | Sort-Object LastWriteTime -Descending
if (-not $trxFiles) {
    Write-Error "No .trx file found in the current directory or subdirectories."
    exit 1
}
$TrxPath = $trxFiles[0].FullName

# Parse TRX XML
[xml]$trx = Get-Content $TrxPath

# Gather results
$results = @($trx.TestRun.Results.UnitTestResult)

# Build summary
$total = $results.Count
$passed = @($results | Where-Object { $_.outcome -eq 'Passed' }).Count
$failed = @($results | Where-Object { $_.outcome -eq 'Failed' }).Count

$summary = @"
## 🧪 Testprotokoll
Erstellt am: $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")

### Test Run Summary
- **Total:** $total
- **Passed:** $passed
- **Failed:** $failed

"@

# Build failed tests section
$failedSection = ""
if ($failed -gt 0) {
    $failedSection = "### Failed Tests`n"
    foreach ($fail in $results | Where-Object { $_.outcome -eq 'Failed' }) {
        $failedSection += "* $($fail.testName)`n"
        if ($fail.ErrorMessage) {
            $failedSection += "    - **Error Message:** $($fail.ErrorMessage)`n"
        }
        $failedSection += "    - **Resolution:** _(Add resolution steps here as you address the failure)_`n"
    }
    $failedSection += "`n"
}

# Markdown table header
$tableHeader = @"
| Testfall-ID | Kurzbeschreibung | Vorbedingungen | Eingabedaten / Aktionen | Erwartetes Ergebnis | Tatsächliches Ergebnis |
|-------------|------------------|----------------|------------------------|--------------------|-----------------------|
"@

# Extract test results for table
$rows = @()
foreach ($unitTestResult in $results) {
    $testId = $unitTestResult.testName
    $outcome = $unitTestResult.outcome

    # Try to get a more descriptive name from the test definition (if available)
    $testDef = $trx.TestRun.TestDefinitions.UnitTest | Where-Object { $_.name -eq $testId }
    if ($testDef) {
        $description = $testDef.Description
    } else {
        $description = ""
    }

    $row = "| $testId | $description | Testumgebung vorbereitet | Automatisierter Testlauf | Test besteht ohne Fehler | $outcome |"
    $rows += $row
}

# Write everything to the Markdown file
$summary | Set-Content $MarkdownPath -Encoding UTF8
if ($failedSection) { $failedSection | Add-Content $MarkdownPath -Encoding UTF8 }
$tableHeader | Add-Content $MarkdownPath -Encoding UTF8
$rows | Add-Content $MarkdownPath -Encoding UTF8