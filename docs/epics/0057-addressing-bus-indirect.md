# Epic 57: Phase 8: addressing modes, memory bus and indirect flow completion

## Epic Issue
- Epic: #57
- Status: planned
- Owner agent: `issue-planner`
- Execution agent: `issue-executor`
- Quality gate agent: `epic-qc`

## Completion Checklist Entry
- Done: [ ]
- Chapter: Phase 8: addressing modes, memory bus and indirect flow completion
- Epic issue: #57
- Status: planned
- QC verdict: TBD
- Follow-up: TBD

## Outcome
Repo ma wyjść poza bootstrapowe immediate/implied/absolute/relative i uzyskać spójny, jawnie opisany zestaw głównych trybów adresowania, oparty o foundation memory-bus gotowy pod przyszłe MMIO.

## Domain Scope
- Area: memory bus foundation, zero page, indexed addressing, indirect flow/data addressing, assembler/docs.
- In scope:
  - kontrakt bus/addressing,
  - zero page i zero page indexed dla uzgodnionego subsetu,
  - absolute indexed `,X` / `,Y` dla uzgodnionego subsetu,
  - `JMP (indirect)`, `(indirect,X)`, `(indirect),Y` dla uzgodnionego subsetu,
  - assembler expected bytes i ASM coverage,
  - chapter, processor guide i workflow docs.
- Out of scope:
  - real devices / pełne MMIO behavior,
  - interrupts i reset flow,
  - labels i dyrektywy assemblera,
  - debugger/disassembler.

## Chapter Narrative
Ten chapter domyka kluczowy dług bootstrapowego procesora: obecnie wiele instrukcji działa tylko w najprostszych forms, co ogranicza realne programy ASM. Faza ma wprowadzić dokładne resolution adresów i foundation memory-bus bez próby jednoczesnego rozwiązania devices, interruptów i parserowego tooling completion.

## Task Issues
- [ ] #69 - define addressing and memory-bus contract with ADR.
- [ ] #70 - add memory-bus foundation and zero-page helpers.
- [ ] #71 - implement zero-page and zero-page indexed data access slice.
- [ ] #72 - implement absolute indexed X/Y data access slice.
- [ ] #73 - implement indirect flow and pointer-based addressing slice.
- [ ] #74 - extend assembler and examples for addressing-mode coverage.
- [ ] #75 - update chapter and processor docs for phase 8.
- [ ] #76 - QC gate przez `epic-qc`.

## Acceptance Criteria
- [ ] Planned addressing modes are decoded, executed and tested with explicit byte-order and page-wrap semantics.
- [ ] Memory access goes through a documented foundation suitable for later MMIO expansion.
- [ ] Assembler stays aligned with the delivered addressing subset.
- [ ] Chapter, processor guide and workflow docs reflect the exact supported forms.
- [ ] Epic passes `epic-qc`, and any follow-up issues are linked before closure.
- [ ] `issue-sync` confirms synced epic body and workflow docs before epic closure.

## Verification Strategy
- Narrow tests:
  - `dotnet test tests/pseudoCPU.Bootstrap.Tests/pseudoCPU.Bootstrap.Tests.csproj --filter "FullyQualifiedName~ZeroPage|FullyQualifiedName~Indexed|FullyQualifiedName~Indirect|FullyQualifiedName~Addressing"`
- Full test project:
  - `dotnet test tests/pseudoCPU.Bootstrap.Tests/pseudoCPU.Bootstrap.Tests.csproj`
- Build:
  - `dotnet build pseudoCPU.sln`
- Format:
  - `dotnet format pseudoCPU.sln --verify-no-changes`

## Documentation Updates
- [ ] `docs/cpu-slice-map.md` po dodaniu nowych forms adresowania.
- [ ] `docs/pseudoCPU-processor-guide.md` po memory/addressing foundation.
- [ ] Ten chapter po przejściu tasków i po QC gate.
- [ ] `docs/current-epic.md`, `docs/current-epic-summary.md` i `docs/epic-chapters.md` dla stanu plan -> qc -> done.

## QC Gate
- QC issue/comment: TBD
- Verdict: TBD
- Follow-up issues:
  - TBD

## Final Notes
- Final status: planned
- Remaining risks: indirect i indexed modes wymagają bardzo precyzyjnych kontraktów page-wrap/byte-order, inaczej łatwo o off-by-one drift.
- Permanent decisions: memory-bus foundation ma przygotować repo pod MMIO, ale bez implementowania real devices w tej fazie.
