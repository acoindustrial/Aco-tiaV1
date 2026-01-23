import tkinter as tk
from tkinter import ttk

from core.app_state import AppState


class DashboardView(ttk.Frame):
    def __init__(self, master: tk.Widget, state: AppState) -> None:
        super().__init__(master, padding=16)
        ttk.Label(self, text="Dashboard", font=("Segoe UI", 16, "bold")).pack(
            anchor=tk.W, pady=(0, 12)
        )
        ttk.Label(self, text=state.status_message).pack(anchor=tk.W)
