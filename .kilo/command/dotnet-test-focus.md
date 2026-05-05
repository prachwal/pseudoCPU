---
description: Uruchom tylko zawezone testy .NET dla zmienionej funkcjonalnosci i zapisz dokladna komende
agent: issue-executor
---
Przygotuj i uruchom waska weryfikacje `.NET` dla zakresu opisanego w `$ARGUMENTS`.

Wymagania:

1. Najpierw ustal, jaki jest najwezszy sensowny projekt testowy i filtr testow.
2. Preferuj `dotnet test <projekt-testowy> --filter "..."`.
3. Nie rozszerzaj uruchomienia do calego repo bez wyraznej potrzeby.
4. Zwroc dokladna komende oraz wynik uruchomienia.
5. Jesli wywolanie jest powiazane z GitHub Issue, przygotuj tez komentarz z wynikiem do dopisania w issue.
