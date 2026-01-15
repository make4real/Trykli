/* Trykli data: toutes les cartes/questions localement */
const buildVariants = (base, variants, formatter) => {
  const output = [];
  base.forEach((item) => {
    variants.forEach((variant) => output.push(formatter(item, variant)));
  });
  return output;
};

const sliceTo = (arr, count) => arr.slice(0, count);

// [1] Qui de nous deux ? (80)
const whoBase = [
  "oublie toujours où il/elle a posé son téléphone",
  "propose un plan spontané qui finit en aventure",
  "s'endort pendant un film",
  "déborde d'idées pour un week-end",
  "répond le plus vite aux messages",
  "choisit toujours le dessert",
  "est le/la plus patient(e)",
  "se souvient du plus de détails",
  "met la musique la plus motivante",
  "sait calmer l'autre en 30 secondes",
  "prend les meilleures photos",
  "a la playlist la plus chill",
  "fait rire l'autre même fatigué(e)",
  "prépare les surprises",
  "a le radar pour les bons plans",
  "commence les conversations profondes",
  "est le/la plus organisé(e)",
  "fait des câlins surprises",
  "trouve les meilleurs jeux",
  "a toujours un mot doux"
];
const whoVariants = [
  "quand on sort",
  "le dimanche",
  "pendant les vacances",
  "en mode cocooning"
];
const whoOfUs = sliceTo(
  buildVariants(whoBase, whoVariants, (base, variant) => `Qui de nous deux ${base} ${variant} ?`),
  80
);

// [10] Tu préfères (120)
const preferPairs = [
  ["un petit-déj au lit", "un dîner surprise"],
  ["une balade nocturne", "un lever de soleil"],
  ["un marathon séries", "un marathon films"],
  ["un week-end nature", "un week-end urbain"],
  ["un message vocal", "une lettre papier"],
  ["un jeu de société", "un jeu d'impro"],
  ["une surprise spontanée", "un plan organisé"],
  ["une playlist commune", "une playlist secrète"],
  ["un défi photo", "un défi cuisine"],
  ["une soirée sans écran", "une soirée playlist"],
  ["un cadeau utile", "un cadeau symbolique"],
  ["un brunch tardif", "un dîner tôt"],
  ["une journée sans plan", "une journée planifiée"],
  ["une aventure gourmande", "une aventure artistique"],
  ["un message codé", "un message direct"],
  ["un cadeau surprise", "un cadeau choisi"],
  ["un voyage proche", "un voyage lointain"],
  ["un moment silencieux", "un moment bavard"],
  ["une soirée jeux", "une soirée karaoké"],
  ["un selfie fun", "une photo posée"],
  ["une sortie street food", "un resto romantique"],
  ["un atelier créatif", "un atelier sportif"],
  ["un café en terrasse", "un chocolat chaud"],
  ["une activité surprise", "une activité classique"],
  ["un défi mémoire", "un défi imagination"],
  ["un message du matin", "un message du soir"],
  ["une séance yoga", "une séance danse"],
  ["un album photo", "une vidéo souvenir"],
  ["un cadeau fait main", "un cadeau premium"],
  ["un objectif fun", "un objectif ambitieux"]
];
const preferVariants = ["Tu préfères", "Tu préfères version fun", "Tu préfères version douce", "Tu préfères version rapide"];
const wouldYouRather = sliceTo(
  preferPairs.flatMap((pair) =>
    preferVariants.map((variant) => `${variant} : ${pair[0]} ou ${pair[1]} ?`)
  ),
  120
);

// [2] Vérités (80) + Gages (80)
const truthBase = [
  "un moment où tu étais fier(ère) de nous",
  "une habitude de l'autre que tu adores",
  "un rêve de voyage à deux",
  "un compliment que tu veux dire",
  "un souvenir qui te fait sourire",
  "une surprise que tu aimerais organiser",
  "une qualité que tu admires",
  "une chose que tu veux apprendre ensemble",
  "un moment où tu t'es senti(e) compris(e)",
  "un petit rituel que tu aimerais créer",
  "un objectif personnel pour 2025",
  "une chanson qui te fait penser à nous",
  "un moment où tu as été touché(e)",
  "un message que tu aimerais recevoir",
  "un geste qui te rassure",
  "une anecdote drôle sur nous",
  "un moment où tu as appris quelque chose",
  "un moment de complicité",
  "un souvenir de vacances",
  "un moment simple qui t'a fait du bien"
];
const truthVariants = ["Raconte", "Explique", "Décris", "Partage"];
const truths = sliceTo(
  buildVariants(truthBase, truthVariants, (base, variant) => `${variant} : ${base}.`),
  80
);

const dareBase = [
  "fais un mini discours d'amour",
  "donne 3 compliments rapides",
  "imite la façon dont l'autre dit bonjour",
  "mime votre premier rendez-vous",
  "improvise un toast",
  "décris l'autre comme une météo",
  "invente un surnom",
  "fais un micro poème",
  "raconte une blague",
  "fais une pose photo",
  "improvise une pub pour votre couple",
  "fais un câlin surprise",
  "faites un high-five synchronisé",
  "décris un souvenir sans dire l'année",
  "fais un compliment en rime",
  "imite un présentateur radio",
  "propose un plan de soirée",
  "fais un mini massage de 10s",
  "mime un fou rire",
  "improvise une petite chanson"
];
const dareVariants = ["Tout de suite", "Mode doux", "Mode express", "Version fun"];
const dares = sliceTo(
  buildVariants(dareBase, dareVariants, (base, variant) => `${variant} : ${base}.`),
  80
);

// [5] Débats absurdes (60)
const debateTopics = [
  "les chaussettes dépareillées", "les playlists sans skip", "les doudous", "les selfies ratés",
  "les réveils musicaux", "les plantes", "les brunchs tardifs", "les mugs XXL",
  "les emojis", "les pizzas en cœur", "les chaussons moelleux", "les jeux de société",
  "les câlins de 3 secondes", "les smoothies", "les messages vocaux"
];
const debateAngles = [
  "devraient être obligatoires", "sont sous-estimés", "sont totalement surestimés", "devraient être patrimoine du couple"
];
const absurdDebates = sliceTo(
  buildVariants(debateTopics, debateAngles, (topic, angle) => `Débat : ${topic} ${angle}.`),
  60
);

// [12] Impro théâtre (60)
const improvRoles = [
  "un(e) DJ romantique", "un(e) détective du quotidien", "un(e) astronaute", "un(e) chef(fe) pâtissier(ère)",
  "un(e) guide touristique", "un(e) coach motivation", "un(e) bibliothécaire curieux(se)", "un(e) photographe",
  "un(e) influenceur(se) cuisine", "un(e) explorateur(trice)"
];
const improvPlaces = [
  "dans un train de nuit", "sur un rooftop", "dans une cabane", "au marché", "dans un musée",
  "dans un ascenseur", "dans une fête de village", "dans un studio photo", "sur une plage", "dans une salle d'attente"
];
const improvTwists = [
  "un secret doit être révélé", "il faut parler uniquement en questions", "une pluie de confettis tombe",
  "vous venez de gagner un prix", "vous devez parler comme dans un film noir", "un cadeau surprise apparaît"
];
const improvCards = sliceTo(
  improvRoles.flatMap((roleA) =>
    improvPlaces.flatMap((place) =>
      improvTwists.map((twist) => ({ roleA, place, twist }))
    )
  ),
  60
);

// [15] Challenges 1 min (80)
const minuteBase = [
  "lister 12 souvenirs", "faire 10 compliments", "trouver 10 mots doux", "mimer 6 émotions",
  "proposer 5 activités", "inventer 3 slogans", "trouver 8 destinations", "faire une mini danse",
  "raconter 5 blagues", "trouver 7 dates importantes", "faire un duel de regards",
  "faire 15 high-fives", "inventer un mini rap", "écrire 3 objectifs", "improviser un mini podcast",
  "mimer 5 films", "dire 12 qualités", "chanter 10 secondes", "faire 10 claps synchronisés", "trouver 8 recettes"
];
const minuteVariants = ["en 60 secondes", "sans rire", "en rythme", "en duo"];
const minuteChallenges = sliceTo(
  buildVariants(minuteBase, minuteVariants, (base, variant) => `Défi 1 minute : ${base} ${variant}.`),
  80
);

// [7] Quiz couple (80)
const quizBase = [
  "son snack préféré", "sa chanson motivation", "son moment préféré de la journée", "sa boisson doudou",
  "son rituel du matin", "son film doudou", "son endroit feel-good", "son geste de tendresse préféré",
  "sa saison favorite", "son dessert favori", "sa couleur préférée", "son objet porte-bonheur",
  "sa sortie préférée", "son talent caché", "sa phrase qui motive", "son type de vacances idéal",
  "son petit plaisir secret", "son endroit rêvé", "son confort food", "sa série réconfort"
];
const quizVariants = ["Quiz duo", "Quiz express", "Quiz surprise", "Quiz rapide"];
const coupleQuiz = sliceTo(
  buildVariants(quizBase, quizVariants, (base, variant) => `${variant} : Quel est ${base} ?`),
  80
);

// [3] Souvenirs (70)
const memoryBase = [
  "notre première rencontre",
  "notre premier fou rire",
  "un moment où tu t'es senti(e) compris(e)",
  "un petit rituel né par hasard",
  "un moment simple qui t'a marqué",
  "une surprise qui t'a touché(e)",
  "une soirée parfaite",
  "un souvenir de vacances",
  "un moment où tu étais fier(ère) de nous",
  "un moment de complicité",
  "un moment où on a appris quelque chose",
  "un moment de calme partagé",
  "un petit geste marquant",
  "un souvenir d'hiver",
  "un souvenir d'été",
  "un moment où on s'est motivé",
  "un moment où on a improvisé",
  "un souvenir qui te fait sourire",
  "un moment qui t'a surpris",
  "un moment de soutien"
];
const memoryVariants = ["Raconte", "Décris", "Partage", "En 3 phrases"];
const memories = sliceTo(
  buildVariants(memoryBase, memoryVariants, (base, variant) => `${variant} : ${base}.`),
  70
);

// [14] Secrets soft (70)
const secretsBase = [
  "un petit rêve de couple",
  "une chose que tu apprécies en secret",
  "un geste qui te rassure",
  "un souvenir que tu veux garder",
  "un moment où tu as voulu dire merci",
  "un compliment que tu gardes pour toi",
  "un endroit où tu veux aller",
  "un moment où tu t'es senti(e) aligné(e)",
  "un petit projet à deux",
  "un geste qui te fait fondre",
  "un moment où tu as été impressionné(e)",
  "un moment où tu voulais plus de temps",
  "un rituel que tu aimerais créer",
  "un mot doux que tu gardes",
  "une surprise que tu imagines"
];
const secretsVariants = ["Secret doux", "Petit aveu", "Confession tendre", "Aveu simple", "Secret calme"];
const secretsSoft = sliceTo(
  buildVariants(secretsBase, secretsVariants, (base, variant) => `${variant} : ${base}.`),
  70
);
const secretsMedium = sliceTo(
  buildVariants(secretsBase, ["Niveau medium", "Plus profond", "Version medium", "Confidence medium", "Honnête"], (base, variant) => `${variant} : ${base}.`),
  70
);

// [13] Blind test parlé (60)
const blindSubjects = [
  "un film", "une chanson", "une série", "un plat", "un lieu", "un jeu", "une couleur", "un sport",
  "une appli", "une boisson", "une saison", "une fête", "un animal", "un métier", "un souvenir"
];
const blindForbidden = ["amour", "couple", "toujours", "jamais", "ensemble", "rendez-vous", "voyage", "surprise"];
const blindTest = sliceTo(
  blindSubjects.flatMap((subject) =>
    blindForbidden.slice(0, 4).map((word, idx) => ({
      subject: `Décris ${subject} sans dire : ${word}, ${blindForbidden[(idx + 1) % blindForbidden.length]}, ${blindForbidden[(idx + 2) % blindForbidden.length]}.`
    }))
  ),
  60
);

// [19] Émotions (50)
const emotions = [
  "joie", "tendresse", "fierté", "nostalgie", "excitation", "apaisement", "surprise", "curiosité",
  "confiance", "admiration", "gratitude", "complicité", "motivation", "chaleur", "sérénité", "espièglerie",
  "espoir", "rassurance", "douceur", "émerveillement", "envie", "calme", "optimisme", "audace", "bienveillance"
];
const emotionPhrases = [
  "Quand on se retrouve après une longue journée.",
  "Quand on prépare un projet ensemble.",
  "Quand on improvise un plan.",
  "Quand on se soutient en silence.",
  "Quand on partage un fou rire."
];
const emotionCards = sliceTo(
  buildVariants(emotions, emotionPhrases, (emotion, phrase) => ({ emotion, phrase })),
  50
);

// [21] Priorités (60)
const prioritySituations = [
  "Un samedi libre", "Un budget cadeau limité", "Une soirée spéciale", "Un dimanche matin", "Un voyage de 3 jours",
  "Un mois chargé", "Un week-end pluvieux", "Une nouvelle tradition", "Une semaine stressante", "Un événement surprise"
];
const priorityChoices = [
  ["repos total", "activité créative", "sortie extérieure", "moment surprise"],
  ["cuisine ensemble", "jeu rapide", "film + plaid", "balade courte"],
  ["planifier l'année", "faire une pause", "défi fun", "moment romantique"],
  ["brunch maison", "marché local", "sport doux", "sieste duo"],
  ["aventure nature", "découverte urbaine", "route gourmande", "séjour slow"],
  ["discussion profonde", "jeu léger", "activité manuelle", "moment silence"]
];
const priorities = sliceTo(
  prioritySituations.flatMap((situation, idx) =>
    priorityChoices.map((choices) => ({ situation, choices }))
  ),
  60
);

// [20] Battle créativité (60)
const creativityThemes = [
  "inventer un slogan", "créer un logo", "imaginer une affiche de film", "inventer un rituel de nouvel an",
  "inventer un hashtag", "créer une mini histoire", "proposer un menu spécial", "imaginer un voyage fictif",
  "créer un toast", "inventer un défi photo", "imaginer un souvenir futur", "créer un bingo couple"
];
const creativityVariants = ["Battle express", "Battle fun", "Battle créatif", "Battle surprise", "Battle duo"];
const creativityBattle = sliceTo(
  buildVariants(creativityThemes, creativityVariants, (theme, variant) => `${variant} : ${theme}.`),
  60
);

// [23] Promesses (60)
const promiseBase = [
  "organiser une micro-surprise par mois",
  "avoir un moment sans écrans par semaine",
  "tester une nouvelle recette ensemble",
  "faire une mini balade chaque dimanche",
  "prendre une photo souvenir chaque mois",
  "faire un check-in émotions chaque semaine",
  "planifier un week-end surprise",
  "dire un compliment sincère par jour",
  "apprendre une nouvelle chose ensemble",
  "faire une soirée jeux",
  "préparer une playlist commune",
  "tenir un carnet de souvenirs"
];
const promiseVariants = ["Promesse 2025", "Engagement doux", "Promesse fun", "Pacte du couple", "Promesse simple"];
const promises = sliceTo(
  buildVariants(promiseBase, promiseVariants, (base, variant) => `${variant} : ${base}.`),
  60
);

// [24] Voix (60)
const voiceRoles = [
  "narrateur de documentaire", "vendeur(se) de marché", "astronaute", "prof de yoga", "guide touristique",
  "commentateur sportif", "agent secret", "chef(fe) étoilé(e)", "animateur radio", "magicien(ne)",
  "robot bienveillant", "journaliste météo", "poète", "coach de vie", "capitaine de navire"
];
const voiceLines = [
  "Raconte une journée parfaite.",
  "Présente votre couple comme une mission.",
  "Décris une surprise romantique.",
  "Lance une annonce spéciale.",
  "Donne une consigne amusante."
];
const voices = sliceTo(
  buildVariants(voiceRoles, voiceLines, (role, line) => ({ role, line })),
  60
);

// [4] Devine la réponse de l'autre (60)
const guessOtherBase = [
  "Quel est ton plan parfait pour samedi ?",
  "Quel snack te rend heureux(se) ?",
  "Quel moment de la journée tu préfères ?",
  "Quel est ton souvenir préféré avec moi ?",
  "Quel cadeau te ferait sourire ?",
  "Quelle musique te booste ?",
  "Quel est ton endroit feel-good ?",
  "Quelle boisson doudou ?",
  "Quel est ton mini rêve 2025 ?",
  "Quelle activité te détend le plus ?"
];
const guessOtherVariants = ["Devine :", "Double devinette :", "Télépathie :", "Mini quiz :", "Question surprise :", "Choix secret :"];
const guessOther = sliceTo(
  buildVariants(guessOtherBase, guessOtherVariants, (base, variant) => `${variant} ${base}`),
  60
);

// [16] Qui ment ? (60)
const liePhrases = [
  "J'ai déjà oublié notre premier rendez-vous.",
  "J'ai déjà chanté faux en public.",
  "J'ai déjà planifié une surprise.",
  "J'ai déjà envoyé un message au mauvais contact.",
  "J'ai déjà dormi devant un film romantique.",
  "J'ai déjà écrit une lettre d'amour.",
  "J'ai déjà inventé une excuse mignonne.",
  "J'ai déjà improvisé un rendez-vous.",
  "J'ai déjà pris trop de photos.",
  "J'ai déjà fait un selfie raté."
];
const whoLies = sliceTo(
  liePhrases.flatMap((phrase) =>
    liePhrases.map((phraseB) => ({ phraseA: phrase, phraseB }))
  ),
  60
);

// [9] Compliments forcés (60)
const complimentThemes = [
  "le style", "l'humour", "l'écoute", "la créativité", "la patience", "la gentillesse", "l'énergie",
  "la capacité à surprendre", "la douceur", "l'organisation", "le sourire", "les yeux", "la voix", "le courage", "le soutien"
];
const complimentVariants = ["Compliment forcé", "Mission douceur", "Compliment flash", "Défi compliment"];
const compliments = sliceTo(
  buildVariants(complimentThemes, complimentVariants, (theme, variant) => `${variant} : Complimente ${theme}.`),
  60
);

// [8] Procès (60)
const trialCases = sliceTo(
  buildVariants(
    ["Avoir mangé le dernier biscuit", "Avoir oublié un message important", "Avoir monopolisé la couette", "Avoir mis la mauvaise playlist", "Avoir laissé traîner une tasse", "Avoir chanté trop fort", "Avoir tardé à répondre", "Avoir choisi une mauvaise série", "Avoir pris plus de frites", "Avoir insisté pour un film"],
    ["Affaire", "Dossier", "Procès", "Tribunal", "Accusation", "Audience"],
    (base, variant) => `${variant} : ${base}.`
  ),
  60
);

// [17] Planifier l'année (60)
const planCategories = ["Défi", "Sortie", "Routine", "Surprise", "Projet", "Habitude"];
const planIdeas = [
  "un mini-voyage à planifier", "une compétence à apprendre", "une tradition mensuelle", "un projet créatif",
  "une routine bien-être", "un moment mensuel", "un défi sportif doux", "une sortie culturelle",
  "un projet gourmand", "une activité solidaire"
];
const planYear = sliceTo(
  buildVariants(planIdeas, planCategories, (idea, category) => ({ category, idea })),
  60
);

// [19] Devine l'émotion (50) is emotionCards above

const DATA = {
  tags: ["Fun", "Romance", "Défis", "Parole"],
  siteGames: [
    {
      id: "26",
      name: "Morpion duo",
      description: "Un morpion rapide à deux.",
      tags: ["Fun", "Défis"],
      duration: "60s",
      rules: [
        "A joue avec ❌, B avec ⭕.",
        "Chacun joue à tour de rôle.",
        "3 alignés = victoire.",
        "Reset pour relancer une manche."
      ],
      type: "ticTacToe"
    },
    {
      id: "1",
      name: "Qui de nous deux ?",
      description: "Comparez vos traits avec humour.",
      tags: ["Fun", "Parole"],
      duration: "libre",
      rules: [
        "Lisez la carte.",
        "Choisissez A, B ou Égalité.",
        "Option Tirage au sort pour départager.",
        "+1 point automatique si A ou B est choisi."
      ],
      type: "who"
    },
    {
      id: "2",
      name: "Vérité ou gage",
      description: "Questions soft ou défis rapides.",
      tags: ["Parole", "Défis"],
      duration: "60s",
      rules: [
        "Choisissez Vérité ou Gage.",
        "Répondez avec sincérité.",
        "Pour un gage, lancez le timer 60s.",
        "Réussi ou Raté pour marquer si besoin."
      ],
      type: "truthOrDare"
    },
    {
      id: "3",
      name: "Le jeu des souvenirs",
      description: "Questions pour se remémorer le meilleur.",
      tags: ["Romance", "Parole"],
      duration: "libre",
      rules: [
        "Tirez une question souvenir.",
        "Répondez tour à tour si souhaité.",
        "Utilisez Sauver ce souvenir si vous l'adorez."
      ],
      type: "memories"
    },
    {
      id: "4",
      name: "Devine la réponse de l'autre",
      description: "Testez votre télépathie.",
      tags: ["Fun", "Parole"],
      duration: "libre",
      rules: [
        "A devine la réponse.",
        "B répond.",
        "Match = +1 compatibilité.",
        "Pas match = continuez."
      ],
      type: "guessOther"
    },
    {
      id: "5",
      name: "Débat absurde",
      description: "Défendez des idées improbables.",
      tags: ["Fun", "Défis"],
      duration: "120s",
      rules: [
        "Tirez un sujet absurde.",
        "Attribuez Pour / Contre au hasard.",
        "Timer 120s.",
        "Verdict A/B/Égalité."
      ],
      type: "debate"
    },
    {
      id: "7",
      name: "Quiz de couple",
      description: "Prouvez que vous vous connaissez.",
      tags: ["Parole"],
      duration: "libre",
      rules: [
        "Chacun répond.",
        "Même réponse = +1 compatibilité.",
        "Continuez pour un score final."
      ],
      type: "coupleQuiz"
    },
    {
      id: "8",
      name: "Le procès",
      description: "Un mini tribunal du couple.",
      tags: ["Fun", "Parole"],
      duration: "libre",
      rules: [
        "Tirez une affaire.",
        "Accusation 30s, défense 30s.",
        "Verdict vote ou aléatoire.",
        "Score optionnel."
      ],
      type: "trial"
    },
    {
      id: "9",
      name: "Compliments forcés",
      description: "Des compliments obligatoires.",
      tags: ["Romance"],
      duration: "libre",
      rules: [
        "Tirez une mission.",
        "Complimentez en vrai.",
        "Validez et passez au suivant."
      ],
      type: "compliments"
    },
    {
      id: "10",
      name: "Tu préfères…",
      description: "Choix impossibles version couple.",
      tags: ["Fun"],
      duration: "libre",
      rules: [
        "Choisissez A ou B.",
        "Indiquez si vous êtes d'accord.",
        "Bouton Pourquoi pour expliquer."
      ],
      type: "wouldYouRather"
    },
    {
      id: "12",
      name: "Impro théâtre",
      description: "Impro guidée en duo.",
      tags: ["Défis", "Fun"],
      duration: "120s",
      rules: [
        "Tirez rôle A, rôle B, lieu, twist.",
        "Timer 120s.",
        "Nouvelle scène pour recommencer."
      ],
      type: "improv"
    },
    {
      id: "13",
      name: "Blind test parlé",
      description: "Décris sans dire les mots interdits.",
      tags: ["Défis"],
      duration: "60s",
      rules: [
        "Décrivez le sujet.",
        "Évitez les mots interdits.",
        "Trouvé ou pas trouvé."
      ],
      type: "blindTest"
    },
    {
      id: "14",
      name: "Secrets soft",
      description: "Confidences safe et douces.",
      tags: ["Romance", "Parole"],
      duration: "libre",
      rules: [
        "Choisissez un niveau.",
        "Partagez en douceur.",
        "Passer est toujours ok."
      ],
      type: "secrets"
    },
    {
      id: "15",
      name: "Challenge 1 minute",
      description: "Défis express en duo.",
      tags: ["Défis"],
      duration: "60s",
      rules: [
        "Tirez un défi.",
        "Timer automatique 60s.",
        "Réussi ou Raté."
      ],
      type: "minuteChallenge"
    },
    {
      id: "16",
      name: "Qui ment ?",
      description: "Deux phrases, une seule vraie.",
      tags: ["Fun", "Parole"],
      duration: "libre",
      rules: [
        "A parle, B devine.",
        "Puis B parle, A devine.",
        "Choisissez Phrase 1 ou 2."
      ],
      type: "whoLies"
    },
    {
      id: "17",
      name: "Planifier l'année à venir",
      description: "Projets 2025 à garder.",
      tags: ["Parole"],
      duration: "libre",
      rules: [
        "Tirez une carte.",
        "Garder ou Skip.",
        "Liste sauvegardée."
      ],
      type: "planYear"
    },
    {
      id: "19",
      name: "Devine l'émotion",
      description: "Jouez une émotion sans la dire.",
      tags: ["Défis", "Fun"],
      duration: "60s",
      rules: [
        "Lisez la phrase.",
        "Un lecteur voit l'émotion.",
        "L'autre devine."
      ],
      type: "emotion"
    },
    {
      id: "20",
      name: "Battle de créativité",
      description: "Votez le plus créatif.",
      tags: ["Défis"],
      duration: "60s",
      rules: [
        "Tirez un thème.",
        "Timer 60s.",
        "Vote A/B/Égalité."
      ],
      type: "creativity"
    },
    {
      id: "21",
      name: "Le jeu des priorités",
      description: "Choisissez l'option la plus importante.",
      tags: ["Parole"],
      duration: "libre",
      rules: [
        "Tirez une situation.",
        "Chacun choisit.",
        "Même choix ou différent."
      ],
      type: "priorities"
    },
    {
      id: "23",
      name: "Promesses",
      description: "Engagements fun ou romantiques.",
      tags: ["Romance", "Parole"],
      duration: "libre",
      rules: [
        "Tirez une promesse.",
        "Garder ou Skip.",
        "Liste sauvegardée."
      ],
      type: "promises"
    },
    {
      id: "24",
      name: "Le jeu des voix",
      description: "Personnages et phrases à jouer.",
      tags: ["Fun", "Défis"],
      duration: "60s",
      rules: [
        "Tirez une voix.",
        "A joue, puis B joue.",
        "Vote meilleur acteur."
      ],
      type: "voices"
    }
  ],
  vocalGames: [
    {
      id: "6",
      name: "Histoire interactive",
      description: "Une histoire guidée avec choix A/B.",
      tags: ["Romance", "Parole"],
      duration: "libre",
      rules: ["Ouvre ChatGPT en mode vocal et lis le texte ci-dessous."],
      text: "Tu es un narrateur. Crée une histoire interactive pour un couple, en 10 étapes maximum. À chaque étape, propose deux choix simples A ou B. Le ton doit être drôle, romantique et léger. Ne sois jamais vulgaire. Attends notre choix avant de continuer."
    },
    {
      id: "11",
      name: "Escape game vocal",
      description: "Énigmes simples en duo.",
      tags: ["Défis", "Parole"],
      duration: "libre",
      rules: ["Ouvre ChatGPT en mode vocal et lis le texte ci-dessous."],
      text: "Tu es le maître d’un escape game. Décris une pièce dans laquelle nous sommes enfermés. Notre objectif est de sortir en résolvant des énigmes simples. Réponds uniquement à nos actions ou questions. Ne donne jamais la solution directement."
    },
    {
      id: "18",
      name: "Scénarios alternatifs",
      description: "Des “Et si…” surprenants.",
      tags: ["Fun", "Parole"],
      duration: "libre",
      rules: ["Ouvre ChatGPT en mode vocal et lis le texte ci-dessous."],
      text: "Propose 8 scénarios ‘Et si…’ pour un couple. Nous en choisissons un, puis tu fais vivre la scène comme une histoire interactive avec des choix."
    },
    {
      id: "22",
      name: "Conseiller amoureux",
      description: "Coaching bienveillant en duo.",
      tags: ["Romance", "Parole"],
      duration: "libre",
      rules: ["Ouvre ChatGPT en mode vocal et lis le texte ci-dessous."],
      text: "Tu es un coach de couple bienveillant. Pose-nous quelques questions pour mieux nous comprendre, puis donne des conseils concrets et positifs. Termine par un petit exercice simple à faire ensemble."
    },
    {
      id: "25",
      name: "Fin alternative de film",
      description: "Un twist final pour votre film.",
      tags: ["Fun", "Parole"],
      duration: "libre",
      rules: ["Ouvre ChatGPT en mode vocal et lis le texte ci-dessous."],
      text: "Donne une fin alternative au film ou à la série que nous allons te citer. Raconte-la comme une vraie scène avec des émotions et un twist final."
    }
  ],
  data: {
    whoOfUs,
    wouldYouRather,
    truths,
    dares,
    absurdDebates,
    improvCards,
    minuteChallenges,
    coupleQuiz,
    memories,
    secretsSoft,
    secretsMedium,
    blindTest,
    emotionCards,
    priorities,
    creativityBattle,
    promises,
    voices,
    guessOther,
    whoLies,
    compliments,
    trialCases,
    planYear
  }
};
