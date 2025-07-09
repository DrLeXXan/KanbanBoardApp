[xml]$trx = Get-Content ".\TestResults\testResults.trx"
$results = $trx.TestRun.Results.UnitTestResult

"## Test Run Summary" | Out-File TESTRUN.md
"Total: $($results.Count)" | Out-File TESTRUN.md -Append
"Passed: $(@($results | Where-Object { $_.outcome -eq 'Passed' }).Count)" | Out-File TESTRUN.md -Append
"Failed: $(@($results | Where-Object { $_.outcome -eq 'Failed' }).Count)" | Out-File TESTRUN.md -Append
"" | Out-File TESTRUN.md -Append

"### Passed Tests" | Out-File TESTRUN.md -Append
$results | Where-Object { $_.outcome -eq 'Passed' } | ForEach-Object {
    "* $($_.testName)" | Out-File TESTRUN.md -Append
}

"### Failed Tests" | Out-File TESTRUN.md -Append
$results | Where-Object { $_.outcome -eq 'Failed' } | ForEach-Object {
    "* $($_.testName)" | Out-File TESTRUN.md -Append
    $errorMessage = $_.ErrorMessage

    if ($errorMessage) {
        "    - **Error Message:** $errorMessage" | Out-File TESTRUN.md -Append
    }
    "    - **Resolution:** _(Add resolution steps here as you address the failure)_" | Out-File TESTRUN.md -Append
}