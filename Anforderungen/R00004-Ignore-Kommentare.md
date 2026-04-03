---
id: R00004
titel: "Ignore-Kommentare"
typ: Feature
status: Dokumentiert
erstellt: 2026-04-03
---

# R00004: Ignore-Kommentare

## Beschreibung

Regeln koennen per Kommentar in der Datei deaktiviert werden: `// cleaner: ignore RuleName`

## Akzeptanzkriterien

- Kommentar `// cleaner: ignore <RuleId>` deaktiviert die Regel fuer diese Datei
- Mehrere Regeln koennen ignoriert werden
- CleanerCommentParser extrahiert die ignorierten Rule-IDs
- RuleFactory filtert ignorierte Regeln heraus

## Implementierung

- `CleanerCommentParser.GetIgnoredRuleIds(fileContent)` parst Kommentare
- `RuleFactory.GetRules()` entfernt ignorierte Regeln aus der Liste
