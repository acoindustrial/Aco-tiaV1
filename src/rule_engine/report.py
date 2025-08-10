from datetime import datetime
from pathlib import Path
from typing import List

from . import RuleResult


def generate_html(results: List[RuleResult], path: Path, lang: str = "en") -> Path:
    path.parent.mkdir(parents=True, exist_ok=True)
    rows = []
    for r in results:
        loc = f"{r.location.block}/{r.location.network}/{r.location.index}"
        rows.append(
            f"<tr><td>{r.code}</td><td>{r.severity}</td><td>{r.message}</td><td>{loc}</td></tr>"
        )
    html = (
        "<html><head><meta charset='utf-8'></head><body>"
        "<h1>Analysis Report</h1>"
        "<table border='1'><tr><th>Rule</th><th>Severity</th><th>Message</th><th>Location</th></tr>"
        + "".join(rows)
        + "</table></body></html>"
    )
    path.write_text(html, encoding="utf-8")
    return path


def generate_pdf(html_path: Path, pdf_path: Path) -> None:
    try:
        import pdfkit

        pdf_path.parent.mkdir(parents=True, exist_ok=True)
        pdfkit.from_file(str(html_path), str(pdf_path))
    except Exception:
        # pdf generation is optional; ignore if it fails
        pass
