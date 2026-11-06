import http from "k6/http";
import { check, sleep } from "k6";
import { textSummary } from "https://jslib.k6.io/k6-summary/0.0.1/index.js";
import { cinemaHtmlReport } from "../lib/cinema-report.js";

const BASE_URL = __ENV.API_BASE_URL || "http://perfo-api:8080/api/v1";

export const options = {
  stages: [
    { duration: "5s", target: 10 },
    { duration: "15s", target: 30 },
    { duration: "5s", target: 0 },
  ],
  thresholds: {
    http_req_duration: ["p(95)<2000"],
    http_req_failed: ["rate<0.02"],
  },
};

export function setup() {
  const loginRes = http.post(
    `${BASE_URL}/auth/login`,
    JSON.stringify({
      email: __ENV.E2E_CUSTOMER_EMAIL || "customer@cinema.local",
      password: __ENV.E2E_CUSTOMER_PASSWORD || "Customer@123",
    }),
    {
      headers: { "Content-Type": "application/json" },
    },
  );

  let token = "";
  if (loginRes.status === 200) {
    const json = loginRes.json();
    token = json.data ? json.data.accessToken : "";
  }

  return { token };
}

export default function (data) {
  const params = {
    headers: {
      "Content-Type": "application/json",
      ...(data.token ? { Authorization: `Bearer ${data.token}` } : {}),
    },
  };

  const moviesRes = http.get(`${BASE_URL}/movies`, params);
  check(moviesRes, {
    "movies catalog status 200": (r) => r.status === 200,
    "movies catalog fast response": (r) => r.timings.duration < 3000,
  });

  sleep(0.2);

  const seatsRes = http.get(`${BASE_URL}/showtimes/1/seats`, params);
  check(seatsRes, {
    "seat map status 200": (r) => r.status === 200,
    "seat map fast response": (r) => r.timings.duration < 3000,
  });

  sleep(0.3);

  const combosRes = http.get(`${BASE_URL}/combos`, params);
  check(combosRes, {
    "combos status 200": (r) => r.status === 200,
  });

  sleep(0.5);
}

export function handleSummary(data) {
  const resultsDir = __ENV.RESULTS_DIR || "/scripts/results";
  return {
    [`${resultsDir}/peak-ticket-rush-report.html`]: cinemaHtmlReport(data, {
      ticketTitle: "Peak Ticket Rush",
      scenarioFile: "peak-ticket-rush.js",
    }),
    stdout: textSummary(data, { indent: " ", enableColors: true }),
  };
}
