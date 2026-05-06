# ADR-0006: Phase 8 addressing modes use explicit opcode slices and RAM-only bus foundation

## Status
Accepted

## Context
Phase 8 needs a stable contract before implementation tasks #70-#73 can land. The repo must avoid a blanket "add all addressing modes" interpretation, and it must keep `JMP (indirect)` separate from pointer-based data modes. The memory layer also needs a preparation step for future MMIO without introducing devices yet.

## Decision
- Use explicit opcode slices for the phase 8 task chain instead of a generic "support addressing mode" statement.
- Define the planned subset as:
  - #71: zero page + zero page indexed for `LDA $zz`, `LDX $zz`, `LDY $zz`, `STA $zz`, `STX $zz`, `STY $zz`, `LDA $zz,X`, `LDX $zz,Y`, `LDY $zz,X`, `STA $zz,X`, `STX $zz,Y`, `STY $zz,X`.
  - #72: absolute indexed `,X` / `,Y` for `LDA $hhhh,X`, `LDA $hhhh,Y`, `LDX $hhhh,Y`, `LDY $hhhh,X`, `STA $hhhh,X`, `STA $hhhh,Y`.
  - #73: indirect data modes for `LDA ($zz,X)`, `STA ($zz,X)`, `LDA ($zz),Y`, `STA ($zz),Y`, plus `JMP ($hhhh)` as a separate flow-control contract.
- Treat 16-bit operands as little-endian (`low byte`, then `high byte`).
- Treat zero-page pointer lookup as wrapping inside `$00xx`.
- Treat `JMP (indirect)` as preserving the 6502-style page-wrap at pointer boundaries.
- Treat `(indirect,X)` and `(indirect),Y` as pointer-based data modes with their own zero-page pointer resolution rules; they do not inherit the `JMP (indirect)` pointer-boundary anomaly.
- Keep the memory-bus foundation RAM-only: a read/write preparation layer for future MMIO, with no devices, no timing model, and no side effects beyond RAM access.

## Consequences
- Later tasks can implement exactly the documented slice without widening the scope silently.
- The processor guide, chapter planning and future code work can refer to one stable contract.
- Bus abstraction work can start without implying MMIO or device semantics.

## Links
- Issue #69
- Epic #57
- Chapter `docs/epics/0057-addressing-bus-indirect.md`
