const pdfjsLib = window['pdfjs-dist/build/pdf'];
pdfjsLib.GlobalWorkerOptions.workerSrc = 'https://cdnjs.cloudflare.com/ajax/libs/pdf.js/3.5.141/pdf.worker.min.js';

const fileInput = document.getElementById('file-input');
const viewer = document.getElementById('viewer');
const bookmarksList = document.getElementById('bookmarks-list');
let pdfName = '';
let indexData = { blocks: {} };
let currentBlock = null;
let language = 'en';
let bookmarks = [];

fileInput.addEventListener('change', async (e) => {
  const file = e.target.files[0];
  if (!file) return;
  pdfName = file.name;
  const arrayBuffer = await file.arrayBuffer();
  const pdf = await pdfjsLib.getDocument({ data: arrayBuffer }).promise;
  indexData = { blocks: {} };
  currentBlock = null;
  language = 'en';
  viewer.innerHTML = '';
  bookmarksList.innerHTML = '';
  for (let pageNum = 1; pageNum <= pdf.numPages; pageNum++) {
    const page = await pdf.getPage(pageNum);
    const viewport = page.getViewport({ scale: 1.2 });
    const canvas = document.createElement('canvas');
    const ctx = canvas.getContext('2d');
    canvas.height = viewport.height;
    canvas.width = viewport.width;
    await page.render({ canvasContext: ctx, viewport }).promise;
    const pageDiv = document.createElement('div');
    pageDiv.className = 'page';
    pageDiv.id = 'page-' + pageNum;
    pageDiv.appendChild(canvas);
    const btn = document.createElement('button');
    btn.textContent = 'Add bookmark here';
    btn.addEventListener('click', () => addBookmark(pageNum));
    pageDiv.appendChild(btn);
    viewer.appendChild(pageDiv);
    await scanPage(page, pageNum);
  }
  loadBookmarks();
});

async function scanPage(page, pageNum) {
  const textContent = await page.getTextContent();
  const lines = textContent.items.map(i => i.str);
  const joined = lines.join(' ');
  if (/Retea/i.test(joined) || /Bloc/i.test(joined)) language = 'ro';
  if (/Network/i.test(joined) || /Block/i.test(joined)) language = 'en';
  for (const line of lines) {
    const blockMatch = line.match(/(Organization block|Function block|Function|Bloc(?: de)?(?: organizare| functie| funcție)|Funcție|Functie)\s+(OB|FB|FC)\d+/i);
    if (blockMatch) {
      const blockId = blockMatch[0].split(' ').slice(-1)[0];
      currentBlock = blockId;
      if (!indexData.blocks[blockId]) indexData.blocks[blockId] = { pages: [], networks: {} };
      if (!indexData.blocks[blockId].pages.includes(pageNum)) indexData.blocks[blockId].pages.push(pageNum);
      continue;
    }
    const netMatch = line.match(/(Network|Retea)\s+(\d+)/i);
    if (netMatch && currentBlock) {
      const netNum = netMatch[2];
      const block = indexData.blocks[currentBlock];
      if (!block.networks[netNum]) block.networks[netNum] = pageNum;
    }
  }
}

function addBookmark(pageNum) {
  const name = prompt('Bookmark name:');
  if (!name) return;
  const type = prompt('Type (Block/Network/Note):', '') || null;
  bookmarks.push({ page: pageNum, name, type });
  saveBookmarks();
  renderBookmarks();
}

function renderBookmarks() {
  bookmarksList.innerHTML = '';
  for (const b of bookmarks) {
    const li = document.createElement('li');
    li.textContent = `p${b.page}: ${b.name}${b.type ? ' (' + b.type + ')' : ''}`;
    bookmarksList.appendChild(li);
  }
}

function saveBookmarks() {
  localStorage.setItem(pdfName + '.bookmarks', JSON.stringify(bookmarks));
  const blob = new Blob([JSON.stringify(bookmarks, null, 2)], { type: 'application/json' });
  downloadBlob(blob, pdfName + '.bookmarks.json');
}

function loadBookmarks() {
  const stored = localStorage.getItem(pdfName + '.bookmarks');
  if (stored) {
    bookmarks = JSON.parse(stored);
    renderBookmarks();
  }
}

document.getElementById('export-btn').addEventListener('click', () => {
  const data = { blocks: indexData.blocks, language, bookmarks };
  const jsonBlob = new Blob([JSON.stringify(data, null, 2)], { type: 'application/json' });
  downloadBlob(jsonBlob, pdfName + '.index.json');
  const html = buildHtmlIndex(pdfName, indexData, language);
  const htmlBlob = new Blob([html], { type: 'text/html' });
  downloadBlob(htmlBlob, pdfName + '.index.html');
});

function buildHtmlIndex(pdfName, index, lang) {
  const wordNetwork = lang === 'ro' ? 'Retea' : 'Network';
  let html = '<!DOCTYPE html><html><body><h1>Index</h1><ul>';
  for (const [block, info] of Object.entries(index.blocks)) {
    html += `<li>${block} (pages ${info.pages.join(', ')})<ul>`;
    for (const [net, page] of Object.entries(info.networks)) {
      html += `<li>${wordNetwork} ${net} - <a href="${pdfName}#page=${page}">page ${page}</a></li>`;
    }
    html += '</ul></li>';
  }
  html += '</ul></body></html>';
  return html;
}

function downloadBlob(blob, filename) {
  const url = URL.createObjectURL(blob);
  const a = document.createElement('a');
  a.href = url;
  a.download = filename;
  a.click();
  setTimeout(() => URL.revokeObjectURL(url), 1000);
}
