---
description: Zaplanuj epic lub taski jako GitHub Issues i utrwal plan w issue
agent: issue-planner
---
Zaplanuj temat opisany w `$ARGUMENTS` jako workflow oparty o GitHub Issues.

Wymagania:

1. Najpierw sprawdz repo i istniejace issues.
2. Jesli temat jest wiekszy niz jeden task, utworz albo zaktualizuj issue typu epic i rozbij go na mniejsze taski.
3. Kazdy task ma miec cel, zakres, kryteria akceptacji, plan weryfikacji i zaleznosci.
4. Dla zadan implementacyjnych zapisz plan waskiej weryfikacji `.NET` z `dotnet test --filter`.
5. Zapisz w issue oczekiwanie, ze executor utworzy nowy branch przed faza pracy oraz po pozytywnym passie wykona commit, push, merge do branchu bazowego, usunie branch roboczy i zamknie issue po pelnym dowiezieniu zakresu.
6. Wszystkie decyzje, numery issue i dalsze kroki zapisz w issue oraz zwroc w odpowiedzi.
