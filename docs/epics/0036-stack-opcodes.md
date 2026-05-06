# Epic 36: Phase 5: 6502 stack opcode slice PHA/PLA/PHP/PLP

## Epic Issue
- Epic: #36
- Status: done
- Owner agent: `issue-planner`
- Execution agent: `issue-executor`
- Quality gate agent: `epic-qc`

## Completion Checklist Entry
- Done: [x]
- Chapter: Phase 5: 6502 stack opcode slice PHA/PLA/PHP/PLP
- Epic issue: #36
- Status: done
- QC verdict: PASS_WITH_FOLLOW_UP
- Follow-up: #45

## Outcome
Repo dodaje drugi stackowy slice 6502 na fundamencie #26: `PHA`, `PLA`, `PHP`, `PLP`, ich testy CPU, assembler, trace/CLI i dokumentacje. Najwazniejszym warunkiem gotowosci jest jawny bootstrapowy kontrakt status byte dla `PHP` / `PLP`, zdefiniowany przed implementacja.

## Domain Scope
- Area: stack opcodes implied, status snapshot, assembler, CLI trace, `docs/cpu-slice-map.md`, `docs/current-epic.md`.
- In scope:
  - `PHA` (`0x48`) implied,
  - `PLA` (`0x68`) implied,
  - `PHP` (`0x08`) implied,
  - `PLP` (`0x28`) implied,
  - status byte contract dla aktualnie wspieranych flag `Carry`, `Zero`, `Negative`,
  - `PLA` aktualizuje `Zero` i `Negative`,
  - `PLP` odtwarza wspierane flagi zgodnie z kontraktem #37,
  - testy interakcji stack opcode slice,
  - assembler expected bytes i ASM end-to-end,
  - trace/CLI visibility,
  - aktualizacja mapy slice i snapshotu aktywnego epica.
- Out of scope:
  - IRQ/NMI/RESET vectors,
  - `BRK` vector behavior beyond current halt contract,
  - `RTI`,
  - pelny status register 6502 jako docelowy model procesora,
  - decimal/interrupt/overflow/break live semantics poza bootstrapowym snapshotem,
  - cycle counting,
  - addressing modes inne niz implied,
  - etykiety assemblera,
  - `.org`, `.byte`, `.word`,
  - nowe komendy CLI.

## Chapter Narrative
Ten chapter rozszerza gotowy model stacka o podstawowe instrukcje odkładania i zdejmowania akumulatora oraz statusu. `PHA`/`PLA` sprawdzają dyscypline stacka dla danych, a `PHP`/`PLP` wprowadzają pierwszy jawny kontrakt status byte bez udawania pełnego status register 6502. Rozdział celowo nie dotyka przerwań, `RTI`, cycle countingu ani pełnych flag procesora.

## Task Issues
- [x] #37 - define bootstrap status byte contract for PHP and PLP.
- [x] #38 - implement PHA and PLA CPU/decode semantics.
- [x] #39 - implement PHP and PLP CPU/decode semantics.
- [x] #40 - add stack opcode interaction and regression tests.
- [x] #41 - extend assembler for PHA/PLA/PHP/PLP and add ASM end-to-end coverage.
- [x] #42 - update CLI trace for stack opcode visibility.
- [x] #43 - update slice map and chapter documentation for stack opcode slice.
- [x] #44 - QC gate przez `epic-qc`.

## Acceptance Criteria
- [x] Bootstrap status byte contract for `PHP` / `PLP` is documented before implementation.
- [x] `PHA`, `PLA`, `PHP`, `PLP` are decoded, executed and tested.
- [x] `PHA` pushes `A` and changes only `SP` / stack memory.
- [x] `PLA` pulls into `A` and updates `Zero` / `Negative`.
- [x] `PHP` pushes status snapshot for currently supported flags.
- [x] `PLP` restores currently supported flags from status snapshot without silently adding unsupported status behavior.
- [x] Stack order and `SP` restoration are covered by tests.
- [x] At least one assembler-driven stack opcode program is covered by tests.
- [x] Trace/CLI remains aligned with implemented instruction set and new mnemonics.
- [x] `docs/cpu-slice-map.md`, `docs/current-epic.md` and `docs/epic-chapters.md` are updated.
- [x] Epic passes `epic-qc`, a ewentualne follow-up issues sa jawnie zalinkowane.

## Verification Strategy
- Narrow tests:
  - `dotnet test tests/pseudoCPU.Bootstrap.Tests/pseudoCPU.Bootstrap.Tests.csproj --filter "FullyQualifiedName~Pha|FullyQualifiedName~Pla|FullyQualifiedName~Php|FullyQualifiedName~Plp|FullyQualifiedName~StackOpcode"`
- Full test project:
  - `dotnet test tests/pseudoCPU.Bootstrap.Tests/pseudoCPU.Bootstrap.Tests.csproj`
- Build:
  - `dotnet build pseudoCPU.sln`
- Format:
  - `dotnet format pseudoCPU.sln --verify-no-changes`
- CLI smoke:
  - `dotnet run --project src/pseudoCPU.Cli -- run-asm --source examples/stack-opcodes.asm --start 0x0600 --max-steps 100 --trace`

## Documentation Updates
- [x] `docs/cpu-slice-map.md` po dodaniu `PHA`, `PLA`, `PHP`, `PLP`.
- [x] Ten chapter po przejsciu taskow i po QC gate.
- [x] `docs/current-epic.md` dla aktywnego stanu i snapshotu weryfikacji.
- [ ] ADR w `docs/adr/`, jesli status byte contract okaze sie decyzja wykraczajaca poza chapter.

## QC Gate
- QC issue/comment: https://github.com/prachwal/pseudoCPU/issues/36#issuecomment-4387471999
- Verdict: PASS_WITH_FOLLOW_UP
- Follow-up issues:
  - #45 - drift dokumentacji w `docs/pseudoCPU-processor-guide.md` po epikach #26 i #36.

## Final Notes
- Final status: done
- Remaining risks: `docs/pseudoCPU-processor-guide.md` pozostaje niespójny z wdrożonym stack slice do czasu zamknięcia #45.
- Permanent decisions: bootstrapowy status byte pozostaje ograniczony do `Carry` bit 0, `Zero` bit 1 i `Negative` bit 7; bity 2-6 pozostają zarezerwowane/ignorowane do osobnego epica na pełny status register.
