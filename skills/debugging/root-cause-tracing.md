# root-cause-tracing

## Purpose
Find and fix the *actual* cause of an issue with evidence.

## Triggers
- Bug reports, flaky tests, production incidents

## Workflow
1. Frame the hypothesis
2. Gather signals (logs/metrics/traces)
3. Create a minimal reproduction
4. Validate the fix with targeted tests
5. Add regression guard

## Checklist
- [ ] Hypothesis documented
- [ ] Repro steps committed
- [ ] Post-fix verification
