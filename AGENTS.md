# AGENTS.md

## Repo State
- Repo zawiera solution `pseudoCPU.sln` oraz projekty `src/pseudoCPU.Core`, `src/pseudoCPU.Cli` i `tests/pseudoCPU.Bootstrap.Tests`.
- Stabilny workflow agentow jest opisany w `docs/agentic-epic-flow.md`; agenci musza go czytac przed planowaniem, wykonaniem albo QC epica.
- Trwajacy epic, biezacy zakres, decyzje fazowe i nastepne kroki zapisuj w `docs/current-epic.md`; nie dopisuj ich do `AGENTS.md`.
- `AGENTS.md` ma zawierac tylko stabilne zasady pracy agentow, workflow i stale ograniczenia repo.
- Bootstrapowy assembler nie wspiera etykiet ani dyrektywy `.org`; branch offsety podawaj jawnie jako relatywny bajt.
- Nie zgaduj dodatkowych projektow, sciezek ani komend build/test poza tym, co jest faktycznie obecne w repo.

## Primary Workflow
- Zrodlem prawdy dla planu, taskow, postepu i wynikow testow sa GitHub Issues oraz `docs/current-epic.md`, nie sam chat.
- Jeden realny GitHub epic issue z labelem `epic` odpowiada jednemu chapterowi w `docs/epic-chapters.md`.
- Nie tworz chapterow dla task issue, QC task issue ani follow-up issue bez labela `epic`.
- QC task nie jest follow-up issue. Pole `Follow-up` w checklist chaptera wypelniaj tylko faktycznymi follow-up issues utworzonymi po QC albo wartoscia `TBD`/`none`.
- Do planowania uzywaj `/issue-plan`; do realizacji taska `/issue-execute`; do zsynchronizowania stanu issue z repo `/issue-sync`.
- Planner ma utrzymywac strukture chapter -> epic -> taski i zapisywac decyzje, kryteria akceptacji, zaleznosci oraz strategie weryfikacji bezposrednio w issue.
- Executor ma dopisywac do issue komentarze startowe, postep, komendy weryfikacyjne, wynik koncowy i jawny sygnal zamkniecia taska.
- Po implementacji epica uruchom subagenta `epic-qc` jako niezalezna bramke kontroli jakosci przed zamknieciem epica.
- Petla zwrotna epica: planner -> executor -> `epic-qc` -> follow-up issue -> planner/executor, az `epic-qc` nie znajdzie blokujacych defektow.
- Jezeli `epic-qc` po weryfikacji silniejszym modelem wykryje brakujacy zakres, regresje, dlug techniczny lub ryzyko projektowe, utworz follow-up issue podobne do istniejacych taskow, np. #17 albo #25, i zalinkuj je z epica.
- Taski implementacyjne powinny zawierac i respektowac sekcje Definition of Done oraz Scope Guard.
- Po kazdym epiku nalezy dopisac lub zaktualizowac komentarz QC gate oraz utrzymywac go jako jawny punkt kontroli.
- Po nowych opcode'ach aktualizuj `docs/cpu-slice-map.md`, zeby stan slice byl jawny i aktualny.
- Dla decyzji architektonicznych dodawaj ADR-y w `docs/adr/` zamiast rozpraszac je po komentarzach.

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
- Kazde `gh issue create`, `gh issue edit`, `gh issue comment` i `gh pr create` z body dluzszym niz jedna linia musi uzywac pliku body oraz flagi `--body-file`; nie przekazuj dlugich tresci inline w shellu.
- Pliki body tworz w katalogu tymczasowym albo w `.tmp/issue-bodies/`; nazwa pliku ma zawierac numer issue lub roboczy slug zadania.
- Nie raportuj uzytkownikowi komunikatu typu "przechodze na prostszy zapis przez plik tymczasowy" jako normalnego kroku pracy; uzycie pliku jest obowiazkowym domyslem, nie fallbackiem.
- Jesli wiedza powstaje w trakcie pracy, utrwal ja w issue body lub komentarzu i odwoluj sie do numerow issue w kolejnych zadaniach.

## Shell And GitHub CLI Hygiene
- W tym srodowisku uzywaj `python3`, nie `python`.
- Przy wysylaniu dluzszych tresci do GitHub (`gh issue create`, `gh issue comment`, `gh issue edit`, `gh pr create`) zawsze zapisuj body do pliku i przekazuj przez `--body-file`, zeby backticki, cudzyslowy i nowe linie nie uszkadzaly wiadomosci.
- Gdy pracujesz z JSON z `gh ... --json` lub innym CLI, preferuj `jq` do odczytu i transformacji zamiast parsowania tekstu wyjsciowego shellowymi hackami.

## .NET And Tests
- Docelowy toolchain repo to `.NET 8` CLI; `global.json` preferuje SDK 8, ale moze roll-forward do nowszego lokalnie zainstalowanego SDK.
- Gdy testy juz istnieja, preferuj zawsze weryfikacje zawezona do zmienionej funkcjonalnosci: `dotnet test tests/pseudoCPU.Bootstrap.Tests/pseudoCPU.Bootstrap.Tests.csproj --filter "..."`.
- Nie uruchamiaj pelnego `dotnet test` dla calego repo bez wyraznej potrzeby; w issue zapisuj dokladna komende filtra uzytego do weryfikacji.
- Dla zmian formatowania i konfiguracji utrzymuj `.editorconfig` jako zrodlo prawdy dla stylu C#.

## Domain Focus
- Repo jest przygotowywane pod pseudo CPU inspirowane zgodnoscia opcode-level z 6502, ale wdrazane etapami w ograniczonym zakresie.
- Kazdy task powinien jasno wskazywac, ktorych opcode'ow, flag, rejestrow, addressing modes lub regul wykonania dotyczy, zeby nie mieszac zakresow.
- Nie zapisuj ogolnie "zgodne z 6502", jezeli istnieje ryzyko off-by-one albo kolejnosci bajtow; wypisz dokladny kontrakt semantyczny w epicu i taskach.
