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
