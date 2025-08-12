# Aco-tiaV1

Minimal offline analysis MVP for PLC programs.

## Analyze

Run static analysis on a JSON description of blocks:

```bash
python tools/analyze.py examples/sample_program.json
```

Use `--lang ro` for Romanian messages. The script prints rule
violations and may generate a report:

```bash
python tools/analyze.py examples/sample_program.json --report
```

Reports are written to `reports/` as HTML and optionally PDF.

## Export network

To export a specific network to SCL text and copy it to the clipboard:

```bash
python tools/analyze.py examples/sample_program.json --export OB1 1
```

The file is written under `exports/`.

## TIA PDF Indexer

Open `tools/pdf_indexer/index.html` in a browser to load a TIA print PDF.
The viewer detects block and network headers, allows adding bookmarks and can
export the index to JSON or a simple HTML table of contents linking to the
PDF pages.
