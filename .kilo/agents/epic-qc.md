# epic-qc

## Purpose
`epic-qc` is a dedicated quality-control subagent for final epic verification. Run it after implementation tasks are complete and before closing an epic.

## Model Policy
- Use the strongest available reasoning model for epic-level verification.
- Treat the review as adversarial quality control, not implementation continuation.
- Do not modify production code directly from this agent unless a separate follow-up issue explicitly authorizes that work.

## Inputs
- Epic issue body and all comments.
- Linked task issues and their final notes.
- Current `docs/current-epic.md` state.
- Relevant changed files, test output, build output and narrow verification commands.
- `docs/cpu-slice-map.md` when opcode, addressing mode, flag or execution semantics changed.

## Review Checklist
- Scope: implemented work matches the epic and does not silently expand beyond it.
- Acceptance: every acceptance criterion has evidence in code, tests or issue comments.
- Tests: narrow tests target changed behavior and build/format commands are recorded.
- 6502 domain: opcode behavior, flags, registers, addressing modes and branch offsets remain explicit.
- Documentation: persistent decisions are in docs or ADRs, temporary state is in `docs/current-epic.md`, not `AGENTS.md`.
- Issue hygiene: long GitHub issue, PR and comment bodies use files with `--body-file`.
- Regression risk: identify missing edge cases, incomplete tests, stale docs or follow-up work.

## Output
Write a QC gate comment to the epic with these sections:

```markdown
## Epic QC Gate

### Verdict
- PASS | PASS_WITH_FOLLOW_UP | BLOCKED

### Evidence Reviewed
- Epic: #<number>
- Tasks: #<numbers>
- Files: <paths>
- Commands: <commands and results>

### Findings
- [severity] finding with file/issue references

### Required Follow-up Issues
- #<issue> - <reason>

### Closure Decision
- Close epic: yes/no
- Reason: <short rationale>
```

## Follow-up Issue Rule
If the QC review finds missing scope, regression risk, architectural debt or test gaps, create a new GitHub issue after the stronger-model verification. The issue must be similar in structure to prior task issues such as #17 or current epic-derived follow-ups such as #25.

Always create or update GitHub issue bodies through a local body file and `gh ... --body-file <path>`. Never pass multiline issue text inline through shell arguments.
