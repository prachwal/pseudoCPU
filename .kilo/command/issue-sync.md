---
description: Zsynchronizuj status GitHub Issue z aktualnym stanem repo i wiedza z pracy
agent: issue-planner
---
Zsynchronizuj issue wskazane w `$ARGUMENTS`.

Wymagania:

1. Sprawdz tresc issue, aktualny stan repo i ostatnie zmiany na branchu.
2. Uzupelnij issue o aktualny status, otwarte ryzyka, brakujace zaleznosci i nastepny krok.
3. Jesli w pracy pojawila sie nowa wiedza architektoniczna lub testowa, dopisz ja do issue.
4. Jesli potrzebne sa kolejne taski, utworz je i polacz numerami issue.
5. Jesli zakonczona faza zostala zmergowana, odnotuj w issue nazwe brancha roboczego, wynik push/merge, fakt usuniecia brancha oraz czy issue powinno zostac zamkniete.
