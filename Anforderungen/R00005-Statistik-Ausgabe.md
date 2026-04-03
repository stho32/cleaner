---
id: R00005
titel: "Statistik-Ausgabe"
typ: Feature
status: Dokumentiert
erstellt: 2026-04-03
---

# R00005: Statistik-Ausgabe

## Beschreibung

Nach dem Scan wird eine Zusammenfassung ausgegeben: Anzahl gescannter Dateien/Ordner und Dateien/Ordner mit Problemen.

## Implementierung

- `IStatisticsCollector` Interface
- `StatisticsCollector` zaehlt Dateien und Probleme
- `Statistics` berechnet und druckt die Zusammenfassung
- `Statistic` Datenklasse fuer einzelne Statistik-Werte
