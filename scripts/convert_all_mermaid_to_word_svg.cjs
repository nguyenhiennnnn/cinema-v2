const { chromium } = require('../frontend/node_modules/playwright');
const fs = require('fs');
const path = require('path');

const MERMAID_DIR = path.resolve(__dirname, '../docs/so-do/mermaid');
const OUTPUT_DIR = path.resolve(__dirname, '../docs/so-do/anh-xem-truoc-svg');

async function convertAll() {
  const browser = await chromium.launch({ headless: true });
  const page = await browser.newPage();

  await page.setContent(`
    <!DOCTYPE html>
    <html>
      <head>
        <meta charset="utf-8" />
        <style>
          * {
            box-sizing: border-box;
            font-family: 'Times New Roman', serif !important;
          }
        </style>
        <script src="https://cdn.jsdelivr.net/npm/mermaid@10/dist/mermaid.min.js"></script>
        <script>
          mermaid.initialize({
            startOnLoad: false,
            theme: 'neutral',
            fontFamily: 'Times New Roman',
            securityLevel: 'loose',
            flowchart: {
              htmlLabels: true,
              useMaxWidth: false
            }
          });
        </script>
      </head>
      <body>
        <div id="container"></div>
      </body>
    </html>
  `);

  const files = fs.readdirSync(MERMAID_DIR).filter(f => f.endsWith('.mmd'));
  console.log(`Found ${files.length} Mermaid files to process.`);

  const summary = [];

  for (const filename of files) {
    const baseName = path.basename(filename, '.mmd');
    const mmdPath = path.join(MERMAID_DIR, filename);
    const outSvgPath = path.join(OUTPUT_DIR, `${baseName}.svg`);
    const code = fs.readFileSync(mmdPath, 'utf-8');

    const result = await page.evaluate(async ({ code, baseName }) => {
      try {
        const uniqueId = `mermaid_${baseName.replace(/[^a-zA-Z0-9]/g, '_')}_${Date.now()}`;
        const { svg } = await mermaid.render(uniqueId, code);
        const container = document.getElementById('container');
        container.innerHTML = svg;
        const svgEl = container.querySelector('svg');

        if (!svgEl) {
          return { error: 'No SVG element generated' };
        }

        // Set explicit Times New Roman on SVG root
        svgEl.setAttribute('font-family', 'Times New Roman');

        // 1. Transform all foreignObjects into native SVG <text> elements
        const foreignObjects = Array.from(svgEl.querySelectorAll('foreignObject'));
        let convertedFoCount = 0;
        let removedEmptyCount = 0;

        for (const fo of foreignObjects) {
          const foX = parseFloat(fo.getAttribute('x') || 0);
          const foY = parseFloat(fo.getAttribute('y') || 0);
          const foWidth = parseFloat(fo.getAttribute('width') || 0);
          const foHeight = parseFloat(fo.getAttribute('height') || 0);

          const div = fo.querySelector('div') || fo;
          const computedStyle = window.getComputedStyle(div);
          const fontSize = parseFloat(computedStyle.fontSize) || 14;
          const fontWeight = computedStyle.fontWeight;
          const isBold = fontWeight === 'bold' || parseInt(fontWeight) >= 600;
          const color = computedStyle.color || '#000000';
          const textAlign = computedStyle.textAlign || 'center';

          // Extract text lines, handling <br>, block elements, and whitespace
          let html = div.innerHTML;
          html = html.replace(/<br\s*\/?>/gi, '\n');
          html = html.replace(/<\/(div|p|li|h[1-6])>/gi, '\n');
          html = html.replace(/<[^>]+>/g, '');

          // Decode HTML entities (e.g. &le; -> ≤, &rarr; -> →, &amp; -> &)
          const decoder = document.createElement('textarea');
          decoder.innerHTML = html;
          const decoded = decoder.value;

          const lines = decoded
            .split('\n')
            .map(l => l.trim())
            .filter(l => l.length > 0);

          if (lines.length === 0) {
            // Remove empty foreignObject (e.g. unlabeled edge labels)
            fo.remove();
            removedEmptyCount++;
            continue;
          }

          // Build SVG <text>
          const textEl = document.createElementNS('http://www.w3.org/2000/svg', 'text');
          textEl.setAttribute('font-family', 'Times New Roman');
          textEl.setAttribute('font-size', `${fontSize}px`);
          if (isBold) {
            textEl.setAttribute('font-weight', 'bold');
          }

          const resolvedColor = (color === 'rgb(0, 0, 0)' || !color) ? '#181818' : color;
          textEl.setAttribute('fill', resolvedColor);

          let textAnchor = 'middle';
          let startX = foX + foWidth / 2;
          if (textAlign === 'left') {
            textAnchor = 'start';
            startX = foX + 5;
          } else if (textAlign === 'right') {
            textAnchor = 'end';
            startX = foX + foWidth - 5;
          }
          textEl.setAttribute('text-anchor', textAnchor);

          const lineHeight = fontSize * 1.25;
          const totalTextHeight = (lines.length - 1) * lineHeight + fontSize;
          const startY = foY + Math.max(0, (foHeight - totalTextHeight) / 2) + fontSize * 0.82;

          lines.forEach((lineText, idx) => {
            const tspan = document.createElementNS('http://www.w3.org/2000/svg', 'tspan');
            tspan.setAttribute('x', startX.toString());
            tspan.setAttribute('y', (startY + idx * lineHeight).toFixed(2));
            tspan.textContent = lineText;
            textEl.appendChild(tspan);
          });

          fo.parentNode.replaceChild(textEl, fo);
          convertedFoCount++;
        }

        // Remove empty <rect></rect> inside <g class="label"> (Mermaid artifact)
        const labelRects = Array.from(svgEl.querySelectorAll('.label > rect'));
        for (const lr of labelRects) {
          if (!lr.getAttribute('width') || lr.getAttribute('width') === '0') {
            lr.remove();
          }
        }

        // 2. CRITICAL FIX: Inline styles & presentation attributes for ALL Edge Paths (Lines)
        // Word's SVG renderer ignores CSS class rules like .flowchart-link { stroke: #666; }
        // Setting both attributes and style guarantees lines are 100% visible in Word!
        const edgePaths = Array.from(svgEl.querySelectorAll('.edgePaths path, path.flowchart-link, .edgePath path'));
        let edgePathCount = 0;
        for (const p of edgePaths) {
          const cs = window.getComputedStyle(p);
          let stroke = cs.stroke;
          if (!stroke || stroke === 'none' || stroke === 'rgb(0, 0, 0)' || stroke === 'rgb(102, 102, 102)') {
            stroke = '#333333'; // clean, crisp dark gray line
          }
          let strokeWidth = cs.strokeWidth;
          if (!strokeWidth || strokeWidth === '0px') {
            strokeWidth = '1.5px';
          }
          let strokeDasharray = cs.strokeDasharray;
          if (strokeDasharray === 'none' || strokeDasharray === '0px') {
            strokeDasharray = '';
          }

          p.setAttribute('stroke', stroke);
          p.setAttribute('stroke-width', strokeWidth);
          p.setAttribute('fill', 'none');
          if (strokeDasharray) {
            p.setAttribute('stroke-dasharray', strokeDasharray);
          }
          p.setAttribute('style', `stroke: ${stroke}; stroke-width: ${strokeWidth}; fill: none;${strokeDasharray ? ` stroke-dasharray: ${strokeDasharray};` : ''}`);
          edgePathCount++;
        }

        // 3. CRITICAL FIX: Inline styles for Markers (Arrowheads)
        const markers = Array.from(svgEl.querySelectorAll('marker'));
        for (const m of markers) {
          const markerChildren = Array.from(m.querySelectorAll('path, circle, polygon, rect'));
          for (const mc of markerChildren) {
            mc.setAttribute('fill', '#333333');
            mc.setAttribute('stroke', '#333333');
            mc.setAttribute('stroke-width', '1');
            mc.setAttribute('style', 'fill: #333333; stroke: #333333; stroke-width: 1;');
          }
        }

        // 4. Inline styles for Node shapes (rect, polygon, circle, ellipse, path)
        const nodeShapes = Array.from(svgEl.querySelectorAll('.nodes .node > rect, .nodes .node > polygon, .nodes .node > circle, .nodes .node > ellipse, .nodes .node > path, .nodes .node [class*="label-container"]'));
        for (const s of nodeShapes) {
          const cs = window.getComputedStyle(s);
          let fill = cs.fill;
          if (!fill || fill === 'none') fill = '#f8f8f8';
          let stroke = cs.stroke;
          if (!stroke || stroke === 'none') stroke = '#555555';
          let strokeWidth = cs.strokeWidth;
          if (!strokeWidth || strokeWidth === '0px') strokeWidth = '1.2px';

          s.setAttribute('fill', fill);
          s.setAttribute('stroke', stroke);
          s.setAttribute('stroke-width', strokeWidth);
          s.setAttribute('style', `fill: ${fill}; stroke: ${stroke}; stroke-width: ${strokeWidth};`);
        }

        // 5. Inline styles for Clusters/subgraphs
        const clusterShapes = Array.from(svgEl.querySelectorAll('.clusters rect, .clusters polygon, .cluster rect, .cluster polygon'));
        for (const cs of clusterShapes) {
          const style = window.getComputedStyle(cs);
          let fill = style.fill;
          if (!fill || fill === 'none') fill = '#fafafa';
          let stroke = style.stroke;
          if (!stroke || stroke === 'none') stroke = '#777777';
          let strokeWidth = style.strokeWidth;
          if (!strokeWidth || strokeWidth === '0px') strokeWidth = '1px';

          cs.setAttribute('fill', fill);
          cs.setAttribute('stroke', stroke);
          cs.setAttribute('stroke-width', strokeWidth);
          cs.setAttribute('style', `fill: ${fill}; stroke: ${stroke}; stroke-width: ${strokeWidth};`);
        }

        // 6. Inline styles for Edge label backgrounds (white mask behind label text)
        const edgeLabelRects = Array.from(svgEl.querySelectorAll('.edgeLabel rect'));
        for (const r of edgeLabelRects) {
          r.setAttribute('fill', '#ffffff');
          r.setAttribute('fill-opacity', '0.9');
          r.setAttribute('stroke', 'none');
          r.setAttribute('style', 'fill: #ffffff; fill-opacity: 0.9; stroke: none;');
        }

        // 7. Process lines & paths in Gantt charts or other elements
        const lines = Array.from(svgEl.querySelectorAll('line'));
        for (const l of lines) {
          const cs = window.getComputedStyle(l);
          let stroke = cs.stroke;
          if (!stroke || stroke === 'none' || stroke === 'currentColor') {
            stroke = '#555555';
          }
          let strokeWidth = cs.strokeWidth;
          if (!strokeWidth || strokeWidth === '0px') strokeWidth = '1px';

          l.setAttribute('stroke', stroke);
          l.setAttribute('stroke-width', strokeWidth);
          l.setAttribute('style', `stroke: ${stroke}; stroke-width: ${strokeWidth};`);
        }

        // 8. Ensure all text and tspan have Times New Roman
        const allTexts = Array.from(svgEl.querySelectorAll('text, tspan'));
        for (const t of allTexts) {
          t.setAttribute('font-family', 'Times New Roman');
          if (!t.getAttribute('fill')) {
            t.setAttribute('fill', '#181818');
          }
        }

        return {
          svg: container.innerHTML,
          convertedFoCount,
          removedEmptyCount,
          edgePathCount,
          finalTextCount: allTexts.length,
          hasFo: container.querySelector('foreignObject') !== null
        };
      } catch (err) {
        return { error: err.message };
      }
    }, { code, baseName });

    if (result.error) {
      console.error(`ERROR rendering ${filename}:`, result.error);
      summary.push({ file: filename, status: 'ERROR', error: result.error });
    } else {
      fs.writeFileSync(outSvgPath, result.svg, 'utf-8');
      console.log(`[OK] ${filename} -> ${baseName}.svg: Lines=${result.edgePathCount}, Texts=${result.finalTextCount}, hasFo=${result.hasFo}`);
      summary.push({
        file: filename,
        status: 'OK',
        edgePaths: result.edgePathCount,
        texts: result.finalTextCount,
        hasFo: result.hasFo
      });
    }
  }

  await browser.close();
  console.log('\n--- Batch Conversion Complete ---');
  console.table(summary);
}

convertAll().catch(err => {
  console.error('Fatal error:', err);
  process.exit(1);
});
