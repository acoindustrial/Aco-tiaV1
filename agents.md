# Politică pentru agenți (Codex)

- Fiecare job/proiect nou se creează în: `/apps/<job>/`.
- Agentul rulează comenzi și scrie fișiere **exclusiv** în subfolderul jobului.
- Nu se creează/editează/șterg fișiere în afara `/apps/<job>/`. Dacă este necesar, agentul se oprește și cere aprobare.
- Dacă `/apps/<job>/` nu există, îl creează înainte de a continua.
- Pentru fiecare job se setează scope explicit în prompt: „Lucrează DOAR în `apps/<job>/`”.
- PR-urile trebuie să conțină doar fișiere din `apps/<job>/`.

_Notă:_ proiectele existente (ex. `app_windows`) rămân cum sunt până le mutăm controlat în `/apps/<job>/`.
