# Current Epic State

Ten plik przechowuje stan biezacego epica i informacje fazowe, ktorych nie nalezy dopisywac do `AGENTS.md`.

## Active Epic
- GitHub issue: #36
- Status: done
- Owner agent: `issue-planner`
- Quality gate agent: `epic-qc`

## Current Scope
- Source of truth for this chapter: epic #36 and child tasks #37-#44.
- Zakres aktywny: stack opcode slice `PHA`, `PLA`, `PHP`, `PLP` na fundamencie stack page `$0100-$01FF`, 8-bitowego `SP` i helperow push/pop z #26.
- Najwazniejsza decyzja kontraktowa: bootstrapowy status byte dla `PHP` / `PLP` musi zostac jawnie opisany w #37 przed implementacja #39.
- Out of scope pozostaje: IRQ/NMI/RESET vectors, `BRK` vector, `RTI`, pelny status register 6502 jako docelowy model procesora, decimal/interrupt/overflow/break live semantics, cycle counting, etykiety assemblera oraz `.org` / `.byte` / `.word`.

## Phase Notes
- Planner utworzyl epic #36 i taski #37-#44 jako kolejny chapter po #26.
- Zalecana kolejnosc wykonania: #37 -> #38 -> #39 -> #40 -> #41 -> #42 -> #43 -> #44.
- Task #37 blokuje `PHP` / `PLP`, bo definiuje status byte contract.
- Task #44 jest obowiazkowa bramka `epic-qc`; epic nie moze zostac zamkniety bez komentarza `Epic QC Gate`.
- Task #43 ma zamknac aktualizacje `docs/cpu-slice-map.md`, chaptera oraz snapshotu tego pliku po implementacji.
- Task #37 zapisuje bootstrapowy kontrakt status byte: `Carry` = bit 0, `Zero` = bit 1, `Negative` = bit 7, a bity 2-6 sa zarezerwowane / ignorowane; `PHP` zapisuje tylko ten snapshot, a `PLP` odtwarza wylacznie wspierane flagi.
- Implementacja taskow #37-#43 jest zakonczona; QC task #44 zakonczyl epic werdyktem `PASS_WITH_FOLLOW_UP` z follow-upem #45.

## QC Feedback Loop
1. `issue-planner` utrzymuje strukture chapter -> epic -> taski.
2. `issue-executor` realizuje taski i zapisuje wyniki w issue.
3. `epic-qc` weryfikuje epica po implementacji silniejszym modelem.
4. Jezeli `epic-qc` wykryje brakujacy zakres, regresje lub ryzyko, tworzy follow-up issue z body zapisanym w pliku i linkuje je z epicem.
5. Petla wraca do planner/executor do czasu braku blokujacych ustalen QC.

## Verification Snapshot
- Narrow test command: `dotnet test tests/pseudoCPU.Bootstrap.Tests/pseudoCPU.Bootstrap.Tests.csproj --filter "FullyQualifiedName~Pha|FullyQualifiedName~Pla|FullyQualifiedName~Php|FullyQualifiedName~Plp|FullyQualifiedName~StackOpcode"`
- Full test command: `dotnet test tests/pseudoCPU.Bootstrap.Tests/pseudoCPU.Bootstrap.Tests.csproj`
- Build command: `dotnet build pseudoCPU.sln`
- Format command: `dotnet format pseudoCPU.sln --verify-no-changes`
- CLI smoke command: `dotnet run --project src/pseudoCPU.Cli -- run-asm --source examples/stack-opcodes.asm --start 0x0600 --max-steps 100 --trace`
- Latest QC result: PASS_WITH_FOLLOW_UP (#45)

## QC Gate
- #44 - completed QC gate task for active epic #36.

## Follow-up Issues
- #45 - sync `docs/pseudoCPU-processor-guide.md` with the implemented stack slice, `SP` model and current supported opcodes.
- Epic QC verdict recorded in comment: https://github.com/prachwal/pseudoCPU/issues/36#issuecomment-4387471999
