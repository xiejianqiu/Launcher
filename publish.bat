SET CUR_PATH=%~dp0
SET OUTPUT=%CUR_PATH%\publish
RD /S /Q %OUTPUT%
REM publish -c Release -o ./publish
REM dotnet publish -c Release -o ./publish /p:PublishSingleFile=true /p:SelfContained=true
SET PARGMA=-p:AssemblyName="Game Launch" -p:Authors="Game Launch"
dotnet publish -c Release -r win-x64 -o ./publish -p:AssemblyName="Game Launch" /p:PublishSingleFile=true /p:SelfContained=true /p:DebugType=None /p:DebugSymbols=false --framework netcoreapp3.1
REM dotnet publish -c Release -r win-x64 -o ./publish -p:AssemblyName="범인수선" -p:Authors="범인수선" /p:PublishSingleFile=true /p:SelfContained=true /p:DebugType=None /p:DebugSymbols=false --framework netcoreapp3.1