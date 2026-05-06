# Epic Chapters

Ten dokument definiuje sposob prowadzenia dokumentacji tak, aby jeden rozdzial dokumentacji odpowiadal jednemu epicowi GitHub.

## Rule

Jeden chapter = jeden epic.

Kazdy nowy epic musi miec jeden dedykowany rozdzial w tym pliku albo w osobnym pliku chaptera podlinkowanym z tego indeksu. Rozdzial nie jest zamiennikiem GitHub issue. GitHub issue pozostaje zrodlem prawdy dla planu, taskow, postepu i wynikow, a chapter jest stabilnym opisem produktowo-technicznym epica.

## Status Values

| Status | Meaning |
|---|---|
| `planned` | Chapter opisany, ale epic nie jest jeszcze aktywnie realizowany. |
| `active` | Epic jest w trakcie planowania albo implementacji. |
| `qc` | Implementacja jest zakonczona i czeka na bramke `epic-qc`. |
| `done` | Epic ma pozytywny QC gate i chapter jest zamkniety. |
| `blocked` | Epic nie moze isc dalej bez decyzji albo zaleznosci. |
| `superseded` | Chapter zostal zastapiony innym epicem/chapterem. |

## Chapter Lifecycle

1. Planner tworzy albo aktualizuje chapter przed utworzeniem taskow epica.
2. Planner tworzy epic issue i task issues, a nastepnie linkuje ich numery w chapterze.
3. Planner aktualizuje `Chapter Completion Checklist` oraz `Current Chapter Index` przy kazdej zmianie statusu epica.
4. Executor aktualizuje issue, a nie chapter, podczas codziennego postepu.
5. Po zakonczeniu epica `epic-qc` zapisuje werdykt QC w issue i aktualizuje finalny status chaptera, jesli trzeba utrwalic decyzje.
6. Tymczasowy stan biezacej pracy trafia do `docs/current-epic.md`; trwale wnioski z epica zostaja w chapterze, ADR albo dokumentacji domenowej.

## Chapter Completion Checklist

Ta tabela jest glowna lista kontrolna realizacji chapterow. Planner musi aktualizowac ja przy planowaniu nowego epica, przejsciu do implementacji, przejsciu do QC oraz zamknieciu epica.

| Done | Chapter | Epic Issue | Status | QC Verdict | Follow-up | Notes |
|---|---|---:|---|---|---|---|
| [ ] | Phase 3 quality control | #25 | active | TBD | TBD | Cleanup po fazie 3: Carry, trace/assembler drift, CLI summary i regresja testow. |
| [ ] | Phase 4: 6502 stack page foundation and JSR/RTS subroutine flow | #26 | qc | TBD | TBD | Stack page `$0100-$01FF`, `SP`, push/pop semantics, `JSR abs`, `RTS`, assembler/CLI/docs and mandatory QC gate #35. |

## Chapter Template

Skopiuj ten szablon dla kazdego nowego epica.

```markdown
## Epic <nr>: <chapter title>

### Epic Issue
- Epic: #<number>
- Status: planned | active | blocked | qc | done | superseded
- Owner agent: `issue-planner`
- Execution agent: `issue-executor`
- Quality gate agent: `epic-qc`

### Completion Checklist Entry
- Done: [ ]
- Chapter: <chapter title>
- Epic issue: #<number>
- Status: planned | active | blocked | qc | done | superseded
- QC verdict: PASS | PASS_WITH_FOLLOW_UP | BLOCKED | TBD
- Follow-up: #<number> | none | TBD

### Outcome
<Jakie zachowanie, mozliwosc albo kontrakt ma istniec po zakonczeniu epica.>

### Domain Scope
- Opcode/register/flag/addressing mode/CLI/doc area: <wypelnij>
- In scope:
  - <punkt>
- Out of scope:
  - <punkt>

### Chapter Narrative
<Opis rozdzialu w dokumentacji. Ma byc stabilny i czytelny bez czytania calego watku issue.>

### Task Issues
- [ ] #<task> - <cel taska>

### Acceptance Criteria
- [ ] <kryterium>

### Verification Strategy
- Narrow tests:
  - `dotnet test <test-project> --filter "..."`
- Build:
  - `dotnet build pseudoCPU.sln`
- Format:
  - `dotnet format pseudoCPU.sln --verify-no-changes`

### Documentation Updates
- [ ] `docs/cpu-slice-map.md`, jesli zmienia sie slice CPU.
- [ ] ADR w `docs/adr/`, jesli zapadla trwala decyzja architektoniczna.
- [ ] Przyklad w `examples/`, jesli epic dodaje nowy przeplyw uzycia.

### QC Gate
- QC issue/comment: #<number or comment link>
- Verdict: PASS | PASS_WITH_FOLLOW_UP | BLOCKED | TBD
- Follow-up issues:
  - #<issue> - <powod>

### Final Notes
- Final status: planned | active | blocked | qc | done | superseded
- Remaining risks:
- Permanent decisions:
```

## Current Chapter Index

| Chapter | Epic Issue | Status | Scope |
|---|---:|---|---|
| Phase 3 quality control | #25 | active | Carry semantics, trace/assembler drift, CLI summary and test regression cleanup |
| Phase 4: 6502 stack page foundation and JSR/RTS subroutine flow | #26 | qc | Stack page `$0100-$01FF`, 8-bit `SP`, push/pop ordering, `JSR abs`, `RTS`, assembler/CLI/docs |

## Epic 25: Phase 3 quality control

### Epic Issue
- Epic: #25
- Status: active
- Owner agent: `issue-planner`
- Execution agent: `issue-executor`
- Quality gate agent: `epic-qc`

### Completion Checklist Entry
- Done: [ ]
- Chapter: Phase 3 quality control
- Epic issue: #25
- Status: active
- QC verdict: TBD
- Follow-up: TBD

### Outcome
Repo wraca do zielonego i spojnego stanu po fazie 3: testy odzwierciedlaja aktualna semantyke `Carry`, trace CLI i assembler nie rozjezdzaja sie w zapisie branch offsetow, a finalny output CLI pokazuje komplet aktualnie wspieranych flag.

### Domain Scope
- Area: aktualny bootstrapowy slice CPU, `Carry`, `CMP #imm`, `CPX #imm`, branch offsety, trace CLI i finalny summary CLI.
- In scope:
  - naprawa regresji testu po zmianie semantyki `CMP #imm`,
  - sprawdzenie testow zależnych od `Carry`,
  - ujednolicenie kontraktu branch offsetow miedzy trace CLI i assemblerem,
  - dopisanie `Carry` do finalnego summary CLI,
  - aktualizacja testow i dokumentacji kontraktu, jesli sie zmieni.
- Out of scope:
  - nowe opcode'y,
  - etykiety assemblera,
  - dyrektywy `.org`, `.byte`, `.word`,
  - pelny status register 6502,
  - cycle counting,
  - przerwania,
  - nowe komendy CLI poza zakresem korekty.

### Chapter Narrative
Ten chapter opisuje epic stabilizacyjny po fazie 3. Jego celem nie jest rozbudowa CPU o nowy zakres funkcjonalny, tylko zamkniecie niespojnosci wykrytych po implementacji: test po `CMP #imm` musi odpowiadac nowemu modelowi `Carry`, output trace nie powinien emitowac skladni nieakceptowanej przez assembler, a finalny summary CLI powinien pokazywac `Carry`, skoro flaga jest juz czescia publicznego zachowania aktualnego slice.

### Task Issues
- [ ] #25 - quality control task dla phase 3 cleanup.

### Acceptance Criteria
- [ ] Projekt testowy `tests/pseudoCPU.Bootstrap.Tests` przechodzi.
- [ ] Test `BootstrapCpuTests.BneDoesNotBranchWhenZeroFlagIsSet` odzwierciedla nowa semantyke `CMP #imm`.
- [ ] Nie ma testow oczekujacych starego zachowania `Carry` po `CMP #imm` albo `CPX #imm`.
- [ ] Trace CLI i assembler maja spojny kontrakt branch offsetow.
- [ ] Finalny output CLI pokazuje `Carry`.
- [ ] Dokumentacja jest zaktualizowana, jesli kontrakt trace/outputu sie zmieni.

### Verification Strategy
- Narrow tests:
  - `dotnet test tests/pseudoCPU.Bootstrap.Tests/pseudoCPU.Bootstrap.Tests.csproj --filter "FullyQualifiedName~BootstrapCpuTests.BneDoesNotBranchWhenZeroFlagIsSet"`
- Full test project:
  - `dotnet test tests/pseudoCPU.Bootstrap.Tests/pseudoCPU.Bootstrap.Tests.csproj`
- Build:
  - `dotnet build pseudoCPU.sln`
- Format:
  - `dotnet format pseudoCPU.sln --verify-no-changes`
- CLI smoke:
  - `dotnet run --project src/pseudoCPU.Cli -- run-asm --source examples/x-counter-loop.asm --start 0x0600 --max-steps 100 --trace`

### Documentation Updates
- [ ] `docs/cpu-slice-map.md`, jesli zmieni sie kontrakt trace, assemblera albo flag.
- [ ] Ten chapter po finalnym QC.

### QC Gate
- QC issue/comment: TBD
- Verdict: TBD
- Follow-up issues:
  - TBD

### Final Notes
- Final status: active
- Remaining risks: testy i kontrakt CLI/assembler wymagaja potwierdzenia po implementacji.
- Permanent decisions: chapter odpowiada epicowi i jest stabilnym opisem po zamknieciu pracy.

## Epic 26: Phase 4: 6502 stack page foundation and JSR/RTS subroutine flow

### Epic Issue
- Epic: #26
- Status: qc
- Owner agent: `issue-planner`
- Execution agent: `issue-executor`
- Quality gate agent: `epic-qc`

### Completion Checklist Entry
- Done: [ ]
- Chapter: Phase 4: 6502 stack page foundation and JSR/RTS subroutine flow
- Epic issue: #26
- Status: qc
- QC verdict: TBD
- Follow-up: TBD

### Outcome
Repo zyskuje bootstrapowy model stacka zgodny semantycznie z 6502 dla aktualnego zakresu: stack page `$0100-$01FF`, 8-bitowy `SP`, poprawne push/pop oraz obsluge `JSR abs` i `RTS`. Po domknieciu chaptera kolejne epiki moga dodawac instrukcje stackowe i przerwania bez zgadywania adresowania, inicjalizacji `SP` i kontraktu powrotu z podprogramu.

### Domain Scope
- Area: stack page `$0100-$01FF`, rejestr `SP`, helpery push/pop, `JSR abs`, `RTS`, assembler, CLI trace, `docs/cpu-slice-map.md` i `docs/current-epic.md`.
- In scope:
  - jawny `SP` jako 8-bitowy rejestr CPU,
  - projektowy kontrakt `LoadProgram -> SP = $FF`,
  - adres efektywny stacka jako `$0100 + SP`,
  - push/pop zgodne z 6502 dla tego slice,
  - dekoder i wykonanie `JSR abs`,
  - dekoder i wykonanie `RTS`,
  - testy stack semantics, wrap-around i subroutine flow,
  - wsparcie assemblera dla `JSR` / `RTS`,
  - aktualizacja trace/CLI, jesli pokazuje mnemoniki instrukcji,
  - aktualizacja mapy slice CPU i chaptera.
- Out of scope:
  - IRQ/NMI/RESET vectors,
  - `BRK` vector,
  - `PHA`, `PLA`, `PHP`, `PLP`,
  - cycle counting,
  - pelny status register 6502 jako bajt,
  - etykiety assemblera,
  - `.org`, `.byte`, `.word`,
  - nowe komendy CLI poza minimalnym smoke/test flow.

### Chapter Narrative
Ten chapter wprowadza pierwszy normalny model stacka 6502 do bootstrapowego pseudoCPU. Zamiast traktowac subroutine flow jako specjalny przypadek skoku, epic ustala wspolny kontrakt stack page, inicjalizacji `SP` i kolejnosci push/pop, a potem opiera na nim `JSR abs` oraz `RTS`. Rozdzial celowo nie dodaje jeszcze innych instrukcji stackowych ani przerwan; jego wartoscia jest stabilny fundament, na ktorym kolejne epiki beda mogly bez driftu budowac `PHA`/`PLA`, wejscie w IRQ/NMI i dalsza zgodnosc 6502.

### Task Issues
- [x] #27 - normalny model stacka 6502 w Core.
- [x] #28 - testy `SP`, push/pop i wrap-around.
- [x] #29 - implementacja `JSR abs` w decoderze i CPU.
- [x] #30 - implementacja `RTS` na bazie stacka.
- [x] #31 - testy pelnego przeplywu podprogramow `JSR` / `RTS`.
- [x] #32 - wsparcie assemblera dla `JSR` / `RTS` i ASM end-to-end.
- [x] #33 - aktualizacja trace/CLI dla nowych mnemonikow.
- [x] #34 - aktualizacja `docs/cpu-slice-map.md`, chaptera i snapshotu biezacego epica.
- [ ] #35 - QC gate przez `epic-qc`.

### Acceptance Criteria
- [ ] `BootstrapCpu` utrzymuje `SP` jako 8-bitowy rejestr i nie wychodzi ze stack page `$0100-$01FF`.
- [ ] `LoadProgram` inicjalizuje `SP` na `$FF` i kontrakt jest zapisany w issue/chapterze.
- [ ] Push zapisuje pod `$0100 + SP`, a potem dekrementuje `SP`; pop najpierw inkrementuje `SP`, a potem czyta z `$0100 + SP`.
- [ ] `JSR abs` zapisuje poprawny adres powrotu i ustawia `PC` na cel wywolania.
- [ ] `RTS` odtwarza adres powrotu ze stacka i wznawia wykonanie na instrukcji po `JSR`.
- [ ] Istnieja testy na wrap-around stacka i na co najmniej jeden nested/sequential subroutine flow.
- [ ] Assembler rozumie `JSR $addr` i `RTS`, a co najmniej jeden test ASM end-to-end pokrywa podprogram.
- [ ] CLI trace, jesli pokazuje instrukcje, umie wypisac `JSR` i `RTS` bez driftu wobec assemblera.
- [ ] `docs/cpu-slice-map.md` odzwierciedla nowy slice po implementacji.
- [ ] Epic przechodzi `epic-qc`, a ewentualne follow-up issues sa jawnie zalinkowane.

### Verification Strategy
- Narrow tests:
  - `dotnet test tests/pseudoCPU.Bootstrap.Tests/pseudoCPU.Bootstrap.Tests.csproj --filter "FullyQualifiedName~Stack|FullyQualifiedName~Jsr|FullyQualifiedName~Rts"`
- Full test project:
  - `dotnet test tests/pseudoCPU.Bootstrap.Tests/pseudoCPU.Bootstrap.Tests.csproj`
- Build:
  - `dotnet build pseudoCPU.sln`
- Format:
  - `dotnet format pseudoCPU.sln --verify-no-changes`
- CLI smoke:
  - `dotnet run --project src/pseudoCPU.Cli -- run-asm --source examples/jsr-rts-subroutine.asm --start 0x0600 --max-steps 100 --trace`

### Documentation Updates
- [ ] `docs/cpu-slice-map.md` po dodaniu `JSR abs`, `RTS` i stack foundation.
- [ ] Ten chapter po przejsciu taskow i po QC gate.
- [ ] `docs/current-epic.md` dla aktywnego stanu i snapshotu weryfikacji.
- [ ] ADR w `docs/adr/` tylko jesli implementacja wymusi trwala decyzje wykraczajaca poza chapter.

### QC Gate
- QC issue/comment: #35
- Verdict: TBD
- Follow-up issues:
  - TBD

### Final Notes
- Final status: qc
- Remaining risks: off-by-one w adresie powrotu `JSR`/`RTS`, kolejnosc bajtow na stacku i drift miedzy assemblerem a trace CLI.
- Permanent decisions: bootstrapowy kontrakt startu `SP` to `$FF` do czasu osobnego epica reset/interrupt; stack semantics maja pozostac wspolnym fundamentem dla przyszlych instrukcji stackowych.
