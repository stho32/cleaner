---
id: R00018
titel: "Roslyn-Analyzer NuGet-Paket"
typ: Verbesserung
status: Offen
prioritaet: Mittel
aufwand: Gross
erstellt: 2026-04-03
quelle: R00001
---

# R00018: Roslyn-Analyzer NuGet-Paket

## Beschreibung

Die einzigartigen architekturellen Regeln als Roslyn DiagnosticAnalyzer NuGet-Paket portieren fuer native IDE-Integration.

## Akzeptanzkriterien

- Neues Projekt `cleaner.Analyzers` als Roslyn DiagnosticAnalyzer
- Mindestens die einzigartigen Regeln portiert (SQL-Placement, Repository-Inheritance, IOSP, Law-of-Demeter)
- NuGet-Paket-Konfiguration
- Funktioniert in Visual Studio und Rider

## Hinweis

Dies ist ein strategisches Grossprojekt. Kann in mehreren Iterationen umgesetzt werden.
