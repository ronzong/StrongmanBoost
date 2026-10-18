$ErrorActionPreference = 'Stop'
$fbRepo = Split-Path -Parent $PSScriptRoot
$fbProject = Join-Path $fbRepo 'StrongmanBoost'
$fbEditor = 'C:\Program Files\Unity\Hub\Editor\2022.3.62f3\Editor\Unity.exe'
$fbLog = Join-Path $fbRepo 'Logs\facebook-build.log'
New-Item -ItemType Directory -Force (Split-Path -Parent $fbLog) | Out-Null
$fbBuild = Start-Process -FilePath $fbEditor -ArgumentList '-batchmode','-nographics','-quit','-buildTarget','WebGL','-projectPath',('"' + $fbProject + '"'),'-executeMethod','FacebookBuild.Build','-logFile',('"' + $fbLog + '"') -WindowStyle Hidden -PassThru -Wait
if ($fbBuild.ExitCode -ne 0) { throw "Build failed ($($fbBuild.ExitCode)). See $fbLog" }
$fbConfig = Get-Content (Join-Path $PSScriptRoot 'facebook-config.json') -Raw | ConvertFrom-Json
Write-Output (Join-Path $fbRepo ("Builds\Facebook\StrongmanBoost-Facebook-" + $fbConfig.appId + '.zip'))
