import { TaskStatus } from "../data/seed.js";

export function badgeClassForStatus(status) {
  if (status === TaskStatus.DONE) return "badge badge--done";
  if (status === TaskStatus.IN_PROGRESS) return "badge badge--progress";
  return "badge badge--todo";
}

export function escapeHtml(value = "") {
  return value
    .replaceAll("&", "&amp;")
    .replaceAll("<", "&lt;")
    .replaceAll(">", "&gt;")
    .replaceAll('"', "&quot;")
    .replaceAll("'", "&#039;");
}
