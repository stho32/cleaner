---
id: R00003
titel: "18+ Validierungsregeln"
typ: Feature
status: Dokumentiert
erstellt: 2026-04-03
---

# R00003: 18+ Validierungsregeln

## Beschreibung

Das Tool implementiert ueber 18 Code-Qualitaetsregeln, die C#-Dateien mittels Roslyn-Parsing pruefen.

## Implementierte Regeln

1. AllowedUsingsRule - Nur erlaubte Namespaces
2. CyclomaticComplexityRule - Zyklomatische Komplexitaet <= 4
3. FileNameMatchingDeclarationRule - Dateiname = Typname
4. ForEachDataSourceRule - Max 2 Dots in ForEach-Datenquelle
5. IfStatementDotsRule - Max 2 Dots in If-Bedingungen (Law of Demeter)
6. IfStatementOperatorRule - Keine Operatoren in If-Bedingungen (IOSP)
7. LinqExpressionLengthRule - Max 2 LINQ-Steps
8. MethodLengthRule - Max 20 Semicolons pro Methode
9. NestedIfStatementsRule - Max 2 Verschachtelungsebenen
10. NoConfigurationManagerRule - Kein ConfigurationManager
11. NoOutAndRefParametersRule - Keine out/ref Parameter
12. NoPublicFieldsRule - Keine public Fields
13. NoPublicGenericListPropertiesRule - Keine public List-Properties
14. NotImplementedExceptionRule - Keine NotImplementedException
15. PublicPropertiesPrivateSettersRule - Public Properties brauchen private Setter
16. RepositoryInheritanceRule - Repository-Klassen nicht ableiten
17. RowLimitRule - Max 500 Zeilen pro Datei
18. SingleDeclarationRule - Eine Deklaration pro Datei
19. SqlInNonRepositoryRule - SQL nur in Repository-Klassen
20. FolderBasedQualityScanner - Max 6 Dateien pro Verzeichnis

## Architektur

- `IRule`-Interface mit `Validate(filePath, fileContent)`
- `RuleFactory` laedt Regeln per Reflection
- Jede Regel hat Id, Name, ShortDescription, LongDescription
