/* Données 100% locales pour Couple New Year Games */
const buildVariants = (base, variants, formatter) => {
  const out = [];
  base.forEach((item) => {
    variants.forEach((variant) => {
      out.push(formatter(item, variant));
    });
  });
  return out;
};

const sliceTo = (arr, count) => arr.slice(0, count);

const whoOfUsBase = [
  "oublie toujours où il/elle a posé son téléphone",
  "sait improviser un plan en 2 minutes",
  "a le plus de playlists",
  "ferait rire l'autre en pleine réunion",
  "commande systématiquement un dessert",
  "se lève le plus tôt le week-end",
  "préfère organiser un voyage surprise",
  "répond le plus vite aux messages",
  "a le meilleur radar à bons plans",
  "propose une soirée jeux spontanée",
  "raconte les meilleures histoires",
  "prend les meilleures photos",
  "a la playlist la plus chill",
  "sait calmer une dispute rapidement",
  "a toujours une idée de cadeau",
  "danse même sans musique",
  "est le/la plus patient(e)",
  "serait le meilleur chef d'un food truck",
  "garde le plus de souvenirs",
  "a le plus d'énergie après minuit"
];

const whoOfUsVariants = [
  "quand on sort en ville",
  "pendant les vacances",
  "les jours de pluie",
  "en mode cosy"
];

const whoOfUs = sliceTo(
  buildVariants(whoOfUsBase, whoOfUsVariants, (base, variant) => `Qui de nous deux ${base} ${variant} ?`),
  60
);

const preferPairs = [
  ["un réveil avec petit déjeuner au lit", "un dîner surprise maison"],
  ["un road trip improvisé", "un week-end cocooning"],
  ["une soirée jeux de société", "une soirée karaoké"],
  ["un message vocal mignon", "une note manuscrite"],
  ["un pique-nique au parc", "un brunch en terrasse"],
  ["danser dans le salon", "chanter dans la voiture"],
  ["un film romantique", "une comédie légère"],
  ["un match d'impro", "un concert intimiste"],
  ["une balade de nuit", "une balade au lever du soleil"],
  ["un défi cuisine", "un défi photo"],
  ["un café en amoureux", "un chocolat chaud en pyjama"],
  ["une séance massage", "une séance yoga duo"],
  ["un souvenir vidéo", "un album photo"],
  ["planifier un projet fou", "se laisser porter"],
  ["un cadeau utile", "un cadeau symbolique"],
  ["une escapade nature", "une escapade urbaine"],
  ["un jeu de questions", "un jeu d'indices"],
  ["un message spontané", "un message bien pensé"],
  ["un dessert chocolaté", "un dessert fruité"],
  ["une soirée sans téléphone", "une soirée avec playlist"],
  ["un défi sportif doux", "un défi créatif"],
  ["un plan long terme", "un plan spontané"],
  ["une playlist chill", "une playlist dansante"],
  ["un souvenir d'enfance partagé", "un souvenir récent"],
  ["un cadeau fait main", "un cadeau choisi ensemble"],
  ["un brunch tardif", "un dîner tôt"],
  ["une aventure gourmande", "une aventure artistique"],
  ["un jeu de rôles", "un jeu de devinettes"],
  ["un moment silencieux", "un moment très bavard"],
  ["une journée sans plan", "une journée ultra organisée"],
  ["un message codé", "un message direct"],
  ["un voyage proche", "un voyage lointain"],
  ["un marathon séries", "un marathon films"],
  ["une mission surprise", "une mission choisie"],
  ["un cadeau souvenir", "un cadeau expérience"],
  ["un restaurant habituel", "une nouvelle adresse"],
  ["une nuit blanche à discuter", "une nuit très reposante"],
  ["un cours ensemble", "un atelier maison"],
  ["un selfie fun", "une photo posée"],
  ["un défi de mémoire", "un défi d'imagination"]
];

const preferVariants = ["Version douce :", "Version fun :"];
const wouldYouRather = sliceTo(
  preferPairs.flatMap((pair, idx) =>
    preferVariants.map((variant) => `${variant} Tu préfères ${pair[0]} ou ${pair[1]} ?`)
  ),
  80
);

const truthBase = [
  "un moment où tu t'es senti(e) ultra fier(ère) de l'autre",
  "un souvenir de notre première rencontre",
  "une habitude de l'autre que tu trouves adorable",
  "une chose que tu veux apprendre avec l'autre en 2025",
  "un moment où tu as vraiment ri grâce à l'autre",
  "une petite attention que tu aimerais recevoir",
  "un rêve de voyage à deux",
  "une chanson qui te fait penser à nous",
  "un trait de caractère que tu admires chez l'autre",
  "un petit rituel que tu aimerais créer",
  "un compliment que tu n'oses pas toujours dire",
  "un défi personnel que l'autre pourrait t'aider à relever",
  "un souvenir qui te rassure quand tu doutes",
  "un message que tu aimerais recevoir ce soir",
  "un moment où tu t'es senti(e) compris(e)",
  "un moment où tu as appris quelque chose sur l'autre",
  "une surprise que tu aimerais organiser",
  "un souvenir de vacances préféré",
  "un moment où tu as été touché(e) par l'autre",
  "une chose que tu veux célébrer ensemble"
];

const truthVariants = [
  "Raconte",
  "Explique",
  "Partage",
  "Décris",
  "Raconte en 3 phrases",
  "Raconte avec un exemple"
];

const truths = sliceTo(
  buildVariants(truthBase, truthVariants, (base, variant) => `${variant} : ${base}.`),
  60
);

const dareBase = [
  "imite la façon dont l'autre dit bonjour",
  "fais un mini discours d'amour de 20 secondes",
  "donne trois compliments rapides",
  "mime votre premier rendez-vous",
  "fais un high five + câlin",
  "raconte une blague nulle",
  "décris l'autre comme une météo",
  "fais une déclaration comme dans un film",
  "invente un surnom et explique pourquoi",
  "fais une pose photo de couple",
  "improvise un micro poème",
  "prépare un plan de soirée en 30 secondes",
  "fais un battle de regards",
  "décris un souvenir sans dire l'année",
  "fais un compliment en rime",
  "propose un mini toast de nouvel an",
  "fais un mini reportage sur l'autre",
  "mime un fou rire",
  "propose un défi doux",
  "fais un petit massage de 10 secondes"
];

const dareVariants = [
  "Tout de suite",
  "Version douce",
  "Mode express"
];

const dares = sliceTo(
  buildVariants(dareBase, dareVariants, (base, variant) => `${variant} : ${base}.`),
  60
);

const absurdThemes = [
  "les chaussettes dépareillées", "les câlins de 3 secondes", "les playlists sans skip",
  "les réveils musicaux", "les pizzas en cœur", "les emojis préférés", "les selfies ratés",
  "les goûts bizarres de glace", "les plantes d'intérieur", "les soirées quiz", "les chaussons moelleux"
];
const absurdAngles = [
  "devraient être obligatoires", "sont sous-estimés", "sont totalement surestimés", "devraient être classés patrimoine du couple"
];
const absurdDebates = sliceTo(
  buildVariants(absurdThemes, absurdAngles, (theme, angle) => `Débat : ${theme} ${angle}.`),
  44
).slice(0, 40);

const improvRoles = [
  "un(e) chef(fe) pâtissier(ère)", "un(e) DJ romantique", "un(e) détective du quotidien", "un(e) explorateur(trice)",
  "un(e) coach motivation", "un(e) astronaute", "un(e) chef(fe) de projet", "un(e) influenceur(se) cuisine",
  "un(e) bibliothécaire curieux(se)", "un(e) scénariste"
];
const improvPlaces = [
  "dans un train de nuit", "sur un rooftop", "dans une cabane", "au marché", "dans un musée",
  "dans un ascenseur", "dans une fête de village", "dans une salle d'attente", "dans un studio photo", "sur une plage"
];
const improvTwists = [
  "un secret doit être révélé", "quelqu'un a oublié son prénom", "un cadeau surprise apparaît",
  "il faut parler uniquement en questions", "vous êtes pressés par un minuteur",
  "une chanson imaginaire commence", "vous devez parler comme dans un film noir",
  "une pluie de confettis tombe", "vous venez de gagner un prix",
  "un animal imaginaire intervient"
];

const improvCards = sliceTo(
  improvRoles.flatMap((role) =>
    improvPlaces.flatMap((place) =>
      improvTwists.map((twist) => ({ role, place, twist }))
    )
  ),
  50
);

const minuteBase = [
  "réciter l'alphabet à l'envers en alternant", "faire 20 pas synchronisés", "lister 10 souvenirs",
  "faire 10 compliments sans répéter", "trouver 12 mots doux", "inventer un slogan de couple",
  "trouver 8 destinations", "mimer 6 émotions", "citer 10 chansons", "faire une mini danse",
  "trouver 15 mots qui riment", "proposer 6 activités", "faire un duel de regards",
  "écrire 3 objectifs sur un papier", "faire une statue vivante", "raconter 5 blagues",
  "faire un jeu de mains", "imiter 4 personnages", "trouver 7 dates importantes", "faire 12 high-fives"
];

const minuteVariants = ["pendant 60 secondes", "sans rire", "en rythme"];
const minuteChallenges = sliceTo(
  buildVariants(minuteBase, minuteVariants, (base, variant) => `Défi 1 minute : ${base} ${variant}.`),
  50
);

const emotionList = [
  "joie", "tendresse", "fierté", "nostalgie", "excitation", "apaisement", "surprise", "curiosité",
  "confiance", "admiration", "gratitude", "complicité", "motivation", "chaleur", "sérénité", "espièglerie",
  "espoir", "rassurance", "douceur", "émerveillement"
];
const emotionPhrases = [
  "Quand tu m'écoutes vraiment.", "Quand on planifie un projet ensemble.", "Quand on se retrouve après une journée chargée.",
  "Quand on rit des mêmes détails.", "Quand on partage un souvenir.", "Quand on improvise." 
];
const emotions = sliceTo(
  buildVariants(emotionList, emotionPhrases, (emotion, phrase) => ({ emotion, phrase })),
  40
);

const prioritySituations = [
  "Un samedi libre", "Un budget cadeau limité", "Une soirée spéciale", "Un dimanche matin",
  "Un voyage de 3 jours", "Un mois chargé", "Un anniversaire surprise", "Un week-end pluvieux",
  "Une nouvelle tradition", "Une semaine stressante"
];
const priorityChoices = [
  ["repos total", "activité créative", "sortie extérieure", "moment surprise"],
  ["cuisine ensemble", "jeu rapide", "film + plaid", "balade courte"],
  ["planifier l'année", "faire une pause", "défi fun", "moment romantique"],
  ["brunch maison", "marché local", "sport doux", "sieste duo"],
  ["aventure nature", "découverte urbaine", "route gourmande", "séjour slow"],
  ["discussion profonde", "jeu léger", "activité manuelle", "moment silence"],
  ["cadeau fait main", "cadeau utile", "cadeau expérience", "cadeau surprise"],
  ["playlist ensemble", "atelier cuisine", "album photo", "plan projet"],
  ["message surprise", "rituel quotidien", "défi mensuel", "moment improvisé"],
  ["souvenir partagé", "objectif perso", "objectif couple", "objectif fun"]
];
const priorities = sliceTo(
  prioritySituations.flatMap((situation, idx) =>
    priorityChoices.map((choices) => ({ situation, choices }))
  ),
  40
);

const promiseBase = [
  "organiser une micro-surprise par mois",
  "avoir un moment sans écrans par semaine",
  "tester une nouvelle recette ensemble",
  "faire une mini balade chaque dimanche",
  "prendre une photo souvenir chaque mois",
  "faire un check-in émotions chaque semaine",
  "planifier un voyage même court",
  "dire un compliment sincère par jour",
  "garder un carnet de souvenirs",
  "soutenir un défi perso de l'autre",
  "apprendre une nouvelle chose ensemble",
  "avoir un rituel de fin de journée",
  "faire une activité sportive douce ensemble",
  "organiser une soirée jeux",
  "déclarer une gratitude quotidienne",
  "essayer un hobby nouveau",
  "préparer une playlist commune",
  "faire une liste de projets",
  "planifier un week-end surprise",
  "garder un moment de calme partagé"
];
const promiseVariants = ["Promesse 2025 :", "Engagement doux :"];
const promises = sliceTo(
  buildVariants(promiseBase, promiseVariants, (base, variant) => `${variant} ${base}.`),
  40
);

const voicesBase = [
  "narrateur de documentaire", "vendeur(se) de marché", "astronaute", "prof de yoga",
  "guide touristique", "commentateur sportif", "agent secret", "chef(fe) étoilé(e)",
  "animateur radio", "magicien(ne)", "hôtesse de l'air", "pirate sympa",
  "robot bienveillant", "journaliste météo", "poète", "coach de vie",
  "capitaine de navire", "gardien(ne) de musée", "bibliothécaire", "éclaireur(se)"
];
const voicesStyles = ["hyper enthousiaste", "super calme"];
const voices = sliceTo(
  buildVariants(voicesBase, voicesStyles, (base, variant) => `${base} ${variant}`),
  40
);

const memoriesBase = [
  "Quel est ton premier souvenir clair avec moi ?",
  "Quel moment t'a fait dire " + '"cette personne compte"' + " ?",
  "Quel fou rire récent te revient ?",
  "Quelle petite habitude de couple est née sans qu'on le décide ?",
  "Quel est ton souvenir préféré de nos vacances ?",
  "Quel moment t'a surpris chez moi ?",
  "Quel plat partagé te rappelle un bon moment ?",
  "Quel trajet ensemble t'a marqué ?",
  "Quel moment simple t'a fait du bien ?",
  "Quel message de moi t'a touché(e) ?",
  "Quel moment de soutien t'a rassuré(e) ?",
  "Quel souvenir d'hiver te réchauffe ?",
  "Quel moment a changé notre routine ?",
  "Quel petit geste te fait sourire ?",
  "Quel moment de silence était parfait ?",
  "Quel événement t'a rendu fier(ère) de nous ?",
  "Quel moment d'impro t'a fait rire ?",
  "Quel moment a renforcé notre complicité ?",
  "Quel souvenir spontané t'a marqué ?",
  "Quel détail de notre début te plaît encore ?",
  "Quel moment tu raconterais à un ami ?",
  "Quelle photo raconte bien notre histoire ?",
  "Quel moment de cuisine a été mémorable ?",
  "Quelle soirée te ressemble ?",
  "Quel moment te rappelle pourquoi on s'aime ?"
];
const memoriesVariants = ["Version courte", "Version détaillée"];
const memories = sliceTo(
  buildVariants(memoriesBase, memoriesVariants, (base, variant) => `${variant} : ${base}`),
  50
);

const coupleQuizBase = [
  "Quel est le snack préféré de l'autre ?",
  "Quelle chanson l'autre met pour se motiver ?",
  "Quel est son endroit préféré pour se ressourcer ?",
  "Quelle est sa saison favorite ?",
  "Quel compliment il/elle préfère entendre ?",
  "Quel est son rituel du matin ?",
  "Quel est son film doudou ?",
  "Quel est son confort food ?",
  "Quel est son petit plaisir secret ?",
  "Quel objet il/elle garde toujours près de lui/elle ?",
  "Quel est son geste de tendresse préféré ?",
  "Quel type de sortie le/la rend heureux(se) ?",
  "Quel est son super-pouvoir social ?",
  "Quel est son talent caché ?",
  "Quelle phrase le/la motive ?",
  "Quel est son endroit rêvé ?",
  "Quel est son dessert favori ?",
  "Quelle couleur lui va le mieux ?",
  "Quel est son moment préféré de la journée ?",
  "Quelle boisson le/la réconforte ?",
  "Quel est son rituel du soir ?",
  "Quelle activité lui donne le sourire ?",
  "Quelle odeur lui rappelle l'enfance ?",
  "Quel cadeau le/la surprendrait ?",
  "Quelle série il/elle peut revoir ?"
];
const coupleQuizVariants = ["Quiz duo", "Quiz express"];
const coupleQuiz = sliceTo(
  buildVariants(coupleQuizBase, coupleQuizVariants, (base, variant) => `${variant} : ${base}`),
  50
);

const secretsBase = [
  "un petit rêve perso que tu n'as jamais dit",
  "un endroit que tu aimerais découvrir avec moi",
  "un moment où tu t'es senti(e) très proche",
  "une qualité de l'autre que tu sous-estimes",
  "un geste qui te rassure",
  "une chanson qui te fait penser à nous",
  "un moment où tu voulais dire je t'aime",
  "un détail qui te touche",
  "un souvenir que tu gardes pour toi",
  "un rituel que tu aimerais créer",
  "un compliment que tu penses souvent",
  "un objectif secret pour l'année",
  "un moment où tu as été impressionné(e)",
  "un message que tu gardes",
  "un petit rêve de couple",
  "un moment où tu as voulu plus de temps",
  "un moment où tu t'es senti(e) aligné(e)",
  "un petit bonheur que tu n'oses pas demander",
  "une activité que tu veux tester",
  "un geste qui te fait fondre"
];
const secretsVariants = ["Secret doux :", "Petit aveu :"];
const secretsSoft = sliceTo(
  buildVariants(secretsBase, secretsVariants, (base, variant) => `${variant} ${base}.`),
  40
);

const creativityBase = [
  "invente un slogan pour votre couple",
  "crée un nom de mission secrète",
  "imagine un logo pour votre duo",
  "propose une affiche de film sur votre histoire",
  "inventez un rituel de nouvel an",
  "imagine un message codé",
  "crée un menu 3 services spécial couple",
  "inventez une tradition du dimanche",
  "imagine un voyage fictif",
  "crée une carte postale",
  "imagine une playlist en 3 mots",
  "inventez un défi photo",
  "crée un bingo couple",
  "imagine une devise",
  "inventez une fête perso",
  "crée un jeu de société rapide",
  "imagine un souvenir futur",
  "crée un toast",
  "inventez un hashtag",
  "crée une mini-histoire"
];
const creativityVariants = ["Battle express :", "Battle fun :"];
const creativityBattle = sliceTo(
  buildVariants(creativityBase, creativityVariants, (base, variant) => `${variant} ${base}.`),
  40
);

const planYearBase = [
  "un mini-voyage à planifier",
  "une compétence à apprendre ensemble",
  "une habitude positive",
  "un projet créatif",
  "une routine bien-être",
  "un moment mensuel",
  "un défi sportif doux",
  "une tradition de couple",
  "un objectif fun",
  "un projet gourmand",
  "une routine de gratitude",
  "une escapade surprise",
  "un challenge photo",
  "une activité culturelle",
  "une soirée dédiée",
  "un rituel matinal",
  "un projet solidaire",
  "un mini rêve",
  "un objectif d'organisation",
  "un souvenir à créer"
];
const planYearVariants = ["Plan 2025 :", "Idée à programmer :"];
const planYear = sliceTo(
  buildVariants(planYearBase, planYearVariants, (base, variant) => `${variant} ${base}.`),
  40
);

const guessOtherBase = [
  "Quel est ton endroit feel-good ?",
  "Quel cadeau surprise te ferait sourire ?",
  "Quel est ton moment préféré avec moi ?",
  "Quelle est ta phrase d'encouragement favorite ?",
  "Quel est ton confort food du moment ?",
  "Quel type de musique te booste ?",
  "Quel est ton souvenir de couple préféré ?",
  "Quel est ton plan parfait pour samedi ?",
  "Quelle est ta boisson doudou ?",
  "Quel est ton rituel du soir ?",
  "Quel est ton mot du moment ?",
  "Quel est ton type de vacances ?",
  "Quel est ton cadeau idéal ?",
  "Quel est ton spot nature préféré ?",
  "Quel est ton objet porte-bonheur ?",
  "Quel est ton film doudou ?",
  "Quelle est ta meilleure playlist ?",
  "Quel est ton moment de la journée préféré ?",
  "Quel est ton plat à partager préféré ?",
  "Quel est ton mini-rêve 2025 ?"
];
const guessOtherVariants = ["Devine :", "Double devinette :"];
const guessOther = sliceTo(
  buildVariants(guessOtherBase, guessOtherVariants, (base, variant) => `${variant} ${base}`),
  40
);

const whoLiesBase = [
  "J'ai déjà oublié notre premier rendez-vous.",
  "J'ai déjà chanté faux en public.",
  "J'ai déjà fait un faux rire en famille.",
  "J'ai déjà envoyé un message au mauvais contact.",
  "J'ai déjà planifié une surprise.",
  "J'ai déjà dormi devant un film romantique.",
  "J'ai déjà oublié une date importante.",
  "J'ai déjà écrit une lettre d'amour.",
  "J'ai déjà inventé une excuse mignonne.",
  "J'ai déjà fait un compliment en rime.",
  "J'ai déjà improvisé un rendez-vous.",
  "J'ai déjà eu un fou rire en réunion.",
  "J'ai déjà mangé le dernier morceau.",
  "J'ai déjà fait un selfie raté.",
  "J'ai déjà oublié mes clés.",
  "J'ai déjà fait une playlist secrète.",
  "J'ai déjà pris trop de photos.",
  "J'ai déjà lu un horoscope amoureux.",
  "J'ai déjà préparé un petit déjeuner surprise.",
  "J'ai déjà fait un plan trop ambitieux."
];
const whoLiesVariants = ["Vérité ou mensonge ?", "Qui ment ?"];
const whoLies = sliceTo(
  buildVariants(whoLiesBase, whoLiesVariants, (base, variant) => `${variant} ${base}`),
  40
);

const blindTestBase = [
  "Décris un film sans dire le titre.",
  "Décris une chanson sans dire les mots.",
  "Décris un lieu de rendez-vous idéal.",
  "Décris une série en 3 indices.",
  "Décris un souvenir en une métaphore.",
  "Décris un dessert sans le nommer.",
  "Décris un objet du quotidien sans le dire.",
  "Décris un jeu de société sans son nom.",
  "Décris une émotion avec un paysage.",
  "Décris une activité du week-end.",
  "Décris un personnage célèbre.",
  "Décris une appli sans l'indiquer.",
  "Décris un sport sans le nom.",
  "Décris une boisson.",
  "Décris une couleur avec des objets.",
  "Décris une tenue idéale.",
  "Décris un moment de la journée.",
  "Décris une saison.",
  "Décris une fête.",
  "Décris un animal de compagnie."
];
const blindTestVariants = ["Blind test parlé :", "Décris sans dire :"];
const blindTest = sliceTo(
  buildVariants(blindTestBase, blindTestVariants, (base, variant) => `${variant} ${base}`),
  40
);

const complimentsBase = [
  "Fais un compliment sur le style de l'autre.",
  "Complimente sa façon de parler.",
  "Complimente son énergie.",
  "Complimente son sens de l'humour.",
  "Complimente sa patience.",
  "Complimente son regard.",
  "Complimente son écoute.",
  "Complimente sa créativité.",
  "Complimente son organisation.",
  "Complimente sa gentillesse.",
  "Complimente sa capacité à rassurer.",
  "Complimente un souvenir partagé.",
  "Complimente sa façon de sourire.",
  "Complimente sa manière de rêver.",
  "Complimente sa capacité à surprendre.",
  "Complimente sa détermination.",
  "Complimente un talent caché.",
  "Complimente son optimisme.",
  "Complimente sa douceur.",
  "Complimente son esprit d'équipe."
];
const complimentsVariants = ["Compliment forcé :", "Mode douceur :"];
const compliments = sliceTo(
  buildVariants(complimentsBase, complimentsVariants, (base, variant) => `${variant} ${base}`),
  40
);

const trialCasesBase = [
  "Avoir mangé le dernier biscuit.",
  "Avoir oublié un message important.",
  "Avoir monopoliser la couette.",
  "Avoir laissé traîner une tasse.",
  "Avoir chanté trop fort.",
  "Avoir pris trop de place sur le canapé.",
  "Avoir oublié la liste de courses.",
  "Avoir fait une blague trop nulle.",
  "Avoir tardé à répondre.",
  "Avoir insisté pour un film en particulier.",
  "Avoir mis la mauvaise playlist.",
  "Avoir mis du temps à se préparer.",
  "Avoir choisi une mauvaise série.",
  "Avoir mangé plus de frites.",
  "Avoir mis un réveil trop tôt."
];
const trialCases = sliceTo(
  buildVariants(trialCasesBase, ["Procès", "Tribunal"], (base, variant) => `${variant} du couple : ${base}`),
  30
);

const DATA = {
  categories: ["Fun", "Romance", "Défis", "Parole"],
  games: [
    {
      id: "1",
      title: "Qui de nous deux ?",
      category: "Fun",
      description: "Comparez vos traits avec humour.",
      rules: [
        "Lisez la question.",
        "Comptez 3, pointez la personne.",
        "Celui/celle qui reçoit le plus de points gagne le tour."
      ],
      dataKey: "whoOfUs",
      hasTimer: false
    },
    {
      id: "2",
      title: "Vérité ou gage intelligent",
      category: "Parole",
      description: "Des vérités soft et des gages doux.",
      rules: [
        "Choisissez Vérité ou Gage.",
        "Répondez avec sincérité.",
        "Pas de malaise, on reste bienveillant."
      ],
      dataKey: "truthOrDare",
      hasTimer: false
    },
    {
      id: "3",
      title: "Le jeu des souvenirs",
      category: "Romance",
      description: "Réveillez les moments forts.",
      rules: [
        "Tirez une question souvenir.",
        "Racontez en détails.",
        "L'autre ajoute un souvenir complémentaire."
      ],
      dataKey: "memories",
      hasTimer: false
    },
    {
      id: "4",
      title: "Devine la réponse de l'autre",
      category: "Fun",
      description: "Testez votre télépathie.",
      rules: [
        "L'un répond mentalement.",
        "L'autre devine.",
        "Comparez et marquez un point si c'est juste."
      ],
      dataKey: "guessOther",
      hasTimer: false
    },
    {
      id: "5",
      title: "Débat absurde",
      category: "Fun",
      description: "Défendez des idées improbables.",
      rules: [
        "Tirez un sujet absurde.",
        "1 minute pour préparer vos arguments.",
        "Le public (vous deux) vote le gagnant."
      ],
      dataKey: "absurdDebates",
      hasTimer: true
    },
    {
      id: "7",
      title: "Quiz de couple personnalisé",
      category: "Parole",
      description: "Montrez que vous vous connaissez.",
      rules: [
        "Répondez chacun de votre côté.",
        "Comparez les réponses.",
        "Un point si vous tombez d'accord."
      ],
      dataKey: "coupleQuiz",
      hasTimer: false
    },
    {
      id: "8",
      title: "Le procès",
      category: "Fun",
      description: "Un mini-tribunal ultra drôle.",
      rules: [
        "Tirez une accusation.",
        "Choisissez un juge au hasard.",
        "Le juge tranche ou tire au sort."
      ],
      dataKey: "trialCases",
      hasTimer: false
    },
    {
      id: "9",
      title: "Compliments forcés",
      category: "Romance",
      description: "Des compliments obligatoires.",
      rules: [
        "Tirez une mission de compliment.",
        "Restez sincères et précis.",
        "L'autre répond par un merci ou un bisou."
      ],
      dataKey: "compliments",
      hasTimer: false
    },
    {
      id: "10",
      title: "Tu préfères… version couple",
      category: "Fun",
      description: "Faites des choix impossibles.",
      rules: [
        "Choisissez A ou B.",
        "Expliquez votre choix.",
        "Celui/celle qui convainc gagne le point."
      ],
      dataKey: "wouldYouRather",
      hasTimer: false
    },
    {
      id: "12",
      title: "Impro théâtre",
      category: "Défis",
      description: "Improvisations guidées et fun.",
      rules: [
        "Tirez un rôle, lieu et twist.",
        "Improvisation 45 secondes.",
        "Changez de rôles au prochain tour."
      ],
      dataKey: "improvCards",
      hasTimer: true
    },
    {
      id: "13",
      title: "Blind test parlé",
      category: "Défis",
      description: "Décris sans dire le nom.",
      rules: [
        "Tirez une carte.",
        "Décrivez sans mots interdits.",
        "L'autre doit deviner en 60s."
      ],
      dataKey: "blindTest",
      hasTimer: true
    },
    {
      id: "14",
      title: "Secrets soft",
      category: "Romance",
      description: "Des confidences douces.",
      rules: [
        "Tirez un secret.",
        "Partagez en toute bienveillance.",
        "L'autre répond avec un mot doux."
      ],
      dataKey: "secretsSoft",
      hasTimer: false
    },
    {
      id: "15",
      title: "Challenge 1 minute",
      category: "Défis",
      description: "Des défis express à deux.",
      rules: [
        "Tirez un défi.",
        "Lancez le timer 60s.",
        "Faites équipe pour réussir."
      ],
      dataKey: "minuteChallenges",
      hasTimer: true
    },
    {
      id: "16",
      title: "Qui ment ?",
      category: "Fun",
      description: "Vérité ou mensonge ?",
      rules: [
        "Lisez la phrase.",
        "L'un dit vrai, l'autre ment.",
        "Devinez qui ment."
      ],
      dataKey: "whoLies",
      hasTimer: false
    },
    {
      id: "17",
      title: "Planifier l'année à venir",
      category: "Parole",
      description: "Projets et intentions 2025.",
      rules: [
        "Tirez une idée.",
        "Décidez d'une action concrète.",
        "Notez-la si vous l'aimez."
      ],
      dataKey: "planYear",
      hasTimer: false
    },
    {
      id: "19",
      title: "Devine l'émotion",
      category: "Défis",
      description: "Mettez en scène une émotion.",
      rules: [
        "Tirez une émotion et une phrase.",
        "Jouez-la sans dire l'émotion.",
        "L'autre devine."
      ],
      dataKey: "emotions",
      hasTimer: true
    },
    {
      id: "20",
      title: "Battle de créativité",
      category: "Défis",
      description: "Qui est le plus créatif ?",
      rules: [
        "Tirez une mission créative.",
        "Vous avez 60s chacun.",
        "Votez pour votre favori."
      ],
      dataKey: "creativityBattle",
      hasTimer: true
    },
    {
      id: "21",
      title: "Le jeu des priorités",
      category: "Parole",
      description: "Choisissez ensemble le plus important.",
      rules: [
        "Tirez une situation.",
        "Classez les 4 choix.",
        "Comparez vos priorités."
      ],
      dataKey: "priorities",
      hasTimer: false
    },
    {
      id: "23",
      title: "Promesses 2025",
      category: "Romance",
      description: "Fixez vos engagements doux.",
      rules: [
        "Tirez une promesse.",
        "Adoptez-la ou adaptez-la.",
        "Célébrez si vous êtes d'accord."
      ],
      dataKey: "promises",
      hasTimer: false
    },
    {
      id: "24",
      title: "Le jeu des voix",
      category: "Fun",
      description: "Parlez avec des voix folles.",
      rules: [
        "Tirez un personnage.",
        "Parlez 30s avec cette voix.",
        "L'autre doit deviner le personnage."
      ],
      dataKey: "voices",
      hasTimer: true
    }
  ],
  chatgptGames: [
    {
      id: "6",
      title: "Histoire interactive",
      prompt: "Joue le rôle d'un narrateur interactif. Propose une histoire romantique et pose des choix aux joueurs toutes les 2 minutes."
    },
    {
      id: "11",
      title: "Escape game vocal",
      prompt: "Crée un mini escape game vocal pour deux joueurs, avec énigmes simples et ambiance fun."
    },
    {
      id: "18",
      title: "Scénarios alternatifs",
      prompt: "Invente des scénarios alternatifs de notre histoire et propose des choix humoristiques."
    },
    {
      id: "22",
      title: "Conseiller amoureux",
      prompt: "Agis comme un coach relationnel bienveillant et propose des idées d'amélioration du quotidien en couple."
    },
    {
      id: "25",
      title: "Fin alternative de film",
      prompt: "Donne des fins alternatives drôles à des films romantiques connus, avec choix pour le couple."
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
    emotions,
    priorities,
    promises,
    voices,
    memories,
    coupleQuiz,
    secretsSoft,
    creativityBattle,
    planYear,
    guessOther,
    whoLies,
    blindTest,
    compliments,
    trialCases
  }
};
