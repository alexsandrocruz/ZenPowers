---
name: root-cause-tracing
description: Use when errors occur deep in execution and you need to trace back to find the original trigger - systematically traces bugs backward through call stack, adding instrumentation when needed, to identify source of invalid data or incorrect behavior
---

# Root Cause Tracing

## Overview

Bugs often manifest deep in the call stack (git init in wrong directory, file created in wrong location, database opened with wrong path). Your instinct is to fix where the error appears, but that's treating a symptom.

**Core principle:** Trace backward through the call chain until you find the original trigger, then fix at the source.

## When to Use

```dot
digraph when_to_use {
    "Bug appears deep in stack?" [shape=diamond];
    "Can trace backwards?" [shape=diamond];
    "Fix at symptom point" [shape=box];
    "Trace to original trigger" [shape=box];
    "BETTER: Also add defense-in-depth" [shape=box];

    "Bug appears deep in stack?" -> "Can trace backwards?" [label="yes"];
    "Can trace backwards?" -> "Trace to original trigger" [label="yes"];
    "Can trace backwards?" -> "Fix at symptom point" [label="no - dead end"];
    "Trace to original trigger" -> "BETTER: Also add defense-in-depth";
}
```

**Use when:**
- Error happens deep in execution (not at entry point)
- Stack trace shows long call chain
- Unclear where invalid data originated
- Need to find which test/code triggers the problem

## The Tracing Process

### 1. Observe the Symptom
```
Error: git init failed in C:\Users\developer\project\src\MyProject.Core
```

### 2. Find Immediate Cause
**What code directly causes this?**
```csharp
await ProcessHelper.RunAsync("git", "init", workingDirectory: projectDir);
```

### 3. Ask: What Called This?
```csharp
WorktreeManager.CreateSessionWorktreeAsync(projectDir, sessionId)
  → called by Session.InitializeWorkspaceAsync()
  → called by Session.CreateAsync()
  → called by test at Project.CreateAsync()
```

### 4. Keep Tracing Up
**What value was passed?**
- `projectDir = ""` (empty string!)
- Empty string as working directory resolves to current directory
- That's the source code directory!

### 5. Find Original Trigger
**Where did empty string come from?**
```csharp
var context = SetupCoreTest(); // Returns { TempDir = "" }
await Project.CreateAsync("name", context.TempDir); // Accessed before setup!
```

## Adding Stack Traces

When you can't trace manually, add instrumentation:

```csharp
// Before the problematic operation
public static async Task GitInitAsync(string directory)
{
    var stackTrace = Environment.StackTrace;
    Console.Error.WriteLine($"DEBUG git init: {new {
        Directory = directory,
        CurrentDirectory = Environment.CurrentDirectory,
        Environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT"),
        StackTrace = stackTrace
    }}");

    await ProcessHelper.RunAsync("git", "init", workingDirectory: directory);
}
```

**Critical:** Use `Console.Error.WriteLine()` in tests (not ILogger - may not show)

**Run and capture:**
```bash
dotnet test --logger "console;verbosity=detailed" 2>&1 | findstr "DEBUG git init"
```

**Analyze stack traces:**
- Look for test file names
- Find the line number triggering the call
- Identify the pattern (same test? same parameter?)

## Finding Which Test Causes Pollution

If something appears during tests but you don't know which test:

Use the bisection script: @find-polluter.sh

```bash
./find-polluter.sh '.git' 'src/**/*.test.ts'
```

Runs tests one-by-one, stops at first polluter. See script for usage.

## Real Example: Empty projectDir

**Symptom:** `.git` created in `packages/core/` (source code)

**Trace chain:**
1. `git init` runs in current directory ← empty working directory parameter
2. WorktreeManager called with empty projectDir
3. Session.CreateAsync() passed empty string
4. Test accessed `context.TempDir` before setup
5. SetupCoreTest() returns `{ TempDir = "" }` initially

**Root cause:** Top-level variable initialization accessing empty value

**Fix:** Made TempDir a property that throws if accessed before setup

**Also added defense-in-depth:**
- Layer 1: Project.CreateAsync() validates directory
- Layer 2: WorkspaceManager validates not empty
- Layer 3: DOTNET_ENVIRONMENT guard refuses git init outside temp directory
- Layer 4: Stack trace logging before git init

## Key Principle

```dot
digraph principle {
    "Found immediate cause" [shape=ellipse];
    "Can trace one level up?" [shape=diamond];
    "Trace backwards" [shape=box];
    "Is this the source?" [shape=diamond];
    "Fix at source" [shape=box];
    "Add validation at each layer" [shape=box];
    "Bug impossible" [shape=doublecircle];
    "NEVER fix just the symptom" [shape=octagon, style=filled, fillcolor=red, fontcolor=white];

    "Found immediate cause" -> "Can trace one level up?";
    "Can trace one level up?" -> "Trace backwards" [label="yes"];
    "Can trace one level up?" -> "NEVER fix just the symptom" [label="no"];
    "Trace backwards" -> "Is this the source?";
    "Is this the source?" -> "Trace backwards" [label="no - keeps going"];
    "Is this the source?" -> "Fix at source" [label="yes"];
    "Fix at source" -> "Add validation at each layer";
    "Add validation at each layer" -> "Bug impossible";
}
```

**NEVER fix just where the error appears.** Trace back to find the original trigger.

## Stack Trace Tips

**In tests:** Use `Console.Error.WriteLine()` not ILogger - logger may be suppressed
**Before operation:** Log before the dangerous operation, not after it fails
**Include context:** Directory, current directory, environment variables, timestamps
**Capture stack:** `Environment.StackTrace` shows complete call chain

## Real-World Impact

From debugging session (2025-10-03):
- Found root cause through 5-level trace
- Fixed at source (getter validation)
- Added 4 layers of defense
- 1847 tests passed, zero pollution
