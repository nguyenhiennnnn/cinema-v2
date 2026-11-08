import fs from 'fs';
import zlib from 'zlib';

const html = fs.readFileSync('frontend/playwright-report/index.html', 'utf8');
const templateMatch = html.match(/<template[^>]*>([\s\S]*?)<\/template>/i);
if (templateMatch) {
  const base64 = templateMatch[1].trim();
  const zipBuf = Buffer.from(base64, 'base64');
  fs.writeFileSync('frontend/playwright-report/report.zip', zipBuf);
  console.log('Saved report.zip, size:', zipBuf.length);
} else {
  console.log('No template match');
}
