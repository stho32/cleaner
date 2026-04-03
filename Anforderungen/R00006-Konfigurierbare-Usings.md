---
id: R00006
titel: "Konfigurierbare Usings"
typ: Feature
status: Dokumentiert
erstellt: 2026-04-03
---

# R00006: Konfigurierbare Usings

## Beschreibung

Erlaubte Namespaces koennen per Textdatei konfiguriert werden. Ohne Konfiguration gelten Default-Usings.

## Akzeptanzkriterien

- CLI-Option `--allowed-usings` fuer Pfad zur Textdatei
- Eine Namespace pro Zeile
- Ohne Datei: Default-Liste (System.*, Microsoft.*, etc.)
- Fehler beim Lesen: Fehlermeldung auf Console, Default-Liste wird verwendet

## Implementierung

- `CommandLineOptions.AllowedUsingsFilePath`
- `FileBasedQualityScanner.LoadAllowedUsingsOrUseDefault()`
- `AllowedUsingsRule.GetDefaultAllowedUsings()`
