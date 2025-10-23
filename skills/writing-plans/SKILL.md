---
name: writing-plans
description: Use when design is complete and you need detailed implementation tasks for engineers with zero codebase context - creates comprehensive implementation plans with exact file paths, complete code examples, and verification steps assuming engineer has minimal domain knowledge
---

# Writing Plans

## Overview

Write comprehensive implementation plans assuming the engineer has zero context for our codebase and questionable taste. Document everything they need to know: which files to touch for each task, code, testing, docs they might need to check, how to test it. Give them the whole plan as bite-sized tasks. DRY. YAGNI. TDD. Frequent commits.

Assume they are a skilled developer, but know almost nothing about our toolset or problem domain. Assume they don't know good test design very well.

**Announce at start:** "I'm using the writing-plans skill to create the implementation plan."

**Context:** This should be run in a dedicated worktree (created by brainstorming skill).

**Save plans to:** `docs/plans/YYYY-MM-DD-<feature-name>.md`

## Bite-Sized Task Granularity

**Each step is one action (2-5 minutes):**
- "Write the failing test" - step
- "Run it to make sure it fails" - step
- "Implement the minimal code to make the test pass" - step
- "Run the tests and make sure they pass" - step
- "Commit" - step

## Plan Document Header

**Every plan MUST start with this header:**

```markdown
# [Feature Name] Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** [One sentence describing what this builds]

**Architecture:** [2-3 sentences about approach]

**Tech Stack:** [Key technologies/libraries - e.g., ASP.NET Core, Entity Framework, xUnit]

---
```

## Task Structure

```markdown
### Task N: [Component Name]

**Files:**
- Create: `src/MyProject.Core/Services/ExactService.cs`
- Modify: `src/MyProject.Core/Models/ExistingModel.cs:123-145`
- Test: `tests/MyProject.Core.Tests/Services/ExactServiceTests.cs`

**Step 1: Write the failing test**

```csharp
[Fact]
public void SpecificBehavior_ShouldReturnExpectedResult()
{
    // Arrange
    var service = new ExactService();
    var input = "test input";
    
    // Act
    var result = service.ProcessInput(input);
    
    // Assert
    Assert.Equal("expected result", result);
}
```

**Step 2: Run test to verify it fails**

Run: `dotnet test tests/MyProject.Core.Tests/Services/ExactServiceTests.cs --filter "SpecificBehavior_ShouldReturnExpectedResult"`
Expected: FAIL with "ExactService does not exist"

**Step 3: Write minimal implementation**

```csharp
namespace MyProject.Core.Services;

public class ExactService
{
    public string ProcessInput(string input)
    {
        return "expected result";
    }
}
```

**Step 4: Run test to verify it passes**

Run: `dotnet test tests/MyProject.Core.Tests/Services/ExactServiceTests.cs --filter "SpecificBehavior_ShouldReturnExpectedResult"`
Expected: PASS

**Step 5: Commit**

```bash
git add tests/MyProject.Core.Tests/Services/ExactServiceTests.cs src/MyProject.Core/Services/ExactService.cs
git commit -m "feat: add specific feature"
```
```

## Remember
- Exact file paths always
- Complete code in plan (not "add validation")
- Exact commands with expected output
- Reference relevant skills with @ syntax
- DRY, YAGNI, TDD, frequent commits

## Execution Handoff

After saving the plan, offer execution choice:

**"Plan complete and saved to `docs/plans/<filename>.md`. Two execution options:**

**1. Subagent-Driven (this session)** - I dispatch fresh subagent per task, review between tasks, fast iteration

**2. Parallel Session (separate)** - Open new session with executing-plans, batch execution with checkpoints

**Which approach?"**

**If Subagent-Driven chosen:**
- **REQUIRED SUB-SKILL:** Use superpowers:subagent-driven-development
- Stay in this session
- Fresh subagent per task + code review

**If Parallel Session chosen:**
- Guide them to open new session in worktree
- **REQUIRED SUB-SKILL:** New session uses superpowers:executing-plans
