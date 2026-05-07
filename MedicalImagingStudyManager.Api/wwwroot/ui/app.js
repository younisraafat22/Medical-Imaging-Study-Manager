const api = {
  patients: "/api/patients",
  studies: "/api/studies",
  reports: "/api/reports"
};

const state = {
  patients: [],
  studies: []
};

const patientForm = document.querySelector("#patientForm");
const studyForm = document.querySelector("#studyForm");
const reportForm = document.querySelector("#reportForm");
const filterForm = document.querySelector("#filterForm");
const toast = document.querySelector("#toast");

document.addEventListener("DOMContentLoaded", async () => {
  setDefaultStudyDate();
  await refreshAll();
});

patientForm.addEventListener("submit", async event => {
  event.preventDefault();
  const data = Object.fromEntries(new FormData(patientForm));
  await postJson(api.patients, data, "Patient created");
  await refreshAll();
});

studyForm.addEventListener("submit", async event => {
  event.preventDefault();
  const data = Object.fromEntries(new FormData(studyForm));
  data.patientId = Number(data.patientId);
  data.studyDate = new Date(data.studyDate).toISOString();
  await postJson(api.studies, data, "Study created");
  await refreshAll();
});

reportForm.addEventListener("submit", async event => {
  event.preventDefault();
  const data = Object.fromEntries(new FormData(reportForm));
  data.studyId = Number(data.studyId);
  await postJson(api.reports, data, "Report created and study finalized");
  await refreshAll();
});

filterForm.addEventListener("submit", async event => {
  event.preventDefault();
  await loadStudies();
  render();
});

document.querySelector("#clearFilters").addEventListener("click", async () => {
  filterForm.reset();
  await loadStudies();
  render();
});

async function refreshAll() {
  await Promise.all([loadPatients(), loadStudies()]);
  render();
}

async function loadPatients() {
  const response = await fetchJson(api.patients);
  state.patients = response.data ?? [];
}

async function loadStudies() {
  const params = new URLSearchParams();
  const filter = Object.fromEntries(new FormData(filterForm));

  if (filter.modality) params.set("modality", filter.modality);
  if (filter.status) params.set("status", filter.status);
  if (filter.date) params.set("date", filter.date);

  const url = params.toString() ? `${api.studies}?${params}` : api.studies;
  const response = await fetchJson(url);
  state.studies = response.data ?? [];
}

function render() {
  renderMetrics();
  renderPatientSelect();
  renderReportStudySelect();
  renderPatients();
  renderStudies();
}

function renderMetrics() {
  document.querySelector("#patientCount").textContent = state.patients.length;
  document.querySelector("#studyCount").textContent = state.studies.length;
  document.querySelector("#urgentCount").textContent = state.studies.filter(study => study.priority === "Urgent").length;
  document.querySelector("#pendingCount").textContent = state.studies.filter(study => study.status === "Pending").length;
}

function renderPatientSelect() {
  const select = document.querySelector("#studyPatient");
  select.innerHTML = state.patients
    .map(patient => `<option value="${patient.id}">${escapeHtml(patient.fullName)} (${escapeHtml(patient.patientNumber)})</option>`)
    .join("");
}

function renderReportStudySelect() {
  const select = document.querySelector("#reportStudy");
  const reportableStudies = state.studies.filter(study => study.status !== "Finalized");
  select.innerHTML = reportableStudies
    .map(study => `<option value="${study.id}">#${study.id} ${escapeHtml(study.modality)} ${escapeHtml(study.bodyPart)} - ${escapeHtml(study.patientName)}</option>`)
    .join("");
}

function renderPatients() {
  const container = document.querySelector("#patientList");

  container.innerHTML = state.patients.map(patient => `
    <article class="patient-item">
      <div class="title">${escapeHtml(patient.fullName)}</div>
      <div class="meta">${escapeHtml(patient.patientNumber)} · ${escapeHtml(patient.gender)} · DOB ${escapeHtml(patient.dateOfBirth)}</div>
    </article>
  `).join("");
}

function renderStudies() {
  const container = document.querySelector("#studyList");

  if (state.studies.length === 0) {
    container.innerHTML = `<p class="meta">No studies match the current filters.</p>`;
    return;
  }

  container.innerHTML = state.studies.map(study => `
    <article class="study-item">
      <div class="study-header">
        <div>
          <div class="title">#${study.id} ${escapeHtml(study.modality)} ${escapeHtml(study.bodyPart)}</div>
          <div class="meta">${escapeHtml(study.patientName)} · ${formatDate(study.studyDate)}</div>
        </div>
      </div>
      <div class="badges">
        <span class="badge ${study.priority === "Urgent" ? "urgent" : ""}">${escapeHtml(study.priority)}</span>
        <span class="badge ${statusClass(study.status)}">${escapeHtml(study.status)}</span>
      </div>
      <div class="actions">
        <button type="button" onclick="markUrgent(${study.id})">Mark Urgent</button>
        <button type="button" class="secondary" onclick="changeStatus(${study.id}, 'Pending')">Pending</button>
        <button type="button" class="secondary" onclick="changeStatus(${study.id}, 'InReview')">In Review</button>
        <button type="button" class="secondary" onclick="changeStatus(${study.id}, 'Finalized')">Finalized</button>
      </div>
    </article>
  `).join("");
}

async function markUrgent(id) {
  await patchJson(`${api.studies}/${id}/urgent`, {}, "Study marked urgent");
  await refreshAll();
}

async function changeStatus(id, status) {
  await patchJson(`${api.studies}/${id}/status`, { status }, `Study changed to ${status}`);
  await refreshAll();
}

async function fetchJson(url) {
  const response = await fetch(url);
  return handleResponse(response);
}

async function postJson(url, body, successMessage) {
  const response = await fetch(url, requestOptions("POST", body));
  await handleResponse(response);
  showToast(successMessage);
}

async function patchJson(url, body, successMessage) {
  const response = await fetch(url, requestOptions("PATCH", body));
  await handleResponse(response);
  showToast(successMessage);
}

function requestOptions(method, body) {
  return {
    method,
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(body)
  };
}

async function handleResponse(response) {
  const payload = await response.json();
  if (!response.ok || payload.success === false) {
    throw new Error(payload.message || "Request failed");
  }
  return payload;
}

function showToast(message) {
  toast.textContent = message;
  toast.classList.add("visible");
  window.setTimeout(() => toast.classList.remove("visible"), 2400);
}

function setDefaultStudyDate() {
  const input = document.querySelector("input[name='studyDate']");
  const now = new Date();
  now.setMinutes(now.getMinutes() - now.getTimezoneOffset());
  input.value = now.toISOString().slice(0, 16);
}

function statusClass(status) {
  if (status === "Finalized") return "finalized";
  if (status === "InReview") return "review";
  return "";
}

function formatDate(value) {
  return new Intl.DateTimeFormat(undefined, {
    dateStyle: "medium",
    timeStyle: "short"
  }).format(new Date(value));
}

function escapeHtml(value) {
  return String(value)
    .replaceAll("&", "&amp;")
    .replaceAll("<", "&lt;")
    .replaceAll(">", "&gt;")
    .replaceAll('"', "&quot;")
    .replaceAll("'", "&#039;");
}
