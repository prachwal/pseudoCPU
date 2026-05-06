# Epic 59: Phase 10: assembler, loader and developer tooling completion

## Epic Issue
- Epic: #59
- Status: planned
- Owner agent: `issue-planner`
- Execution agent: `issue-executor`
- Quality gate agent: `epic-qc`

## Completion Checklist Entry
- Done: [ ]
- Chapter: Phase 10: assembler, loader and developer tooling completion
- Epic issue: #59
- Status: planned
- QC verdict: TBD
- Follow-up: TBD

## Outcome
Repo ma przejść od bootstrapowego, ręcznie sterowanego ASM flow do wygodniejszego środowiska pracy z labels, dyrektywami, formalniejszym loaderem i minimalnymi narzędziami inspekcyjnymi dla programów.

## Domain Scope
- Area: assembler parser/generator, loader/program format, CLI inspection/debugging, docs.
- In scope:
  - kontrakt labels, directives i tooling,
  - labels oraz branch target resolution,
  - `.org`, `.byte`, `.word`,
  - structured loader/program format i integracja CLI,
  - disassembler lub równoważny inspection flow,
  - minimalny debugger/interactive inspection scope,
  - chapter, processor guide i workflow docs.
- Out of scope:
  - nowe CPU opcodes niezwiązane z parserem/tooling,
  - pełne IDE integration,
  - advanced symbolic debugging beyond agreed minimal scope.

## Chapter Narrative
Ten chapter domyka developer experience wokół CPU. Aktualny assembler i CLI wystarczają do małych bootstrapowych programów, ale nie do wygodnej pracy nad większym codebase’em ASM. Faza ma wprowadzić minimalny, trwały tooling completion bez przeskoku w pełne środowisko IDE czy rozbudowany debugger symboliczny.

## Task Issues
- [ ] #85 - define labels, directives, loader and tooling contract.
- [ ] #86 - implement labels and branch target resolution.
- [ ] #87 - implement `.org`, `.byte` and `.word` directives.
- [ ] #88 - add structured program loader and CLI integration.
- [ ] #89 - add disassembler and inspection flow.
- [ ] #90 - add minimal debugger commands and tooling regression coverage.
- [ ] #91 - update chapter and processor docs for phase 10.
- [ ] #92 - QC gate przez `epic-qc`.

## Acceptance Criteria
- [ ] Assembler supports the planned labels/directives contract with precise byte output.
- [ ] Loader and CLI inspection workflows are documented and covered by tests.
- [ ] Minimal debugging/inspection flow is available within the agreed scope.
- [ ] Tooling improvements do not regress existing ASM/CLI flows.
- [ ] Epic passes `epic-qc`, and any follow-up issues are linked before closure.
- [ ] `issue-sync` confirms synced epic body and workflow docs before epic closure.

## Verification Strategy
- Narrow tests:
  - `dotnet test tests/pseudoCPU.Bootstrap.Tests/pseudoCPU.Bootstrap.Tests.csproj --filter "FullyQualifiedName~Assembler|FullyQualifiedName~Directive|FullyQualifiedName~Label|FullyQualifiedName~Cli"`
- Full test project:
  - `dotnet test tests/pseudoCPU.Bootstrap.Tests/pseudoCPU.Bootstrap.Tests.csproj`
- Build:
  - `dotnet build pseudoCPU.sln`
- Format:
  - `dotnet format pseudoCPU.sln --verify-no-changes`

## Documentation Updates
- [ ] `docs/pseudoCPU-processor-guide.md` po assembler/tooling completion.
- [ ] Ten chapter po przejściu tasków i po QC gate.
- [ ] `docs/current-epic.md`, `docs/current-epic-summary.md` i `docs/epic-chapters.md` dla stanu plan -> qc -> done.

## QC Gate
- QC issue/comment: TBD
- Verdict: TBD
- Follow-up issues:
  - TBD

## Final Notes
- Final status: planned
- Remaining risks: łatwo przesadzić scope debuggera i inspection flow; trzeba pilnować minimalnego, trwałego kontraktu zamiast pełnego IDE/debuggera.
- Permanent decisions: labels i dyrektywy trafiają do dedykowanej fazy zgodnie z ADR-0001, a tooling scope pozostaje celowo ograniczony do potrzeb repo.
