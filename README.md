# AgentTIA Offline MVP

This repository hosts a simplified offline MVP for AgentTIA. It contains:

- **Agent.Core** – minimal parser and model for SCL/STL files.
- **Agent.Engine** – CLI for analyzing source folders.
- **AgentTIA.UI** – WPF UI (Windows only) for browsing parsed blocks.
- Sample assets, symbol packs, configs and build tools.

Use `tools/build.ps1 -Offline` to publish the UI and engine. Run `tools/verify.ps1` to generate file hashes.
