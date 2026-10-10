param([switch]$DesktopSmoke)
$ErrorActionPreference='Stop'
if ($PSVersionTable.PSVersion.Major -lt 7) { throw 'PowerShell 7 or newer is required.' }
if (-not $IsWindows) { throw 'This process supervision script is Windows-specific; use the documented dotnet commands on other hosts.' }
$workspace=Split-Path $PSScriptRoot -Parent
$dotnet=(Get-Command dotnet -ErrorAction Stop).Source
$runId='verify-'+(Get-Date -Format 'yyyyMMdd-HHmmss')+'-'+[guid]::NewGuid().ToString('N').Substring(0,6)
$deadline=[DateTime]::UtcNow.AddSeconds(480)
$steps=@(
    @{Name='restore';Args=@('restore','HmiStudio.slnx','--disable-parallel');Seconds=150},
    @{Name='build';Args=@('build','HmiStudio.slnx','-c','Release','--no-restore','--disable-build-servers','-m:1','-p:UseSharedCompilation=false');Seconds=120},
    @{Name='contracts';Args=@('run','--project','tests/HmiStudio.Core.Tests','-c','Release','--no-build');Seconds=45},
    @{Name='format';Args=@('format','HmiStudio.slnx','--verify-no-changes','--no-restore');Seconds=90}
)
if($DesktopSmoke){
    $steps+=@{Name='desktop';Args=@('src/HmiStudio.Desktop/bin/Release/net10.0/HmiStudio.Desktop.dll','--smoke-test',(Join-Path $workspace ('artifacts/'+$runId+'/desktop')));Seconds=35}
}
foreach($step in $steps){
    $remaining=[int]($deadline-[DateTime]::UtcNow).TotalSeconds
    if($remaining -le 0){throw 'Verification overall deadline exceeded.'}
    & (Join-Path $PSScriptRoot 'Invoke-BoundedProcess.ps1') -Name ($runId+'-'+$step.Name) -Executable $dotnet -Arguments $step.Args -WorkingDirectory $workspace -TimeoutSeconds ([Math]::Min($remaining,$step.Seconds))
    if($LASTEXITCODE -ne 0){throw ('Verification failed: '+$step.Name+'; exit '+$LASTEXITCODE)}
}
Write-Output ('Verification passed: '+$runId)
