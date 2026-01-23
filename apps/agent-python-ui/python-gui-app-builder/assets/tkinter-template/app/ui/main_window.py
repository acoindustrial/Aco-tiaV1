import tkinter as tk
from tkinter import ttk

from core.app_state import AppState
from core.feature_registry import FeatureRegistry
from ui.navigation import Navigation
from ui.views.dashboard_view import DashboardView


class MainWindow:
    def __init__(self) -> None:
        self.root = tk.Tk()
        self.root.title("Tkinter App")
        self.root.geometry("1000x700")

        self.state = AppState()
        self.registry = FeatureRegistry()
        self.registry.register("dashboard", "Dashboard", DashboardView)

        self._build_layout()

    def _build_layout(self) -> None:
        container = ttk.Frame(self.root, padding=8)
        container.pack(fill=tk.BOTH, expand=True)

        self.navigation = Navigation(container, self.registry, self._show_view)
        self.navigation.pack(side=tk.LEFT, fill=tk.Y)

        self.content = ttk.Frame(container)
        self.content.pack(side=tk.RIGHT, fill=tk.BOTH, expand=True)

        self._show_view("dashboard")

    def _show_view(self, feature_id: str) -> None:
        for child in self.content.winfo_children():
            child.destroy()

        view_class = self.registry.get(feature_id)
        view = view_class(self.content, self.state)
        view.pack(fill=tk.BOTH, expand=True)

    def run(self) -> None:
        self.root.mainloop()
