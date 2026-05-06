# Epic Chapters Index

Ten plik jest lekkim indeksem chapterow. Pelne chaptery trzymamy w `docs/epics/*.md`, zeby agenci nie musieli ladowac calej historii epikow przy kazdym zadaniu.

## Rule

Jeden realny GitHub epic issue z labelem `epic` = jeden chapter file w `docs/epics/`.

Tylko issue z labelem `epic` moze byc opisane jako `Epic <nr>`. Taski, QC taski i follow-upy bez labela `epic` moga byc linkowane jako references, ale nie sa chapterami epica.

## Status Values

| Status | Meaning |
|---|---|
| `planned` | Chapter opisany, ale epic nie jest jeszcze aktywnie realizowany. |
| `active` | Epic jest w trakcie planowania albo implementacji. |
| `qc` | Implementacja jest zakonczona i czeka na bramke `epic-qc`. |
| `done` | Epic ma pozytywny QC gate i chapter jest zamkniety. |
| `blocked` | Epic nie moze isc dalej bez decyzji albo zaleznosci. |
| `superseded` | Chapter zostal zastapiony innym epicem/chapterem. |

## Chapter Completion Checklist

| Done | Chapter | Epic Issue | Status | QC Verdict | Follow-up | File | Notes |
|---|---|---:|---|---|---|---|---|
| [x] | Phase 4: 6502 stack page foundation and JSR/RTS subroutine flow | #26 | done | PASS | none | `docs/epics/0026-stack-jsr-rts.md` | Stack page `$0100-$01FF`, `SP`, `JSR abs`, `RTS`. |
| [x] | Phase 5: 6502 stack opcode slice PHA/PLA/PHP/PLP | #36 | done | PASS_WITH_FOLLOW_UP | #45 | `docs/epics/0036-stack-opcodes.md` | `PHA`, `PLA`, `PHP`, `PLP`, bootstrap status byte contract. |

## QC References

| Reference | Issue | Related Epic | Status | Notes |
|---|---:|---:|---|---|
| QC Reference 25: Phase 3 quality control | #25 | #18 | closed | Review po fazie 3: Carry semantics, trace/assembler drift, CLI summary i regresja testow. |

## Current Chapter Index

| Chapter | Epic Issue | Status | File | Scope |
|---|---:|---|---|---|
| Phase 4: 6502 stack page foundation and JSR/RTS subroutine flow | #26 | done | `docs/epics/0026-stack-jsr-rts.md` | Stack page `$0100-$01FF`, 8-bit `SP`, push/pop ordering, `JSR abs`, `RTS`, assembler/CLI/docs |
| Phase 5: 6502 stack opcode slice PHA/PLA/PHP/PLP | #36 | done | `docs/epics/0036-stack-opcodes.md` | `PHA`, `PLA`, `PHP`, `PLP`, bootstrap status byte contract, assembler/CLI/docs |

## New Chapter File Template

Create a new file under `docs/epics/<epic-number>-<slug>.md`.

```markdown
# Epic <nr>: <chapter title>

## Epic Issue
- Epic: #<number>
- Status: planned | active | blocked | qc | done | superseded
- Owner agent: `issue-planner`
- Execution agent: `issue-executor`
- Quality gate agent: `epic-qc`

## Completion Checklist Entry
- Done: [ ]
- Chapter: <chapter title>
- Epic issue: #<number>
- Status: planned | active | blocked | qc | done | superseded
- QC verdict: PASS | PASS_WITH_FOLLOW_UP | BLOCKED | TBD
- Follow-up: #<number> | none | TBD

## Outcome
<What should be true after this epic is completed.>

## Domain Scope
- Area: <domain/component/module>
- In scope:
  - <item>
- Out of scope:
  - <item>

## Chapter Narrative
<Stable product/technical explanation of this epic.>

## Task Issues
- [ ] #<task> - <task goal>

## Acceptance Criteria
- [ ] <criterion>

## Verification Strategy
- Build:
  - `<command>`
- Test:
  - `<command>`
- Format/lint:
  - `<command>`
- Smoke/e2e:
  - `<command>`

## Documentation Updates
- [ ] <doc path>

## QC Gate
- QC issue/comment: #<number or comment link>
- Verdict: PASS | PASS_WITH_FOLLOW_UP | BLOCKED | TBD
- Follow-up issues:
  - #<issue> - <reason>

## Final Notes
- Final status: planned | active | blocked | qc | done | superseded
- Remaining risks:
- Permanent decisions:
```
