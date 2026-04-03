---
id: R00013
titel: "Duplizierte ShouldIgnoreThisFolder-Logik extrahieren"
typ: Verbesserung
status: Offen
prioritaet: Mittel
aufwand: Klein
erstellt: 2026-04-03
quelle: R00001
---

# R00013: Duplizierte ShouldIgnoreThisFolder-Logik extrahieren

## Beschreibung

Identische `ShouldIgnoreThisFolder`-Methode in `LatestChangedFilesDirectoryWalker` und `RecursiveDirectoryWalker` in gemeinsame Klasse extrahieren.

## Akzeptanzkriterien

- Gemeinsame Hilfsklasse `DirectoryFilter` mit der Ignore-Logik
- Beide Walker nutzen die gemeinsame Klasse
- Alle bestehenden Tests bestehen weiterhin
