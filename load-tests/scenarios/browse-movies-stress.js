import http from "k6/http";
import { check, sleep } from "k6";
import { textSummary } from "https://jslib.k6.io/k6-summary/0.0.1/index.js";
import { cinemaHtmlReport } from "../lib/cinema-report.js";

const BASE_URL = __ENV.API_BASE_URL || "http://cinema-api:8080/api/v1";

export const options = {
  stages: [
    { duration: "5s", target: 20 },
    { duration: "15s", target: 50 },
    { duration: "5s", target: 0 },
  ],
  thresholds: {
    http_req_duration: ["p(95)<400"],
    http_req_failed: ["rate<0.01"],
  },
};

export default function () {
  const res1 = http.get(`${BASE_URL}/movies`);
  check(res1, {
    "get movies 200": (r) => r.status === 200,
  });

  const res2 = http.get(`${BASE_URL}/movies/1`);
  check(res2, {
    "get movie detail 200": (r) => r.status === 200,
  });

  sleep(0.1);
}

export function handleSummary(data) {
  const resultsDir = __ENV.RESULTS_DIR || "/scripts/results";
  return {
    [`${resultsDir}/browse-movies-stress-report.html`]: cinemaHtmlReport(data, {
      ticketTitle: "Browse Movies — Stress Test",
      scenarioFile: "browse-movies-stress.js",
    }),
    stdout: textSummary(data, { indent: " ", enableColors: true }),
  };
}
