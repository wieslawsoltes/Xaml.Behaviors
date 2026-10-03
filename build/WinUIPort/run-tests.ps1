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
.PARAMETER Class
  Runs only the tests of this class (full name; xUnit v3 -class).
.PARAMETER Method
  Runs only this test method (full name, wildcards allowed; xUnit v3 -method).
.PARAMETER Summary
  A file that receives the summary lines (in addition to the output) and, at the end, "RUN COMPLETE".
#>
param(
    [string] $Results = (Join-Path $PSScriptRoot '..\..\artifacts\winui-tests'),
    [string] $Filter = '',
    [int] $TimeoutMinutes = 15,
    [string] $Class = '',
    [string] $Method = '',
    [string] $Summary = ''
)

$summaryLines = [System.Collections.Generic.List[string]]::new()
function Write-Summary([string] $line) {
    Write-Output $line
    $summaryLines.Add($line)
}

# The summary file is written once, at the end (it may be watched while the tests run).
if ($Summary) { Remove-Item -Path $Summary -ErrorAction SilentlyContinue }

$ErrorActionPreference = 'Stop'
$root = Resolve-Path (Join-Path $PSScriptRoot '..\..')
$rid = if ($env:PROCESSOR_ARCHITECTURE -eq 'ARM64') { 'win-arm64' } else { 'win-x64' }
New-Item -ItemType Directory -Force -Path $Results | Out-Null
# The tests run in their output folder: a relative results path must not follow them.
$Results = (Resolve-Path $Results).Path
$failed = $false
$arguments = '-noLogo -noColor -parallel none'
if ($Class) { $arguments += " -class `"$Class`"" }
if ($Method) { $arguments += " -method `"$Method`"" }

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
        $startInfo = [System.Diagnostics.ProcessStartInfo]::new($exe.FullName, $arguments)
        $startInfo.WorkingDirectory = $output
        $startInfo.UseShellExecute = $false
        $startInfo.RedirectStandardOutput = $true
        $startInfo.RedirectStandardError = $true
        $process = [System.Diagnostics.Process]::Start($startInfo)
        $stdout = $process.StandardOutput.ReadToEndAsync()
        $stderr = $process.StandardError.ReadToEndAsync()
        $timedOut = -not $process.WaitForExit($TimeoutMinutes * 60 * 1000)
        if ($timedOut) {
            $process.Kill($true)
            $process.WaitForExit()
        }

        # A process left behind by the test run could keep the output pipes open: do not wait for them forever.
        $null = $stdout.Wait(60000)
        $null = $stderr.Wait(10000)
        $text = $(if ($stdout.IsCompleted) { $stdout.Result } else { '(output not available)' }) + $(if ($stderr.IsCompleted) { $stderr.Result } else { '' })
        if ($timedOut) { $text += "`nTIMEOUT: stopped after $TimeoutMinutes minutes." }
        Set-Content -Path $log -Value $text -Encoding utf8
        if ($timedOut -or $process.ExitCode -ne 0) { $failed = $true }
    }
    finally {
        Pop-Location
    }

    # (PowerShell variables ignore case: $totals, not $summary, which is the -Summary parameter.)
    $totals = Select-String -Path $log -Pattern 'Total:|TIMEOUT' | Select-Object -Last 1
    Write-Summary ("{0}: {1}" -f $project.Name, $(if ($totals) { $totals.Line.Trim() } else { 'no summary' }))
}

if ($Summary) {
    $summaryLines.Add('RUN COMPLETE')
    Set-Content -Path $Summary -Value $summaryLines
}
if ($failed) { exit 1 }
