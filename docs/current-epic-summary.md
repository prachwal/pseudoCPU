# Current Epic Summary

## State
- Current active epic: none
- Last completed epic: #36
- Last QC verdict: PASS_WITH_FOLLOW_UP
- Follow-up from last epic: #45 completed

## Last Completed Epic
- Epic: #36 - Phase 5: 6502 stack opcode slice PHA/PLA/PHP/PLP
- Chapter file: `docs/epics/0036-stack-opcodes.md`
- Tasks: #37-#44
- Follow-up: #45 completed
- Status: done

## Key Domain Decisions
- Stack page: `$0100-$01FF`
- `SP`: 8-bit register
- Bootstrap start `SP`: `$FF`
- Bootstrap status byte:
  - Carry = bit 0
  - Zero = bit 1
  - Negative = bit 7
  - bits 2-6 reserved / ignored

## Next Action
- For a new small bug: use small-task mode.
- For a new multi-task feature, domain change, contract change or architecture change: use epic mode.
- Before any CPU/opcode/flag/stack/assembler/CLI work: read `docs/6502-domain-rules.md`.

## Context Files
- Workflow: `docs/agentic-epic-flow.md`
- Token policy: `docs/token-aware-agent-flow.md`
- Domain rules: `docs/6502-domain-rules.md`
- Current state: `docs/current-epic.md`
- Epic index: `docs/epic-chapters.md`
- Last chapter: `docs/epics/0036-stack-opcodes.md`
- CPU slice map: `docs/cpu-slice-map.md`
