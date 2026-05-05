# AGENTS.md

## Repo State
- Repo zawiera na razie tylko scaffolding workflow: `AGENTS.md`, `.kilo/`, `.github/ISSUE_TEMPLATE/` i `global.json`.
- Nie ma jeszcze solution, projektow `.csproj`, kodu aplikacji ani testow. Nie zgaduj nazw projektow, sciezek ani komend build/test poza tym, co pojawi sie pozniej w repo.

## Primary Workflow
- Zrodlem prawdy dla planu, taskow, postepu i wynikow testow sa GitHub Issues, nie sam chat.
- Do planowania uzywaj `/issue-plan`; do realizacji taska `/issue-execute`; do zsynchronizowania stanu issue z repo `/issue-sync`.
- Planner ma utrzymywac strukture epic -> taski i zapisywac decyzje, kryteria akceptacji, zaleznosci oraz strategie weryfikacji bezposrednio w issue.
- Executor ma dopisywac do issue komentarze startowe, postep, komendy weryfikacyjne i wynik koncowy z dokladnym statusem testow.

## Git Branch Workflow
- Kazda faza pracy startuje z nowego brancha roboczego utworzonego przed rozpoczeciem zmian.
- Po kazdym passie zakonczonym pozytywna, waska weryfikacja wykonaj commit wszystkich zmian z tej fazy, wypchnij branch roboczy, zmerguj go do branchu bazowego i usun branch roboczy.
- Jesli potrzebny jest kolejny pass lub kolejna faza, utworz nowy branch zamiast kontynuowac prace na poprzednim.
- W pustym repo bez historii commitow pierwszy commit moze powstac na branchu roboczym; po weryfikacji potraktuj ustawienie `main` na ten commit jako merge inicjalizujacy i usun branch roboczy.
- Nazwa brancha, wynik push/merge oraz moment zamkniecia issue po zakonczeniu prac trzeba odnotowac w issue lub komentarzu issue dla danej fazy.

## GitHub Issues
- Zarzadzaj issue przez `gh issue ...`; nie zakladaj recznej pracy w przegladarce.
- Nowe taski i epiki tworz z sekcjami zgodnymi z szablonami w `.github/ISSUE_TEMPLATE/`.
- Kazdy task implementacyjny musi zawierac: cel, zakres, kryteria akceptacji, plan weryfikacji i zaleznosci.
- Jesli wiedza powstaje w trakcie pracy, utrwal ja w issue body lub komentarzu i odwoluj sie do numerow issue w kolejnych zadaniach.

## .NET And Tests
- Docelowy toolchain repo to `.NET 8` CLI; `global.json` preferuje SDK 8, ale moze roll-forward do nowszego lokalnie zainstalowanego SDK.
- Dopoki repo nie ma solution/test projects, nie wymyslaj komend `dotnet new`, `dotnet build` ani `dotnet test`, chyba ze uzytkownik wyraznie zleci bootstrap kodu.
- Gdy testy juz istnieja, preferuj zawsze weryfikacje zawezona do zmienionej funkcjonalnosci: `dotnet test <projekt-testowy> --filter "..."`.
- Nie uruchamiaj pelnego `dotnet test` dla calego repo bez wyraznej potrzeby; w issue zapisuj dokladna komende filtra uzytego do weryfikacji.

## Domain Focus
- Repo jest przygotowywane pod pseudo CPU inspirowane zgodnoscia opcode-level z 6502, ale wdrazane etapami w ograniczonym zakresie.
- Kazdy task powinien jasno wskazywac, ktorych opcode'ow, flag, rejestrow, addressing modes lub regul wykonania dotyczy, zeby nie mieszac zakresow.
