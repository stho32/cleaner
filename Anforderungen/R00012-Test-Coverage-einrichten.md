---
id: R00012
titel: "Test-Coverage einrichten"
typ: Verbesserung
status: Erledigt
prioritaet: Hoch
aufwand: Klein
erstellt: 2026-04-03
quelle: R00001
---

# R00012: Test-Coverage einrichten

## Beschreibung

Coverage-Messung einrichten mit coverlet. Coverage-Report bei jedem Build.

## Akzeptanzkriterien

- `coverlet.collector` im Test-Projekt
- `dotnet test` erzeugt Coverage-Report
- Coverage wird in CI gemessen
