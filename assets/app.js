/* Couple New Year Games - logique SPA */
const STORAGE_KEY = "couple-new-year-games";

const state = {
  view: "home",
  filter: "Tous",
  search: "",
  currentGameId: null,
  history: [],
  scores: {},
  rapidMode: false,
  theme: "dark",
  timer: { remaining: 60, running: false, handle: null, displayHandle: null }
};

const elements = {
  homeView: document.getElementById("homeView"),
  gameView: document.getElementById("gameView"),
  gameGrid: document.getElementById("gameGrid"),
  filters: document.getElementById("filters"),
  searchInput: document.getElementById("searchInput"),
  gameTitle: document.getElementById("gameTitle"),
  gameCategory: document.getElementById("gameCategory"),
  gameRules: document.getElementById("gameRules"),
  cardDisplay: document.getElementById("cardDisplay"),
  historyList: document.getElementById("historyList"),
  scoreA: document.getElementById("scoreA"),
  scoreB: document.getElementById("scoreB"),
  timerBlock: document.getElementById("timerBlock"),
  timerLabel: document.getElementById("timerLabel"),
  timerBtn: document.getElementById("timerBtn"),
  rapidModeToggle: document.getElementById("rapidModeToggle"),
  chatgptList: document.getElementById("chatgptList")
};

const saveState = () => {
  localStorage.setItem(
    STORAGE_KEY,
    JSON.stringify({
      scores: state.scores,
      filter: state.filter,
      search: state.search,
      rapidMode: state.rapidMode,
      theme: state.theme
    })
  );
};

const loadState = () => {
  const saved = localStorage.getItem(STORAGE_KEY);
  if (!saved) return;
  try {
    const data = JSON.parse(saved);
    state.scores = data.scores || {};
    state.filter = data.filter || "Tous";
    state.search = data.search || "";
    state.rapidMode = Boolean(data.rapidMode);
    state.theme = data.theme || "dark";
  } catch (err) {
    console.warn("Impossible de charger le state", err);
  }
};

const setTheme = (theme) => {
  state.theme = theme;
  document.body.dataset.theme = theme;
  saveState();
};

const resetTimer = () => {
  state.timer.remaining = 60;
  elements.timerLabel.textContent = `${state.timer.remaining}s`;
};

const stopTimer = () => {
  state.timer.running = false;
  clearInterval(state.timer.handle);
  state.timer.handle = null;
};

const startTimer = () => {
  stopTimer();
  state.timer.running = true;
  state.timer.remaining = 60;
  elements.timerLabel.textContent = `${state.timer.remaining}s`;
  state.timer.handle = setInterval(() => {
    state.timer.remaining -= 1;
    elements.timerLabel.textContent = `${state.timer.remaining}s`;
    if (state.timer.remaining <= 0) {
      stopTimer();
      elements.timerLabel.textContent = "Terminé";
    }
  }, 1000);
};

const getGameById = (id) => DATA.games.find((game) => game.id === id);

const formatCard = (game, card) => {
  if (!card) return "";
  if (game.dataKey === "improvCards") {
    return `Rôle : ${card.role}\nLieu : ${card.place}\nTwist : ${card.twist}`;
  }
  if (game.dataKey === "emotions") {
    return `Emotion à jouer : ${card.emotion}\nPhrase neutre : ${card.phrase}`;
  }
  if (game.dataKey === "priorities") {
    return `Situation : ${card.situation}\nChoix : ${card.choices.join(" · ")}`;
  }
  return card;
};

const updateHistory = (text) => {
  if (!text) return;
  state.history.unshift(text);
  state.history = state.history.slice(0, 10);
  elements.historyList.innerHTML = state.history.map((item) => `<li>${item}</li>`).join("");
};

const updateScores = () => {
  const score = state.scores[state.currentGameId] || { A: 0, B: 0 };
  elements.scoreA.textContent = score.A;
  elements.scoreB.textContent = score.B;
};

const updateTimerVisibility = (game) => {
  elements.timerBlock.style.display = game.hasTimer ? "flex" : "none";
  if (!game.hasTimer) {
    stopTimer();
  } else {
    resetTimer();
  }
};

const getRandomItem = (arr) => arr[Math.floor(Math.random() * arr.length)];

const drawCard = (game) => {
  const data = DATA.data;
  if (game.dataKey === "truthOrDare") {
    const pool = Math.random() > 0.5 ? data.truths : data.dares;
    const label = pool === data.truths ? "Vérité" : "Gage";
    return `${label} : ${getRandomItem(pool)}`;
  }
  if (game.dataKey === "trialCases") {
    const judge = Math.random() > 0.5 ? "Joueur A" : "Joueur B";
    return `Juge aléatoire : ${judge}\n${getRandomItem(data.trialCases)}`;
  }
  return getRandomItem(data[game.dataKey]);
};

const renderCard = (game) => {
  const card = drawCard(game);
  const text = formatCard(game, card);
  elements.cardDisplay.textContent = text;
  updateHistory(text.replace(/\n/g, " — "));
  if (state.rapidMode) {
    clearTimeout(state.timer.displayHandle);
    state.timer.displayHandle = setTimeout(() => renderCard(game), 6000);
  }
};

const renderGameView = (game) => {
  state.view = "game";
  state.currentGameId = game.id;
  state.history = [];
  elements.historyList.innerHTML = "";
  elements.homeView.classList.remove("active");
  elements.gameView.classList.add("active");

  elements.gameTitle.textContent = `${game.id}. ${game.title}`;
  elements.gameCategory.textContent = game.category;
  elements.gameRules.innerHTML = game.rules.map((rule) => `<li>${rule}</li>`).join("");
  elements.rapidModeToggle.checked = state.rapidMode;
  updateTimerVisibility(game);
  updateScores();
  renderCard(game);
};

const renderHomeView = () => {
  state.view = "home";
  elements.homeView.classList.add("active");
  elements.gameView.classList.remove("active");
  elements.searchInput.value = state.search;
  clearTimeout(state.timer.displayHandle);
};

const renderFilters = () => {
  const filters = ["Tous", ...DATA.categories];
  elements.filters.innerHTML = filters
    .map((filter) => {
      const activeClass = state.filter === filter ? "primary" : "ghost";
      return `<button class="${activeClass}" data-filter="${filter}">${filter}</button>`;
    })
    .join("");
};

const renderGameGrid = () => {
  const term = state.search.toLowerCase();
  const filtered = DATA.games.filter((game) => {
    const matchesFilter = state.filter === "Tous" || game.category === state.filter;
    const matchesSearch =
      game.title.toLowerCase().includes(term) ||
      game.description.toLowerCase().includes(term) ||
      game.category.toLowerCase().includes(term);
    return matchesFilter && matchesSearch;
  });

  elements.gameGrid.innerHTML = filtered
    .map((game) => {
      return `
        <article class="card-tile">
          <span class="tag">${game.category}</span>
          <h3>${game.id}. ${game.title}</h3>
          <p>${game.description}</p>
          <button class="primary" data-game="${game.id}">Lancer</button>
        </article>
      `;
    })
    .join("");
};

const renderChatGPTList = () => {
  elements.chatgptList.innerHTML = DATA.chatgptGames
    .map((game) => {
      return `
        <div class="chatgpt-card">
          <strong>${game.id}. ${game.title}</strong>
          <p>${game.prompt}</p>
          <button class="ghost" data-prompt="${game.prompt.replace(/"/g, '&quot;')}">Copier prompt</button>
        </div>
      `;
    })
    .join("");
};

const updateScore = (player, action) => {
  const current = state.scores[state.currentGameId] || { A: 0, B: 0 };
  current[player] = Math.max(0, current[player] + (action === "plus" ? 1 : -1));
  state.scores[state.currentGameId] = current;
  updateScores();
  saveState();
};

const initEvents = () => {
  document.getElementById("randomGameBtn").addEventListener("click", () => {
    const game = getRandomItem(DATA.games);
    renderGameView(game);
  });

  document.getElementById("toggleThemeBtn").addEventListener("click", () => {
    setTheme(state.theme === "dark" ? "light" : "dark");
  });

  elements.filters.addEventListener("click", (event) => {
    const target = event.target.closest("button[data-filter]");
    if (!target) return;
    state.filter = target.dataset.filter;
    saveState();
    renderFilters();
    renderGameGrid();
  });

  elements.searchInput.addEventListener("input", (event) => {
    state.search = event.target.value;
    saveState();
    renderGameGrid();
  });

  elements.gameGrid.addEventListener("click", (event) => {
    const target = event.target.closest("button[data-game]");
    if (!target) return;
    const game = getGameById(target.dataset.game);
    if (game) renderGameView(game);
  });

  document.getElementById("backHomeBtn").addEventListener("click", () => {
    renderHomeView();
    renderGameGrid();
  });

  document.getElementById("newRoundBtn").addEventListener("click", () => {
    const game = getGameById(state.currentGameId);
    if (game) renderCard(game);
  });

  document.getElementById("passBtn").addEventListener("click", () => {
    const game = getGameById(state.currentGameId);
    if (game) renderCard(game);
  });

  elements.timerBtn.addEventListener("click", () => {
    if (state.timer.running) {
      stopTimer();
      resetTimer();
    } else {
      startTimer();
    }
  });

  elements.rapidModeToggle.addEventListener("change", (event) => {
    state.rapidMode = event.target.checked;
    saveState();
  });

  document.getElementById("resetScoreBtn").addEventListener("click", () => {
    state.scores[state.currentGameId] = { A: 0, B: 0 };
    updateScores();
    saveState();
  });

  document.querySelector(".scoreboard").addEventListener("click", (event) => {
    const target = event.target.closest("button[data-score]");
    if (!target) return;
    updateScore(target.dataset.score, target.dataset.action);
  });

  elements.chatgptList.addEventListener("click", (event) => {
    const target = event.target.closest("button[data-prompt]");
    if (!target) return;
    const prompt = target.dataset.prompt;
    if (navigator.clipboard) {
      navigator.clipboard.writeText(prompt);
      target.textContent = "Copié !";
      setTimeout(() => (target.textContent = "Copier prompt"), 1500);
    } else {
      const temp = document.createElement("textarea");
      temp.value = prompt;
      document.body.appendChild(temp);
      temp.select();
      document.execCommand("copy");
      temp.remove();
      target.textContent = "Copié !";
      setTimeout(() => (target.textContent = "Copier prompt"), 1500);
    }
  });
};

const init = () => {
  loadState();
  setTheme(state.theme);
  renderFilters();
  renderGameGrid();
  renderChatGPTList();
  initEvents();
  renderHomeView();
};

window.addEventListener("DOMContentLoaded", init);
