<#
.SYNOPSIS
  Runs the WinUI test projects (tests/WinUI) built in Release for the architecture of the machine, one at a time.
.DESCRIPTION
  The tests run on the interactive desktop and inject real keyboard and mouse input: do not use the mouse or keyboard
  while they run. Build first: dotnet build WinUIBehaviors.slnx -c Release.
.PARAMETER Results
  The folder that receives the output of each test project (default: artifacts/winui-tests).
.PARAMETER Filter
  Runs only the test projects whose folder name contains this text.
.PARAMETER TimeoutMinutes
  The time after which a test project that has not finished is stopped (and reported as failed).
.PARAMETER Summary
  A file that receives the summary lines (in addition to the output) and, at the end, "RUN COMPLETE".
#>
param(
    [string] $Results = (Join-Path $PSScriptRoot '..\..\artifacts\winui-tests'),
    [string] $Filter = '',
    [int] $TimeoutMinutes = 15,
    [string] $Summary = ''
)

function Write-Summary([string] $line) {
    Write-Output $line
    if ($Summary) { Add-Content -Path $Summary -Value $line }
}

if ($Summary) { Set-Content -Path $Summary -Value '' }

$ErrorActionPreference = 'Stop'
$root = Resolve-Path (Join-Path $PSScriptRoot '..\..')
$rid = if ($env:PROCESSOR_ARCHITECTURE -eq 'ARM64') { 'win-arm64' } else { 'win-x64' }
New-Item -ItemType Directory -Force -Path $Results | Out-Null
$failed = $false

foreach ($project in Get-ChildItem (Join-Path $root 'tests\WinUI') -Directory | Where-Object { $_.Name -ne 'Shared' -and $_.Name -like "*$Filter*" }) {
    $output = Join-Path $project.FullName "bin\Release\net10.0-windows10.0.19041.0\$rid"
    $exe = Get-ChildItem $output -Filter 'Xaml.Behaviors.WinUI.*.exe' -ErrorAction SilentlyContinue | Select-Object -First 1
    if (-not $exe) {
        Write-Summary "$($project.Name): not built"
        $failed = $true
        continue
    }

    $log = Join-Path $Results "$($project.Name).txt"
    Push-Location $output
    try {
        # cmd redirects the output as is (PowerShell would turn the lines the runner writes to stderr into errors).
        $process = Start-Process -FilePath 'cmd.exe' -NoNewWindow -PassThru `
            -ArgumentList "/c `"`"$($exe.FullName)`" -noLogo -noColor -parallel none > `"$log`" 2>&1`""
        if (-not $process.WaitForExit($TimeoutMinutes * 60 * 1000)) {
            Get-Process -Name $exe.BaseName -ErrorAction SilentlyContinue | Stop-Process -Force
            Add-Content -Path $log -Value "TIMEOUT: stopped after $TimeoutMinutes minutes."
            $failed = $true
        }
        elseif ($process.ExitCode -ne 0) { $failed = $true }
    }
    finally {
        Pop-Location
    }

    $summary = Select-String -Path $log -Pattern 'Total:' | Select-Object -Last 1
    Write-Summary ("{0}: {1}" -f $project.Name, $(if ($summary) { $summary.Line.Trim() } else { 'no summary' }))
}

if ($Summary) { Add-Content -Path $Summary -Value 'RUN COMPLETE' }
if ($failed) { exit 1 }
