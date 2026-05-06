# ADR-0001: Bootstrap assembler remains label-less until label phase

## Status
Accepted

## Context
Bootstrap assembler is intentionally minimal and already operates with manual relative offsets.

## Decision
Keep labels out of the bootstrap assembler until a dedicated label phase is planned and approved.

## Consequences
- Branch offsets stay explicit in current tasks.
- Parser complexity remains low.
- Label support is not added incidentally during opcode work.

## Links
- Issue #24
