export const TaskStatus = {
  TODO: "À faire",
  IN_PROGRESS: "En cours",
  DONE: "Terminé",
};

export const TaskPriority = {
  HIGH: "Haute",
  MEDIUM: "Moyenne",
  LOW: "Basse",
};

export const seedProject = {
  id: "fridgeflow",
  name: "FridgeFlow",
  tagline: "Roadmap complète — Du concept à la publication sur les stores",
  phases: [
    {
      id: "phase-01",
      order: 1,
      title: "Fondations",
      description:
        "Poser une base produit cohérente avant de construire : identité visuelle, flux écrans et accès outils.",
      timeline: "Cette semaine",
      tasks: [
        {
          id: "task-01-01",
          order: 1,
          title: "Design system",
          description:
            "Définir les couleurs, la typographie et le style visuel global. Ajouter un rendu de validation avant FlutterFlow.",
          status: TaskStatus.DONE,
          priority: TaskPriority.HIGH,
          estimate: "4 h",
          note: "Le socle visuel débloque toutes les autres étapes.",
        },
        {
          id: "task-01-02",
          order: 2,
          title: "Wireframes",
          description:
            "Créer des schémas simples de chaque écran pour valider le flux utilisateur complet avant développement.",
          status: TaskStatus.IN_PROGRESS,
          priority: TaskPriority.HIGH,
          estimate: "6 h",
        },
        {
          id: "task-01-03",
          order: 3,
          title: "Ouvrir les comptes",
          description:
            "Configurer FlutterFlow, OpenAI Platform, Firebase, Apple Developer et Google Play Console.",
          status: TaskStatus.TODO,
          priority: TaskPriority.MEDIUM,
          estimate: "3 h",
          note: "Apple Developer: abonnement annuel. Google Play: frais unique.",
        },
      ],
    },
    {
      id: "phase-02",
      order: 2,
      title: "Design sur FlutterFlow",
      description:
        "Monter le squelette visuel dans FlutterFlow, avec composants réutilisables et enchaînement d'écrans.",
      timeline: "Semaines 1 — 2",
      tasks: [
        {
          id: "task-02-01",
          order: 1,
          title: "Créer le projet",
          description:
            "Créer un nouveau projet FlutterFlow, choisir Flutter et connecter Firebase dès le départ.",
          status: TaskStatus.TODO,
          priority: TaskPriority.HIGH,
          estimate: "2 h",
        },
        {
          id: "task-02-02",
          order: 2,
          title: "Implémenter le design system",
          description:
            "Importer couleurs et polices, puis créer les composants réutilisables (boutons, cartes, inputs).",
          status: TaskStatus.TODO,
          priority: TaskPriority.HIGH,
          estimate: "6 h",
        },
        {
          id: "task-02-03",
          order: 3,
          title: "Construire les écrans statiques",
          description:
            "Créer les écrans dans l'ordre : Onboarding → Accueil → Scan → Config → Menu → Recette → Courses.",
          status: TaskStatus.TODO,
          priority: TaskPriority.MEDIUM,
          estimate: "12 h",
        },
        {
          id: "task-02-04",
          order: 4,
          title: "Configurer la navigation",
          description: "Relier tous les écrans avec les transitions et parcours attendus.",
          status: TaskStatus.TODO,
          priority: TaskPriority.MEDIUM,
          estimate: "3 h",
        },
      ],
    },
    {
      id: "phase-03",
      order: 3,
      title: "Logique & intégrations",
      description:
        "Brancher les données et l'intelligence : Firebase, scan photo via OpenAI et génération de menus/recettes.",
      timeline: "Semaines 3 — 4",
      tasks: [
        {
          id: "task-03-01",
          order: 1,
          title: "Connecter Firebase",
          description:
            "Créer la structure de données pour utilisateurs, inventaires, menus et recettes.",
          status: TaskStatus.TODO,
          priority: TaskPriority.HIGH,
          estimate: "10 h",
        },
        {
          id: "task-03-02",
          order: 2,
          title: "API OpenAI — Scan photo",
          description:
            "Transformer une photo en liste d'ingrédients détectés et l'afficher dans l'app.",
          status: TaskStatus.TODO,
          priority: TaskPriority.HIGH,
          estimate: "8 h",
        },
        {
          id: "task-03-03",
          order: 3,
          title: "API OpenAI — Génération de menu",
          description:
            "Utiliser inventaire + préférences pour générer un menu JSON structuré exploitable dans le calendrier.",
          status: TaskStatus.TODO,
          priority: TaskPriority.MEDIUM,
          estimate: "8 h",
        },
        {
          id: "task-03-04",
          order: 4,
          title: "Génération des recettes",
          description:
            "Pour chaque plat du menu, appeler l'API pour obtenir une recette complète adaptée au nombre de personnes.",
          status: TaskStatus.TODO,
          priority: TaskPriority.MEDIUM,
          estimate: "8 h",
        },
      ],
    },
    {
      id: "phase-04",
      order: 4,
      title: "Polish & tests",
      description:
        "Valider la qualité produit sur appareil réel, corriger les bugs et fluidifier l'expérience.",
      timeline: "Semaines 5 — 6",
      tasks: [
        {
          id: "task-04-01",
          order: 1,
          title: "Tests sur vrai appareil",
          description:
            "Tester chaque flux de bout en bout via TestFlight (iOS) ou APK (Android).",
          status: TaskStatus.TODO,
          priority: TaskPriority.HIGH,
          estimate: "6 h",
        },
        {
          id: "task-04-02",
          order: 2,
          title: "Corriger les bugs",
          description: "Prévoir une semaine complète pour correction et stabilisation.",
          status: TaskStatus.TODO,
          priority: TaskPriority.HIGH,
          estimate: "1 semaine",
        },
        {
          id: "task-04-03",
          order: 3,
          title: "Optimiser le design",
          description: "Ajuster visuels, animations et micro-interactions pour une app fluide.",
          status: TaskStatus.TODO,
          priority: TaskPriority.MEDIUM,
          estimate: "5 h",
        },
        {
          id: "task-04-04",
          order: 4,
          title: "Tester les performances",
          description: "Vérifier la rapidité des appels API et la réactivité globale de l'app.",
          status: TaskStatus.TODO,
          priority: TaskPriority.MEDIUM,
          estimate: "4 h",
        },
      ],
    },
    {
      id: "phase-05",
      order: 5,
      title: "Préparation publication",
      description:
        "Assembler les livrables de soumission stores, configurer la monétisation et générer les builds finaux.",
      timeline: "Semaine 7",
      tasks: [
        {
          id: "task-05-01",
          order: 1,
          title: "Créer les assets de publication",
          description:
            "Préparer icône 1024×1024, screenshots iPhone 6.7, description FR+EN et politique de confidentialité.",
          status: TaskStatus.TODO,
          priority: TaskPriority.HIGH,
          estimate: "5 h",
        },
        {
          id: "task-05-02",
          order: 2,
          title: "Configurer les achats in-app",
          description:
            "Mettre en place les abonnements premium dans App Store Connect et Google Play Console.",
          status: TaskStatus.TODO,
          priority: TaskPriority.MEDIUM,
          estimate: "3 h",
        },
        {
          id: "task-05-03",
          order: 3,
          title: "Build final",
          description:
            "Générer les artefacts IPA (iOS) et AAB (Android) directement depuis FlutterFlow.",
          status: TaskStatus.TODO,
          priority: TaskPriority.HIGH,
          estimate: "2 h",
        },
      ],
    },
    {
      id: "phase-06",
      order: 6,
      title: "Publication",
      description: "Soumettre sur stores, suivre la review et finaliser la mise en ligne.",
      timeline: "Semaine 8",
      tasks: [
        {
          id: "task-06-01",
          order: 1,
          title: "Soumettre sur App Store",
          description:
            "Soumettre via App Store Connect. Prévoir 1 à 3 jours de review et un aller-retour possible.",
          status: TaskStatus.TODO,
          priority: TaskPriority.HIGH,
          estimate: "2 h",
        },
        {
          id: "task-06-02",
          order: 2,
          title: "Soumettre sur Google Play",
          description:
            "Publier via Play Console. Review plus souple, quelques heures à 2 jours.",
          status: TaskStatus.TODO,
          priority: TaskPriority.HIGH,
          estimate: "2 h",
        },
        {
          id: "task-06-03",
          order: 3,
          title: "FridgeFlow est live",
          description: "Valider la publication sur les deux stores et annoncer le lancement.",
          status: TaskStatus.TODO,
          priority: TaskPriority.MEDIUM,
          estimate: "1 h",
          note: "Moment de célébration 🎉",
        },
      ],
    },
  ],
};
