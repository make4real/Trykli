# FridgeFlow

Application web de suivi interactif de roadmap pour le projet FridgeFlow.

## Lancer localement

```bash
python3 -m http.server 4173
```

Puis ouvrir `http://localhost:4173`.

## Fonctionnalités

- Dashboard avec progression globale.
- Vue roadmap complète par phase.
- Détail de phase avec checklist interactive.
- Détail de tâche (statut, priorité, estimation, note éditable).
- Statistiques globales et reset des données locales.
- Persistance locale via `localStorage`.
