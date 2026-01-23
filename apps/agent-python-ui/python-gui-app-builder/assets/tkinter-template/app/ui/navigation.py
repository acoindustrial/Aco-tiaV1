import tkinter as tk
from tkinter import ttk
from typing import Callable

from core.feature_registry import FeatureRegistry


class Navigation(ttk.Frame):
    def __init__(
        self,
        master: tk.Widget,
        registry: FeatureRegistry,
        on_select: Callable[[str], None],
    ) -> None:
        super().__init__(master, padding=8)
        self._registry = registry
        self._on_select = on_select
        self._build()

    def _build(self) -> None:
        ttk.Label(self, text="Features", font=("Segoe UI", 12, "bold")).pack(
            anchor=tk.W, pady=(0, 8)
        )
        for feature_id, meta in self._registry.items():
            button = ttk.Button(
                self,
                text=meta["label"],
                command=lambda fid=feature_id: self._on_select(fid),
            )
            button.pack(fill=tk.X, pady=2)
