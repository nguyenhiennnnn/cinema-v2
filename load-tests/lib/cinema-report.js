function fmtInt(n) {
  if (n === undefined || n === null || isNaN(n)) return "—";
  var s = Math.round(n).toString();
  var out = "";
  for (var i = 0; i < s.length; i++) {
    var posFromEnd = s.length - i;
    out += s[i];
    if (posFromEnd > 1 && posFromEnd % 3 === 1) out += ".";
  }
  return out;
}

function fmtFixed(n, digits) {
  if (n === undefined || n === null || isNaN(n)) return "—";
  return n.toFixed(digits === undefined ? 2 : digits).replace(".", ",");
}

function fmtMs(n) {
  if (n === undefined || n === null || isNaN(n)) return "—";
  if (n >= 1000) return fmtFixed(n / 1000, 2) + " s";
  return fmtFixed(n, 1) + " ms";
}

function fmtBytes(n) {
  if (n === undefined || n === null || isNaN(n)) return "—";
  var units = ["B", "KB", "MB", "GB"];
  var i = 0;
  var v = n;
  while (v >= 1024 && i < units.length - 1) {
    v = v / 1024;
    i++;
  }
  return fmtFixed(v, i === 0 ? 0 : 2) + " " + units[i];
}

function fmtPercent(n, digits) {
  if (n === undefined || n === null || isNaN(n)) return "—";
  return fmtFixed(n * 100, digits === undefined ? 2 : digits) + "%";
}

function esc(str) {
  if (str === undefined || str === null) return "";
  return String(str)
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;")
    .replace(/"/g, "&quot;");
}

function parseDurationToMs(str) {
  // Hỗ trợ "5s", "1m30s", "500ms", "1h"
  if (!str) return 0;
  var re = /(\d+(?:\.\d+)?)(ms|s|m|h)/g;
  var total = 0;
  var match;
  while ((match = re.exec(str)) !== null) {
    var val = parseFloat(match[1]);
    var unit = match[2];
    if (unit === "ms") total += val;
    else if (unit === "s") total += val * 1000;
    else if (unit === "m") total += val * 60 * 1000;
    else if (unit === "h") total += val * 60 * 60 * 1000;
  }
  return total;
}

function collectChecks(group) {
  var list = [];
  if (!group) return list;
  var checks = group.checks;
  if (checks) {
    if (Array.isArray(checks)) {
      checks.forEach(function (c) {
        list.push(c);
      });
    } else {
      Object.keys(checks).forEach(function (k) {
        list.push(checks[k]);
      });
    }
  }
  var groups = group.groups;
  if (groups) {
    if (Array.isArray(groups)) {
      groups.forEach(function (g) {
        list = list.concat(collectChecks(g));
      });
    } else {
      Object.keys(groups).forEach(function (k) {
        list = list.concat(collectChecks(groups[k]));
      });
    }
  }
  return list;
}

function thresholdOk(val) {
  if (typeof val === "boolean") return val;
  if (val && typeof val.ok === "boolean") return val.ok;
  return true;
}

function collectThresholds(metrics) {
  var rows = [];
  Object.keys(metrics).forEach(function (name) {
    var m = metrics[name];
    if (m && m.thresholds) {
      Object.keys(m.thresholds).forEach(function (expr) {
        rows.push({
          metric: name,
          expr: expr,
          ok: thresholdOk(m.thresholds[expr]),
        });
      });
    }
  });
  return rows;
}

function getValues(m) {
  if (!m) return {};
  if (m.values && typeof m.values === "object") {
    var merged = {};
    Object.keys(m.values).forEach(function (k) {
      merged[k] = m.values[k];
    });
    if (m.thresholds) merged.thresholds = m.thresholds;
    if (m.type) merged.type = m.type;
    return merged;
  }
  return m;
}

function isTrend(m) {
  return typeof m.avg === "number";
}
function isRateCheckLike(m) {
  return (
    typeof m.value === "number" &&
    (typeof m.passes === "number" || typeof m.fails === "number")
  );
}
function isCounter(m) {
  return typeof m.count === "number" && typeof m.avg !== "number";
}
function isGauge(m) {
  return (
    typeof m.value === "number" &&
    !isRateCheckLike(m) &&
    typeof m.count !== "number"
  );
}

var METRIC_LABELS = {
  http_reqs: "Tổng số requests",
  http_req_duration: "Thời gian phản hồi (tổng)",
  http_req_waiting: "Thời gian chờ server (TTFB)",
  http_req_connecting: "Thời gian kết nối TCP",
  http_req_tls_handshaking: "Thời gian bắt tay TLS",
  http_req_sending: "Thời gian gửi request",
  http_req_receiving: "Thời gian nhận response",
  http_req_blocked: "Thời gian chờ tài nguyên (blocked)",
  http_req_failed: "Tỷ lệ request lỗi",
  iteration_duration: "Thời gian mỗi iteration",
  iterations: "Số iterations",
  vus: "VUs hiện tại",
  vus_max: "VUs tối đa",
  data_sent: "Dữ liệu đã gửi",
  data_received: "Dữ liệu đã nhận",
  checks: "Checks tổng hợp",
};

function metricLabel(name) {
  return METRIC_LABELS[name] || name;
}

var REPORT_CSS = `
  @import url('https://cdn.jsdelivr.net/npm/@fontsource-variable/oswald/index.css');
  @import url('https://cdn.jsdelivr.net/npm/@fontsource-variable/geist/index.css');

  :root[data-theme="light"] {
    --background: oklch(0.972 0.008 72);
    --foreground: oklch(0.17 0.02 40);
    --card: oklch(0.995 0.004 70);
    --card-foreground: var(--foreground);
    --popover: oklch(1 0 0);
    --primary: oklch(0.42 0.18 24);
    --primary-foreground: oklch(0.98 0.006 75);
    --secondary: oklch(0.9 0.016 55);
    --secondary-foreground: oklch(0.26 0.026 40);
    --muted: oklch(0.925 0.011 60);
    --muted-foreground: oklch(0.45 0.022 45);
    --accent: oklch(0.64 0.13 76);
    --accent-foreground: oklch(0.19 0.03 50);
    --destructive: oklch(0.53 0.2 27);
    --destructive-foreground: oklch(0.98 0.006 75);
    --success: oklch(0.5 0.11 152);
    --success-foreground: oklch(0.98 0.006 75);
    --warning: oklch(0.62 0.14 68);
    --warning-foreground: oklch(0.2 0.03 50);
    --border: oklch(0.85 0.017 55);
    --input: oklch(0.85 0.017 55);
    --shadow-color: oklch(0 0 0 / 0.08);
  }
  :root[data-theme="dark"] {
    --background: oklch(0.125 0.011 35);
    --foreground: oklch(0.93 0.009 65);
    --card: oklch(0.165 0.014 38);
    --card-foreground: var(--foreground);
    --popover: oklch(0.195 0.015 38);
    --primary: oklch(0.56 0.19 23);
    --primary-foreground: oklch(0.98 0.006 75);
    --secondary: oklch(0.235 0.015 40);
    --secondary-foreground: oklch(0.86 0.012 60);
    --muted: oklch(0.195 0.013 40);
    --muted-foreground: oklch(0.6 0.017 50);
    --accent: oklch(0.72 0.12 78);
    --accent-foreground: oklch(0.16 0.02 45);
    --destructive: oklch(0.6 0.2 27);
    --destructive-foreground: oklch(0.98 0.006 75);
    --success: oklch(0.62 0.13 152);
    --success-foreground: oklch(0.13 0.02 152);
    --warning: oklch(0.68 0.14 68);
    --warning-foreground: oklch(0.18 0.03 50);
    --border: oklch(1 0 0 / 10%);
    --input: oklch(1 0 0 / 14%);
    --shadow-color: oklch(0 0 0 / 0.35);
  }

  * { box-sizing: border-box; }

  html { -webkit-text-size-adjust: 100%; }

  body {
    margin: 0;
    background: var(--background);
    color: var(--foreground);
    font-family: "Geist Variable", "Geist", ui-sans-serif, system-ui, sans-serif;
    line-height: 1.45;
    padding: 2.5rem 1.25rem 5rem;
  }

  .font-heading {
    font-family: "Oswald Variable", "Oswald", sans-serif;
    letter-spacing: -0.01em;
  }
  .font-mono {
    font-family: ui-monospace, "SFMono-Regular", "Geist Mono Variable", monospace;
  }
  .tabular { font-variant-numeric: tabular-nums; }
  .text-eyebrow {
    font-size: 0.6875rem;
    font-weight: 600;
    letter-spacing: 0.14em;
    text-transform: uppercase;
    color: var(--muted-foreground);
  }

  .wrap { max-width: 960px; margin: 0 auto; }

  /* ---- motif: sprocket-rule (lỗ phim 35mm) ---- */
  .sprocket-rule {
    height: 0.3rem;
    background-image: repeating-linear-gradient(to right, var(--border) 0 3px, transparent 3px 11px);
    background-position: center;
  }

  /* ---- motif: perforation (đường xé vé) ---- */
  .perforation {
    background-image: repeating-linear-gradient(to right, var(--border) 0 6px, transparent 6px 12px);
    background-repeat: no-repeat;
    height: 1px;
  }

  /* ---- motif: ticket-notch (khấc bấm lỗ vé) ---- */
  .ticket-notch { --notch-bg: var(--background); --notch-size: 1.25rem; position: relative; }
  .ticket-notch::before, .ticket-notch::after {
    content: ""; position: absolute; top: 50%; width: var(--notch-size); height: var(--notch-size);
    background: var(--notch-bg); border-radius: 9999px; transform: translateY(-50%); z-index: 1;
  }
  .ticket-notch::before { left: calc(var(--notch-size) / -2); }
  .ticket-notch::after { right: calc(var(--notch-size) / -2); }

  /* ---- motif: clip-corner (góc vé xé) ---- */
  .clip-corner {
    clip-path: polygon(0 0, 100% 0, 100% calc(100% - 0.5rem), calc(100% - 0.5rem) 100%, 0 100%);
  }

  a { color: inherit; }

  /* ================= Marquee header ================= */
  .marquee {
    background: var(--primary);
    color: var(--primary-foreground);
    border-radius: 0.39rem;
    padding: 1.5rem 1.75rem;
    display: flex;
    justify-content: space-between;
    align-items: flex-start;
    gap: 1rem;
    flex-wrap: wrap;
  }
  .marquee .text-eyebrow { color: color-mix(in oklch, var(--primary-foreground) 70%, transparent); }
  .marquee h1 {
    margin: 0.15rem 0 0.35rem;
    font-size: clamp(1.5rem, 3.2vw, 2.25rem);
    text-transform: uppercase;
    line-height: 1.05;
  }
  .marquee .subtitle { font-size: 0.9rem; opacity: 0.85; }
  .marquee .film-icon { opacity: 0.9; flex-shrink: 0; }

  .status-pill {
    font-family: "Oswald Variable", sans-serif;
    text-transform: uppercase;
    font-weight: 600;
    font-size: 0.85rem;
    letter-spacing: 0.04em;
    padding: 0.45rem 1rem 0.45rem 0.85rem;
    border-radius: 0.225rem;
  }
  .status-pill.pass { background: var(--success); color: var(--success-foreground); }
  .status-pill.fail { background: var(--destructive); color: var(--destructive-foreground); }

  /* ================= Ticket stub meta strip ================= */
  .stub {
    background: var(--card);
    color: var(--card-foreground);
    border: 1px solid var(--border);
    border-top: none;
    border-radius: 0 0 0.39rem 0.39rem;
    padding: 1rem 1.75rem 1.15rem;
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(120px, 1fr));
    gap: 1rem 1.5rem;
    margin-bottom: 1.75rem;
  }
  .stub .item .val { font-size: 1.05rem; margin-top: 0.15rem; }
  .stub .item .val.mono { font-family: ui-monospace, "SFMono-Regular", monospace; font-size: 0.9rem; }

  /* ================= Section ================= */
  section { margin-bottom: 2rem; }
  section > h2 {
    font-family: "Oswald Variable", sans-serif;
    text-transform: uppercase;
    font-size: 1rem;
    letter-spacing: 0.02em;
    margin: 0 0 0.15rem;
    display: flex;
    align-items: center;
    gap: 0.6rem;
  }
  section > h2 .idx {
    font-family: ui-monospace, monospace;
    font-size: 0.75rem;
    color: var(--muted-foreground);
    border: 1px solid var(--border);
    border-radius: 0.225rem;
    padding: 0.05rem 0.4rem;
  }
  section > .sprocket-rule { margin-bottom: 1rem; }

  /* ================= Cards grid (counters/gauges) ================= */
  .card-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(150px, 1fr)); gap: 0.85rem; }
  .stat-card {
    background: var(--card);
    border: 1px solid var(--border);
    border-radius: 0.39rem;
    padding: 0.9rem 1rem;
  }
  .stat-card .val {
    font-family: "Oswald Variable", sans-serif;
    font-size: 1.55rem;
    font-variant-numeric: tabular-nums;
    margin-top: 0.2rem;
  }
  .stat-card .unit { font-size: 0.75rem; color: var(--muted-foreground); margin-left: 0.25rem; }

  /* ================= Threshold list ================= */
  .list { background: var(--card); border: 1px solid var(--border); border-radius: 0.39rem; overflow: hidden; }
  .list-row {
    display: flex; align-items: center; justify-content: space-between; gap: 1rem;
    padding: 0.75rem 1.1rem;
  }
  .list-row + .list-row { border-top: none; position: relative; }
  .list-row + .list-row::before {
    content: ""; display: block; position: absolute; top: 0; left: 0; right: 0; height: 1px;
    background-image: repeating-linear-gradient(to right, var(--border) 0 6px, transparent 6px 12px);
  }
  .list-row .expr { font-family: ui-monospace, monospace; font-size: 0.82rem; color: var(--muted-foreground); }
  .badge {
    font-family: "Oswald Variable", sans-serif;
    text-transform: uppercase;
    font-weight: 600;
    font-size: 0.72rem;
    letter-spacing: 0.03em;
    padding: 0.25rem 0.65rem 0.25rem 0.5rem;
    border-radius: 0.225rem;
    white-space: nowrap;
  }
  .badge.pass { background: color-mix(in oklch, var(--success) 18%, var(--card)); color: var(--success); border: 1px solid color-mix(in oklch, var(--success) 40%, transparent); }
  .badge.fail { background: color-mix(in oklch, var(--destructive) 18%, var(--card)); color: var(--destructive); border: 1px solid color-mix(in oklch, var(--destructive) 40%, transparent); }

  /* ================= Checks ================= */
  .check-row .bar-track { background: var(--muted); border-radius: 999px; height: 6px; width: 100%; margin-top: 0.4rem; overflow: hidden; }
  .check-row .bar-fill { height: 100%; border-radius: 999px; }
  .check-row .bar-fill.pass { background: var(--success); }
  .check-row .bar-fill.fail { background: var(--destructive); }
  .check-row .head { display: flex; justify-content: space-between; gap: 1rem; align-items: baseline; }
  .check-row .name { font-weight: 500; }
  .check-row .nums { font-family: ui-monospace, monospace; font-size: 0.78rem; color: var(--muted-foreground); white-space: nowrap; }
  .check-row { padding: 0.85rem 1.1rem; }

  /* ================= Trend / timing table ================= */
  .trend-row { padding: 0.85rem 1.1rem; }
  .trend-row .head { display: flex; justify-content: space-between; align-items: baseline; gap: 1rem; flex-wrap: wrap; }
  .trend-row .name { font-weight: 500; }
  .trend-row .name .vi { color: var(--muted-foreground); font-weight: 400; font-size: 0.8rem; margin-left: 0.4rem; }
  .trend-row .stats { font-family: ui-monospace, monospace; font-size: 0.76rem; color: var(--muted-foreground); display: flex; gap: 0.9rem; flex-wrap: wrap; margin-top: 0.3rem; }
  .trend-row .stats b { color: var(--foreground); font-weight: 600; }
  .trend-row .scale { position: relative; height: 10px; background: var(--muted); border-radius: 999px; margin-top: 0.55rem; overflow: hidden; }
  .trend-row .scale .seg-avg { position: absolute; top: 0; bottom: 0; left: 0; background: color-mix(in oklch, var(--primary) 45%, transparent); }
  .trend-row .scale .mark-p95 { position: absolute; top: -2px; bottom: -2px; width: 2px; background: var(--accent); }
  .trend-row .scale .mark-max { position: absolute; top: 0; bottom: 0; width: 1px; background: var(--border); }

  /* ================= Footer ================= */
  footer { margin-top: 2.5rem; }
  footer .sprocket-rule { margin-bottom: 1rem; }
  footer .foot-text { display: flex; justify-content: space-between; gap: 1rem; flex-wrap: wrap; font-size: 0.78rem; color: var(--muted-foreground); }
  footer .foot-text .mono { font-family: ui-monospace, monospace; }
  footer .thanks { text-align: center; margin-top: 1.5rem; font-family: "Oswald Variable", sans-serif; text-transform: uppercase; letter-spacing: 0.08em; font-size: 0.8rem; color: var(--muted-foreground); }

  .theme-toggle {
    font-family: ui-monospace, monospace;
    font-size: 0.72rem;
    text-transform: uppercase;
    letter-spacing: 0.04em;
    background: var(--card);
    color: var(--foreground);
    border: 1px solid var(--border);
    border-radius: 0.225rem;
    padding: 0.35rem 0.65rem;
    cursor: pointer;
  }
  .toolbar { display: flex; justify-content: flex-end; margin-bottom: 0.75rem; }

  @media (max-width: 640px) {
    body { padding: 1.25rem 0.85rem 3rem; }
    .marquee { flex-direction: column; }
    .status-pill { align-self: flex-start; }
  }
`;

function renderThresholds(thresholds) {
  if (!thresholds.length) {
    return '<div class="list"><div class="list-row"><span class="expr">Không có threshold nào được cấu hình.</span></div></div>';
  }
  var rows = thresholds
    .map(function (t) {
      return (
        '<div class="list-row">' +
        '<div><div style="font-weight:500">' +
        esc(metricLabel(t.metric)) +
        '</div><div class="expr">' +
        esc(t.metric) +
        " &middot; " +
        esc(t.expr) +
        "</div></div>" +
        '<span class="badge ' +
        (t.ok ? "pass" : "fail") +
        '">' +
        (t.ok ? "Đạt" : "Không đạt") +
        "</span>" +
        "</div>"
      );
    })
    .join("");
  return '<div class="list">' + rows + "</div>";
}

function renderChecks(checks) {
  if (!checks.length) {
    return '<div class="list"><div class="list-row"><span class="expr">Không có check nào.</span></div></div>';
  }
  var rows = checks
    .map(function (c) {
      var passes = c.passes || 0;
      var fails = c.fails || 0;
      var total = passes + fails;
      var rate = total > 0 ? passes / total : 1;
      var ok = fails === 0;
      return (
        '<div class="list-row check-row" style="display:block;">' +
        '<div class="head"><span class="name">' +
        esc(c.name || c.path || "check") +
        '</span><span class="nums">' +
        fmtInt(passes) +
        " / " +
        fmtInt(total) +
        " &middot; " +
        fmtPercent(rate, 1) +
        "</span></div>" +
        '<div class="bar-track"><div class="bar-fill ' +
        (ok ? "pass" : "fail") +
        '" style="width:' +
        (rate * 100).toFixed(2) +
        '%"></div></div>' +
        "</div>"
      );
    })
    .join("");
  return '<div class="list">' + rows + "</div>";
}

function renderTrends(trendEntries) {
  if (!trendEntries.length) {
    return '<div class="list"><div class="list-row"><span class="expr">Không có dữ liệu thời gian.</span></div></div>';
  }
  var rows = trendEntries
    .map(function (entry) {
      var name = entry[0];
      var m = entry[1];
      var max = m.max || 0;
      var avgPct = max > 0 ? (m.avg / max) * 100 : 0;
      var p95 = m["p(95)"];
      var p95Pct =
        max > 0 && typeof p95 === "number" ? (p95 / max) * 100 : null;
      return (
        '<div class="list-row trend-row" style="display:block;">' +
        '<div class="head"><span class="name">' +
        esc(name) +
        '<span class="vi">' +
        esc(metricLabel(name)) +
        "</span></span></div>" +
        '<div class="stats">' +
        "<span>min <b>" +
        fmtMs(m.min) +
        "</b></span>" +
        "<span>avg <b>" +
        fmtMs(m.avg) +
        "</b></span>" +
        "<span>med <b>" +
        fmtMs(m.med) +
        "</b></span>" +
        "<span>p90 <b>" +
        fmtMs(m["p(90)"]) +
        "</b></span>" +
        "<span>p95 <b>" +
        fmtMs(m["p(95)"]) +
        "</b></span>" +
        "<span>max <b>" +
        fmtMs(m.max) +
        "</b></span>" +
        "</div>" +
        '<div class="scale"><div class="seg-avg" style="width:' +
        avgPct.toFixed(2) +
        '%"></div>' +
        (p95Pct !== null
          ? '<div class="mark-p95" style="left:' +
            p95Pct.toFixed(2) +
            '%"></div>'
          : "") +
        "</div>" +
        "</div>"
      );
    })
    .join("");
  return '<div class="list">' + rows + "</div>";
}

function renderStatCards(cards) {
  return (
    '<div class="card-grid">' +
    cards
      .map(function (c) {
        return (
          '<div class="stat-card"><div class="text-eyebrow">' +
          esc(c.label) +
          '</div><div class="val">' +
          esc(c.value) +
          (c.unit ? '<span class="unit">' + esc(c.unit) + "</span>" : "") +
          "</div></div>"
        );
      })
      .join("") +
    "</div>"
  );
}

export function cinemaHtmlReport(data, opts) {
  opts = opts || {};
  var title = opts.ticketTitle || "BÁO CÁO KIỂM THỬ TẢI";
  var scenarioFile = opts.scenarioFile || "";
  var metrics = data.metrics || {};
  var checks = collectChecks(data.root_group);
  var thresholds = collectThresholds(metrics);
  var overallOk = thresholds.every(function (t) {
    return t.ok;
  });

  var trendEntries = [];
  var counters = [];
  var gauges = [];
  var checksMetric = null;

  Object.keys(metrics)
    .filter(function (k) {
      return k.indexOf("{") === -1; // bỏ sub-metric có tag
    })
    .forEach(function (k) {
      var raw = metrics[k];
      if (!raw) return;
      var m = getValues(raw);
      if (k === "checks") {
        checksMetric = m;
        return;
      }
      if (isTrend(m)) trendEntries.push([k, m]);
      else if (isCounter(m)) counters.push([k, m]);
      else if (isGauge(m)) gauges.push([k, m]);
    });

  var totalChecksPasses = checks.reduce(function (a, c) {
    return a + (c.passes || 0);
  }, 0);
  var totalChecksFails = checks.reduce(function (a, c) {
    return a + (c.fails || 0);
  }, 0);
  var checksRate =
    checksMetric && typeof checksMetric.value === "number"
      ? checksMetric.value
      : totalChecksPasses + totalChecksFails > 0
        ? totalChecksPasses / (totalChecksPasses + totalChecksFails)
        : 1;

  var httpReqs = getValues(metrics.http_reqs);
  var httpFailed = getValues(metrics.http_req_failed);
  var httpDuration = getValues(metrics.http_req_duration);
  var vusMax = getValues(metrics.vus_max);
  var iterationsM = getValues(metrics.iterations);

  var runDurationMs =
    data.state && typeof data.state.testRunDurationMs === "number"
      ? data.state.testRunDurationMs
      : (function () {
          var stages =
            data.options && data.options.stages ? data.options.stages : [];
          return stages.reduce(function (sum, s) {
            return sum + parseDurationToMs(s.duration);
          }, 0);
        })();

  var generatedAt = new Date();
  var generatedAtStr = generatedAt.toLocaleString
    ? generatedAt.toISOString().replace("T", " ").slice(0, 19) + " UTC"
    : String(generatedAt);

  var heroCards = [
    { label: "Tổng requests", value: fmtInt(httpReqs.count) },
    { label: "Request rate", value: fmtFixed(httpReqs.rate, 1), unit: "req/s" },
    {
      label: "Tỷ lệ lỗi HTTP",
      value: fmtPercent(httpFailed.value || 0, 2),
    },
    { label: "p95 thời gian phản hồi", value: fmtMs(httpDuration["p(95)"]) },
    { label: "Checks đạt", value: fmtPercent(checksRate, 1) },
    { label: "VUs tối đa", value: fmtInt(vusMax.value || vusMax.max) },
  ];

  var trafficCards = counters
    .map(function (entry) {
      var k = entry[0];
      var m = entry[1];
      var value =
        k === "data_sent" || k === "data_received"
          ? fmtBytes(m.count)
          : fmtInt(m.count);
      var unit =
        k === "data_sent" || k === "data_received"
          ? undefined
          : fmtFixed(m.rate, 1) + "/s";
      return { label: metricLabel(k), value: value, unit: unit };
    })
    .concat(
      gauges
        .filter(function (entry) {
          return entry[0] !== "vus_max"; // đã hiển thị ở phần Tổng quan / cuống vé
        })
        .map(function (entry) {
          var k = entry[0];
          var m = entry[1];
          return { label: metricLabel(k), value: fmtInt(m.value) };
        }),
    );

  var ticketCode = (scenarioFile || "K6-RUN")
    .replace(/\.[jt]s$/i, "")
    .toUpperCase()
    .replace(/[^A-Z0-9]+/g, "-");

  var html =
    "<!doctype html>" +
    '<html lang="vi">' +
    "<head>" +
    '<meta charset="utf-8" />' +
    '<meta name="viewport" content="width=device-width, initial-scale=1" />' +
    "<title>" +
    esc(title) +
    " — Cinema Load Test</title>" +
    "<style>" +
    REPORT_CSS +
    "</style>" +
    "</head>" +
    "<body data-theme-body>" +
    '<div class="wrap">' +
    '<div class="toolbar"><button class="theme-toggle" id="themeToggle" type="button">' +
    '<svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" style="vertical-align:-2px;margin-right:0.35rem"><circle cx="12" cy="12" r="4"></circle><path d="M12 2v2M12 20v2M4.9 4.9l1.4 1.4M17.7 17.7l1.4 1.4M2 12h2M20 12h2M4.9 19.1l1.4-1.4M17.7 6.3l1.4-1.4"></path></svg>' +
    "Đổi giao diện Sáng/Tối</button></div>" +
    '<header class="marquee">' +
    "<div>" +
    '<div class="text-eyebrow">Cinema &middot; Load Test Report</div>' +
    "<h1>" +
    esc(title) +
    "</h1>" +
    '<div class="subtitle">' +
    (scenarioFile ? esc(scenarioFile) + " &middot; " : "") +
    esc(generatedAtStr) +
    "</div>" +
    "</div>" +
    '<span class="status-pill clip-corner ' +
    (overallOk ? "pass" : "fail") +
    '">' +
    (overallOk ? "✓ Đạt ngưỡng" : "✕ Vượt ngưỡng") +
    "</span>" +
    "</header>" +
    '<div class="stub ticket-notch">' +
    '<div class="item"><div class="text-eyebrow">Mã vé</div><div class="val mono font-mono">' +
    esc(ticketCode) +
    "-" +
    generatedAt.getTime().toString().slice(-6) +
    "</div></div>" +
    '<div class="item"><div class="text-eyebrow">Thời lượng kịch bản</div><div class="val tabular">' +
    (runDurationMs ? fmtMs(runDurationMs) : "—") +
    "</div></div>" +
    '<div class="item"><div class="text-eyebrow">VUs tối đa</div><div class="val tabular">' +
    fmtInt(vusMax.value || vusMax.max) +
    "</div></div>" +
    '<div class="item"><div class="text-eyebrow">Iterations</div><div class="val tabular">' +
    fmtInt(iterationsM.count) +
    "</div></div>" +
    '<div class="item"><div class="text-eyebrow">Checks</div><div class="val tabular">' +
    fmtInt(totalChecksPasses) +
    "/" +
    fmtInt(totalChecksPasses + totalChecksFails) +
    "</div></div>" +
    "</div>" +
    "<section>" +
    '<h2><span class="idx">00</span> Tổng quan</h2>' +
    '<div class="sprocket-rule"></div>' +
    renderStatCards(heroCards) +
    "</section>" +
    "<section>" +
    '<h2><span class="idx">01</span> Ngưỡng kiểm thử (Thresholds)</h2>' +
    '<div class="sprocket-rule"></div>' +
    renderThresholds(thresholds) +
    "</section>" +
    "<section>" +
    '<h2><span class="idx">02</span> Checks</h2>' +
    '<div class="sprocket-rule"></div>' +
    renderChecks(checks) +
    "</section>" +
    "<section>" +
    '<h2><span class="idx">03</span> Chi tiết thời gian phản hồi</h2>' +
    '<div class="sprocket-rule"></div>' +
    renderTrends(trendEntries) +
    "</section>" +
    "<section>" +
    '<h2><span class="idx">04</span> Lưu lượng &amp; tài nguyên</h2>' +
    '<div class="sprocket-rule"></div>' +
    renderStatCards(trafficCards) +
    "</section>" +
    "<footer>" +
    '<div class="sprocket-rule"></div>' +
    '<div class="foot-text">' +
    '<span>Sinh tự động bởi k6 &middot; script <span class="mono">' +
    esc(scenarioFile || "—") +
    "</span></span>" +
    '<span class="mono">' +
    esc(generatedAtStr) +
    "</span>" +
    "</div>" +
    '<div class="thanks">— cảm ơn quý khách đã ghé rạp kiểm thử —</div>' +
    "</footer>" +
    "</div>" +
    "<script>" +
    "(function(){" +
    "var root=document.documentElement;" +
    'var saved="dark";' +
    'root.setAttribute("data-theme", saved);' +
    'document.getElementById("themeToggle").addEventListener("click", function(){' +
    'var cur = root.getAttribute("data-theme");' +
    'root.setAttribute("data-theme", cur === "dark" ? "light" : "dark");' +
    "});" +
    "})();" +
    "</script>" +
    "</body>" +
    "</html>";

  return html;
}
