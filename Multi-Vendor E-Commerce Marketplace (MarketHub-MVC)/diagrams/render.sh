#!/usr/bin/env bash
# Re-render all diagrams from their editable sources.
# Requirements: Node.js + @mermaid-js/mermaid-cli (mmdc), Graphviz (dot, neato).
# If mmdc cannot find a browser, create puppeteer.json with {"args":["--no-sandbox"]} and add -p puppeteer.json.
set -e
cd "$(dirname "$0")"
mkdir -p png svg
for f in src/*.mmd; do b=$(basename "$f" .mmd)
  mmdc -c mermaid.config.json -b white -i "$f" -o "png/$b.png" -s 2
  mmdc -c mermaid.config.json -b white -i "$f" -o "svg/$b.svg"
done
for f in src/*.dot; do b=$(basename "$f" .dot)
  if grep -q "layout=neato" "$f"; then ENGINE=neato; else ENGINE=dot; fi
  $ENGINE -Tpng -Gdpi=120 "$f" -o "png/$b.png"
  $ENGINE -Tsvg "$f" -o "svg/$b.svg"
done
echo "Done."
