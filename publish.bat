REM Í¸?Ù¤ÖµÓðÍóð¤÷»?ú¼£¬Üú??Ùþ?????
SET CUR_PATH=%CD%
SET OUTPUT=%CUR_PATH%\publish
REM publish -c Release -o ./publish
REM dotnet publish -c Release -o ./publish /p:PublishSingleFile=true /p:SelfContained=true
SET PARGMA=-p:AssemblyName="Game Launch" -p:Authors="Game Launch"
dotnet publish -c Release -r win-x64 -o ./publish -p:AssemblyName="Game Launch" /p:PublishSingleFile=true /p:SelfContained=true /p:DebugType=None /p:DebugSymbols=false --framework netcoreapp3.1
REM dotnet publish -c Release -r win-x64 -o ./publish -p:AssemblyName="¹üÀÎ¼ö¼±" -p:Authors="¹üÀÎ¼ö¼±" /p:PublishSingleFile=true /p:SelfContained=true /p:DebugType=None /p:DebugSymbols=false --framework netcoreapp3.1
cd %OUTPUT%
REM del *.pdb
REM del Launcher.deps.json