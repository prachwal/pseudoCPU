# Current Epic State

Ten plik przechowuje stan biezacego epica i informacje fazowe, ktorych nie nalezy dopisywac do `AGENTS.md`.

## Active Epic
- GitHub issue: none
- Status: none
- Owner agent: `issue-planner`
- Quality gate agent: `epic-qc`

## Last Completed Epic
- GitHub issue: #36
- Chapter file: `docs/epics/0036-stack-opcodes.md`
- Status: done
- QC verdict: PASS_WITH_FOLLOW_UP
- Follow-up: #45
- Follow-up status: completed

## Current Scope
- No active epic scope.
- Use small-task mode for small local changes.
- Use epic mode for multi-task, domain, contract, architecture or documentation changes.
- Before CPU/opcode/flag/stack/assembler/CLI work, read `docs/6502-domain-rules.md`.

## Last Completed Scope Snapshot
- Source of truth: epic #36 and child tasks #37-#44.
- Zakres zakonczony: stack opcode slice `PHA`, `PLA`, `PHP`, `PLP` na fundamencie stack page `$0100-$01FF`, 8-bitowego `SP` i helperow push/pop z #26.
- Bootstrapowy status byte: `Carry` = bit 0, `Zero` = bit 1, `Negative` = bit 7, a bity 2-6 sa zarezerwowane / ignorowane; `PHP` zapisuje tylko ten snapshot, a `PLP` odtwarza wylacznie wspierane flagi.
- Out of scope pozostaje: IRQ/NMI/RESET vectors, `BRK` vector, `RTI`, pelny status register 6502 jako docelowy model procesora, decimal/interrupt/overflow/break live semantics, cycle counting, etykiety assemblera oraz `.org` / `.byte` / `.word`.

## Phase Notes
- Epic #36 i taski #37-#44 sa zakonczone.
- QC task #44 zakonczyl epic werdyktem `PASS_WITH_FOLLOW_UP` z follow-upem #45.
- Follow-up #45 zostal zamkniety jako completed.
- Brak aktywnego epica po #36.

## QC Feedback Loop
1. `issue-planner` utrzymuje strukture chapter -> epic -> taski.
2. `issue-executor` realizuje taski i zapisuje wyniki w issue.
3. `epic-qc` weryfikuje epica po implementacji silniejszym modelem.
4. `issue-sync` synchronizuje glowne body epica, relevantny chapter, `docs/epic-chapters.md`, `docs/current-epic.md` i `docs/current-epic-summary.md`.
5. Jezeli `epic-qc` wykryje brakujacy zakres, regresje lub ryzyko, tworzy follow-up issue z body zapisanym w pliku i linkuje je z epicem.
6. Petla wraca do planner/executor do czasu braku blokujacych ustalen QC i spójnego sync.

## Verification Snapshot
- Last narrow test command: `dotnet test tests/pseudoCPU.Bootstrap.Tests/pseudoCPU.Bootstrap.Tests.csproj --filter "FullyQualifiedName~Pha|FullyQualifiedName~Pla|FullyQualifiedName~Php|FullyQualifiedName~Plp|FullyQualifiedName~StackOpcode"`
- Last full test command: `dotnet test tests/pseudoCPU.Bootstrap.Tests/pseudoCPU.Bootstrap.Tests.csproj`
- Last build command: `dotnet build pseudoCPU.sln`
- Last format command: `dotnet format pseudoCPU.sln --verify-no-changes`
- Last CLI smoke command: `dotnet run --project src/pseudoCPU.Cli -- run-asm --source examples/stack-opcodes.asm --start 0x0600 --max-steps 100 --trace`
- Latest QC result: PASS_WITH_FOLLOW_UP (#45)

## QC Gate
- #44 - completed QC gate task for epic #36.

## Follow-up Issues
- #45 - completed: sync `docs/pseudoCPU-processor-guide.md` with the implemented stack slice, `SP` model and current supported opcodes.
- Epic QC verdict recorded in comment: https://github.com/prachwal/pseudoCPU/issues/36#issuecomment-4387471999
