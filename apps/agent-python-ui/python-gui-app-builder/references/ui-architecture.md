# Scalable UI Architecture (Python Desktop)

## Goals
- Keep UI extensible for many features.
- Isolate business logic from UI code.
- Make navigation and feature registration declarative.

## Recommended Layout
```
app/
  main.py
  ui/
    main_window.py
    navigation.py
    views/
      dashboard_view.py
  core/
    app_state.py
    feature_registry.py
  services/
    data_store.py
    logger.py
```

## Feature Registry Pattern
- Use a registry that maps feature IDs to:
  - Display name
  - Icon (optional)
  - View class or factory
  - Command/shortcut metadata
- UI uses the registry to render navigation and load views dynamically.

## State Management
- Keep shared state in `core/app_state.py`.
- Expose state mutations through methods rather than direct attribute writes.
- UI views read state through the state object, not from each other.

## Navigation & Layout
- Main window holds a sidebar + stacked view container.
- Sidebar triggers view loading via registry.
- Provide consistent layout padding and typography across views.

## Extensibility Tips
- Add new features by adding a view class + registry entry.
- Keep feature-specific services in `services/`.
- Avoid circular imports by using factories or lazy imports in registry.
