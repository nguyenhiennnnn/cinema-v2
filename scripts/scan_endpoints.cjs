const fs = require('fs');
const path = require('path');

function getFiles(dir) {
  let results = [];
  const list = fs.readdirSync(dir);
  list.forEach(file => {
    file = path.join(dir, file);
    const stat = fs.statSync(file);
    if (stat && stat.isDirectory()) results = results.concat(getFiles(file));
    else if (file.endsWith('Controller.cs')) results.push(file);
  });
  return results;
}

const files = getFiles('backend/Cinema.Api/Controllers');
let total = 0;
const report = [];

files.sort().forEach(f => {
  const content = fs.readFileSync(f, 'utf8');
  const controllerName = path.basename(f, '.cs');
  const routeMatch = content.match(/\[Route\("([^"]+)"\)\]/);
  const routePrefix = routeMatch ? routeMatch[1] : '';

  let classPolicy = 'None';
  const classAuthMatch = content.match(/\[Authorize(?:\(Policy\s*=\s*Policies\.([A-Za-z0-9_]+)\))?\]\s*(?:public|internal)\s+class/);
  if (classAuthMatch) {
    classPolicy = classAuthMatch[1] || 'Authorize';
  }

  const lines = content.split('\n');
  const endpoints = [];

  for (let i = 0; i < lines.length; i++) {
    const line = lines[i];
    // Any public [async] method
    const methodMatch = line.match(/public\s+(?:async\s+)?[A-Za-z0-9_<>,\.\?\s]+\s+([A-Za-z0-9_]+)\s*\(/);
    if (methodMatch) {
      const actionName = methodMatch[1];
      if (['Equals', 'GetHashCode', 'GetType', 'ToString'].includes(actionName)) continue;

      // look backwards for attributes
      const attrs = [];
      for (let j = i - 1; j >= 0; j--) {
        const prev = lines[j].trim();
        if (prev.startsWith('[') && prev.endsWith(']')) {
          attrs.unshift(prev);
        } else if (prev === '' || prev.startsWith('//')) {
          continue;
        } else {
          break;
        }
      }

      let verb = null;
      let subRoute = '';
      let policy = classPolicy;
      let rateLimit = '';

      for (const attr of attrs) {
        // match multiple attributes in one line or separate lines, e.g. [HttpGet, HttpPost]
        const verbMatches = [...attr.matchAll(/\[?Http(Get|Post|Put|Delete|Patch)(?:\("?([^"]*)"?\))?\]?/g)];
        if (verbMatches.length > 0) {
          for (const vm of verbMatches) {
            verb = vm[1].toUpperCase();
            subRoute = vm[2] || '';
          }
        }
        const polM = attr.match(/\[Authorize\s*\(\s*(?:Policy\s*=\s*)?Policies\.([A-Za-z0-9_]+)\s*\)\]/);
        if (polM) {
          policy = polM[1];
        } else if (attr === '[Authorize]') {
          policy = 'Authorize';
        } else if (attr === '[AllowAnonymous]') {
          policy = 'AllowAnonymous';
        }
        const rlM = attr.match(/EnableRateLimiting\("([^"]+)"\)/);
        if (rlM) {
          rateLimit = rlM[1];
        }
      }

      // Check if method has [HttpGet] or similar
      if (verb) {
        let fullRoute = routePrefix;
        if (subRoute) {
          if (subRoute.startsWith('/')) fullRoute = subRoute;
          else fullRoute = (fullRoute.endsWith('/') ? fullRoute : fullRoute + '/') + subRoute;
        }
        endpoints.push({ verb, route: fullRoute, action: actionName, policy, rateLimit });
        total++;
      }
    }
  }
  report.push({ controllerName, endpoints });
});

console.log('TOTAL ENDPOINTS:', total);
report.forEach(r => {
  console.log('\n### ' + r.controllerName);
  console.log('| Method | Route | Policy | Khách | Customer | Employee | Admin | Rate limit | Hàm |');
  console.log('|---|---|---|:-:|:-:|:-:|:-:|---|---|');
  r.endpoints.forEach(e => {
    let k = '–', c = '–', emp = '–', a = '–';
    if (e.policy === 'None' || e.policy === 'AllowAnonymous') {
      k = '✓'; c = '✓'; emp = '✓'; a = '✓';
    } else if (e.policy === 'CustomerOnly') {
      c = '✓';
    } else if (e.policy === 'StaffOnly') {
      emp = '✓'; a = '✓';
    } else if (e.policy === 'AdminOnly') {
      a = '✓';
    } else if (e.policy === 'Authorize') {
      c = '✓'; emp = '✓'; a = '✓';
    }
    const polDisplay = (e.policy === 'None' ? 'Công khai (không attribute, không có FallbackPolicy)' : e.policy);
    console.log(`| ${e.verb} | \`/${e.route.replace(/^\//, '')}\` | ${polDisplay} | ${k} | ${c} | ${emp} | ${a} | ${e.rateLimit} | \`${e.action}\` |`);
  });
});
