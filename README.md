# cleaner

[![CodeScene Code Health](https://codescene.io/projects/38361/status-badges/code-health)](https://codescene.io/projects/38361)

Cleaner ist der Versuch der technischen Unterstützung folgenden gerade in Arbeit befindlichen A3's.

obsidian://open?vault=Training2&file=A3s%2FA3%20Probleml%C3%B6sungstemplate

Es handelt sich um ein Experiment für ein Werkzeug, dass technologische und strukturelle Drift und Innovation sichtbar machen und dem entgegen wirken kann.

- Es soll eine Übersicht über unerwünschte Abweichungen darstellen.
- Ggf. noch unbekannte Muster erkennen und nachfragen.
- Ein nächstes einfaches Refactoring aufstellen, so dass man Lösungen schrittweise in den neuen Lösungsstandard überführen kann.

## Geprüfte Regeln

1. [X] Nur erlaubte Usings. (nuget-Pakete müssen explizit erlaubt werden)
2. [X] IOSP: If-Statements dürfen keine Expressions enthalten
3. [X] Linq: Es sind nur Ketten bis zu 2 Steps erlaubt, damit die Lesbarkeit der Ketten gewährleistet ist.
4. [X] Methodenlänge: Methoden dürfen nur 10 Zeilen lang sein.
5. [X] NotImplementedException: Code darf keine NotImplementedExceptions beinhalten
6. [X] Public Properties dürfen keine public setter haben (immutability)
7. [X] Dateien dürfen insgesamt nicht länger als 500 Zeilen sein.
8. [X] Dateien dürfen nur eine Deklaration (class, interface, enum, struct) beinhalten
9. [X] Dateien sollten immer nach dem innerhalb deklarierten Typ heißen
10. [X] Keine public properties mit generic lists als Typ, auch wenn sie nur getter haben.
11. [X] Keine public fields.
12. [X] Keine Out / Ref Parameter verwenden.
13. [X] Das .Net-interne Konfigurationsmanagement sollte nicht verwendet werden.
14. [ ] Es gibt bestimmte Standard-Bibliotheken, die immer verwendet werden sollten. (Core, Frontend falls UI...)
15. [X] SQL ist nur in Klassen erlaubt, die auf *Repository enden.
16. [X] Klassen die auf *Repository enden sollten nicht von anderen Klassen abgeleitet sein.
17. [X] Klassen, die auf *Repository enden haben mindestens einen IDataAccessor-Konstruktor-Parameter
18. [ ] ? Klassen, die auf *Repository enden werden alle über eine gemeinsame Factory erstellt
19. [ ] Wir verwenden ausschließlich den Logger aus der Core-Bibliothek.
20. [ ] Jede Anwendung muss in der Program.cs in der Main-Funktion eine bestimmte Sequenz an Befehlen enthalten.
21. [ ] Es sind nur bestimmte .Net-Versionen erlaubt.
22. [ ] Alle Referenzen sollten aktuell sein. (Nuget)
23. [ ] Alle Referenzen sollten aktuell sein. (Eigenes Artefaktsystem)
24. [ ] ? Struktur-Regeln? Was sollte wo sein?
25. [ ] ? Keine Enums
26. [ ] ? Keine switch-Statements
27. [ ] ? Isolations-Regeln: Vielleicht kann man Regeln formulieren, die es wahrscheinlich machen, dass man Themen in Subnamespaces zusammenfasst, statt nach Klassenarten zu sortieren.
28. [ ] Wenn CSS vorhanden ist, sollte es sich nur um max. das Frontend-Framework und 1-2 zusätzliche CSS-Dateien handeln.
29. [ ] Wenn Javascript vorhanden ist, sollten keine Vendor-Pakete eingebaut sein (das ist Aufgabe des Frontend Frameworks)
30. [ ] Kein Typescript. R. C++. (Explizite Liste erlaubter und nicht erlaubter Dateitypen?)
31. [ ] At the upmost execution level there should be a catch all for exceptions and it should be implemented in a way, that those exceptions are logged.
32. [ ] ? Jedes Git-Repository sollte explizit als entweder "aktiv zu warten" oder "nicht aktiv zu warten" klassifiziert sein (an einem gemeinsamen Ort)
33. [ ] ? Für aktiv zu wartende Git-Repositories sollte bei den Anforderungen ein MOC-* existieren.
34. [ ] #region verbieten
35. [X] Methoden, die if statements enthalten, die tiefer als 2 Ebenen verschachtelt sind.


