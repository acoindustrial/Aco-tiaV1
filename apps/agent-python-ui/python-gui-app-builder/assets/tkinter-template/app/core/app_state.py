class AppState:
    def __init__(self) -> None:
        self.status_message = "Ready"

    def set_status(self, message: str) -> None:
        self.status_message = message
