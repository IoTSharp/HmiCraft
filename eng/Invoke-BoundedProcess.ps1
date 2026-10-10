param([Parameter(Mandatory)][string]$Name,[Parameter(Mandatory)][string]$Executable,[Parameter(Mandatory)][string[]]$Arguments,[int]$TimeoutSeconds=120,[string]$WorkingDirectory=(Split-Path $PSScriptRoot -Parent),[string]$LogDirectory=(Join-Path (Split-Path $PSScriptRoot -Parent) 'artifacts\process'))
$ErrorActionPreference='Stop'
if ($PSVersionTable.PSVersion.Major -lt 7) { throw 'PowerShell 7 required' }
[IO.Directory]::CreateDirectory($LogDirectory) | Out-Null
$stdoutPath=Join-Path $LogDirectory ($Name+'.stdout.log')
$stderrPath=Join-Path $LogDirectory ($Name+'.stderr.log')
[IO.File]::Open($stdoutPath,[IO.FileMode]::CreateNew).Dispose()
[IO.File]::Open($stderrPath,[IO.FileMode]::CreateNew).Dispose()
$psi=[Diagnostics.ProcessStartInfo]::new()
$psi.FileName=$Executable
$psi.WorkingDirectory=$WorkingDirectory
$psi.UseShellExecute=$false
$psi.CreateNoWindow=$true
$psi.WindowStyle=[Diagnostics.ProcessWindowStyle]::Hidden
$psi.RedirectStandardOutput=$true
$psi.RedirectStandardError=$true
$Arguments | ForEach-Object { $psi.ArgumentList.Add($_) }
$p=[Diagnostics.Process]::new()
$p.StartInfo=$psi
$owned=[Collections.Generic.Dictionary[int,object]]::new()
$clock=[Diagnostics.Stopwatch]::StartNew()
$timedOut=$false
try {
  if (-not $p.Start()) { throw 'Failed to start' }
  $started=$p.StartTime.ToUniversalTime()
  $out=$p.StandardOutput.ReadToEndAsync()
  $err=$p.StandardError.ReadToEndAsync()
  $identity=Get-CimInstance Win32_Process -Filter "ProcessId = $($p.Id)"
  if ($identity) { $owned.Add([int]$identity.ProcessId,$identity) }
  [pscustomobject]@{pid=$p.Id;startedUtc=$started;parent=$identity.ParentProcessId;commandLine=$identity.CommandLine;arguments=$Arguments;timeoutSeconds=$TimeoutSeconds} | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath (Join-Path $LogDirectory ($Name+'.process.json'))
  $maxChecks=$TimeoutSeconds+1
  for ($check=0; $check -lt $maxChecks -and $clock.Elapsed.TotalSeconds -lt $TimeoutSeconds; $check++) {
    if ($p.WaitForExit(1000)) { break }
    $ids=@($owned.Keys)
    if ($ids.Count -gt 0 -and $ids.Count -le 128) {
      $filter=($ids | ForEach-Object { "ParentProcessId = $_" }) -join ' OR '
      @(Get-CimInstance Win32_Process -Filter $filter | Select-Object -First 128) | ForEach-Object { if ($_.CreationDate.ToUniversalTime() -ge $started.AddSeconds(-1) -and -not $owned.ContainsKey([int]$_.ProcessId)) { $owned.Add([int]$_.ProcessId,$_) } }
      if ($owned.Count -gt 128) { throw 'Owned process count exceeded 128; stopping this task tree.' }
    }
    if ($check -gt 0 -and $check % 15 -eq 0) { Write-Output ('Running '+$Name+'; elapsed '+[int]$clock.Elapsed.TotalSeconds+'s; owned processes '+$owned.Count) }
    if (-not $p.HasExited) { Start-Sleep -Milliseconds 750 }
  }
  if (-not $p.HasExited) { $timedOut=$true; throw ('Deadline exceeded: '+$Name) }
  $out.Wait(5000) | Out-Null
  $err.Wait(5000) | Out-Null
  if ($out.IsCompleted) { [IO.File]::WriteAllText($stdoutPath,$out.Result) }
  if ($err.IsCompleted) { [IO.File]::WriteAllText($stderrPath,$err.Result) }
  [pscustomobject]@{Name=$Name;ExitCode=$p.ExitCode;ElapsedSeconds=[int]$clock.Elapsed.TotalSeconds;Stdout=$stdoutPath;Stderr=$stderrPath} | ConvertTo-Json -Compress
  if ($out.IsCompleted) { Write-Output $out.Result }
  if ($err.IsCompleted -and $err.Result) { Write-Output $err.Result }
  $resultCode=$p.ExitCode
} finally {
  $cleanupDeadline=[DateTime]::UtcNow.AddSeconds(30)
  if ($identity -and -not $p.HasExited) {
    $rootLive=Get-CimInstance Win32_Process -Filter "ProcessId = $($p.Id)"
    if ($rootLive -and $rootLive.CreationDate -eq $identity.CreationDate -and $rootLive.ParentProcessId -eq $identity.ParentProcessId -and $rootLive.CommandLine -eq $identity.CommandLine) { $p.Kill($true); $p.WaitForExit(5000) | Out-Null }
  }
  @($owned.Values) | Sort-Object CreationDate -Descending | Select-Object -First 256 | ForEach-Object {
    if ([DateTime]::UtcNow -gt $cleanupDeadline) { throw 'Cleanup deadline exceeded; retain identity receipts for follow-up.' }
    $live=Get-CimInstance Win32_Process -Filter "ProcessId = $($_.ProcessId)"
    if ($live -and $live.CreationDate -eq $_.CreationDate -and $live.ParentProcessId -eq $_.ParentProcessId -and $live.CommandLine -eq $_.CommandLine) {
      $ownedProcess=Get-Process -Id $live.ProcessId -ErrorAction SilentlyContinue
      if ($ownedProcess) { $ownedProcess.Kill($true); $ownedProcess.WaitForExit(5000) | Out-Null }
    }
  }
  if ($out -and $out.IsCompleted) { [IO.File]::WriteAllText($stdoutPath,$out.Result) }
  if ($err -and $err.IsCompleted) { [IO.File]::WriteAllText($stderrPath,$err.Result) }
  @($owned.Values) | Select-Object ProcessId,ParentProcessId,CreationDate,CommandLine | ConvertTo-Json -Depth 4 | Set-Content -LiteralPath (Join-Path $LogDirectory ($Name+'.owned-processes.json'))
  $p.Dispose()
}
if ($timedOut) { exit 124 }
exit $resultCode
