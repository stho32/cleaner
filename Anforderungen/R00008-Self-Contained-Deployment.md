---
id: R00008
titel: "Self-contained Deployment"
typ: Feature
status: Dokumentiert
erstellt: 2026-04-03
---

# R00008: Self-contained Deployment

## Beschreibung

Das Tool wird als self-contained Single-File-Executable fuer Windows und Linux gebaut und als GitHub Release veroeffentlicht.

## Akzeptanzkriterien

- Build fuer linux-x64 und win-x64
- Single-File-Publish (keine zusaetzlichen DLLs noetig)
- Self-contained (kein .NET Runtime auf Zielmaschine noetig)
- Automatischer GitHub Release bei Push

## Implementierung

- GitHub Actions Workflow `.github/workflows/dotnet.yml`
- `dotnet publish` mit `--self-contained true -p:PublishSingleFile=true`
- Auto-Tag mit Timestamp, Release via `softprops/action-gh-release`
- `build-and-deploy-locally.ps1` fuer lokales Deployment nach C:\Tools\
