# Token-Aware Agent Flow

Ten dokument opisuje, jak uzywac workflow agentowego bez niepotrzebnego rozrostu kontekstu i tokenow.

## Goal

Agenci maja czytac tylko te pliki, ktore sa potrzebne do aktualnego trybu pracy. Pelny workflow epicowy jest kosztowny tokenowo i nie powinien byc uruchamiany dla malych zmian.

## Work Modes

### Small-task mode

Uzywaj dla zmian, ktore:

- dotycza jednego malego buga,
- dotycza jednego pliku albo waskiego zestawu plikow,
- nie zmieniaja publicznego kontraktu,
- nie wymagaja osobnego chaptera,
- nie wymagaja `epic-qc`,
- nie zmieniaja architektury ani domeny.

Small-task mode wymaga:

- jednego task issue,
- waskiego scope,
- waskiej komendy testowej,
- komentarza finalnego,
- bez chaptera,
- bez `docs/current-epic.md`, chyba ze task nalezy do aktywnego epica.

### Epic mode

Uzywaj dla zmian, ktore:

- maja wiele taskow,
- zmieniaja publiczny kontrakt,
- dotykaja architektury,
- dotykaja domeny CPU/opcode/assembler/CLI,
- wymagaja dokumentacji trwalej,
- wymagaja niezaleznego QC.

Epic mode wymaga:

- epic issue,
- task issues,
- chapter file w `docs/epics/`,
- wpisu w `docs/epic-chapters.md`,
- `docs/current-epic.md`,
- `docs/current-epic-summary.md`,
- `epic-qc`,
- `issue-sync` przed zamknieciem.

## Context Loading Rules

### Always read

- `AGENTS.md`
- wlasny agent file
- issue, nad ktorym pracujesz

### Read only for epic planning / QC / sync

- `docs/agentic-epic-flow.md`
- `docs/epic-chapters.md`
- relevant `docs/epics/<epic>.md`
- `docs/current-epic.md`
- `docs/current-epic-summary.md`

### Read only for CPU-domain work

- `docs/6502-domain-rules.md`
- `docs/cpu-slice-map.md`
- `docs/pseudoCPU-processor-guide.md`

### Avoid reading by default

- all historical epic chapter files,
- all old issue comments,
- full repo tree,
- full test suite files unrelated to task,
- docs unrelated to current scope.

## File Structure

Use this token-aware structure:

```text
AGENTS.md

docs/
  agentic-epic-flow.md
  token-aware-agent-flow.md
  6502-domain-rules.md
  current-epic.md
  current-epic-summary.md
  epic-chapters.md
  epics/
    0026-stack-jsr-rts.md
    0036-stack-opcodes.md
  cpu-slice-map.md
  pseudoCPU-processor-guide.md
  adr/
```

## Epic Chapter Rule

`docs/epic-chapters.md` is an index only.

Full chapter content belongs in:

```text
docs/epics/<epic-number>-<slug>.md
```

This prevents agents from loading every historical chapter when they only need the current one.

## Current Epic Summary Rule

`docs/current-epic-summary.md` must stay short. Target: 30-80 lines.

It should include:

- active or last epic,
- task list,
- status,
- QC state,
- follow-up issues,
- next action,
- required files for context.

Agents should read `current-epic-summary.md` before `current-epic.md` when they only need routing context.

## Output Token Rules

Agents should not repeat full issue bodies or full chapter content in final responses.

Final responses should include only:

- issue numbers,
- changed files,
- commands run,
- pass/fail status,
- next action,
- risks/blockers.

## QC Token Rules

`epic-qc` may load broader context, but should still prefer:

1. epic issue,
2. task final comments,
3. changed file list,
4. relevant docs,
5. targeted code files,
6. test output.

It should not read unrelated old epics unless a regression, dependency or reference requires it.

## Issue Sync Token Rules

`issue-sync` should be mechanical and narrow. It should read:

- epic issue body,
- task states,
- QC comment,
- current chapter file,
- `docs/current-epic.md`,
- `docs/current-epic-summary.md`,
- `docs/epic-chapters.md` index.

It should not re-review code unless sync evidence is missing.
