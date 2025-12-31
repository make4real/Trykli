/* Trykli SPA logic */
const STORAGE_KEY = "trykli-state";

const state = {
  view: "home",
  filter: "Tous",
  search: "",
  currentGame: null,
  history: [],
  scores: {},
  saved: {
    memories: [],
    planYear: [],
    promises: []
  },
  lastCards: {},
  timer: {
    remaining: 60,
    duration: 60,
    running: false,
    handle: null
  },
  step: {
    guessOther: "A",
    priorities: { stage: "A", choiceA: null },
    emotionRevealed: false
  }
};

const el = {
  homeView: document.getElementById("homeView"),
  gameView: document.getElementById("gameView"),
  siteGameGrid: document.getElementById("siteGameGrid"),
  vocalGameGrid: document.getElementById("vocalGameGrid"),
  filters: document.getElementById("filters"),
  searchInput: document.getElementById("searchInput"),
  gameTitle: document.getElementById("gameTitle"),
  gameMeta: document.getElementById("gameMeta"),
  rulesList: document.getElementById("rulesList"),
  cardDisplay: document.getElementById("cardDisplay"),
  extraDisplay: document.getElementById("extraDisplay"),
  gameControls: document.getElementById("gameControls"),
  historyList: document.getElementById("historyList"),
  savedList: document.getElementById("savedList"),
  scoreA: document.getElementById("scoreA"),
  scoreB: document.getElementById("scoreB"),
  timerLabel: document.getElementById("timerLabel"),
  timerStartBtn: document.getElementById("timerStartBtn"),
  timerStopBtn: document.getElementById("timerStopBtn"),
  timerResetBtn: document.getElementById("timerResetBtn")
};

const resetControlsContainer = () => {
  const fresh = el.gameControls.cloneNode(false);
  fresh.id = el.gameControls.id;
  el.gameControls.replaceWith(fresh);
  el.gameControls = fresh;
};

const saveState = () => {
  localStorage.setItem(
    STORAGE_KEY,
    JSON.stringify({
      filter: state.filter,
      search: state.search,
      scores: state.scores,
      saved: state.saved
    })
  );
};

const loadState = () => {
  const saved = localStorage.getItem(STORAGE_KEY);
  if (!saved) return;
  try {
    const parsed = JSON.parse(saved);
    state.filter = parsed.filter || "Tous";
    state.search = parsed.search || "";
    state.scores = parsed.scores || {};
    state.saved = parsed.saved || state.saved;
  } catch (error) {
    console.warn("Trykli: impossible de charger le state", error);
  }
};

const setTimerDuration = (seconds) => {
  state.timer.duration = seconds;
  state.timer.remaining = seconds;
  el.timerLabel.textContent = `${seconds}s`;
};

const stopTimer = () => {
  state.timer.running = false;
  clearInterval(state.timer.handle);
  state.timer.handle = null;
};

const startTimer = () => {
  stopTimer();
  state.timer.running = true;
  state.timer.remaining = state.timer.duration;
  el.timerLabel.textContent = `${state.timer.remaining}s`;
  state.timer.handle = setInterval(() => {
    state.timer.remaining -= 1;
    el.timerLabel.textContent = `${state.timer.remaining}s`;
    if (state.timer.remaining <= 0) {
      stopTimer();
      el.timerLabel.textContent = "Terminé";
    }
  }, 1000);
};

const resetTimer = () => {
  stopTimer();
  setTimerDuration(state.timer.duration);
};

const updateScores = () => {
  const score = state.scores[state.currentGame?.id] || { A: 0, B: 0 };
  el.scoreA.textContent = score.A;
  el.scoreB.textContent = score.B;
};

const updateHistory = (text) => {
  state.history.unshift(text);
  state.history = state.history.slice(0, 10);
  el.historyList.innerHTML = state.history.map((item) => `<li>${item}</li>`).join("");
};

const updateSavedList = (gameType) => {
  if (gameType === "memories") {
    el.savedList.textContent = `Souvenirs sauvegardés : ${state.saved.memories.length}`;
  } else if (gameType === "planYear") {
    el.savedList.textContent = `Plans gardés : ${state.saved.planYear.length}`;
  } else if (gameType === "promises") {
    el.savedList.textContent = `Promesses gardées : ${state.saved.promises.length}`;
  } else {
    el.savedList.textContent = "";
  }
};

const getRandomItem = (arr) => arr[Math.floor(Math.random() * arr.length)];

const preventRepeat = (gameId, candidateText) => {
  const last = state.lastCards[gameId];
  if (last && last === candidateText) return false;
  return true;
};

const drawUnique = (gameId, pool, formatter) => {
  let attempt = 0;
  while (attempt < 8) {
    const item = getRandomItem(pool);
    const text = formatter(item);
    if (preventRepeat(gameId, text)) {
      state.lastCards[gameId] = text;
      return { item, text };
    }
    attempt += 1;
  }
  const fallback = formatter(pool[0]);
  state.lastCards[gameId] = fallback;
  return { item: pool[0], text: fallback };
};

const renderHome = () => {
  state.view = "home";
  el.homeView.classList.add("active");
  el.gameView.classList.remove("active");
  el.searchInput.value = state.search;
  renderGameGrids();
};

const renderGameGrids = () => {
  const term = state.search.toLowerCase();
  const filter = state.filter;

  const filterMatch = (game) => {
    if (filter === "Tous") return true;
    return game.tags.includes(filter);
  };

  const searchMatch = (game) => {
    const haystack = `${game.name} ${game.description} ${game.tags.join(" ")}`.toLowerCase();
    return haystack.includes(term);
  };

  const renderCards = (list, target) => {
    target.innerHTML = list
      .filter((game) => filterMatch(game) && searchMatch(game))
      .map((game) => {
        const tags = game.tags.map((tag) => `<span class="tag">${tag}</span>`).join("");
        return `
          <article class="card-tile">
            <div class="tags">${tags}</div>
            <h3>${game.id}. ${game.name}</h3>
            <p>${game.description}</p>
            <div class="duration">Durée : ${game.duration}</div>
            <button class="primary" data-game="${game.id}" data-source="${game.source}">Ouvrir</button>
          </article>
        `;
      })
      .join("");
  };

  const siteGames = DATA.siteGames.map((game) => ({ ...game, source: "site" }));
  const vocalGames = DATA.vocalGames.map((game) => ({ ...game, source: "vocal" }));

  renderCards(siteGames, el.siteGameGrid);
  renderCards(vocalGames, el.vocalGameGrid);
};

const renderFilters = () => {
  const filters = ["Tous", ...DATA.tags];
  el.filters.innerHTML = filters
    .map((filter) => {
      const cls = filter === state.filter ? "primary" : "ghost";
      return `<button class="${cls}" data-filter="${filter}">${filter}</button>`;
    })
    .join("");
};

const setGameView = (game, source) => {
  state.view = "game";
  state.currentGame = { ...game, source };
  state.history = [];
  state.step = {
    guessOther: "A",
    priorities: { stage: "A", choiceA: null },
    emotionRevealed: false
  };
  el.historyList.innerHTML = "";
  el.homeView.classList.remove("active");
  el.gameView.classList.add("active");
  el.gameTitle.textContent = `${game.id}. ${game.name}`;
  el.gameMeta.textContent = `${game.tags.join(" · ")} · ${game.duration}`;
  el.rulesList.innerHTML = game.rules.map((rule) => `<li>${rule}</li>`).join("");
  el.extraDisplay.textContent = "";
  updateScores();
  setTimerDuration(60);
  buildControls();
  renderCard();
};

const renderCard = () => {
  const game = state.currentGame;
  if (!game) return;

  if (game.source === "vocal") {
    el.cardDisplay.textContent = game.text;
    updateHistory("Texte vocal prêt à copier");
    updateSavedList(null);
    return;
  }

  let result;
  switch (game.type) {
    case "who":
      result = drawUnique(game.id, DATA.data.whoOfUs, (item) => item);
      break;
    case "truthOrDare":
      el.extraDisplay.textContent = "Choisissez Vérité ou Gage.";
      result = { text: "Choisissez un mode pour tirer une carte.", item: null };
      break;
    case "memories":
      result = drawUnique(game.id, DATA.data.memories, (item) => item);
      break;
    case "guessOther":
      result = drawUnique(game.id, DATA.data.guessOther, (item) => item);
      break;
    case "debate":
      result = drawUnique(game.id, DATA.data.absurdDebates, (item) => item);
      break;
    case "coupleQuiz":
      result = drawUnique(game.id, DATA.data.coupleQuiz, (item) => item);
      break;
    case "trial":
      result = drawUnique(game.id, DATA.data.trialCases, (item) => item);
      break;
    case "compliments":
      result = drawUnique(game.id, DATA.data.compliments, (item) => item);
      break;
    case "wouldYouRather": {
      result = drawUnique(game.id, DATA.data.wouldYouRather, (item) => item);
      break;
    }
    case "improv": {
      const { item, text } = drawUnique(game.id, DATA.data.improvCards, (card) =>
        `Rôle A : ${card.roleA}\nRôle B : ${getRandomItem(DATA.data.improvCards).roleA}\nLieu : ${card.place}\nTwist : ${card.twist}`
      );
      result = { item, text };
      break;
    }
    case "blindTest":
      result = drawUnique(game.id, DATA.data.blindTest, (item) => item.subject);
      break;
    case "secrets":
      result = drawUnique(game.id, DATA.data.secretsSoft, (item) => item);
      break;
    case "minuteChallenge":
      result = drawUnique(game.id, DATA.data.minuteChallenges, (item) => item);
      setTimerDuration(60);
      startTimer();
      break;
    case "whoLies":
      result = drawUnique(game.id, DATA.data.whoLies, (item) => `Phrase 1 : ${item.phraseA}\nPhrase 2 : ${item.phraseB}`);
      break;
    case "planYear":
      result = drawUnique(game.id, DATA.data.planYear, (item) => `${item.category} : ${item.idea}`);
      break;
    case "emotion":
      result = drawUnique(game.id, DATA.data.emotionCards, (item) => `Phrase : ${item.phrase}`);
      state.step.emotionRevealed = false;
      break;
    case "creativity":
      result = drawUnique(game.id, DATA.data.creativityBattle, (item) => item);
      break;
    case "priorities":
      result = drawUnique(game.id, DATA.data.priorities, (item) => `Situation : ${item.situation}\nChoix : ${item.choices.join(" · ")}`);
      state.step.priorities = { stage: "A", choiceA: null };
      break;
    case "promises":
      result = drawUnique(game.id, DATA.data.promises, (item) => item);
      break;
    case "voices":
      result = drawUnique(game.id, DATA.data.voices, (item) => `Voix : ${item.role}\nPhrase : ${item.line}`);
      break;
    default:
      result = { text: "", item: null };
  }

  el.cardDisplay.textContent = result.text;
  if (result.text) updateHistory(result.text.replace(/\n/g, " — "));
  updateSavedList(game.type);
};

const buildControls = () => {
  const game = state.currentGame;
  resetControlsContainer();
  el.gameControls.innerHTML = "";
  el.extraDisplay.textContent = "";
  updateSavedList(game.type);

  if (game.source === "vocal") {
    el.gameControls.innerHTML = `
      <button class="primary" id="copyVocalBtn">Copier le texte</button>
      <p class="extra">Ouvre ChatGPT sur ton téléphone en mode vocal et lis ce texte.</p>
    `;
    document.getElementById("copyVocalBtn").addEventListener("click", () => {
      navigator.clipboard.writeText(game.text);
      el.extraDisplay.textContent = "Texte copié !";
    });
    return;
  }

  switch (game.type) {
    case "who":
      el.gameControls.innerHTML = `
        <div class="controls-row">
          <button class="primary" data-choice="A">A</button>
          <button class="primary" data-choice="B">B</button>
          <button class="ghost" data-choice="egalite">Égalité</button>
        </div>
        <button class="ghost" id="randomChoiceBtn">Tirage au sort</button>
      `;
      el.gameControls.addEventListener("click", (event) => {
        const btn = event.target.closest("button[data-choice]");
        if (!btn) return;
        const choice = btn.dataset.choice;
        if (choice === "A" || choice === "B") {
          updateScore(choice, "plus");
        }
        el.extraDisplay.textContent = `Choix : ${choice.toUpperCase()}`;
      });
      document.getElementById("randomChoiceBtn").addEventListener("click", () => {
        const pick = Math.random() > 0.5 ? "A" : "B";
        updateScore(pick, "plus");
        el.extraDisplay.textContent = `Tirage : ${pick}`;
      });
      break;
    case "truthOrDare":
      el.gameControls.innerHTML = `
        <div class="controls-row">
          <button class="primary" id="truthBtn">Vérité</button>
          <button class="primary" id="dareBtn">Gage</button>
        </div>
        <div class="controls-row">
          <button class="ghost" data-success="A">Réussi A</button>
          <button class="ghost" data-success="B">Réussi B</button>
          <button class="ghost" data-success="fail">Raté</button>
        </div>
      `;
      document.getElementById("truthBtn").addEventListener("click", () => {
        const result = drawUnique(game.id, DATA.data.truths, (item) => item);
        el.cardDisplay.textContent = result.text;
        updateHistory(result.text);
        el.extraDisplay.textContent = "Mode Vérité";
      });
      document.getElementById("dareBtn").addEventListener("click", () => {
        const result = drawUnique(game.id, DATA.data.dares, (item) => item);
        el.cardDisplay.textContent = result.text;
        updateHistory(result.text);
        el.extraDisplay.textContent = "Mode Gage (timer 60s)";
        setTimerDuration(60);
        startTimer();
      });
      el.gameControls.addEventListener("click", (event) => {
        const btn = event.target.closest("button[data-success]");
        if (!btn) return;
        const value = btn.dataset.success;
        if (value === "A" || value === "B") updateScore(value, "plus");
      });
      break;
    case "memories":
      el.gameControls.innerHTML = `
        <div class="controls-row">
          <button class="ghost" id="toggleTurnBtn">Tour par tour : Off</button>
          <button class="ghost" id="nextPlayerBtn">Changer joueur</button>
        </div>
        <button class="primary" id="saveMemoryBtn">Sauver ce souvenir</button>
      `;
      let turnMode = false;
      let currentPlayer = "A";
      document.getElementById("toggleTurnBtn").addEventListener("click", (event) => {
        turnMode = !turnMode;
        event.target.textContent = `Tour par tour : ${turnMode ? "On" : "Off"}`;
        el.extraDisplay.textContent = turnMode ? `Tour de ${currentPlayer}` : "";
      });
      document.getElementById("nextPlayerBtn").addEventListener("click", () => {
        currentPlayer = currentPlayer === "A" ? "B" : "A";
        if (turnMode) el.extraDisplay.textContent = `Tour de ${currentPlayer}`;
      });
      document.getElementById("saveMemoryBtn").addEventListener("click", () => {
        state.saved.memories.push(el.cardDisplay.textContent);
        saveState();
        updateSavedList("memories");
      });
      break;
    case "guessOther":
      el.gameControls.innerHTML = `
        <div class="controls-row">
          <button class="primary" id="guessBtn">A devine</button>
          <button class="primary" id="answerBtn">B répond</button>
        </div>
        <div class="controls-row">
          <button class="ghost" id="matchBtn">Match</button>
          <button class="ghost" id="noMatchBtn">Pas match</button>
        </div>
      `;
      document.getElementById("guessBtn").addEventListener("click", () => {
        state.step.guessOther = "A";
        el.extraDisplay.textContent = "A devine, B ne regarde pas";
      });
      document.getElementById("answerBtn").addEventListener("click", () => {
        state.step.guessOther = "B";
        el.extraDisplay.textContent = "B répond maintenant";
      });
      document.getElementById("matchBtn").addEventListener("click", () => {
        updateScore("A", "plus");
        el.extraDisplay.textContent = "Compatibilité +1";
      });
      document.getElementById("noMatchBtn").addEventListener("click", () => {
        el.extraDisplay.textContent = "Pas match, continuez";
      });
      break;
    case "debate":
      el.gameControls.innerHTML = `
        <div class="controls-row">
          <button class="ghost" id="assignDebateBtn">Attribuer Pour/Contre</button>
          <button class="ghost" id="invertDebateBtn">Inverser</button>
        </div>
        <div class="controls-row">
          <button class="ghost" data-verdict="A">Verdict A</button>
          <button class="ghost" data-verdict="B">Verdict B</button>
          <button class="ghost" data-verdict="egalite">Égalité</button>
        </div>
      `;
      let assign = { A: "Pour", B: "Contre" };
      const updateAssign = () => {
        el.extraDisplay.textContent = `A : ${assign.A} · B : ${assign.B}`;
      };
      document.getElementById("assignDebateBtn").addEventListener("click", () => {
        assign = Math.random() > 0.5 ? { A: "Pour", B: "Contre" } : { A: "Contre", B: "Pour" };
        updateAssign();
        setTimerDuration(120);
      });
      document.getElementById("invertDebateBtn").addEventListener("click", () => {
        assign = { A: assign.B, B: assign.A };
        updateAssign();
      });
      el.gameControls.addEventListener("click", (event) => {
        const btn = event.target.closest("button[data-verdict]");
        if (!btn) return;
        const verdict = btn.dataset.verdict;
        if (verdict === "A" || verdict === "B") updateScore(verdict, "plus");
        el.extraDisplay.textContent = `Verdict : ${verdict.toUpperCase()}`;
      });
      updateAssign();
      break;
    case "coupleQuiz":
      el.gameControls.innerHTML = `
        <div class="controls-row">
          <button class="ghost" id="sameQuizBtn">Même réponse</button>
          <button class="ghost" id="diffQuizBtn">Différent</button>
        </div>
      `;
      document.getElementById("sameQuizBtn").addEventListener("click", () => {
        updateScore("A", "plus");
        el.extraDisplay.textContent = "Compatibilité +1";
      });
      document.getElementById("diffQuizBtn").addEventListener("click", () => {
        el.extraDisplay.textContent = "Différent, continuez";
      });
      break;
    case "trial":
      el.gameControls.innerHTML = `
        <div class="controls-row">
          <button class="ghost" id="accusationBtn">Accusation 30s</button>
          <button class="ghost" id="defenseBtn">Défense 30s</button>
        </div>
        <div class="controls-row">
          <button class="ghost" data-verdict="A">Verdict A</button>
          <button class="ghost" data-verdict="B">Verdict B</button>
          <button class="ghost" data-verdict="egalite">Égalité</button>
          <button class="ghost" id="randomVerdictBtn">Aléatoire</button>
        </div>
      `;
      document.getElementById("accusationBtn").addEventListener("click", () => {
        setTimerDuration(30);
        startTimer();
      });
      document.getElementById("defenseBtn").addEventListener("click", () => {
        setTimerDuration(30);
        startTimer();
      });
      document.getElementById("randomVerdictBtn").addEventListener("click", () => {
        const pick = Math.random() > 0.5 ? "A" : "B";
        updateScore(pick, "plus");
        el.extraDisplay.textContent = `Verdict aléatoire : ${pick}`;
      });
      el.gameControls.addEventListener("click", (event) => {
        const btn = event.target.closest("button[data-verdict]");
        if (!btn) return;
        const verdict = btn.dataset.verdict;
        if (verdict === "A" || verdict === "B") updateScore(verdict, "plus");
        el.extraDisplay.textContent = `Verdict : ${verdict.toUpperCase()}`;
      });
      break;
    case "compliments":
      el.gameControls.innerHTML = `<button class="primary" id="validComplimentBtn">Validé</button>`;
      document.getElementById("validComplimentBtn").addEventListener("click", renderCard);
      break;
    case "wouldYouRather":
      el.gameControls.innerHTML = `
        <div class="controls-row">
          <button class="primary" data-choice="A">Choix A</button>
          <button class="primary" data-choice="B">Choix B</button>
        </div>
        <div class="controls-row">
          <button class="ghost" id="sameChoiceBtn">Même choix</button>
          <button class="ghost" id="diffChoiceBtn">Différent</button>
          <button class="ghost" id="whyBtn">Pourquoi ?</button>
        </div>
      `;
      document.getElementById("sameChoiceBtn").addEventListener("click", () => {
        el.extraDisplay.textContent = "Vous avez choisi la même option.";
      });
      document.getElementById("diffChoiceBtn").addEventListener("click", () => {
        el.extraDisplay.textContent = "Choix différents, racontez pourquoi.";
      });
      document.getElementById("whyBtn").addEventListener("click", () => {
        el.extraDisplay.textContent = "Expliquez votre choix en une phrase.";
      });
      break;
    case "improv":
      el.gameControls.innerHTML = `
        <button class="primary" id="newSceneBtn">Nouvelle scène</button>
        <button class="ghost" id="timer120Btn">Timer 120s</button>
      `;
      document.getElementById("newSceneBtn").addEventListener("click", renderCard);
      document.getElementById("timer120Btn").addEventListener("click", () => {
        setTimerDuration(120);
        startTimer();
      });
      break;
    case "blindTest":
      el.gameControls.innerHTML = `
        <div class="controls-row">
          <button class="ghost" data-found="A">Trouvé A</button>
          <button class="ghost" data-found="B">Trouvé B</button>
          <button class="ghost" data-found="no">Pas trouvé</button>
        </div>
      `;
      el.gameControls.addEventListener("click", (event) => {
        const btn = event.target.closest("button[data-found]");
        if (!btn) return;
        const found = btn.dataset.found;
        if (found === "A" || found === "B") updateScore(found, "plus");
      });
      break;
    case "secrets":
      el.gameControls.innerHTML = `
        <div class="controls-row">
          <button class="ghost" id="softSecretBtn">Niveau soft</button>
          <button class="ghost" id="mediumSecretBtn">Niveau medium</button>
        </div>
      `;
      document.getElementById("softSecretBtn").addEventListener("click", () => {
        const result = drawUnique(game.id, DATA.data.secretsSoft, (item) => item);
        el.cardDisplay.textContent = result.text;
        updateHistory(result.text);
      });
      document.getElementById("mediumSecretBtn").addEventListener("click", () => {
        const result = drawUnique(game.id, DATA.data.secretsMedium, (item) => item);
        el.cardDisplay.textContent = result.text;
        updateHistory(result.text);
      });
      break;
    case "minuteChallenge":
      el.gameControls.innerHTML = `
        <div class="controls-row">
          <button class="ghost" data-success="A">Réussi A</button>
          <button class="ghost" data-success="B">Réussi B</button>
          <button class="ghost" data-success="fail">Raté</button>
        </div>
      `;
      el.gameControls.addEventListener("click", (event) => {
        const btn = event.target.closest("button[data-success]");
        if (!btn) return;
        const value = btn.dataset.success;
        if (value === "A" || value === "B") updateScore(value, "plus");
      });
      break;
    case "whoLies":
      el.gameControls.innerHTML = `
        <div class="controls-row">
          <button class="ghost" id="phrase1Btn">Phrase 1</button>
          <button class="ghost" id="phrase2Btn">Phrase 2</button>
        </div>
      `;
      document.getElementById("phrase1Btn").addEventListener("click", () => {
        el.extraDisplay.textContent = "Choix : Phrase 1. Révélation orale.";
      });
      document.getElementById("phrase2Btn").addEventListener("click", () => {
        el.extraDisplay.textContent = "Choix : Phrase 2. Révélation orale.";
      });
      break;
    case "planYear":
      el.gameControls.innerHTML = `
        <div class="controls-row">
          <button class="primary" id="keepPlanBtn">Garder</button>
          <button class="ghost" id="skipPlanBtn">Skip</button>
        </div>
      `;
      document.getElementById("keepPlanBtn").addEventListener("click", () => {
        state.saved.planYear.push(el.cardDisplay.textContent);
        saveState();
        updateSavedList("planYear");
        renderCard();
      });
      document.getElementById("skipPlanBtn").addEventListener("click", renderCard);
      break;
    case "emotion":
      el.gameControls.innerHTML = `
        <button class="ghost" id="revealEmotionBtn">Afficher émotion au lecteur</button>
        <div id="emotionOptions" class="controls-grid"></div>
      `;
      document.getElementById("revealEmotionBtn").addEventListener("click", () => {
        const card = getEmotionCard();
        el.extraDisplay.textContent = `Émotion : ${card.emotion}`;
        state.step.emotionRevealed = true;
      });
      renderEmotionOptions();
      break;
    case "creativity":
      el.gameControls.innerHTML = `
        <div class="controls-row">
          <button class="ghost" data-vote="A">Vote A</button>
          <button class="ghost" data-vote="B">Vote B</button>
          <button class="ghost" data-vote="egalite">Égalité</button>
        </div>
      `;
      el.gameControls.addEventListener("click", (event) => {
        const btn = event.target.closest("button[data-vote]");
        if (!btn) return;
        const vote = btn.dataset.vote;
        if (vote === "A" || vote === "B") updateScore(vote, "plus");
        el.extraDisplay.textContent = `Vote : ${vote.toUpperCase()}`;
      });
      break;
    case "priorities":
      el.gameControls.innerHTML = `
        <div class="controls-row" id="priorityButtons"></div>
      `;
      renderPriorityButtons();
      break;
    case "promises":
      el.gameControls.innerHTML = `
        <div class="controls-row">
          <button class="primary" id="keepPromiseBtn">Garder</button>
          <button class="ghost" id="skipPromiseBtn">Skip</button>
        </div>
      `;
      document.getElementById("keepPromiseBtn").addEventListener("click", () => {
        state.saved.promises.push(el.cardDisplay.textContent);
        saveState();
        updateSavedList("promises");
        renderCard();
      });
      document.getElementById("skipPromiseBtn").addEventListener("click", renderCard);
      break;
    case "voices":
      el.gameControls.innerHTML = `
        <div class="controls-row">
          <button class="ghost" id="voiceA">A joue</button>
          <button class="ghost" id="voiceB">B joue</button>
        </div>
        <div class="controls-row">
          <button class="ghost" data-vote="A">Vote A</button>
          <button class="ghost" data-vote="B">Vote B</button>
          <button class="ghost" data-vote="egalite">Égalité</button>
        </div>
      `;
      el.gameControls.addEventListener("click", (event) => {
        const btn = event.target.closest("button[data-vote]");
        if (!btn) return;
        const vote = btn.dataset.vote;
        if (vote === "A" || vote === "B") updateScore(vote, "plus");
        el.extraDisplay.textContent = `Vote : ${vote.toUpperCase()}`;
      });
      break;
    default:
      break;
  }
};

const getEmotionCard = () => {
  const cardText = state.lastCards[state.currentGame.id];
  return DATA.data.emotionCards.find((card) => card.phrase && cardText?.includes(card.phrase)) || getRandomItem(DATA.data.emotionCards);
};

const renderEmotionOptions = () => {
  const optionsContainer = document.getElementById("emotionOptions");
  if (!optionsContainer) return;
  const card = getEmotionCard();
  const pool = DATA.data.emotionCards.map((item) => item.emotion);
  const set = new Set([card.emotion]);
  while (set.size < 6) {
    set.add(getRandomItem(pool));
  }
  const options = Array.from(set).sort(() => Math.random() - 0.5);
  optionsContainer.innerHTML = options
    .map((emotion) => `<button class="ghost" data-emotion="${emotion}">${emotion}</button>`)
    .join("");
  optionsContainer.addEventListener("click", (event) => {
    const btn = event.target.closest("button[data-emotion]");
    if (!btn) return;
    const choice = btn.dataset.emotion;
    if (choice === card.emotion) {
      updateScore("A", "plus");
      el.extraDisplay.textContent = `Bravo, c'était ${card.emotion}`;
    } else {
      el.extraDisplay.textContent = `Raté, c'était ${card.emotion}`;
    }
  }, { once: true });
};

const renderPriorityButtons = () => {
  const buttonWrap = document.getElementById("priorityButtons");
  if (!buttonWrap) return;
  const lastText = state.lastCards[state.currentGame.id] || "";
  const choiceLine = lastText.split("Choix : ")[1] || "";
  const choices = choiceLine.split(" · ").filter(Boolean);
  const labels = ["A", "B", "C", "D"];
  buttonWrap.innerHTML = choices
    .map((choice, index) => `<button class="ghost" data-choice="${labels[index]}">${labels[index]}: ${choice}</button>`)
    .join("");
  buttonWrap.addEventListener("click", (event) => {
    const btn = event.target.closest("button[data-choice]");
    if (!btn) return;
    const choice = btn.dataset.choice;
    if (state.step.priorities.stage === "A") {
      state.step.priorities = { stage: "B", choiceA: choice };
      el.extraDisplay.textContent = `Choix A : ${choice} (à B de jouer)`;
    } else {
      const match = state.step.priorities.choiceA === choice;
      el.extraDisplay.textContent = match ? "Même choix !" : "Choix différent.";
      state.step.priorities = { stage: "A", choiceA: null };
    }
  });
};

const updateScore = (player, action) => {
  const score = state.scores[state.currentGame.id] || { A: 0, B: 0 };
  score[player] = Math.max(0, score[player] + (action === "plus" ? 1 : -1));
  state.scores[state.currentGame.id] = score;
  updateScores();
  saveState();
};

const initEvents = () => {
  document.getElementById("randomSiteBtn").addEventListener("click", () => {
    const game = getRandomItem(DATA.siteGames);
    setGameView(game, "site");
  });

  document.getElementById("randomVocalBtn").addEventListener("click", () => {
    const game = getRandomItem(DATA.vocalGames);
    setGameView(game, "vocal");
  });

  el.filters.addEventListener("click", (event) => {
    const btn = event.target.closest("button[data-filter]");
    if (!btn) return;
    state.filter = btn.dataset.filter;
    saveState();
    renderFilters();
    renderGameGrids();
  });

  el.searchInput.addEventListener("input", (event) => {
    state.search = event.target.value;
    saveState();
    renderGameGrids();
  });

  el.siteGameGrid.addEventListener("click", (event) => {
    const btn = event.target.closest("button[data-game]");
    if (!btn) return;
    const game = DATA.siteGames.find((g) => g.id === btn.dataset.game);
    if (game) setGameView(game, "site");
  });

  el.vocalGameGrid.addEventListener("click", (event) => {
    const btn = event.target.closest("button[data-game]");
    if (!btn) return;
    const game = DATA.vocalGames.find((g) => g.id === btn.dataset.game);
    if (game) setGameView(game, "vocal");
  });

  document.getElementById("backHomeBtn").addEventListener("click", () => {
    renderHome();
  });

  document.getElementById("newBtn").addEventListener("click", renderCard);
  document.getElementById("passBtn").addEventListener("click", renderCard);

  document.querySelector(".scoreboard").addEventListener("click", (event) => {
    const btn = event.target.closest("button[data-score]");
    if (!btn) return;
    updateScore(btn.dataset.score, btn.dataset.action);
  });

  document.getElementById("resetScoreBtn").addEventListener("click", () => {
    state.scores[state.currentGame.id] = { A: 0, B: 0 };
    updateScores();
    saveState();
  });

  el.timerStartBtn.addEventListener("click", startTimer);
  el.timerStopBtn.addEventListener("click", stopTimer);
  el.timerResetBtn.addEventListener("click", resetTimer);
};

const init = () => {
  loadState();
  renderFilters();
  renderGameGrids();
  initEvents();
  renderHome();
};

window.addEventListener("DOMContentLoaded", init);
