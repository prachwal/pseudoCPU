# Current Epic State

Ten plik przechowuje stan biezacego epica i informacje fazowe, ktorych nie nalezy dopisywac do `AGENTS.md`.

## Active Epic
- GitHub issue: none
- Status: idle
- Owner agent: none
- Quality gate agent: none
- Chapter file: n/a

## Planned Epic Queue
1. #57 - `docs/epics/0057-addressing-bus-indirect.md`
2. #58 - `docs/epics/0058-reset-interrupts-timing.md`
3. #59 - `docs/epics/0059-assembler-loader-tooling.md`

## Last Completed Epic
- GitHub issue: #56
- Chapter file: `docs/epics/0056-status-alu-branches.md`
- Status: done
- QC verdict: PASS
- Follow-up: none
- Follow-up status: none

## Current Scope
- Phase 7 is complete; its status/ALU/branch slice is now in the completed epic record.
- Remaining planned phases cover addressing/memory, reset/interrupt/timing, and assembler/tooling completion.
- Before CPU/opcode/flag/stack/assembler/CLI work, read `docs/6502-domain-rules.md`.

## Active Task Plan
1. `#57` - next planned epic, pending planner routing.
2. `issue-sync` - completed for #56.

## Phase Notes
- Four condensed completion epics are now planned: #56-#59.
- #56 is complete and ready for closure.
- #57-#59 remain queued.
- Implementation tasks #60-#67 are complete.
- #68 is complete; QC verdict is PASS.
- Follow-up issues are none.

## QC Feedback Loop
1. `issue-planner` utrzymuje strukture chapter -> epic -> taski.
2. `issue-executor` realizuje taski i zapisuje wyniki w issue.
3. `epic-qc` weryfikuje epica po implementacji silniejszym modelem.
4. `issue-sync` synchronizuje glowne body epica, relevantny chapter, `docs/epic-chapters.md`, `docs/current-epic.md` i `docs/current-epic-summary.md`.
5. Jezeli `epic-qc` wykryje brakujacy zakres, regresje lub ryzyko, tworzy follow-up issue z body zapisanym w pliku i linkuje je z epicem.
6. Petla wraca do planner/executor do czasu braku blokujacych ustalen QC i spójnego sync.

## Verification Snapshot
- Planned narrow test command: `dotnet test tests/pseudoCPU.Bootstrap.Tests/pseudoCPU.Bootstrap.Tests.csproj --filter "FullyQualifiedName~Adc|FullyQualifiedName~Sbc|FullyQualifiedName~Bit|FullyQualifiedName~Branch|FullyQualifiedName~Status"`
- Planned full test command: `dotnet test tests/pseudoCPU.Bootstrap.Tests/pseudoCPU.Bootstrap.Tests.csproj`
- Planned build command: `dotnet build pseudoCPU.sln`
- Planned format command: `dotnet format pseudoCPU.sln --verify-no-changes`
- Latest QC result: PASS

## QC Gate
- #68 - completed QC gate task for epic #56.

## Follow-up Issues
- none.
- `issue-sync` completed; #56 is ready for closure.
