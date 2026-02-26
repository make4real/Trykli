(() => {
  const STORAGE_KEY = "soundkey_tasks_v2";

  const SEED = {
    version: 1,
    updatedAt: "2026-02-26T00:00:00Z",
    sections: [
      {
        id: "sec_site_v2",
        title: "Site V2",
        items: [
          { id: "t_site_1", text: "Nouveau design V2 en ligne", done: true, prio: 1, createdAt: "2026-02-26T00:00:00Z" },
          { id: "t_site_2", text: "Pages principales créées", done: true, prio: 1, createdAt: "2026-02-26T00:00:00Z" },
          { id: "t_site_3", text: "Header/menu fonctionnel", done: true, prio: 1, createdAt: "2026-02-26T00:00:00Z" },
          { id: "t_site_4", text: "Pages /customize actives", done: true, prio: 1, createdAt: "2026-02-26T00:00:00Z" }
        ]
      },
      {
        id: "sec_admin_supabase",
        title: "Personnalisation / Admin / Supabase",
        items: [
          { id: "t_admin_1", text: "Upload admin vers Supabase OK", done: true, prio: 1, createdAt: "2026-02-26T00:00:00Z" },
          { id: "t_admin_2", text: "Token généré OK", done: true, prio: 1, createdAt: "2026-02-26T00:00:00Z" },
          { id: "t_admin_3", text: "Fichiers reçus dans Supabase OK", done: true, prio: 1, createdAt: "2026-02-26T00:00:00Z" },
          { id: "t_admin_4", text: "Visualiseur d’images indépendant des options (OK)", done: true, prio: 1, createdAt: "2026-02-26T00:00:00Z" }
        ]
      },
      {
        id: "sec_conversion",
        title: "Conversion & contenus",
        items: [
          { id: "t_conv_1", text: "Faire pack photos produit (fond propre + angles)", done: false, prio: 0, createdAt: "2026-02-26T00:00:00Z" },
          { id: "t_conv_2", text: "Faire photos packaging", done: false, prio: 0, createdAt: "2026-02-26T00:00:00Z" },
          { id: "t_conv_3", text: "Faire photos portées (mise en situation)", done: false, prio: 0, createdAt: "2026-02-26T00:00:00Z" },
          { id: "t_conv_4", text: "Faire photos macro NFC/QR", done: false, prio: 0, createdAt: "2026-02-26T00:00:00Z" },
          { id: "t_conv_5", text: "Remplacer toutes les images temporaires du site", done: false, prio: 0, createdAt: "2026-02-26T00:00:00Z" }
        ]
      },
      {
        id: "sec_admin_lien",
        title: "Admin & lien lecteur",
        items: [
          { id: "t_link_1", text: "Après Publish : afficher automatiquement le lien du player (copier-coller)", done: false, prio: 0, createdAt: "2026-02-26T00:00:00Z" },
          { id: "t_link_2", text: "Ajouter bouton “Copier le lien”", done: false, prio: 0, createdAt: "2026-02-26T00:00:00Z" },
          { id: "t_link_3", text: "Vérifier que le lien mène au bon lecteur pour le token", done: false, prio: 0, createdAt: "2026-02-26T00:00:00Z" }
        ]
      },
      {
        id: "sec_checkout",
        title: "Checkout & commande",
        items: [
          { id: "t_check_1", text: "Vérifier paiement/checkout (flow complet)", done: false, prio: 0, createdAt: "2026-02-26T00:00:00Z" },
          { id: "t_check_2", text: "Achat test de bout en bout", done: false, prio: 0, createdAt: "2026-02-26T00:00:00Z" },
          { id: "t_check_3", text: "Vérifier email de confirmation", done: false, prio: 0, createdAt: "2026-02-26T00:00:00Z" },
          { id: "t_check_4", text: "Page confirmation claire (résumé + next steps)", done: false, prio: 0, createdAt: "2026-02-26T00:00:00Z" }
        ]
      },
      {
        id: "sec_seo",
        title: "SEO & partage",
        items: [
          { id: "t_seo_1", text: "Corriger Open Graph (image + titre + description)", done: false, prio: 1, createdAt: "2026-02-26T00:00:00Z" },
          { id: "t_seo_2", text: "Meta title / description pages clés", done: false, prio: 1, createdAt: "2026-02-26T00:00:00Z" },
          { id: "t_seo_3", text: "Favicon/manifest si utilisé", done: false, prio: 1, createdAt: "2026-02-26T00:00:00Z" }
        ]
      },
      {
        id: "sec_legal",
        title: "Pages légales & support",
        items: [
          { id: "t_legal_1", text: "Page Contact clean", done: false, prio: 1, createdAt: "2026-02-26T00:00:00Z" },
          { id: "t_legal_2", text: "FAQ simple", done: false, prio: 1, createdAt: "2026-02-26T00:00:00Z" },
          { id: "t_legal_3", text: "Conditions / confidentialité (si prévu)", done: false, prio: 1, createdAt: "2026-02-26T00:00:00Z" }
        ]
      },
      {
        id: "sec_tracking",
        title: "Tracking & lancement",
        items: [
          { id: "t_track_1", text: "Analytics (si utilisé) : vérifier tracking", done: false, prio: 1, createdAt: "2026-02-26T00:00:00Z" },
          { id: "t_track_2", text: "Checklist pré-lancement (mobile/desktop)", done: false, prio: 1, createdAt: "2026-02-26T00:00:00Z" },
          { id: "t_track_3", text: "Préparer 3 contenus Instagram/TikTok (démo produit)", done: false, prio: 1, createdAt: "2026-02-26T00:00:00Z" }
        ]
      }
    ]
  };

  let state = loadState();

  const sectionsContainer = document.getElementById("sectionsContainer");
  const progressText = document.getElementById("progressText");
  const progressFill = document.getElementById("progressFill");

  const searchInput = document.getElementById("searchInput");
  const prioFilter = document.getElementById("prioFilter");
  const statusFilter = document.getElementById("statusFilter");
  const sortBy = document.getElementById("sortBy");

  document.getElementById("addSectionBtn").addEventListener("click", addSection);
  document.getElementById("exportBtn").addEventListener("click", exportJson);
  document.getElementById("importInput").addEventListener("change", importJson);
  document.getElementById("resetBtn").addEventListener("click", resetSeed);
  [searchInput, prioFilter, statusFilter, sortBy].forEach((el) => el.addEventListener("input", render));

  render();

  function nowIso() { return new Date().toISOString(); }
  function uid(prefix) { return `${prefix}_${Math.random().toString(36).slice(2, 10)}`; }
  function deepCopy(obj) { return JSON.parse(JSON.stringify(obj)); }

  function loadState() {
    const raw = localStorage.getItem(STORAGE_KEY);
    if (!raw) {
      const seed = deepCopy(SEED);
      saveState(seed);
      return seed;
    }
    try {
      const parsed = JSON.parse(raw);
      validateState(parsed);
      return parsed;
    } catch {
      const seed = deepCopy(SEED);
      saveState(seed);
      return seed;
    }
  }

  function validateState(candidate) {
    if (!candidate || !Array.isArray(candidate.sections)) throw new Error("invalid");
  }

  function saveState(next) {
    next.updatedAt = nowIso();
    localStorage.setItem(STORAGE_KEY, JSON.stringify(next));
  }

  function commit() {
    saveState(state);
    render();
  }

  function allTasks() {
    return state.sections.flatMap((sec) => sec.items.map((item) => ({ secId: sec.id, item })));
  }

  function render() {
    renderProgress();
    sectionsContainer.innerHTML = "";
    for (const section of state.sections) {
      sectionsContainer.appendChild(renderSection(section));
    }
  }

  function renderProgress() {
    const tasks = allTasks().map((x) => x.item);
    const total = tasks.length;
    const done = tasks.filter((t) => t.done).length;
    const pct = total ? Math.round((done / total) * 100) : 0;
    progressText.textContent = `${pct}% · ${done}/${total}`;
    progressFill.style.width = `${pct}%`;
  }

  function renderSection(section) {
    const wrap = document.createElement("section");
    wrap.className = "section";

    const head = document.createElement("div");
    head.className = "section-head";

    const title = document.createElement("div");
    title.className = "section-title";
    title.textContent = section.title;

    const renameBtn = mkButton("Rename", () => renameSection(section.id));
    const upBtn = mkButton("↑", () => moveSection(section.id, -1));
    const downBtn = mkButton("↓", () => moveSection(section.id, 1));
    const addTaskBtn = mkButton("+ Tâche", () => addTask(section.id));
    const delBtn = mkButton("Delete", () => deleteSection(section.id), "danger");

    head.append(title, renameBtn, upBtn, downBtn, addTaskBtn, delBtn);

    const list = document.createElement("div");
    list.className = "task-list";

    const filtered = filteredAndSortedTasks(section.items);
    if (filtered.length === 0) {
      const empty = document.createElement("div");
      empty.className = "empty";
      empty.textContent = "Aucune tâche avec les filtres actuels.";
      list.appendChild(empty);
    } else {
      for (const task of filtered) list.appendChild(renderTask(section.id, task));
    }

    wrap.append(head, list);
    return wrap;
  }

  function filteredAndSortedTasks(tasks) {
    const search = searchInput.value.trim().toLowerCase();
    const pf = prioFilter.value;
    const sf = statusFilter.value;

    let next = tasks.filter((task) => {
      if (pf !== "all" && String(task.prio) !== pf) return false;
      if (sf === "done" && !task.done) return false;
      if (sf === "todo" && task.done) return false;
      if (search && !task.text.toLowerCase().includes(search)) return false;
      return true;
    });

    const mode = sortBy.value;
    next.sort((a, b) => {
      if (mode === "priority") return a.prio - b.prio || a.createdAt.localeCompare(b.createdAt);
      if (mode === "status") return Number(a.done) - Number(b.done) || a.prio - b.prio;
      return a.createdAt.localeCompare(b.createdAt);
    });

    return next;
  }

  function renderTask(sectionId, task) {
    const node = document.getElementById("taskTemplate").content.firstElementChild.cloneNode(true);
    node.dataset.taskId = task.id;
    if (task.done) node.classList.add("done");

    const done = node.querySelector(".task-done");
    const text = node.querySelector(".task-text");
    const prio = node.querySelector(".task-prio");
    const sectionSel = node.querySelector(".task-section");

    done.checked = task.done;
    text.value = task.text;
    prio.value = String(task.prio);

    for (const sec of state.sections) {
      const opt = document.createElement("option");
      opt.value = sec.id;
      opt.textContent = sec.title;
      if (sec.id === sectionId) opt.selected = true;
      sectionSel.appendChild(opt);
    }

    done.addEventListener("change", () => {
      updateTask(sectionId, task.id, { done: done.checked });
    });

    node.querySelector(".task-save").addEventListener("click", () => {
      updateTask(sectionId, task.id, {
        text: text.value.trim() || task.text,
        prio: Number(prio.value)
      });
      if (sectionSel.value !== sectionId) moveTask(task.id, sectionSel.value);
    });

    node.querySelector(".task-delete").addEventListener("click", () => {
      deleteTask(sectionId, task.id);
    });

    return node;
  }

  function mkButton(text, handler, className = "") {
    const btn = document.createElement("button");
    btn.type = "button";
    btn.textContent = text;
    btn.className = className;
    btn.addEventListener("click", handler);
    return btn;
  }

  function addSection() {
    const title = prompt("Nom de la catégorie ?")?.trim();
    if (!title) return;
    state.sections.push({ id: uid("sec"), title, items: [] });
    commit();
  }

  function renameSection(sectionId) {
    const section = state.sections.find((s) => s.id === sectionId);
    if (!section) return;
    const title = prompt("Nouveau nom", section.title)?.trim();
    if (!title) return;
    section.title = title;
    commit();
  }

  function deleteSection(sectionId) {
    if (state.sections.length <= 1) {
      alert("Impossible de supprimer la dernière catégorie.");
      return;
    }
    const section = state.sections.find((s) => s.id === sectionId);
    if (!section) return;

    const inbox = getOrCreateInbox();
    const choice = prompt(
      `Supprimer "${section.title}" ? Tape MOVE pour déplacer les tâches vers Inbox, DELETE pour tout supprimer.`
    );
    if (!choice) return;

    const idx = state.sections.findIndex((s) => s.id === sectionId);
    if (idx < 0) return;

    if (choice.toUpperCase() === "MOVE") {
      inbox.items.push(...section.items);
      state.sections.splice(idx, 1);
      commit();
      return;
    }

    if (choice.toUpperCase() === "DELETE" && confirm("Confirmer suppression de la catégorie et de ses tâches ?")) {
      state.sections.splice(idx, 1);
      commit();
    }
  }

  function moveSection(sectionId, offset) {
    const idx = state.sections.findIndex((s) => s.id === sectionId);
    const next = idx + offset;
    if (idx < 0 || next < 0 || next >= state.sections.length) return;
    [state.sections[idx], state.sections[next]] = [state.sections[next], state.sections[idx]];
    commit();
  }

  function addTask(sectionId) {
    const section = state.sections.find((s) => s.id === sectionId);
    if (!section) return;
    const text = prompt("Texte de la tâche ?")?.trim();
    if (!text) return;
    const prio = Number(prompt("Priorité (0=P0, 1=P1, 2=P2)", "1"));
    section.items.push({
      id: uid("t"),
      text,
      done: false,
      prio: [0, 1, 2].includes(prio) ? prio : 1,
      createdAt: nowIso()
    });
    commit();
  }

  function updateTask(sectionId, taskId, patch) {
    const section = state.sections.find((s) => s.id === sectionId);
    const task = section?.items.find((i) => i.id === taskId);
    if (!task) return;
    Object.assign(task, patch);
    commit();
  }

  function deleteTask(sectionId, taskId) {
    const section = state.sections.find((s) => s.id === sectionId);
    if (!section) return;
    section.items = section.items.filter((i) => i.id !== taskId);
    commit();
  }

  function moveTask(taskId, targetSectionId) {
    let moving = null;
    for (const sec of state.sections) {
      const i = sec.items.findIndex((x) => x.id === taskId);
      if (i >= 0) {
        moving = sec.items.splice(i, 1)[0];
        break;
      }
    }
    const target = state.sections.find((s) => s.id === targetSectionId);
    if (moving && target) {
      target.items.push(moving);
      commit();
    }
  }

  function getOrCreateInbox() {
    let inbox = state.sections.find((s) => s.id === "inbox");
    if (!inbox) {
      inbox = { id: "inbox", title: "Inbox", items: [] };
      state.sections.unshift(inbox);
    }
    return inbox;
  }

  function exportJson() {
    const blob = new Blob([JSON.stringify(state, null, 2)], { type: "application/json" });
    const url = URL.createObjectURL(blob);
    const a = document.createElement("a");
    a.href = url;
    a.download = `soundkey_tasks_${Date.now()}.json`;
    a.click();
    URL.revokeObjectURL(url);
  }

  function importJson(event) {
    const file = event.target.files[0];
    if (!file) return;
    const reader = new FileReader();
    reader.onload = () => {
      try {
        const parsed = JSON.parse(String(reader.result));
        validateState(parsed);
        state = parsed;
        commit();
      } catch {
        alert("Fichier JSON invalide.");
      } finally {
        event.target.value = "";
      }
    };
    reader.readAsText(file);
  }

  function resetSeed() {
    if (!confirm("Reset complet vers le seed initial ?")) return;
    state = deepCopy(SEED);
    commit();
  }
})();
