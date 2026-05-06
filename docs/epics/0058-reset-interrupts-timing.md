# Epic 58: Phase 9: reset, interrupts and timing model completion

## Epic Issue
- Epic: #58
- Status: planned
- Owner agent: `issue-planner`
- Execution agent: `issue-executor`
- Quality gate agent: `epic-qc`

## Completion Checklist Entry
- Done: [ ]
- Chapter: Phase 9: reset, interrupts and timing model completion
- Epic issue: #58
- Status: planned
- QC verdict: TBD
- Follow-up: TBD

## Outcome
Repo ma przejść od prostego bootstrap runnera do jawnego execution modelu z resetem, przerwaniami i evidence timingowym, bez którego trudno mówić o dalszej zgodności CPU beyond toy programs.

## Domain Scope
- Area: reset vectors, interrupt entry/return, timing foundation, CLI/trace visibility, docs.
- In scope:
  - reset/startup vector contract,
  - `BRK` interrupt entry,
  - `IRQ`, `NMI`, `RTI`,
  - cycle counter foundation i timing assertions dla uzgodnionego subsetu,
  - trace/CLI visibility,
  - chapter, processor guide i workflow docs.
- Out of scope:
  - MMIO devices,
  - labels i dyrektywy assemblera,
  - debugger/disassembler,
  - unrelated performance tuning.

## Chapter Narrative
Ten chapter domyka execution model procesora. Do tej pory `BRK` pełnił bootstrapową rolę zatrzymania, a CPU nie miał pełnego życia po resecie i przez przerwania. Faza ma wprowadzić precyzyjny, testowalny kontrakt reset/interrupt/timing bez próby jednoczesnego rozwiązania tooling completion i devices.

## Task Issues
- [ ] #77 - define reset, interrupt and timing contract with ADR.
- [ ] #78 - implement reset vector boot and startup flow.
- [ ] #79 - implement BRK interrupt entry semantics.
- [ ] #80 - implement IRQ, NMI and RTI semantics.
- [ ] #81 - add cycle-count foundation and timing assertions.
- [ ] #82 - extend CLI trace and tests for interrupt and timing visibility.
- [ ] #83 - update chapter and processor docs for phase 9.
- [ ] #84 - QC gate przez `epic-qc`.

## Acceptance Criteria
- [ ] Reset/interrupt flow is explicit, tested and reflected in stack/status semantics.
- [ ] `BRK`, `IRQ`, `NMI`, `RTI` and planned timing foundation are implemented without silent scope drift.
- [ ] Trace/CLI and docs remain aligned with the delivered execution model.
- [ ] Timing evidence is explicit for the agreed subset and not guessed from semantic tests alone.
- [ ] Epic passes `epic-qc`, and any follow-up issues are linked before closure.
- [ ] `issue-sync` confirms synced epic body and workflow docs before epic closure.

## Verification Strategy
- Narrow tests:
  - `dotnet test tests/pseudoCPU.Bootstrap.Tests/pseudoCPU.Bootstrap.Tests.csproj --filter "FullyQualifiedName~Reset|FullyQualifiedName~Interrupt|FullyQualifiedName~Brk|FullyQualifiedName~Rti|FullyQualifiedName~Cycle"`
- Full test project:
  - `dotnet test tests/pseudoCPU.Bootstrap.Tests/pseudoCPU.Bootstrap.Tests.csproj`
- Build:
  - `dotnet build pseudoCPU.sln`
- Format:
  - `dotnet format pseudoCPU.sln --verify-no-changes`

## Documentation Updates
- [ ] `docs/cpu-slice-map.md` dla interrupt/timing related state, jeśli mapa zostanie rozszerzona o execution model notes.
- [ ] `docs/pseudoCPU-processor-guide.md` po reset/interrupt/timing foundation.
- [ ] Ten chapter po przejściu tasków i po QC gate.
- [ ] `docs/current-epic.md`, `docs/current-epic-summary.md` i `docs/epic-chapters.md` dla stanu plan -> qc -> done.

## QC Gate
- QC issue/comment: TBD
- Verdict: TBD
- Follow-up issues:
  - TBD

## Final Notes
- Final status: planned
- Remaining risks: łatwo pomylić kolejność push/pop statusu i adresu powrotu; kontrakt musi pozostać jawny na poziomie tasków i testów.
- Permanent decisions: timing ma być wdrażany jako jawne evidence, bez wcześniejszego zgadywania pełnej cycle accuracy całego CPU.
