---
id: R00002
titel: "CLI-basierter C#-Code-Scanner"
typ: Feature
status: Dokumentiert
erstellt: 2026-04-03
---

# R00002: CLI-basierter C#-Code-Scanner

## Beschreibung

Das Tool scannt rekursiv ein angegebenes Verzeichnis nach `.cs`-Dateien und prueft diese gegen definierte Qualitaetsregeln. Ergebnisse werden auf der Konsole ausgegeben.

## Akzeptanzkriterien

- Verzeichnispfad wird per CLI-Argument angegeben
- Alle `.cs`-Dateien im Verzeichnis und Unterverzeichnissen werden gefunden
- Bestimmte Verzeichnisse werden ignoriert (bin, obj, .git, etc.)
- Ergebnisse werden farblich hervorgehoben auf der Konsole ausgegeben
- Exit-Code signalisiert ob Probleme gefunden wurden

## Implementierung

- `Program.cs` als Einstiegspunkt
- `CompositeQualityScanner` orchestriert File- und Folder-basierte Scans
- `RecursiveDirectoryWalker` durchlaeuft das Verzeichnis
- `ValidationMessagePrinter` formatiert die Ausgabe
