# GitHub issue body files

## Rule
All multiline GitHub issue, PR and comment bodies must be written to a file first and passed to GitHub CLI with `--body-file`.

## Applies To
- `gh issue create`
- `gh issue edit`
- `gh issue comment`
- `gh pr create`

## Required Pattern
```bash
mkdir -p .tmp/issue-bodies
cat > .tmp/issue-bodies/<issue-or-task-slug>.md <<'EOF'
<markdown body>
EOF

gh issue create --title "<title>" --label "task" --body-file .tmp/issue-bodies/<issue-or-task-slug>.md
```

For comments:

```bash
cat > .tmp/issue-bodies/<issue-number>-qc-gate.md <<'EOF'
<markdown comment>
EOF

gh issue comment <issue-number> --body-file .tmp/issue-bodies/<issue-number>-qc-gate.md
```

## Forbidden Pattern
Do not use multiline inline shell strings for GitHub bodies:

```bash
gh issue create --body "...multiline markdown..."
gh issue comment 25 --body "...multiline markdown..."
gh pr create --body "...multiline markdown..."
```

## Rationale
Backticks, quotes, shell interpolation and new lines frequently corrupt issue text. File-backed bodies are the default workflow, not a fallback.
