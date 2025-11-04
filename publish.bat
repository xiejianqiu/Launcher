SET CUR_PATH=%CD%
SET OUTPUT=%CUR_PATH%\publish
dotnet publish -c Release -o ./publish
cd %OUTPUT%
del *.pdb
del Launcher.deps.json