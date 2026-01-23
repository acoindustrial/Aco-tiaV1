# Packaging Python GUI Apps

## PyInstaller (default)
- Use `--noconfirm --onefile` for single executable builds.
- Add `--windowed` for GUI apps to avoid console window (Windows/macOS).
- Include data files with `--add-data` and icons with `--icon`.

Example:
```
pyinstaller --noconfirm --onefile --windowed \
  --name MyApp \
  --add-data "assets:assets" \
  --icon assets/app.ico \
  app/main.py
```

## Nuitka (performance-oriented)
- Use when startup time or performance is critical.
- Requires a compiler toolchain; confirm availability.

Example:
```
python -m nuitka --onefile --standalone --windows-disable-console \
  --output-filename=MyApp.exe app/main.py
```

## Common Pitfalls
- Missing Qt plugins (PySide6/PyQt6): include platform plugins directory.
- Relative paths: resolve paths based on `sys._MEIPASS` for PyInstaller builds.
- Icons on macOS: set `--osx-bundle-identifier` and `.icns` icon files.
