from collections.abc import Iterable
from typing import Type

import tkinter as tk


class FeatureRegistry:
    def __init__(self) -> None:
        self._features: dict[str, dict[str, object]] = {}

    def register(self, feature_id: str, label: str, view: Type[tk.Frame]) -> None:
        self._features[feature_id] = {"label": label, "view": view}

    def get(self, feature_id: str) -> Type[tk.Frame]:
        return self._features[feature_id]["view"]  # type: ignore[return-value]

    def items(self) -> Iterable[tuple[str, dict[str, object]]]:
        return self._features.items()
