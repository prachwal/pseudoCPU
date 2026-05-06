# Agentic Epic Flow

Ten dokument definiuje stabilny workflow pracy agentow dla planowania, realizacji i kontroli jakosci epikow w repo `pseudoCPU`.

## Core Rule

Jeden realny GitHub epic issue = jeden chapter file w `docs/epics/`.

GitHub issue pozostaje zrodlem prawdy dla planu, taskow, postepu, wynikow testow i decyzji. Chapter jest trwalym opisem produktowo-technicznym epica, czytelnym po zamknieciu pracy.

## Object Types

### Epic issue

Epic issue to GitHub issue z labelem `epic`. Tylko takie issue moze miec chapter file w `docs/epics/`.

Epic musi zawierac: Outcome, In Scope, Out Of Scope, Task Breakdown, Recommended Execution Order, Verification Strategy, Acceptance Criteria, Knowledge Log i Progress Log.

### Task issue

Task issue to pojedynczy wykonawczy albo kontrolny krok epica. Task nie jest chapterem.

Task musi zawierac: Goal, Scope, Definition of Done, Scope Guard, Acceptance Criteria, Verification, Dependencies, Progress Log i Final Notes.

### QC task

QC task jest zwyklym task issue uruchamiajacym `epic-qc`. QC task nie jest follow-up issue i nie powinien byc wpisywany w polu `Follow-up` checklisty chaptera.

### Follow-up issue

Follow-up issue powstaje dopiero wtedy, gdy `epic-qc` wykryje brak, regresje, niespojnosc kontraktu, ryzyko albo dlug techniczny wymagajacy osobnej pracy.

### Current epic state

`docs/current-epic.md` przechowuje stan aktywnego epica: numer issue, zakres, biezace decyzje, snapshot weryfikacji i follow-upy.

`docs/current-epic-summary.md` przechowuje krotki routing context dla agentow.

Nie przechowuj biezacej fazy w `AGENTS.md`.

### Epic chapter index

`docs/epic-chapters.md` jest lekkim indeksem chapterow. Nie przechowuj tam pelnych historycznych chapterow.

### Epic chapter file

Pelny chapter realnego epica trzymamy w `docs/epics/<epic-number>-<slug>.md`.

Nie tworz chaptera dla task issue, QC task issue ani follow-up issue bez labela `epic`.

### Issue sync gate

`issue-sync` synchronizuje glowne body epica z child taskami, komentarzem QC, relevantnym chapter file, `docs/epic-chapters.md`, `docs/current-epic.md` i `docs/current-epic-summary.md`. To osobny gate przed zamknieciem epica.

## Work Modes

### Small-task mode

Uzywaj dla malych zmian bez zmiany kontraktu publicznego i bez potrzeby chaptera.

Wymaga:

- jednego task issue,
- waskiego scope,
- waskiej komendy testowej,
- finalnego komentarza,
- bez `epic-qc`,
- bez `issue-sync`, chyba ze task nalezy do aktywnego epica.

### Epic mode

Uzywaj dla zmian wieloetapowych, domenowych, kontraktowych, architektonicznych albo wymagajacych QC.

Wymaga:

- epic issue,
- task issues,
- chapter file w `docs/epics/`,
- wpisu w `docs/epic-chapters.md`,
- `docs/current-epic.md`,
- `docs/current-epic-summary.md`,
- `epic-qc`,
- `issue-sync` przed closure.

## Planner Flow

`issue-planner` wykonuje te kroki:

1. Sprawdza aktualny stan repo, istniejace issues i lokalne instrukcje.
2. Identyfikuje, czy praca wymaga small-task mode, nowego epica, taska czy follow-upa.
3. Dla nowego epica tworzy chapter file w `docs/epics/<epic-number>-<slug>.md`.
4. Dodaje wpis do `docs/epic-chapters.md`.
5. Tworzy GitHub epic issue z labelem `epic`.
6. Tworzy task issues z jasno okreslonym zakresem.
7. Linkuje taski w epic issue i chapterze.
8. Aktualizuje `docs/current-epic.md` i `docs/current-epic-summary.md` jako stan aktywnego epica.
9. Dodaje obowiazkowy task QC gate na koncu epica.
10. Dodaje obowiazkowy krok `issue-sync` przed zamknieciem epica.
11. Przekazuje pierwszy wykonawczy task do `issue-executor`.

## Executor Flow

`issue-executor` wykonuje jeden task naraz:

1. Czyta task issue, parent epic i lokalne instrukcje.
2. Tworzy nowy branch roboczy przed zmianami.
3. Dodaje komentarz startowy do issue przez plik body.
4. Implementuje tylko zakres issue.
5. Uruchamia waskie testy z issue.
6. Przed finalizacja robi jawny finalization checkpoint: zapisuje postep, potwierdza wynik testow i dzieli prace na osobny pass finalizacyjny, jesli istnieje ryzyko utraty koncowki przez limit sesji.
7. Finalization pass obejmuje tylko commit, push, merge, cleanup brancha, finalny komentarz i ewentualne zamkniecie issue.
8. Jeśli finalizacja nie domyka sie w ograniczonej liczbie prob, executor przestaje powtarzac ta sama petle i zapisuje task jako `failed` albo `blocked` z konkretnym nastepnym ruchem.
9. Zamyka issue tylko wtedy, gdy Definition of Done jest spelnione.

## QC Flow

`epic-qc` wykonuje bramke jakosci po taskach implementacyjnych i dokumentacyjnych:

1. Czyta epic issue, child task issues, komentarze, `docs/current-epic-summary.md`, relevantny chapter file, `docs/epic-chapters.md`, `AGENTS.md` i potrzebne dokumenty domenowe.
2. Sprawdza, czy scope epica jest dowieziony bez cichego rozszerzania.
3. Sprawdza acceptance criteria, testy, build, formatowanie i dokumentacje.
4. Sprawdza, czy chapter file istnieje tylko dla realnego epic issue.
5. Sprawdza, czy `Chapter Completion Checklist` nie myli QC taska z follow-up issue.
6. Jesli sa defekty, tworzy follow-up issues przez plik body.
7. Dodaje komentarz `Epic QC Gate` do epica.
8. Aktualizuje chapter status i indeks.
9. Nie zamyka epica, dopoki `issue-sync` nie potwierdzi synchronizacji glownego body epica.

## Epic Body Sync Gate

Przed zamknieciem epica musi zostac uruchomiony `issue-sync` albo rownowazny krok synchronizacji.

Epic closure is forbidden while the main epic issue body still has stale unchecked task or acceptance checkboxes that are already completed in child issues, QC comments or chapter docs.

`issue-sync` musi sprawdzic i zaktualizowac:

- `Task Breakdown` w glownym body epica,
- `Acceptance Criteria` w glownym body epica,
- placeholdery sciezek i nazw plikow,
- link do komentarza `Epic QC Gate`,
- follow-up issues po `PASS_WITH_FOLLOW_UP` albo `BLOCKED`,
- `Final Notes` w glownym body epica,
- `docs/epic-chapters.md`,
- relevantny `docs/epics/<epic>.md`,
- `docs/current-epic.md`,
- `docs/current-epic-summary.md`.

Glowny epic body musi byc aktualizowany przez plik body i `gh issue edit --body-file <file>`.

## GitHub CLI Body Rule

Kazde multiline body dla GitHub CLI musi byc zapisane do pliku i przekazane przez `--body-file`.

Dotyczy to `gh issue create`, `gh issue edit`, `gh issue comment` i `gh pr create`.

Nie przekazuj wieloliniowego markdownu przez inline `--body`. Plik body jest domyslnym mechanizmem, nie fallbackiem po bledzie skladni.

## Chapter Checklist Status Rules

| Status | When to use |
|---|---|
| `planned` | Chapter/epic jest zaplanowany, ale taski nie sa jeszcze wykonywane. |
| `active` | Epic albo taski sa aktualnie wykonywane. |
| `qc` | Implementacja skonczona, czeka na `epic-qc`. |
| `done` | QC gate zakonczony pozytywnie i epic mozna zamknac. |
| `blocked` | Epic wymaga decyzji lub zaleznosci. |
| `superseded` | Epic/chapter zostal zastapiony innym zakresem. |

`Done` ustawiaj na `[x]` tylko dla `done`.

`QC Verdict` ustawiaj na `TBD`, `PASS`, `PASS_WITH_FOLLOW_UP` albo `BLOCKED`.

`Follow-up` ustawiaj na `TBD` przed QC, `none` jesli QC nie utworzyl follow-upow albo numery faktycznych follow-up issues. Nie wpisuj numeru QC taska jako follow-up.

## Domain Precision Rule

Szczegolowe reguly domenowe 6502 sa w `docs/6502-domain-rules.md`.

Kazdy epic i task domenowy musi precyzowac kontrakt: opcode, flagi, rejestry, addressing modes, zmiany assemblera, trace/CLI i dokumenty.

## Definition of Ready for Epic Execution

Epic jest gotowy do realizacji, gdy:

- ma label `epic`,
- ma chapter file w `docs/epics/`,
- ma wpis w `docs/epic-chapters.md`,
- ma komplet task issues,
- taski sa linkowane z epica i chaptera,
- `docs/current-epic.md` i `docs/current-epic-summary.md` wskazuja aktywny epic,
- jest wskazany task QC gate,
- jest wskazany krok `issue-sync` przed zamknieciem,
- acceptance criteria zawieraja testy i dokumentacje,
- kontrakty domenowe sa precyzyjne,
- placeholdery typu `examples/<file>.asm` maja task, ktory zamieni je na realne pliki przed QC.

## Definition of Done for Epic

Epic jest zakonczony, gdy:

- wszystkie child task issues sa zamkniete albo jawnie superseded,
- testy, build i formatowanie sa zapisane z wynikami w issue,
- dokumentacja domenowa i chapter sa zaktualizowane,
- `epic-qc` dodal komentarz `Epic QC Gate`,
- follow-up issues sa utworzone i zalinkowane, jesli QC wykryl braki,
- glowne body epica jest zsynchronizowane: taski `[x]`, acceptance `[x]`, realne sciezki, link QC gate, follow-up issues i final notes,
- `issue-sync` dodal komentarz synchronizacji albo final notes potwierdzajace synchronizacje body,
- `Chapter Completion Checklist` ma poprawny status,
- `docs/current-epic.md` i `docs/current-epic-summary.md` sa zaktualizowane albo przygotowane pod kolejny epic.
