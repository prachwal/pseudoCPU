---
description: Wyswietl aktywne taski z GitHub Issues
agent: issue-planner
---
Wyswietl aktywne taski z GitHub Issues dla `$ARGUMENTS`.

Wymagania:

1. Uzyj `gh issue list` do pokazania otwartych issue z etykieta `task`.
2. Jesli to pomoze, dolacz rowniez aktywne epiki powiazane z taskami.
3. Wyswietl przynajmniej numer, tytul i status kazdego aktywnego taska.
4. Jesli nie ma aktywnych taskow, zwroc jednoznaczna informacje o braku otwartych taskow.
5. Nie modyfikuj issue; to polecenie jest tylko do odczytu.
