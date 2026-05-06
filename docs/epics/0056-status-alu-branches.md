# Epic 56: Phase 7: full status register and core flag/arithmetic completion

## Epic Issue
- Epic: #56
- Status: planned
- Owner agent: `issue-planner`
- Execution agent: `issue-executor`
- Quality gate agent: `epic-qc`

## Completion Checklist Entry
- Done: [ ]
- Chapter: Phase 7: full status register and core flag/arithmetic completion
- Epic issue: #56
- Status: planned
- QC verdict: TBD
- Follow-up: TBD

## Outcome
Repo ma przejść z bootstrapowego modelu `Carry` / `Zero` / `Negative` do znacznie pełniejszej semantyki status register i kluczowych opcodes ALU oraz branchy zależnych od flag.

## Domain Scope
- Area: status register, control flags, ALU immediate, accumulator shifts/rotates, flag-driven branches, assembler/CLI/docs.
- In scope:
  - pełny status register contract,
  - `ADC #imm`, `SBC #imm`,
  - `BIT` w jednym jawnie uzgodnionym trybie,
  - `ASL A`, `LSR A`, `ROL A`, `ROR A`,
  - `CLC`, `SEC`, `CLI`, `SEI`, `CLD`, `SED`, `CLV`,
  - `BCC`, `BCS`, `BMI`, `BPL`, `BVC`, `BVS`,
  - assembler expected bytes, trace/CLI visibility, docs.
- Out of scope:
  - nowe addressing modes poza wskazanym `BIT`,
  - interrupt execution flow,
  - cycle counting jako warunek fazy,
  - labels i dyrektywy assemblera.

## Chapter Narrative
Ten chapter domyka brakujący środek ciężkości semantyki CPU: status register przestaje być tylko bootstrapowym snapshotem dla kilku flag, a staje się bazą pod ALU, branch logic i dalsze execution features. Faza ma utrzymać precyzyjne kontrakty flag bez mieszania zakresu z interruptami, nowymi trybami adresowania czy toolingiem parsera.
ADR-0005 utrwala canonical layout status byte `N V - B D I Z C` oraz bootstrapowy subset `C / Z / N` jako jawny punkt odniesienia dla kolejnych tasków fazy 7.

## Task Issues
- [ ] #60 - define full status register contract and ADR.
- [ ] #61 - add full status register foundation in core.
- [ ] #62 - implement flag-control opcodes.
- [ ] #63 - implement `ADC` and `SBC` immediate semantics.
- [ ] #64 - implement `BIT` and accumulator shift/rotate semantics.
- [ ] #65 - implement remaining flag-driven branches and regression coverage.
- [ ] #66 - align assembler and CLI trace with status and arithmetic slice.
- [ ] #67 - update slice map and chapter docs for phase 7 core completion.
- [ ] #68 - QC gate przez `epic-qc`.

## Acceptance Criteria
- [ ] Full status register contract is explicit and reflected in core/state/stack behavior.
- [ ] Planned status, ALU and branch opcodes are decoded, executed and tested.
- [ ] Flag semantics remain explicit for `C`, `Z`, `N`, `V`, `D`, `I`, `B` within the planned scope.
- [ ] Branch offset semantics remain tested relative to `PC` after operand fetch.
- [ ] Assembler, trace/CLI and docs remain aligned with the delivered semantics.
- [ ] Epic passes `epic-qc`, and any follow-up issues are linked before closure.
- [ ] `issue-sync` confirms synced epic body and workflow docs before epic closure.

## Verification Strategy
- Narrow tests:
  - `dotnet test tests/pseudoCPU.Bootstrap.Tests/pseudoCPU.Bootstrap.Tests.csproj --filter "FullyQualifiedName~Adc|FullyQualifiedName~Sbc|FullyQualifiedName~Bit|FullyQualifiedName~Branch|FullyQualifiedName~Status"`
- Full test project:
  - `dotnet test tests/pseudoCPU.Bootstrap.Tests/pseudoCPU.Bootstrap.Tests.csproj`
- Build:
  - `dotnet build pseudoCPU.sln`
- Format:
  - `dotnet format pseudoCPU.sln --verify-no-changes`

## Documentation Updates
- [ ] `docs/cpu-slice-map.md` po wdrożeniu nowych opcode'ów i status semantics.
- [ ] `docs/pseudoCPU-processor-guide.md` po pełnym status/ALU slice.
- [x] `docs/adr/0005-status-register-contract.md` dokumentuje kontrakt status register dla #60.
- [ ] Ten chapter po przejściu tasków i po QC gate.
- [ ] `docs/current-epic.md`, `docs/current-epic-summary.md` i `docs/epic-chapters.md` dla stanu plan -> qc -> done.

## QC Gate
- QC issue/comment: TBD
- Verdict: TBD
- Follow-up issues:
  - TBD

## Final Notes
- Final status: planned
- Remaining risks: decimal-mode semantics i rola `B` / `I` muszą być precyzyjnie zawężone, aby nie wejść przedwcześnie w interrupt flow.
- Permanent decisions: status register ma być wdrażany jawnie i testowalnie, bez ukrytego rozszerzania contractu na execution model kolejnej fazy.
