const fs = require('fs'), path = require('path');
const D = require('docx');
let sharp; try { sharp = require('sharp'); } catch (e) { sharp = require('/home/claude/.npm-global/lib/node_modules/sharp'); }

const ROOT = '/home/claude/pkg';
const FONT = 'Arial', MONO = 'Consolas';
const F = { ascii: FONT, hAnsi: FONT, cs: FONT, eastAsia: FONT };
const FM = { ascii: MONO, hAnsi: MONO, cs: MONO, eastAsia: MONO };
const AR = /[\u0600-\u06FF\u0750-\u077F]/;
const hasAr = s => AR.test(s);
const NAVY = '1F3864', TEAL = '1B6E7E';

// ---------- inline parsing ----------
function groupScript(text, rtlPara) {
  const parts = text.split(/(\s+)/).filter(x => x.length);
  const groups = [];
  let cur = null;
  for (const p of parts) {
    if (/^\s+$/.test(p)) { if (cur) cur.text += p; else groups.push(cur = { rtl: rtlPara, text: p }); continue; }
    let rtl;
    if (hasAr(p)) rtl = true;
    else if (/[A-Za-z0-9]/.test(p)) rtl = false;
    else rtl = cur ? cur.rtl : rtlPara;
    if (cur && cur.rtl === rtl) cur.text += p; else groups.push(cur = { rtl, text: p });
  }
  return groups;
}
function inline(text, o = {}) {
  const size = o.size || 21, rtlPara = o.rtl;
  const runs = [];
  const toks = text.split(/(`[^`]+`|\*\*[^*]+\*\*)/).filter(x => x !== '');
  for (const t of toks) {
    if (t.startsWith('`') && t.endsWith('`') && t.length > 1) {
      runs.push(new D.TextRun({ text: t.slice(1, -1), font: FM, size: size - 2, color: o.color || '8B1A1A', shading: { type: D.ShadingType.CLEAR, fill: 'F1F1F1', color: 'auto' } }));
    } else if (t.startsWith('**') && t.endsWith('**') && t.length > 4) {
      runs.push(...inline(t.slice(2, -2), { ...o, bold: true }));
    } else {
      for (const g of groupScript(t, rtlPara)) {
        runs.push(new D.TextRun({ text: (!g.rtl && /^\s*\./.test(g.text) ? '\u200E' : '') + g.text, font: F, size, bold: !!o.bold, italics: !!o.italics, color: o.color, rightToLeft: g.rtl }));
      }
    }
  }
  return runs;
}
const strip = s => s.replace(/`/g, '').replace(/\*\*/g, '');

// ---------- state ----------
let numCount = 0; const numberingConfigs = [];
let hid = 0; const tocEntries = [];
const sections = []; let cur = null;
const sizes = {};

function newSection(orient) {
  cur = { orient, children: [] }; sections.push(cur); return cur;
}
const add = (...c) => cur.children.push(...c);

function P(text, o = {}) {
  const rtl = hasAr(text);
  return new D.Paragraph({
    bidirectional: rtl, spacing: { after: o.after ?? 100, before: o.before ?? 0, line: o.line ?? 300, lineRule: D.LineRuleType.AUTO },
    children: inline(text, { rtl, size: o.size, bold: o.bold, color: o.color }), keepNext: o.keepNext,
    shading: o.fill ? { type: D.ShadingType.CLEAR, fill: o.fill, color: 'auto' } : undefined,
    indent: o.indent, alignment: o.align
  });
}
function Heading(level, text) {
  const id = 'h' + (++hid);
  const rtl = hasAr(text);
  const first = cur.children.length === 0;
  if (level <= 2) tocEntries.push({ level, text, id });
  add(new D.Paragraph({
    heading: [D.HeadingLevel.HEADING_1, D.HeadingLevel.HEADING_2, D.HeadingLevel.HEADING_3][level - 1],
    bidirectional: true, pageBreakBefore: level === 1 && !first, keepNext: true,
    children: [new D.Bookmark({ id, children: inline(text, { rtl: true, size: [36, 28, 24][level - 1], bold: true, color: level === 3 ? TEAL : NAVY }) })]
  }));
}
const BORDER = { style: D.BorderStyle.SINGLE, size: 4, color: 'BFBFBF' };
const BORDERS = { top: BORDER, bottom: BORDER, left: BORDER, right: BORDER };

function Table(rows) {
  const header = rows[0], body = rows.slice(1);
  const nCols = header.length;
  const total = 9638;
  const isKV = nCols === 2 && /البند/.test(header[0]);
  let widths;
  if (isKV) widths = [2300, total - 2300];
  else {
    const score = header.map((_, c) => {
      const lens = rows.map(r => strip((r[c] || '')).replace(/<br>/g, ' ').length);
      const avg = lens.reduce((a, b) => a + b, 0) / lens.length, mx = Math.max(...lens);
      return Math.min(60, Math.max(7, avg * 0.6 + mx * 0.4));
    });
    const s = score.reduce((a, b) => a + b, 0);
    const minW = header.map((_, c) => {
      let mw = 0;
      for (const r of rows) for (const seg of strip(r[c] || '').split(/<br\s*\/?>/)) for (const w of seg.split(/\s+/)) mw = Math.max(mw, w.length);
      return Math.min(2400, mw * (nCols >= 9 ? 82 : 98) + 260);
    });
    widths = score.map((x, c) => Math.max(minW[c], Math.round(total * x / s)));
    let diff = total - widths.reduce((a, b) => a + b, 0);
    let guard = 0;
    while (diff !== 0 && guard++ < 50) {
      const idx = diff > 0 ? widths.indexOf(Math.max(...widths)) : widths.map((w, c) => w - minW[c]).reduce((bi, v, c, a) => v > a[bi] ? c : bi, 0);
      const adj = diff > 0 ? diff : -Math.min(-diff, Math.max(0, widths[idx] - minW[idx]));
      if (adj === 0) break;
      widths[idx] += adj; diff -= adj;
    }
  }
  const fs_ = nCols >= 9 ? 15 : nCols >= 6 ? 17 : 18;
  const mkCell = (txt, c, isHead, rIdx) => {
    const paras = String(txt).split(/<br\s*\/?>/).map(seg => {
      const rtl = true;
      return new D.Paragraph({
        bidirectional: true, spacing: { after: 30, before: 30, line: 260, lineRule: D.LineRuleType.AUTO },
        alignment: (!hasAr(seg) && nCols >= 9) ? D.AlignmentType.CENTER : undefined,
        children: inline(seg, { rtl, size: fs_, bold: isHead || (isKV && c === 0), color: isHead ? 'FFFFFF' : undefined })
      });
    });
    return new D.TableCell({
      width: { size: widths[c], type: D.WidthType.DXA }, borders: BORDERS,
      margins: { top: 40, bottom: 40, left: 90, right: 90 }, verticalAlign: D.VerticalAlign.CENTER,
      shading: isHead ? { type: D.ShadingType.CLEAR, fill: NAVY, color: 'auto' } : (isKV && c === 0 ? { type: D.ShadingType.CLEAR, fill: 'EAF0F8', color: 'auto' } : (rIdx % 2 === 1 && !isKV ? { type: D.ShadingType.CLEAR, fill: 'F7F9FC', color: 'auto' } : undefined)),
      children: paras
    });
  };
  const tr = [new D.TableRow({ tableHeader: true, cantSplit: true, children: header.map((h, c) => mkCell(h, c, true, 0)) })];
  body.forEach((r, i) => {
    const cells = []; for (let c = 0; c < nCols; c++) cells.push(mkCell(r[c] ?? '', c, false, i));
    tr.push(new D.TableRow({ cantSplit: true, children: cells }));
  });
  add(new D.Table({ width: { size: total, type: D.WidthType.DXA }, columnWidths: widths, rows: tr, visuallyRightToLeft: true, layout: D.TableLayoutType.FIXED }));
  add(new D.Paragraph({ spacing: { after: 120 }, children: [] }));
}

function Code(lines) {
  lines.forEach((ln, i) => {
    add(new D.Paragraph({
      alignment: D.AlignmentType.LEFT, keepLines: true,
      spacing: { before: i === 0 ? 80 : 0, after: i === lines.length - 1 ? 140 : 0, line: 245, lineRule: D.LineRuleType.AUTO },
      shading: { type: D.ShadingType.CLEAR, fill: 'F3F4F6', color: 'auto' }, indent: { left: 100, right: 100 },
      children: [new D.TextRun({ text: ln.replace(/\t/g, '  ') || ' ', font: FM, size: 16, color: '1F2937' })]
    }));
  });
}
function List(items, ordered) {
  let ref = 'bul';
  if (ordered) {
    ref = 'num' + (++numCount);
    numberingConfigs.push({ reference: ref, levels: [{ level: 0, format: D.LevelFormat.DECIMAL, text: '%1.', alignment: D.AlignmentType.START, style: { paragraph: { indent: { left: 540, hanging: 360 } } } }] });
  }
  for (const it of items) {
    const rtl = hasAr(it);
    add(new D.Paragraph({ numbering: { reference: ref, level: 0 }, bidirectional: rtl, spacing: { after: 70, line: 290, lineRule: D.LineRuleType.AUTO }, children: inline(it, { rtl }) }));
  }
}
function Quote(text) {
  const rtl = hasAr(text);
  add(new D.Paragraph({
    bidirectional: rtl, spacing: { before: 80, after: 140, line: 300, lineRule: D.LineRuleType.AUTO },
    shading: { type: D.ShadingType.CLEAR, fill: 'FFF6DD', color: 'auto' }, indent: { left: 200, right: 200 },
    border: { top: { style: D.BorderStyle.SINGLE, size: 4, color: 'F0C36D', space: 4 }, bottom: { style: D.BorderStyle.SINGLE, size: 4, color: 'F0C36D', space: 4 } },
    children: inline(text, { rtl })
  }));
}

// ---------- markdown block parser ----------
function parseBlocks(md) {
  const L = md.split('\n'); const out = []; let i = 0;
  while (i < L.length) {
    let ln = L[i];
    if (!ln.trim() || ln.trim() === '---pagebreak---') { i++; continue; }
    if (ln.startsWith('@@')) {
      const [id, label, title] = ln.slice(2).split('|').map(s => s.trim());
      const body = []; i++;
      while (i < L.length && !L[i].startsWith('@@') && L[i].trim() !== '---pagebreak---') body.push(L[i++]);
      out.push({ t: 'diagram', id, label, title, body: body.join('\n') }); continue;
    }
    if (ln.startsWith('```')) { const code = []; i++; while (i < L.length && !L[i].startsWith('```')) code.push(L[i++]); i++; out.push({ t: 'code', lines: code }); continue; }
    let m;
    if ((m = ln.match(/^(#{1,3}) (.*)$/))) { out.push({ t: 'h', level: m[1].length, text: m[2].trim() }); i++; continue; }
    if (ln.startsWith('|')) {
      const rows = [];
      while (i < L.length && L[i].startsWith('|')) {
        const cells = L[i].trim().replace(/^\|/, '').replace(/\|$/, '').split('|').map(s => s.trim());
        if (!cells.every(c => /^:?-{2,}:?$/.test(c))) rows.push(cells); i++;
      }
      out.push({ t: 'table', rows }); continue;
    }
    if (ln.startsWith('> ')) { const q = []; while (i < L.length && L[i].startsWith('>')) q.push(L[i++].replace(/^>\s?/, '')); out.push({ t: 'quote', text: q.join(' ') }); continue; }
    if (/^- /.test(ln)) { const it = []; while (i < L.length && /^- /.test(L[i])) it.push(L[i++].slice(2)); out.push({ t: 'ul', items: it }); continue; }
    if (/^\d+\. /.test(ln)) { const it = []; while (i < L.length && /^\d+\. /.test(L[i])) it.push(L[i++].replace(/^\d+\. /, '')); out.push({ t: 'ol', items: it }); continue; }
    out.push({ t: 'p', text: ln.trim() }); i++;
  }
  return out;
}

function emitBlocks(blocks) {
  for (const b of blocks) {
    if (b.t === 'h') Heading(b.level, b.text);
    else if (b.t === 'p') add(P(b.text));
    else if (b.t === 'ul') List(b.items, false);
    else if (b.t === 'ol') List(b.items, true);
    else if (b.t === 'quote') Quote(b.text);
    else if (b.t === 'table') Table(b.rows);
    else if (b.t === 'code') Code(b.lines);
    else if (b.t === 'diagram') emitDiagram(b);
  }
}

// ---------- diagrams ----------
function emitDiagram(b) {
  const sz = sizes[b.id]; if (!sz) throw new Error('no image ' + b.id);
  const ratio = sz.w / sz.h;
  const landscape = ratio >= 1.15;
  const expl = parseBlocks(b.body);
  const explLen = b.body.length;
  const attach = landscape && ratio >= 2.0 && explLen < 650;
  let maxW, maxH;
  if (landscape) { maxW = 10.2; maxH = attach ? 4.2 : 5.9; } else { maxW = 6.5; maxH = 8.4; }
  let dw = Math.min(maxW, maxH * ratio), dh = dw / ratio;
  newSection(landscape ? 'L' : 'P');
  Heading(2, `${b.label}: ${b.title}`);
  const ext = fs.existsSync(`${ROOT}/diagrams/src/${b.id}.mmd`) ? '.mmd (Mermaid)' : '.dot (Graphviz)';
  add(new D.Paragraph({
    alignment: D.AlignmentType.CENTER, spacing: { before: 60, after: 60 }, keepNext: true,
    children: [new D.ImageRun({ type: 'png', data: fs.readFileSync(`${ROOT}/diagrams/png/${b.id}.png`), transformation: { width: Math.round(dw * 96), height: Math.round(dh * 96) }, altText: { title: b.title, description: b.title, name: b.id } })]
  }));
  add(new D.Paragraph({ alignment: D.AlignmentType.CENTER, spacing: { after: 100 }, children: [new D.TextRun({ text: `${b.label} | Editable source: diagrams/src/${b.id}${ext} | Image: diagrams/png/${b.id}.png`, font: F, size: 15, color: '6B7280' })] }));
  if (!attach) newSection('P');
  emitBlocks(expl);
  newSection('P');
}

// ---------- footer ----------
function footer() {
  return new D.Footer({ children: [new D.Paragraph({
    alignment: D.AlignmentType.CENTER, bidirectional: false,
    border: { top: { style: D.BorderStyle.SINGLE, size: 4, color: 'BFBFBF', space: 4 } },
    children: [new D.TextRun({ text: 'MarketHub-MVC | Project Documentation | ', font: F, size: 16, color: '6B7280' }), new D.TextRun({ text: 'Page ', font: F, size: 16, color: '6B7280' }), new D.TextRun({ children: [D.PageNumber.CURRENT], font: F, size: 16, color: '6B7280' })]
  })] });
}
const pageProps = (o, start) => ({
  page: {
    size: { width: 11906, height: 16838, orientation: o === 'L' ? D.PageOrientation.LANDSCAPE : D.PageOrientation.PORTRAIT },
    margin: o === 'L' ? { top: 850, bottom: 900, left: 850, right: 850, footer: 400 } : { top: 1134, bottom: 1134, left: 1134, right: 1134, footer: 500 },
    pageNumbers: start ? { start: 1 } : undefined
  }
});

// ---------- main ----------
(async () => {
  const imgs = fs.readdirSync(`${ROOT}/diagrams/png`).filter(f => f.endsWith('.png'));
  for (const f of imgs) { const m = await sharp(`${ROOT}/diagrams/png/${f}`).metadata(); sizes[f.replace('.png', '')] = { w: m.width, h: m.height }; }

  const files = fs.readdirSync(`${ROOT}/content`).filter(f => /^\d\d_.*\.md$/.test(f)).sort();
  newSection('P');
  for (const f of files) {
    const blocks = parseBlocks(fs.readFileSync(`${ROOT}/content/${f}`, 'utf8'));
    emitBlocks(blocks);
  }
  // drop empty trailing sections
  const body = sections.filter(s => s.children.length);

  // cover
  const cover = [];
  const cp = (t, o) => cover.push(new D.Paragraph({ alignment: D.AlignmentType.CENTER, bidirectional: hasAr(t), spacing: { before: o.before || 0, after: o.after || 120 }, border: o.border, children: inline(t, { rtl: hasAr(t), size: o.size, bold: o.bold, color: o.color }) }));
  cp('MarketHub-MVC', { before: 2600, size: 96, bold: true, color: NAVY, after: 100 });
  cp('Multi-Vendor E-Commerce Marketplace', { size: 40, color: TEAL, after: 200, border: { bottom: { style: D.BorderStyle.SINGLE, size: 12, color: TEAL, space: 12 } } });
  cp('وثيقة المشروع الكاملة', { before: 300, size: 56, bold: true, color: '111827', after: 100 });
  cp('تحليل، تصميم، تخطيط وتوثيق مشروع Full-Stack .NET لفريق من خمسة مطورين', { size: 28, color: '374151', after: 600 });
  cp('ASP.NET Core MVC | .NET 10 (LTS) | SQL Server | Entity Framework Core | ASP.NET Core Identity | Stripe (Test Mode)', { size: 20, color: '6B7280', after: 500 });
  cp('Team: Member A | Member B | Member C | Member D | Member E', { size: 22, color: '374151', after: 80 });
  cp('Version 1.0 | October 2026', { size: 22, color: '374151', after: 80 });
  cp('Language: Arabic (Egyptian dialect) with English technical terms', { size: 20, color: '6B7280', after: 80 });

  // TOC
  const toc = [];
  toc.push(new D.Paragraph({ bidirectional: true, spacing: { after: 200 }, children: inline('جدول المحتويات (Table of Contents)', { rtl: true, size: 36, bold: true, color: NAVY }) }));
  const noB = { style: D.BorderStyle.NONE, size: 0, color: 'FFFFFF' };
  const rowsT = tocEntries.map(e => {
    const rtl = hasAr(e.text);
    const sizeT = e.level === 1 ? 21 : 18;
    const title = new D.Paragraph({ bidirectional: true, spacing: { before: e.level === 1 ? 90 : 0, after: 0 }, indent: e.level === 2 ? { start: 360 } : undefined,
      children: [new D.InternalHyperlink({ anchor: e.id, children: inline(e.text, { rtl: true, size: sizeT, bold: e.level === 1, color: e.level === 1 ? NAVY : '374151' }) })] });
    const pg = new D.Paragraph({ alignment: D.AlignmentType.LEFT, spacing: { before: e.level === 1 ? 90 : 0, after: 0 }, children: [new D.PageReference(e.id)] });
    const bb = { top: noB, left: noB, right: noB, bottom: { style: D.BorderStyle.DOTTED, size: 2, color: 'D1D5DB' } };
    return new D.TableRow({ cantSplit: true, children: [
      new D.TableCell({ width: { size: 8638, type: D.WidthType.DXA }, borders: bb, margins: { top: 15, bottom: 15, left: 60, right: 60 }, children: [title] }),
      new D.TableCell({ width: { size: 1000, type: D.WidthType.DXA }, borders: bb, margins: { top: 15, bottom: 15, left: 60, right: 60 }, children: [pg] })] });
  });
  toc.push(new D.Table({ width: { size: 9638, type: D.WidthType.DXA }, columnWidths: [8638, 1000], rows: rowsT, visuallyRightToLeft: true, layout: D.TableLayoutType.FIXED }));

  const docSections = [
    { properties: pageProps('P'), children: cover },
    { properties: pageProps('P', true), footers: { default: footer() }, children: toc },
    ...body.map(s => ({ properties: pageProps(s.orient), footers: { default: footer() }, children: s.children }))
  ];
  const doc = new D.Document({
    creator: 'MarketHub-MVC Team', title: 'MarketHub-MVC Project Documentation', description: 'Multi-Vendor E-Commerce Marketplace blueprint',
    styles: {
      default: { document: { run: { font: F, size: 21 }, paragraph: { spacing: { line: 300, lineRule: D.LineRuleType.AUTO } } } },
      paragraphStyles: [
        { id: 'Heading1', name: 'Heading 1', basedOn: 'Normal', next: 'Normal', quickFormat: true, run: { font: F, size: 36, bold: true, color: NAVY }, paragraph: { spacing: { before: 120, after: 200 }, outlineLevel: 0, border: { bottom: { style: D.BorderStyle.SINGLE, size: 8, color: NAVY, space: 6 } } } },
        { id: 'Heading2', name: 'Heading 2', basedOn: 'Normal', next: 'Normal', quickFormat: true, run: { font: F, size: 28, bold: true, color: NAVY }, paragraph: { spacing: { before: 280, after: 120 }, outlineLevel: 1 } },
        { id: 'Heading3', name: 'Heading 3', basedOn: 'Normal', next: 'Normal', quickFormat: true, run: { font: F, size: 24, bold: true, color: TEAL }, paragraph: { spacing: { before: 200, after: 100 }, outlineLevel: 2 } }
      ]
    },
    numbering: { config: [{ reference: 'bul', levels: [{ level: 0, format: D.LevelFormat.BULLET, text: '•', alignment: D.AlignmentType.START, style: { paragraph: { indent: { left: 540, hanging: 300 } } } }] }, ...numberingConfigs] },
    features: { updateFields: false },
    sections: docSections
  });
  const buf = await D.Packer.toBuffer(doc);
  fs.mkdirSync(`${ROOT}/out`, { recursive: true });
  fs.writeFileSync(`${ROOT}/out/MarketHub-MVC_Project_Documentation.docx`, buf);
  console.log('docx bytes', buf.length, 'toc entries', tocEntries.length, 'sections', docSections.length);
})().catch(e => { console.error(e); process.exit(1); });
