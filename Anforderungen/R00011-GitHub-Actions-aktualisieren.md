---
id: R00011
titel: "GitHub Actions Workflow aktualisieren"
typ: Verbesserung
status: Erledigt
prioritaet: Hoch
aufwand: Klein
erstellt: 2026-04-03
quelle: R00001
---

# R00011: GitHub Actions Workflow aktualisieren

## Beschreibung

Veraltete Action-Versionen (v1/v2/v3) und deprecated `::set-output` Syntax aktualisieren.

## Aenderungen

1. `actions/checkout@v2` -> `@v4`
2. `actions/setup-dotnet@v1` -> `@v4`
3. `actions/upload-artifact@v3` -> `@v4`
4. `actions/download-artifact@v3` -> `@v4`
5. `softprops/action-gh-release@v1` -> `@v2`
6. `::set-output` -> `$GITHUB_OUTPUT`
