(() => {
  /**
   * Stable data format:
   * {
   *   version: number,
   *   updatedAt: "YYYY-MM-DD",
   *   sections: [{ title: string, items: [{ id: string, text: string, done: boolean, prio: 0|1|2 }] }],
   *   notes: string[]
   * }
   */
  const STORAGE_KEY = "soundkey_tasks_v2";

  const SEED = {
    version: 2,
    updatedAt: "2026-02-26",
    sections: [
      {
        title: "Site V2",
        items: [
          { id: "site-v2-design", text: "Nouveau design V2 en ligne", done: true, prio: 1 },
          { id: "site-v2-pages", text: "Pages principales créées", done: true, prio: 1 },
          { id: "site-v2-header", text: "Header/menu fonctionnel", done: true, prio: 1 },
          { id: "site-v2-customize", text: "Pages /customize actives", done: true, prio: 1 }
        ]
      },
      {
        title: "Personnalisation / Admin / Supabase",
        items: [
          { id: "admin-upload", text: "Upload admin vers Supabase OK", done: true, prio: 1 },
          { id: "admin-token", text: "Token généré OK", done: true, prio: 1 },
          { id: "admin-files", text: "Fichiers reçus dans Supabase OK", done: true, prio: 1 },
          { id: "admin-visualizer", text: "Visualiseur d’images indépendant des options (OK)", done: true, prio: 1 }
        ]
      },
      {
        title: "Conversion & contenus",
        items: [
          { id: "conv-pack", text: "Faire pack photos produit (fond propre + angles)", done: false, prio: 0 },
          { id: "conv-packaging", text: "Faire photos packaging", done: false, prio: 0 },
          { id: "conv-use", text: "Faire photos portées (mise en situation)", done: false, prio: 0 },
          { id: "conv-macro", text: "Faire photos macro NFC/QR", done: false, prio: 0 },
          { id: "conv-replace", text: "Remplacer toutes les images temporaires du site", done: false, prio: 0 }
        ]
      },
      {
        title: "Admin & lien lecteur",
        items: [
          { id: "player-link-after-publish", text: "Après Publish : afficher automatiquement le lien du player (copier-coller)", done: false, prio: 0 },
          { id: "player-link-copy", text: "Ajouter bouton “Copier le lien”", done: false, prio: 0 },
          { id: "player-link-check", text: "Vérifier que le lien mène au bon lecteur pour le token", done: false, prio: 0 }
        ]
      },
      {
        title: "Checkout & commande",
        items: [
          { id: "checkout-flow", text: "Vérifier paiement/checkout (flow complet)", done: false, prio: 0 },
          { id: "checkout-e2e", text: "Achat test de bout en bout", done: false, prio: 0 },
          { id: "checkout-email", text: "Vérifier email de confirmation", done: false, prio: 0 },
          { id: "checkout-confirm", text: "Page confirmation claire (résumé + next steps)", done: false, prio: 0 }
        ]
      },
      {
        title: "SEO & partage",
        items: [
          { id: "seo-og", text: "Corriger Open Graph (image + titre + description)", done: false, prio: 1 },
          { id: "seo-meta", text: "Meta title / description pages clés", done: false, prio: 1 },
          { id: "seo-favicon", text: "Favicon/manifest si utilisé", done: false, prio: 1 }
        ]
      },
      {
        title: "Pages légales & support",
        items: [
          { id: "legal-contact", text: "Page Contact clean", done: false, prio: 1 },
          { id: "legal-faq", text: "FAQ simple", done: false, prio: 1 },
          { id: "legal-privacy", text: "Conditions / confidentialité (si prévu)", done: false, prio: 1 }
        ]
      },
      {
        title: "Tracking & lancement",
        items: [
          { id: "launch-analytics", text: "Analytics (si utilisé) : vérifier tracking", done: false, prio: 1 },
          { id: "launch-checklist", text: "Checklist pré-lancement (mobile/desktop)", done: false, prio: 1 },
          { id: "launch-content", text: "Préparer 3 contenus Instagram/TikTok (démo produit)", done: false, prio: 1 }
        ]
      }
    ],
    notes: [
      "Astuce : fais un achat test dès que paiements + variantes sont prêts.",
      "Les photos produit = priorité #1 avant le lancement."
    ]
  };

  const els = {
    sections: document.getElementById("sections"),
    progressPercent: document.getElementById("progressPercent"),
    progressCount: document.getElementById("progressCount"),
    progressBar: document.getElementById("progressBar"),
    progressFill: document.getElementById("progressFill"),
    notes: document.getElementById("notes"),
    exportBtn: document.getElementById("exportBtn"),
    importInput: document.getElementById("importInput"),
    resetBtn: document.getElementById("resetBtn")
  };

  let data = load();

  function deepClone(value) {
    return JSON.parse(JSON.stringify(value));
  }

  function load() {
    const raw = localStorage.getItem(STORAGE_KEY);
    if (!raw) return deepClone(SEED);
    try {
      const parsed = JSON.parse(raw);
      if (!Array.isArray(parsed.sections)) throw new Error("Invalid payload");
      return parsed;
    } catch (error) {
      console.warn("Invalid localStorage payload, fallback to seed", error);
      return deepClone(SEED);
    }
  }

  function save() {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(data));
  }

  function getBadge(prio) {
    const value = Number.isInteger(prio) ? prio : 2;
    return `<span class="badge p${value}">P${value}</span>`;
  }

  function render() {
    const sectionsHTML = data.sections
      .map((section, sectionIndex) => {
        const tasksHTML = section.items
          .map((item, itemIndex) => `
            <label class="task ${item.done ? "done" : ""}">
              <input type="checkbox" data-section="${sectionIndex}" data-item="${itemIndex}" ${item.done ? "checked" : ""} />
              <span class="text">${item.text}</span>
              ${getBadge(item.prio)}
            </label>
          `)
          .join("");

        return `<article class="section"><h2>${section.title}</h2>${tasksHTML}</article>`;
      })
      .join("");

    els.sections.innerHTML = sectionsHTML;
    renderProgress();
    renderNotes();
  }

  function renderProgress() {
    const allItems = data.sections.flatMap((section) => section.items);
    const total = allItems.length;
    const done = allItems.filter((item) => item.done).length;
    const percent = total ? Math.round((done / total) * 100) : 0;

    els.progressPercent.textContent = `${percent}%`;
    els.progressCount.textContent = `${done} / ${total} tâches faites`;
    els.progressFill.style.width = `${percent}%`;
    els.progressBar.setAttribute("aria-valuenow", String(percent));
  }

  function renderNotes() {
    const list = (data.notes || []).map((note) => `<li>${note}</li>`).join("");
    els.notes.innerHTML = `<strong>Notes</strong><ul>${list}</ul>`;
  }

  function onToggle(event) {
    const target = event.target;
    if (!(target instanceof HTMLInputElement) || target.type !== "checkbox") return;

    const sectionIndex = Number(target.dataset.section);
    const itemIndex = Number(target.dataset.item);
    const item = data.sections?.[sectionIndex]?.items?.[itemIndex];
    if (!item) return;

    item.done = target.checked;
    data.updatedAt = new Date().toISOString().slice(0, 10);
    save();
    render();
  }

  function exportJson() {
    const blob = new Blob([JSON.stringify(data, null, 2)], { type: "application/json" });
    const url = URL.createObjectURL(blob);
    const a = document.createElement("a");
    a.href = url;
    a.download = `soundkey_tasks_v2_${data.updatedAt || "export"}.json`;
    a.click();
    URL.revokeObjectURL(url);
  }

  function importJson(event) {
    const file = event.target.files?.[0];
    if (!file) return;
    const reader = new FileReader();
    reader.onload = () => {
      try {
        const parsed = JSON.parse(String(reader.result));
        if (!Array.isArray(parsed.sections)) throw new Error("Format invalide: sections manquantes");
        data = parsed;
        save();
        render();
      } catch (error) {
        alert(`Import impossible: ${error.message}`);
      }
      event.target.value = "";
    };
    reader.readAsText(file);
  }

  function resetToSeed() {
    data = deepClone(SEED);
    save();
    render();
  }

  els.sections.addEventListener("change", onToggle);
  els.exportBtn.addEventListener("click", exportJson);
  els.importInput.addEventListener("change", importJson);
  els.resetBtn.addEventListener("click", resetToSeed);

  save();
  render();
})();
