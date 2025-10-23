# tdd-red-green-refactor

## Purpose
Drive implementation via failing tests, make them pass, then refactor.

## Triggers
- implementing a new feature
- reproducing a bug with a test first

## Workflow
1. **RED**: write a failing test
2. **GREEN**: implement minimal code to pass
3. **REFACTOR**: improve design without breaking tests

## Checklist
- [ ] Tests describe behavior, not internals
- [ ] One reason to fail at a time
- [ ] Refactor with safety nets (more tests)

## Artifacts
- `tests/<Feature>Tests.cs`
- `src/<Feature>.cs`
