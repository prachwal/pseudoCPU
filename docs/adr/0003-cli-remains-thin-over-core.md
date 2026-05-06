# ADR-0003: CLI remains a thin host over pseudoCPU.Core

## Status
Accepted

## Context
The CLI is used to load and trace bootstrap programs, while execution semantics live in the core project.

## Decision
Keep CLI behavior minimal and delegate CPU semantics to `pseudoCPU.Core`.

## Consequences
- Core logic stays reusable and testable.
- CLI changes should not duplicate CPU rules.
- User-facing commands stay limited to the current scope.

## Links
- Issue #24
