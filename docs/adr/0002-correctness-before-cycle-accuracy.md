# ADR-0002: CPU prioritizes functional correctness before cycle accuracy

## Status
Accepted

## Context
Current work focuses on a small, testable bootstrap CPU slice rather than full 6502 timing.

## Decision
Implement opcode behavior and observable state first; defer cycle accuracy to a later phase.

## Consequences
- Tests can validate semantics earlier.
- Cycle counting is not a dependency for bootstrap progress.
- Timing work must be planned separately.

## Links
- Issue #24
