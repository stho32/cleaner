# Cleaner

C#-basiertes CLI-Tool zur statischen Code-Analyse. Prueft C#-Codebasen gegen definierte Qualitaetsregeln (IOSP, Law of Demeter, Repository-Pattern, Methodenlaenge, etc.).

## Build & Test

```bash
cd Source/cleaner
dotnet build
dotnet test
dotnet test --collect:"XPlat Code Coverage"   # mit Coverage
```

Lokales Deployment (Windows): `.\build-and-deploy-locally.ps1` → kopiert nach `C:\Tools\cleaner.exe`

## Architektur

3-Projekt-Solution:
- **cleaner** — CLI-Einstiegspunkt (`Program.cs`)
- **cleaner.Domain** — Kernlogik (Regeln, Scanner, Traversal)
- **cleaner.Domain.Tests** — Unit-Tests (NUnit)

Kern-Patterns:
- `IRule` Interface → alle Regeln implementieren `Validate(filePath, fileContent)`
- `RuleFactory` laedt Regeln per Reflection, filtert per Ignore-Kommentare
- `CompositeQualityScanner` orchestriert File- und Folder-basierte Scans
- `IDirectoryWalker` (Strategy) fuer rekursiven vs. Latest-Changed-Files Scan

## Konventionen

- `TreatWarningsAsErrors` in allen Projekten
- `Nullable enable` ueberall
- Regeln geben `ValidationMessage[]` zurueck (nie null)
- Jede Regel hat `Id`, `Name`, `ShortDescription`, `LongDescription`
- Ignore-Kommentar: `// cleaner: ignore <RuleId>`

## CLI-Verwendung

```bash
cleaner -d <verzeichnis>              # Verzeichnis scannen
cleaner -d <verzeichnis> -s 10        # Nur 10 zuletzt geaenderte Dateien
cleaner -d <verzeichnis> -a usings.txt # Eigene Allowed-Usings-Liste
cleaner --list-rules                   # Alle Regeln auflisten
cleaner --stop-on-first               # Bei erstem Fund stoppen
```
