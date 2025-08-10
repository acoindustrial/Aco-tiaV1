import argparse
import json
from pathlib import Path
import sys

sys.path.append(str(Path(__file__).resolve().parents[1] / "src"))

from rule_engine.engine import analyze
from rule_engine.report import generate_html, generate_pdf


def load_blocks(path: Path):
    with open(path, "r", encoding="utf-8") as f:
        return json.load(f)


def export_network(blocks, block_name: str, network_index: int, export_dir: Path):
    export_dir.mkdir(parents=True, exist_ok=True)
    for b in blocks:
        if b.get("name") == block_name:
            try:
                network = b["networks"][network_index - 1]
            except (KeyError, IndexError):
                return None
            text = "\n".join(network)
            out_path = export_dir / f"{block_name}_N{network_index}.scl"
            out_path.write_text(text, encoding="utf-8")
            try:
                import pyperclip

                pyperclip.copy(text)
            except Exception:
                pass
            return out_path
    return None


def main():
    parser = argparse.ArgumentParser(description="Run rule analysis")
    parser.add_argument("input", help="Path to program JSON")
    parser.add_argument("--lang", default="en", choices=["en", "ro"])
    parser.add_argument("--report", action="store_true", help="Generate HTML/PDF report")
    parser.add_argument(
        "--export", nargs=2, metavar=("BLOCK", "NETWORK"), help="Export network as SCL"
    )
    args = parser.parse_args()

    blocks = load_blocks(Path(args.input))
    results = analyze(blocks, lang=args.lang)
    for r in results:
        loc = f"{r.location.block}/{r.location.network}/{r.location.index}"
        print(f"{r.code} {r.severity} {r.message} @ {loc}")

    if args.report:
        reports_dir = Path("reports")
        html_path = reports_dir / "report.html"
        pdf_path = reports_dir / "report.pdf"
        generate_html(results, html_path, lang=args.lang)
        generate_pdf(html_path, pdf_path)
        print(f"Report written to {html_path}")

    if args.export:
        block, net = args.export
        exported = export_network(blocks, block, int(net), Path("exports"))
        if exported:
            print(f"Network exported to {exported}")
        else:
            print("Failed to export network")


if __name__ == "__main__":
    main()
