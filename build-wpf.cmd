@echo off
setlocal
set "MSBUILD=C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"
if not exist "%MSBUILD%" (
  echo MSBuild was not found.
  exit /b 1
)
"%MSBUILD%" "LanControlWpf\LanControlWpf.csproj" /restore /t:Rebuild /p:Configuration=Release /v:minimal
if errorlevel 1 exit /b %errorlevel%
if not exist "release" mkdir "release"
copy /y "LanControlWpf\bin\Release\LanControlTool.exe" "release\LanControlTool.exe" >nul
echo Build complete: release\LanControlTool.exe
