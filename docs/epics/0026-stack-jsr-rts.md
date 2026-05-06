# Epic 26: Phase 4: 6502 stack page foundation and JSR/RTS subroutine flow

## Epic Issue
- Epic: #26
- Status: done
- Owner agent: `issue-planner`
- Execution agent: `issue-executor`
- Quality gate agent: `epic-qc`

## Completion Checklist Entry
- Done: [x]
- Chapter: Phase 4: 6502 stack page foundation and JSR/RTS subroutine flow
- Epic issue: #26
- Status: done
- QC verdict: PASS
- Follow-up: none

## Outcome
Repo zyskuje bootstrapowy model stacka zgodny semantycznie z 6502 dla aktualnego zakresu: stack page `$0100-$01FF`, 8-bitowy `SP`, poprawne push/pop oraz obsluge `JSR abs` i `RTS`.

## Domain Scope
- Area: stack page `$0100-$01FF`, rejestr `SP`, helpery push/pop, `JSR abs`, `RTS`, assembler, CLI trace, `docs/cpu-slice-map.md` i `docs/current-epic.md`.
- In scope:
  - jawny `SP` jako 8-bitowy rejestr CPU,
  - projektowy kontrakt `LoadProgram -> SP = $FF`,
  - adres efektywny stacka jako `$0100 + SP`,
  - push/pop zgodne z 6502 dla tego slice,
  - dekoder i wykonanie `JSR abs`,
  - dekoder i wykonanie `RTS`,
  - testy stack semantics, wrap-around i subroutine flow,
  - wsparcie assemblera dla `JSR` / `RTS`,
  - aktualizacja trace/CLI,
  - aktualizacja mapy slice CPU i chaptera.
- Out of scope:
  - IRQ/NMI/RESET vectors,
  - `BRK` vector,
  - `PHA`, `PLA`, `PHP`, `PLP`,
  - cycle counting,
  - pelny status register 6502 jako bajt,
  - etykiety assemblera,
  - `.org`, `.byte`, `.word`,
  - nowe komendy CLI poza minimalnym smoke/test flow.

## Chapter Narrative
Ten chapter wprowadza pierwszy normalny model stacka 6502 do bootstrapowego pseudoCPU. Zamiast traktowac subroutine flow jako specjalny przypadek skoku, epic ustala wspolny kontrakt stack page, inicjalizacji `SP` i kolejnosci push/pop, a potem opiera na nim `JSR abs` oraz `RTS`.

## Task Issues
- [x] #27 - normalny model stacka 6502 w Core.
- [x] #28 - testy `SP`, push/pop i wrap-around.
- [x] #29 - implementacja `JSR abs` w decoderze i CPU.
- [x] #30 - implementacja `RTS` na bazie stacka.
- [x] #31 - testy pelnego przeplywu podprogramow `JSR` / `RTS`.
- [x] #32 - wsparcie assemblera dla `JSR` / `RTS` i ASM end-to-end.
- [x] #33 - aktualizacja trace/CLI dla nowych mnemonikow.
- [x] #34 - aktualizacja `docs/cpu-slice-map.md`, chaptera i snapshotu biezacego epica.
- [x] #35 - QC gate przez `epic-qc`.

## Acceptance Criteria
- [x] `BootstrapCpu` utrzymuje `SP` jako 8-bitowy rejestr i nie wychodzi ze stack page `$0100-$01FF`.
- [x] `LoadProgram` inicjalizuje `SP` na `$FF` i kontrakt jest zapisany w issue/chapterze.
- [x] Push zapisuje pod `$0100 + SP`, a potem dekrementuje `SP`; pop najpierw inkrementuje `SP`, a potem czyta z `$0100 + SP`.
- [x] `JSR abs` zapisuje poprawny adres powrotu i ustawia `PC` na cel wywolania.
- [x] `RTS` odtwarza adres powrotu ze stacka i wznawia wykonanie na instrukcji po `JSR`.
- [x] Istnieja testy na wrap-around stacka i na co najmniej jeden nested/sequential subroutine flow.
- [x] Assembler rozumie `JSR $addr` i `RTS`, a co najmniej jeden test ASM end-to-end pokrywa podprogram.
- [x] CLI trace, jesli pokazuje instrukcje, umie wypisac `JSR` i `RTS` bez driftu wobec assemblera.
- [x] `docs/cpu-slice-map.md` odzwierciedla nowy slice po implementacji.
- [x] Epic przechodzi `epic-qc`, a ewentualne follow-up issues sa jawnie zalinkowane.

## Verification Strategy
- Narrow tests:
  - `dotnet test tests/pseudoCPU.Bootstrap.Tests/pseudoCPU.Bootstrap.Tests.csproj --filter "FullyQualifiedName~Stack|FullyQualifiedName~Jsr|FullyQualifiedName~Rts"`
- Full test project:
  - `dotnet test tests/pseudoCPU.Bootstrap.Tests/pseudoCPU.Bootstrap.Tests.csproj`
- Build:
  - `dotnet build pseudoCPU.sln`
- Format:
  - `dotnet format pseudoCPU.sln --verify-no-changes`
- CLI smoke:
  - `dotnet run --project src/pseudoCPU.Cli -- run-asm --source examples/jsr-rts-subroutine.asm --start 0x0600 --max-steps 100 --trace`

## Documentation Updates
- [x] `docs/cpu-slice-map.md` po dodaniu `JSR abs`, `RTS` i stack foundation.
- [x] Ten chapter po przejsciu taskow i po QC gate.
- [x] `docs/current-epic.md` dla aktywnego stanu i snapshotu weryfikacji.

## QC Gate
- QC issue/comment: https://github.com/prachwal/pseudoCPU/issues/26#issuecomment-4386834276
- Verdict: PASS
- Follow-up issues:
  - none

## Final Notes
- Final status: done
- Remaining risks: none blocking after QC; future epics still need separate work for reset/interrupt flow and additional stack opcodes.
- Permanent decisions: bootstrapowy kontrakt startu `SP` to `$FF` do czasu osobnego epica reset/interrupt; stack semantics maja pozostac wspolnym fundamentem dla przyszlych instrukcji stackowych.
