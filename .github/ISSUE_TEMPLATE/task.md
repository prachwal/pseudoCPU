---
name: Task
about: Pojedynczy task implementacyjny lub projektowy utrzymywany przez planner/executor
title: "[TASK] "
labels: task
---

## Goal

## Scope
- 

## Definition of Done
- [ ] Kod jest sformatowany.
- [ ] Dodano lub zaktualizowano testy.
- [ ] Narrow test filter przechodzi.
- [ ] `dotnet build pseudoCPU.sln` przechodzi.
- [ ] `dotnet format pseudoCPU.sln --verify-no-changes` przechodzi.
- [ ] `AGENTS.md` / docs zaktualizowane, jeśli zmienił się kontrakt.
- [ ] Nie rozszerzono zakresu poza issue.
- [ ] Wynik zapisany w komentarzu issue.

## Scope Guard
Do not implement unless explicitly requested:
- labels in assembler,
- `.org`, `.byte`, `.word`,
- new addressing modes,
- cycle counting,
- interrupts,
- stack/subroutines,
- unrelated opcode groups,
- new CLI commands outside this issue.

## Acceptance Criteria
- [ ] 

## Verification
- Planned command:
  - `dotnet test <test-project> --filter "..."`

## Dependencies
- Depends on:
- Blocks:

## Progress Log
- YYYY-MM-DD HH:MM - issue created

## Final Notes
- Changed files:
- Test command:
- Test result:
- Remaining risks:
