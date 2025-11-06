SET CUR_PATH=%CD%
SET OUTPUT=%CUR_PATH%\publish
REM dotnet publish -c Release -o ./publish
dotnet publish -c Release -r win-x64 -o ./publish  /p:PublishSingleFile=true /p:DebugType=None /p:DebugSymbols=false --framework netcoreapp3.1
cd %OUTPUT%
del *.pdb
del Launcher.deps.json