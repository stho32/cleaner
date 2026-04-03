---
id: R00007
titel: "Latest-Changed-Files Modus"
typ: Feature
status: Dokumentiert
erstellt: 2026-04-03
---

# R00007: Latest-Changed-Files Modus

## Beschreibung

Statt alle Dateien zu scannen, kann ein Modus aktiviert werden, der nur die N zuletzt geaenderten Dateien prueft.

## Akzeptanzkriterien

- CLI-Option `-s` / `--latest-changed-files` mit Anzahl
- Dateien werden nach letztem Aenderungsdatum sortiert
- Nur die N neuesten Dateien werden gescannt

## Implementierung

- `CommandLineOptions.LatestChangedFiles` (int?)
- `LatestChangedFilesDirectoryWalker` sortiert nach LastWriteTime
- `Program.GetMatchingDirectoryWalker()` waehlt den passenden Walker
