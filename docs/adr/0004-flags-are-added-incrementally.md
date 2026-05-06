# ADR-0004: Status flags are added incrementally

## Status
Accepted

## Context
Current bootstrap work exposes only the flags required by implemented opcode slices.

## Decision
Add processor flags in small phases instead of modeling the full status register up front.

## Consequences
- `Zero` and `Negative` remain the current public contract.
- `Carry` and other flags require explicit phase work.
- Task scope stays narrow and easier to verify.

## Links
- Issue #24
