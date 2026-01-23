---
name: python-gui-app-builder
description: Build medium-to-large Python applications that always include a graphical UI, verify generated code, and package executables (PyInstaller/Nuitka). Use when users request Python software with GUI screens, extensible UI architecture, validation/tests, or desktop app packaging.
---

# Python GUI App Builder

## Overview
Build Python desktop applications with an always-on graphical UI, modular architecture, verification steps, and executable packaging. Prefer predictable, extensible structures that make it easy to add many features over time.

## Workflow (follow in order)
1. **Collect requirements**
   - Capture: target OS, required features, data sources, offline/online needs, persistence, and packaging expectations.
   - Confirm UI expectations: navigation model, number of screens, and visual complexity.
2. **Select UI toolkit**
   - Default to **PySide6** for modern widgets and scalability.
   - Use **tkinter** when dependencies must be minimal or user asks for stdlib-only.
   - Use **PyQt6** or **DearPyGui** only when explicitly requested.
3. **Design extensible UI architecture**
   - Organize features as modules with a registry for easy addition/removal.
   - Use a main window with a navigation sidebar and a central stacked view.
   - Maintain separation of UI (views), domain logic (services), and state (models).
   - See `references/ui-architecture.md` for structure patterns.
4. **Implement core app**
   - Start from a template in `assets/` if it fits the toolkit choice.
   - Implement features as self-contained panels/screens with clear interfaces.
   - Provide a command palette or feature list for fast access.
5. **Verify generated code**
   - Add unit tests for non-UI logic.
   - Add smoke tests that initialize the UI without crashing.
   - Run linters/formatters where practical (ruff/black/pytest).
6. **Package executables**
   - Use PyInstaller as default; use Nuitka if asked for performance.
   - Handle assets, icons, and data files explicitly.
   - Follow `references/packaging.md` for platform-specific guidance.

## UI Design Guidelines (optimize for many features)
- Prefer a **left sidebar + stacked content** pattern.
- Use a **feature registry** that maps feature IDs to menu items and view classes.
- Keep settings and global actions in a top bar or toolbar.
- Provide clear empty states and error banners for user feedback.
- Use consistent iconography and keyboard shortcuts for power users.

## Verification Checklist
- UI launches and renders primary navigation.
- Each feature view loads without exceptions.
- Data read/write paths have tests or validation.
- Packaging command succeeds and output executable runs.

## Resources
### references/
- `ui-architecture.md` for scalable UI patterns and module layout.
- `packaging.md` for PyInstaller/Nuitka instructions and common pitfalls.

### assets/
- `tkinter-template/` for a minimal, extensible app skeleton.
