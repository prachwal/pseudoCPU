# Epic 46: Phase 6: 6502 Y register foundation and Y-counter instruction slice

## Epic Issue
- Epic: #46
- Status: blocked
- Owner agent: `issue-planner`
- Execution agent: `issue-executor`
- Quality gate agent: `epic-qc`

## Completion Checklist Entry
- Done: [ ]
- Chapter: Phase 6: 6502 Y register foundation and Y-counter instruction slice
- Epic issue: #46
- Status: blocked
- QC verdict: BLOCKED
- Follow-up: #55

## Outcome
Repo dodaje bootstrapowy rejestr `Y` jako drugi 8-bitowy rejestr indeksowy i domyka pionowy slice instrukcji `LDY #imm`, `INY`, `DEY`, `CPY #imm`, `STY abs`. Po zakończeniu epica procesor, assembler, CLI trace i dokumentacja mają jawny kontrakt dla `Y`, a mapy slice i snapshot aktywnego epica pozostają spójne.

## Domain Scope
- Area: bootstrap CPU register foundation, Y-counter opcodes, assembler, CLI trace, `docs/cpu-slice-map.md`, `docs/pseudoCPU-processor-guide.md`, `docs/current-epic.md`, `docs/current-epic-summary.md`.
- In scope:
  - `Y` jako 8-bitowy rejestr indeksowy,
  - `LDY #imm` (`0xA0`),
  - `INY` (`0xC8`),
  - `DEY` (`0x88`),
  - `CPY #imm` (`0xC0`),
  - `STY abs` (`0x8C`),
  - `LDY` / `INY` / `DEY` aktualizują `Zero` i `Negative`,
  - `CPY` aktualizuje `Carry`, `Zero` i `Negative` bez mutowania `Y`,
  - `STY` zapisuje `Y` do pamięci bez zmian flag,
  - testy CPU/decode, assembler expected bytes, ASM end-to-end i trace/CLI visibility,
  - aktualizacja mapy slice, processor guide i snapshotu aktywnego epica.
- Out of scope:
  - `ADC`, `SBC`, rotacje, shifty i operacje bitowe,
  - nowe tryby adresowania poza immediate / implied / absolute wymaganymi przez ten slice,
  - zero page i indexed addressing by `Y`,
  - przerwania, wektory resetu, `RTI`, cycle counting,
  - etykiety assemblera,
  - `.org`, `.byte`, `.word`,
  - nowe komendy CLI.

## Chapter Narrative
Ten chapter dokumentuje zamknięty zakres budowy rejestru `Y` i instrukcji licznikowych wokół niego. Slice jest strukturalnie podobny do wcześniejszego `X`-counter slice, ale nie rozszerza modelu o tryby indeksowane ani pełny status register. `CPY` pozostaje porównaniem bootstrapowym opartym o `Carry` / `Zero` / `Negative`.

## Task Issues
- [x] #47 - add bootstrap `Y` register foundation in core.
- [x] #48 - implement `LDY`, `INY` and `DEY` CPU/decode semantics.
- [x] #49 - implement `CPY` and `STY` CPU/decode semantics.
- [x] #50 - add `Y` register interaction and regression tests.
- [x] #51 - extend assembler and add ASM end-to-end coverage for `Y` slice.
- [x] #52 - update CLI trace for `Y` register visibility.
- [x] #53 - update slice map and chapter documentation for `Y` slice.
- [x] #54 - run epic QC gate for `Y` register slice.

## Acceptance Criteria
- [ ] Bootstrap CPU exposes 8-bit `Y` register without regressing current `A`, `X`, `SP`, `PC` contracts.
- [ ] `LDY #imm`, `INY`, `DEY`, `CPY #imm`, `STY abs` are decoded, executed and tested.
- [ ] `LDY`, `INY`, `DEY` update only `Y`, `PC` and `Zero` / `Negative` according to their contract.
- [ ] `INY` and `DEY` use 8-bit wrap-around semantics.
- [ ] `CPY #imm` sets `Carry`, `Zero` and `Negative` according to `Y - operand` without mutating `Y`.
- [ ] `STY abs` writes `Y` in little-endian absolute addressing order and does not change flags.
- [ ] At least one assembler-driven program exercises the new `Y` slice.
- [ ] Trace/CLI remains aligned with implemented instruction set and visible register state.
- [ ] `docs/cpu-slice-map.md`, `docs/pseudoCPU-processor-guide.md`, `docs/current-epic.md`, `docs/current-epic-summary.md` and `docs/epic-chapters.md` are updated.
- [ ] `dotnet build pseudoCPU.sln` passes.
- [ ] `dotnet test tests/pseudoCPU.Bootstrap.Tests/pseudoCPU.Bootstrap.Tests.csproj` passes.
- [ ] `dotnet format pseudoCPU.sln --verify-no-changes` passes.
- [ ] Epic passes `epic-qc`; if not, follow-up issues are created and linked before closure.
- [ ] `issue-sync` synchronizes the main epic body and chapter docs before epic closure.

## Verification Strategy
- Narrow tests:
  - `dotnet test tests/pseudoCPU.Bootstrap.Tests/pseudoCPU.Bootstrap.Tests.csproj --filter "FullyQualifiedName~Y|FullyQualifiedName~Ldy|FullyQualifiedName~Iny|FullyQualifiedName~Dey|FullyQualifiedName~Cpy|FullyQualifiedName~Sty"`
- Full test project:
  - `dotnet test tests/pseudoCPU.Bootstrap.Tests/pseudoCPU.Bootstrap.Tests.csproj`
- Build:
  - `dotnet build pseudoCPU.sln`
- Format:
  - `dotnet format pseudoCPU.sln --verify-no-changes`
- CLI smoke:
  - `dotnet run --project src/pseudoCPU.Cli -- run-asm --source examples/y-counter-loop.asm --start 0x0600 --max-steps 100 --trace`

## Documentation Updates
- [x] `docs/cpu-slice-map.md` po dodaniu `LDY`, `INY`, `DEY`, `CPY`, `STY`.
- [x] `docs/pseudoCPU-processor-guide.md` po dopisaniu rejestru `Y` i semantyki instrukcji.
- [x] `docs/epic-chapters.md` po dodaniu aktywnego chaptera do indeksu.
- [x] `docs/current-epic.md` po ustawieniu stanu epica na `blocked`.
- [x] `docs/current-epic-summary.md` po ustawieniu stanu routingowego na `blocked`.

## QC Gate
- QC issue/comment: #54
- Verdict: BLOCKED
- Follow-up issues:
  - #55 - fix `LDX` immediate zero-flag regression discovered during QC.

## Final Notes
- Final status: blocked
- Remaining risks: follow-up #55 is required to resolve the failing full bootstrap test (`BootstrapCpuTests.LdxImmediateUpdatesZeroAndNegativeFlags(value: 0, expectedZero: False, expectedNegative: False)`).
- Permanent decisions: `Y` pozostaje 8-bitowym rejestrem bootstrapowym; `CPY` używa bootstrapowego modelu flag `Carry` / `Zero` / `Negative` bez rozszerzania status register.
