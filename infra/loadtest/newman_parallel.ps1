# infra/loadtest/newman_parallel.ps1
# PowerShell script for parallel load testing using Newman (compatible with PowerShell 5.1+)

# Number of parallel jobs and iterations per job
$parallelJobs = 10
$iterationsPerJob = 100

# Path to Postman collection file
$collectionPath = Join-Path $PSScriptRoot "..\postman_collection.json"

# Array to keep track of background jobs
$jobs = @()

for ($i = 1; $i -le $parallelJobs; $i++) {
    # Each job passes the collection path, iteration count, and index
    $jobs += Start-Job -Name "LoadTest$i" -ArgumentList $collectionPath, $iterationsPerJob, $i -ScriptBlock {
        param($colPath, $iters, $idx)
        Write-Host ("Starting instance {0}: {1} iterations..." -f $idx, $iters)
        # Run Newman without environment file
        newman run $colPath `
            --iteration-count $iters `
            --reporters "cli,json" `
            --reporter-json-export "report-$idx.json"
        Write-Host ("Instance {0} completed" -f $idx)
    }
}

# Wait for all jobs to finish
$jobs | Wait-Job

# Retrieve and display job outputs
foreach ($job in $jobs) {
    Receive-Job $job | Out-Host
}

Write-Host "All load-test jobs completed."
