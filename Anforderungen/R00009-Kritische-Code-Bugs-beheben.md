---
id: R00009
titel: "Kritische Code-Bugs beheben"
typ: Bug
status: Offen
prioritaet: Hoch
aufwand: Klein
erstellt: 2026-04-03
quelle: R00001
---

# R00009: Kritische Code-Bugs beheben

## Beschreibung

Mindestens 5 nachgewiesene Bugs in Regel-Implementierungen und Skripten.

## Bugs

1. **IfStatementOperatorRule.cs:57-63** — Fehlende Klammern bei Operator-Praezedenz (`&&` vor `||`)
2. **NoPublicFieldsRule.cs:41-46** — `IsPublic` prueft Modifier falsch (ein Modifier kann nicht gleichzeitig Public und nicht Static sein)
3. **AllowedUsingsRule.cs:101-103** — `IsSystemNamespace` erkennt `using System;` nicht (nur `System.`)
4. **MethodLengthRule.cs:48** — Fehlermeldung sagt "limit of 10" obwohl MaxLength=20
5. **run.sh** — Verweist auf `spamfilter` statt `cleaner`

## Akzeptanzkriterien

- Alle 5 Bugs sind behoben
- Fuer jeden Bug existiert ein Test
- Alle bestehenden Tests bestehen weiterhin
