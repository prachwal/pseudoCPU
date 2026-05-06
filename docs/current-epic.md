# Current Epic State

Ten plik przechowuje stan biezacego epica i informacje fazowe, ktorych nie nalezy dopisywac do `AGENTS.md`.

## Active Epic
- GitHub issue: #26
- Status: done
- Owner agent: `issue-planner`
- Quality gate agent: `epic-qc`

## Current Scope
- Source of truth for this chapter: epic #26 and child tasks #27-#35.
- Zakres aktywny: stack page `$0100-$01FF`, 8-bitowy `SP`, push/pop zgodne z 6502, `JSR abs`, `RTS`, assembler `JSR`/`RTS`, trace/CLI dla nowych mnemonikow oraz dokumentacja slice.
- Projektowy kontrakt planistyczny dla tego epica: `LoadProgram` inicjalizuje `SP` do `$FF` do czasu osobnego epica reset/interrupt.
- Out of scope pozostaje: IRQ/NMI/RESET vectors, `BRK` vector, `PHA`/`PLA`/`PHP`/`PLP`, cycle counting, pelny status register, etykiety assemblera oraz `.org` / `.byte` / `.word`.

## Phase Notes
- Planner utworzyl epic #26 i taski #27-#35 jako kolejny chapter po stabilizacji #25.
- Zalecana kolejnosc wykonania: #27 -> #28 -> #29 -> #30 -> #31 -> #32 -> #33 -> #34 -> #35.
- Task #35 jest obowiazkowa bramka `epic-qc`; epic nie moze zostac zamkniety bez komentarza `Epic QC Gate`.
- Task #34 ma zamknac aktualizacje `docs/cpu-slice-map.md`, chaptera oraz snapshotu tego pliku po implementacji.
- Implementacja taskow #27-#34 i QC gate #35 sa zakonczone; epic ma werdykt PASS i moze zostac zamkniety.

## QC Feedback Loop
1. `issue-planner` utrzymuje strukture epic -> taski.
2. `issue-executor` realizuje taski i zapisuje wyniki w issue.
3. `epic-qc` weryfikuje epica po implementacji silniejszym modelem.
4. Jezeli `epic-qc` wykryje brakujacy zakres, regresje lub ryzyko, tworzy follow-up issue z body zapisanym w pliku i linkuje je z epicem.
5. Petla wraca do planner/executor do czasu braku blokujacych ustalen QC.

## Verification Snapshot
- Narrow test command: `dotnet test tests/pseudoCPU.Bootstrap.Tests/pseudoCPU.Bootstrap.Tests.csproj --filter "FullyQualifiedName~Stack|FullyQualifiedName~Jsr|FullyQualifiedName~Rts"`
- Build command: `dotnet build pseudoCPU.sln`
- Format command: `dotnet format pseudoCPU.sln --verify-no-changes`
- CLI smoke command: `dotnet run --project src/pseudoCPU.Cli -- run-asm --source examples/jsr-rts-subroutine.asm --start 0x0600 --max-steps 100 --trace`
- Latest QC result: PASS

## Follow-up Issues
- #17 - reference pattern for prior follow-up/task issue.
- #25 - prior quality-control reference before phase 4.
- #35 - planned QC gate task for the active epic.
- none for epic #26 (QC PASS, 2026-05-06).
