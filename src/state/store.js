import { seedProject, TaskStatus } from "../data/seed.js";

const STORAGE_KEY = "fridgeflow-project-state-v1";

function deepClone(value) {
  return JSON.parse(JSON.stringify(value));
}

function hydrateState() {
  try {
    const raw = localStorage.getItem(STORAGE_KEY);
    if (!raw) return deepClone(seedProject);

    const parsed = JSON.parse(raw);
    if (!parsed?.phases?.length) return deepClone(seedProject);
    return parsed;
  } catch {
    return deepClone(seedProject);
  }
}

let state = hydrateState();
const listeners = new Set();

function notify() {
  localStorage.setItem(STORAGE_KEY, JSON.stringify(state));
  listeners.forEach((listener) => listener(state));
}

export function getState() {
  return state;
}

export function subscribe(listener) {
  listeners.add(listener);
  return () => listeners.delete(listener);
}

export function resetState() {
  state = deepClone(seedProject);
  notify();
}

function mutateTask(taskId, mutator) {
  let changed = false;
  state.phases = state.phases.map((phase) => ({
    ...phase,
    tasks: phase.tasks.map((task) => {
      if (task.id !== taskId) return task;
      changed = true;
      return mutator(task);
    }),
  }));

  if (changed) notify();
}

export function updateTaskStatus(taskId, status) {
  mutateTask(taskId, (task) => ({ ...task, status }));
}

export function toggleTask(taskId) {
  mutateTask(taskId, (task) => ({
    ...task,
    status: task.status === TaskStatus.DONE ? TaskStatus.TODO : TaskStatus.DONE,
  }));
}

export function updateTaskNote(taskId, note) {
  mutateTask(taskId, (task) => ({ ...task, note }));
}

export function getPhaseById(phaseId) {
  return state.phases.find((phase) => phase.id === phaseId);
}

export function getTaskById(taskId) {
  for (const phase of state.phases) {
    const task = phase.tasks.find((item) => item.id === taskId);
    if (task) return { task, phase };
  }
  return null;
}
