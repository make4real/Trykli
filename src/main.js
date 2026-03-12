import { TaskStatus } from "./data/seed.js";
import {
  getPhaseById,
  getState,
  getTaskById,
  resetState,
  subscribe,
  toggleTask,
  updateTaskNote,
  updateTaskStatus,
} from "./state/store.js";
import {
  getNextPriorityTask,
  getPhaseMetrics,
  getProjectMetrics,
} from "./utils/progress.js";
import { badgeClassForStatus, escapeHtml } from "./components/ui.js";

const app = document.getElementById("app");

function getRoute() {
  const hash = window.location.hash || "#/dashboard";
  const [_, section, id] = hash.split("/");
  return { section: section || "dashboard", id };
}

function header(project, metrics) {
  return `
    <header class="topbar">
      <div>
        <p class="eyebrow">Project Tracker</p>
        <h1>${project.name}</h1>
        <p>${project.tagline}</p>
      </div>
      <div class="progress-ring">
        <span>${metrics.progress}%</span>
        <small>Global</small>
      </div>
    </header>
    <nav class="tabs">
      <a href="#/dashboard">Dashboard</a>
      <a href="#/roadmap">Roadmap</a>
      <a href="#/stats">Statistiques</a>
    </nav>
  `;
}

function renderDashboard(project) {
  const metrics = getProjectMetrics(project);
  const next = getNextPriorityTask(project);

  return `
    <section class="grid stats-grid">
      <article class="card stat"><h3>Progression globale</h3><p>${metrics.progress}%</p></article>
      <article class="card stat"><h3>Tâches totales</h3><p>${metrics.total}</p></article>
      <article class="card stat"><h3>Terminées</h3><p>${metrics.done}</p></article>
      <article class="card stat"><h3>Restantes</h3><p>${metrics.remaining}</p></article>
    </section>

    <section class="grid two-col">
      <article class="card">
        <h2>Aperçu des phases</h2>
        ${project.phases
          .map((phase) => {
            const phaseMetrics = getPhaseMetrics(phase);
            return `<a class="phase-row" href="#/phase/${phase.id}">
              <div>
                <p class="eyebrow">Phase ${String(phase.order).padStart(2, "0")}</p>
                <h3>${phase.title}</h3>
              </div>
              <div class="phase-meta">
                <span>${phaseMetrics.done}/${phaseMetrics.total}</span>
                <span>${phaseMetrics.progress}%</span>
              </div>
            </a>`;
          })
          .join("")}
      </article>

      <article class="card">
        <h2>Prochaine étape suggérée</h2>
        ${
          next
            ? `<h3>${next.task.title}</h3>
               <p>${next.task.description}</p>
               <p><strong>Phase :</strong> ${next.phase.title}</p>
               <a class="button" href="#/task/${next.task.id}">Voir le détail</a>`
            : `<p>Toutes les tâches sont terminées. Bravo 🎉</p>`
        }
      </article>
    </section>
  `;
}

function renderRoadmap(project) {
  if (!project.phases.length) {
    return `<section class="card empty"><h2>Aucune phase</h2><p>Ajoutez des phases pour démarrer la roadmap.</p></section>`;
  }

  return `<section class="roadmap-list">
    ${project.phases
      .map((phase) => {
        const metrics = getPhaseMetrics(phase);
        return `<article class="card roadmap-item">
          <div class="roadmap-head">
            <div>
              <p class="eyebrow">Phase ${String(phase.order).padStart(2, "0")} · ${phase.timeline}</p>
              <h2>${phase.title}</h2>
              <p>${phase.description}</p>
            </div>
            <a class="button" href="#/phase/${phase.id}">Ouvrir</a>
          </div>
          <div class="meter"><div style="width:${metrics.progress}%"></div></div>
          <div class="legend"><span>${metrics.done}/${metrics.total} terminées</span><span>${metrics.progress}%</span></div>
        </article>`;
      })
      .join("")}
  </section>`;
}

function taskItem(task) {
  return `<li class="task-item">
      <label>
        <input data-action="toggle-task" data-task-id="${task.id}" type="checkbox" ${
    task.status === TaskStatus.DONE ? "checked" : ""
  } />
        <span>${task.title}</span>
      </label>
      <div class="task-actions">
        <span class="${badgeClassForStatus(task.status)}">${task.status}</span>
        <a href="#/task/${task.id}">Détail</a>
      </div>
    </li>`;
}

function renderPhaseDetail(project, phaseId) {
  const phase = getPhaseById(phaseId);
  if (!phase) {
    return `<section class="card empty"><h2>Phase introuvable</h2><a href="#/roadmap">Retour roadmap</a></section>`;
  }

  const metrics = getPhaseMetrics(phase);
  return `<section class="card">
    <p class="eyebrow">Phase ${String(phase.order).padStart(2, "0")} · ${phase.timeline}</p>
    <h2>${phase.title}</h2>
    <p>${phase.description}</p>
    <div class="meter"><div style="width:${metrics.progress}%"></div></div>
    <div class="legend"><span>${metrics.done}/${metrics.total} terminées</span><span>${metrics.progress}%</span></div>
    <ul class="task-list">${phase.tasks.map(taskItem).join("")}</ul>
  </section>`;
}

function renderTaskDetail(taskId) {
  const result = getTaskById(taskId);
  if (!result) return `<section class="card empty"><h2>Tâche introuvable</h2></section>`;

  const { task, phase } = result;
  return `<section class="card task-detail">
    <p class="eyebrow">${phase.title}</p>
    <h2>${task.title}</h2>
    <p>${task.description}</p>

    <div class="meta-grid">
      <div><strong>Statut</strong><select data-action="status-change" data-task-id="${task.id}">
        ${Object.values(TaskStatus)
          .map((status) => `<option value="${status}" ${status === task.status ? "selected" : ""}>${status}</option>`)
          .join("")}
      </select></div>
      <div><strong>Priorité</strong><p>${task.priority}</p></div>
      <div><strong>Estimation</strong><p>${task.estimate || "N/A"}</p></div>
      <div><strong>Phase</strong><p>${phase.title}</p></div>
    </div>

    <label class="note-box">
      <strong>Note</strong>
      <textarea data-action="note-change" data-task-id="${task.id}" rows="4">${escapeHtml(task.note || "")}</textarea>
    </label>
  </section>`;
}

function renderStats(project) {
  const metrics = getProjectMetrics(project);

  return `<section class="grid stats-grid">
      <article class="card stat"><h3>À faire</h3><p>${metrics.todo}</p></article>
      <article class="card stat"><h3>En cours</h3><p>${metrics.inProgress}</p></article>
      <article class="card stat"><h3>Terminées</h3><p>${metrics.done}</p></article>
      <article class="card stat"><h3>Progression</h3><p>${metrics.progress}%</p></article>
    </section>
    <section class="card">
      <h2>Progression par phase</h2>
      ${project.phases
        .map((phase) => {
          const phaseMetrics = getPhaseMetrics(phase);
          return `<div class="phase-progress-row">
            <span>${phase.title}</span>
            <span>${phaseMetrics.progress}%</span>
            <div class="meter"><div style="width:${phaseMetrics.progress}%"></div></div>
          </div>`;
        })
        .join("")}
      <button class="button button--ghost" data-action="reset-state">Réinitialiser la roadmap</button>
    </section>`;
}

function renderApp() {
  const project = getState();
  const metrics = getProjectMetrics(project);
  const route = getRoute();

  let content = "";
  if (route.section === "dashboard") content = renderDashboard(project);
  if (route.section === "roadmap") content = renderRoadmap(project);
  if (route.section === "phase") content = renderPhaseDetail(project, route.id);
  if (route.section === "task") content = renderTaskDetail(route.id);
  if (route.section === "stats") content = renderStats(project);

  app.innerHTML = `<main class="container">${header(project, metrics)}${content}</main>`;
}

window.addEventListener("hashchange", renderApp);
subscribe(renderApp);

document.addEventListener("change", (event) => {
  const target = event.target;
  const action = target?.dataset?.action;
  const taskId = target?.dataset?.taskId;

  if (action === "toggle-task") toggleTask(taskId);
  if (action === "status-change") updateTaskStatus(taskId, target.value);
  if (action === "note-change") updateTaskNote(taskId, target.value);
});

document.addEventListener("click", (event) => {
  const target = event.target;
  const action = target?.dataset?.action;

  if (action === "reset-state") {
    event.preventDefault();
    resetState();
  }
});

if (!window.location.hash) {
  window.location.hash = "#/dashboard";
}

renderApp();
