import { TaskStatus } from "../data/seed.js";

export function asPercent(completed, total) {
  if (!total) return 0;
  return Math.round((completed / total) * 100);
}

export function getTaskCounts(tasks) {
  return tasks.reduce(
    (acc, task) => {
      acc.total += 1;
      if (task.status === TaskStatus.DONE) acc.done += 1;
      if (task.status === TaskStatus.IN_PROGRESS) acc.inProgress += 1;
      if (task.status === TaskStatus.TODO) acc.todo += 1;
      return acc;
    },
    { total: 0, done: 0, inProgress: 0, todo: 0 },
  );
}

export function getProjectMetrics(project) {
  const allTasks = project.phases.flatMap((phase) => phase.tasks);
  const counts = getTaskCounts(allTasks);

  return {
    ...counts,
    remaining: counts.total - counts.done,
    progress: asPercent(counts.done, counts.total),
  };
}

export function getPhaseMetrics(phase) {
  const counts = getTaskCounts(phase.tasks);
  return {
    ...counts,
    progress: asPercent(counts.done, counts.total),
  };
}

export function getNextPriorityTask(project) {
  return project.phases
    .flatMap((phase) => phase.tasks.map((task) => ({ task, phase })))
    .find(({ task }) => task.status !== TaskStatus.DONE);
}
