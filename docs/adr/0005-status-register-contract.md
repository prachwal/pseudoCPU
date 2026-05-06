# ADR-0005: Canonical status register contract is explicit and phased

## Status
Accepted

## Context
Phase 7 needs one shared status-register contract before flag-control, arithmetic and branch tasks can expand the observable CPU state. The current bootstrap implementation only guarantees `Carry`, `Zero`, and `Negative`, while the remaining bits stay reserved until later tasks define them.

## Decision
- Treat the canonical status byte as `N V - B D I Z C`.
- Map named bits as `C=0`, `Z=1`, `I=2`, `D=3`, `B=4`, reserved bit `5`, `V=6`, `N=7`.
- Keep the runtime bootstrap contract limited to `C`, `Z`, and `N` until the phase 7 implementation tasks land.
- Use this canonical byte as the future stack snapshot contract for `PHP` / `PLP`; until then, reserved bits remain ignored by bootstrap behavior.
- Phase 7 consumes the contract in order: `#61` stores the register shape, `#62` adds flag-control opcodes, `#63` expands arithmetic flag semantics, `#64` adds `BIT` and shifts/rotates, and `#65` finishes flag-driven branches.

## Consequences
- The bit layout is fixed before implementation starts.
- Later tasks can share one status model instead of re-deriving bit meanings.
- Bootstrap behavior stays backward compatible until the implementation tasks are delivered.
- Bits not yet modeled stay explicitly out of scope instead of being inferred ad hoc.

## Links
- Issue #60
- Epic #56
- Chapter `docs/epics/0056-status-alu-branches.md`
