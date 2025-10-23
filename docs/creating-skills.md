# Creating Skills (Markdown-first)

A **skill** is a markdown file with:
- **Purpose** (what problem it solves)
- **When to activate** (triggers/contexts)
- **Workflow** (steps)
- **Checklists** (verification-before-completion)
- **Artifacts** (what gets produced)
- **References**

## Template

```md
# <skill-name>

## Purpose
<brief>

## Triggers
- when user asks for...
- when task type == ...

## Workflow
1. ...
2. ...
3. ...

## Checklist
- [ ] Inputs validated
- [ ] Edge cases addressed
- [ ] Tests updated

## Artifacts
- plan.md / patchset.md / pr-notes.md

## Examples
<snippets or prompts>
```
