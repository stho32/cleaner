Push-Location .\Source\cleaner\cleaner

$publishDir = ".\publish"  # Verzeichnis für den Publish-Output
# Aktualisiert den dotnet publish Befehl, um den Anwendungshost einzuschließen
dotnet publish -c Release -r win-x64 -o $publishDir --self-contained true -p:PublishSingleFile=true -p:UseAppHost=true
Copy-Item "$publishDir\cleaner.exe" C:\Tools\cleaner.exe -Force

Pop-Location
