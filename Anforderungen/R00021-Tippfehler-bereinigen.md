---
id: R00021
titel: "Tippfehler und Inkonsistenzen bereinigen"
typ: Verbesserung
status: Offen
prioritaet: Niedrig
aufwand: Klein
erstellt: 2026-04-03
quelle: R00001
---

# R00021: Tippfehler und Inkonsistenzen bereinigen

## Beschreibung

Tippfehler in Variablennamen korrigieren und inkonsistente Patterns vereinheitlichen.

## Aenderungen

1. `complexityExceedsThresold` -> `complexityExceedsThreshold` (CyclomaticComplexityRule)
2. `shoudStopScan` -> `shouldStopScan` (RecursiveDirectoryWalker)
3. `isASubstentialAmountOfSqlPresent` -> `isSubstantialAmountOfSqlPresent` (SqlInNonRepositoryRule)
4. Einheitlicher Rueckgabetyp `ValidationMessage[]` (nicht nullable) fuer CyclomaticComplexityRule
