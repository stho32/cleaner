---
id: R00010
titel: "Roslyn SyntaxTree Caching"
typ: Verbesserung
status: Offen
prioritaet: Hoch
aufwand: Mittel
erstellt: 2026-04-03
quelle: R00001
---

# R00010: Roslyn SyntaxTree Caching

## Beschreibung

Jede der 18 Regeln parst die Datei separat mit `CSharpSyntaxTree.ParseText()`. Einmaliges Parsen und Weitergabe des SyntaxTree an alle Regeln.

## Akzeptanzkriterien

- `IRule.Validate` erhaelt den vorgeparsten SyntaxTree/Root
- Jede Datei wird nur einmal geparst
- Alle bestehenden Tests bestehen weiterhin
- Messbare Performance-Verbesserung bei grossen Codebasen
