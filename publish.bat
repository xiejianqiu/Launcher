SET CUR_PATH=%CD%
SET OUTPUT=%CUR_PATH%\publish
REM publish -c Release -o ./publish
REM dotnet publish -c Release -o ./publish /p:PublishSingleFile=true /p:SelfContained=true
REM 独立部署
dotnet publish -c Release -r win-x64 -o ./publish  /p:PublishSingleFile=true /p:SelfContained=true /p:DebugType=None /p:DebugSymbols=false --framework netcoreapp3.1
cd %OUTPUT%
REM del *.pdb
REM del Launcher.deps.json