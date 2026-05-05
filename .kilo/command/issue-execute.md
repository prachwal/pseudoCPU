---
description: Wykonaj task opisany w GitHub Issue i zaktualizuj issue o postep oraz wyniki testow
agent: issue-executor
---
Wykonaj task z GitHub Issue wskazany w `$ARGUMENTS`.

Wymagania:

1. Odczytaj issue i lokalne instrukcje repo.
2. Dodaj komentarz startowy do issue.
3. Przed implementacja utworz nowy branch dla tej fazy pracy.
4. Zaimplementuj tylko zakres tego taska.
5. Uruchom najwezsza sensowna weryfikacje; jesli testy istnieja, preferuj `dotnet test <projekt-testowy> --filter "..."`.
6. Po pozytywnym passie wykonaj commit, push, merge do branchu bazowego i usun branch roboczy.
7. Dodaj komentarz koncowy z lista zmian, nazwa brancha, dokladna komenda testowa i wynikiem, a nastepnie zamknij issue jesli zakres zostal zakonczony.
8. W odpowiedzi zwroc numer issue, zmienione pliki i wynik weryfikacji.
