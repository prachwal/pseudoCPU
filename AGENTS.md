# AGENTS.md

## Repo State
- Repo zawiera solution `pseudoCPU.sln` oraz projekty `src/pseudoCPU.Core`, `src/pseudoCPU.Cli` i `tests/pseudoCPU.Bootstrap.Tests`.
- Bootstrap CPU obejmuje slice `LDA #imm`, `TAX`, `INX`, `STA abs`, `BRK`, `CMP #imm`, `JMP abs`, `BEQ rel` i `BNE rel`; nie rozszerzaj zakresu bez osobnego issue.
- Następny planowany zakres fazy 3 to rozszerzenie slice o `Carry` i licznikową pętlę na `X`.
- Bootstrapowy assembler nie wspiera etykiet ani dyrektywy `.org`; branch offsety podawaj jawnie jako relatywny bajt.
- `CMP #imm` aktualizuje tylko `Zero` i `Negative`; brak modelu `Carry` jest świadomym ograniczeniem tego etapu.
- Nie zgaduj dodatkowych projektow, sciezek ani komend build/test poza tym, co jest faktycznie obecne w repo.

## Primary Workflow
- Zrodlem prawdy dla planu, taskow, postepu i wynikow testow sa GitHub Issues, nie sam chat.
- Do planowania uzywaj `/issue-plan`; do realizacji taska `/issue-execute`; do zsynchronizowania stanu issue z repo `/issue-sync`.
- Planner ma utrzymywac strukture epic -> taski i zapisywac decyzje, kryteria akceptacji, zaleznosci oraz strategie weryfikacji bezposrednio w issue.
- Executor ma dopisywac do issue komentarze startowe, postep, komendy weryfikacyjne, wynik koncowy i jawny sygnal zamkniecia taska.
- Taski implementacyjne powinny zawierac i respektowac sekcje Definition of Done oraz Scope Guard.
- Po kazdym epiku nalezy dopisac lub zaktualizowac komentarz QC gate oraz utrzymywac go jako jawny punkt kontroli.
- Po nowych opcode'ach aktualizuj `docs/cpu-slice-map.md`, zeby stan slice byl jawny i aktualny.
- Dla decyzji architektonicznych dodawaj ADR-y w `docs/adr/` zamiast rozpraszać je po komentarzach.

## Git Branch Workflow
- Kazda faza pracy startuje z nowego brancha roboczego utworzonego przed rozpoczeciem zmian.
- Po kazdym passie zakonczonym pozytywna, waska weryfikacja wykonaj commit wszystkich zmian z tej fazy, wypchnij branch roboczy, zmerguj go do branchu bazowego, usun branch roboczy i zamknij issue, jesli task jest zakonczony.
- Jesli potrzebny jest kolejny pass lub kolejna faza, utworz nowy branch zamiast kontynuowac prace na poprzednim.
- W pustym repo bez historii commitow pierwszy commit moze powstac na branchu roboczym; po weryfikacji potraktuj ustawienie `main` na ten commit jako merge inicjalizujacy i usun branch roboczy.
- Nazwa brancha, wynik push/merge i status zamkniecia issue trzeba odnotowac w issue lub komentarzu issue dla danej fazy.

## GitHub Issues
- Zarzadzaj issue przez `gh issue ...`; nie zakladaj recznej pracy w przegladarce.
- Nowe taski i epiki tworz z sekcjami zgodnymi z szablonami w `.github/ISSUE_TEMPLATE/`.
- Kazdy task implementacyjny musi zawierac: cel, zakres, kryteria akceptacji, plan weryfikacji i zaleznosci.
- Jesli wiedza powstaje w trakcie pracy, utrwal ja w issue body lub komentarzu i odwoluj sie do numerow issue w kolejnych zadaniach.

## Shell And GitHub CLI Hygiene
- W tym srodowisku uzywaj `python3`, nie `python`.
- Przy wysylaniu dluzszych tresci do GitHub (`gh issue comment`, `gh issue edit`, `gh pr create`) unikaj inline shell stringow; zapisuj body do pliku i przekazuj przez `--body-file`, zeby backticki, cudzyslowy i nowe linie nie uszkadzaly wiadomosci.
- Gdy pracujesz z JSON z `gh ... --json` lub innym CLI, preferuj `jq` do odczytu i transformacji zamiast parsowania tekstu wyjsciowego shellowymi hackami.

## .NET And Tests
- Docelowy toolchain repo to `.NET 8` CLI; `global.json` preferuje SDK 8, ale moze roll-forward do nowszego lokalnie zainstalowanego SDK.
- Gdy testy juz istnieja, preferuj zawsze weryfikacje zawezona do zmienionej funkcjonalnosci: `dotnet test tests/pseudoCPU.Bootstrap.Tests/pseudoCPU.Bootstrap.Tests.csproj --filter "..."`.
- Nie uruchamiaj pelnego `dotnet test` dla calego repo bez wyraznej potrzeby; w issue zapisuj dokladna komende filtra uzytego do weryfikacji.
- Dla zmian formatowania i konfiguracji utrzymuj `.editorconfig` jako zrodlo prawdy dla stylu C#.

## Domain Focus
- Repo jest przygotowywane pod pseudo CPU inspirowane zgodnoscia opcode-level z 6502, ale wdrazane etapami w ograniczonym zakresie.
- Kazdy task powinien jasno wskazywac, ktorych opcode'ow, flag, rejestrow, addressing modes lub regul wykonania dotyczy, zeby nie mieszac zakresow.
