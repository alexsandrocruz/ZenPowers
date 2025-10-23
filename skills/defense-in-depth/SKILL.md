---
name: defense-in-depth
description: Use when invalid data causes failures deep in execution, requiring validation at multiple system layers - validates at every layer data passes through to make bugs structurally impossible
---

# Defense-in-Depth Validation

## Overview

When you fix a bug caused by invalid data, adding validation at one place feels sufficient. But that single check can be bypassed by different code paths, refactoring, or mocks.

**Core principle:** Validate at EVERY layer data passes through. Make the bug structurally impossible.

## Why Multiple Layers

Single validation: "We fixed the bug"
Multiple layers: "We made the bug impossible"

Different layers catch different cases:
- Entry validation catches most bugs
- Business logic catches edge cases
- Environment guards prevent context-specific dangers
- Debug logging helps when other layers fail

## The Four Layers

### Layer 1: Entry Point Validation
**Purpose:** Reject obviously invalid input at API boundary

```csharp
public static async Task<Project> CreateProjectAsync(string name, string workingDirectory)
{
    if (string.IsNullOrWhiteSpace(workingDirectory))
    {
        throw new ArgumentException("Working directory cannot be empty or whitespace", nameof(workingDirectory));
    }
    
    if (!Directory.Exists(workingDirectory))
    {
        throw new DirectoryNotFoundException($"Working directory does not exist: {workingDirectory}");
    }
    
    var dirInfo = new DirectoryInfo(workingDirectory);
    if (!dirInfo.Exists)
    {
        throw new ArgumentException($"Working directory is not accessible: {workingDirectory}", nameof(workingDirectory));
    }
    
    // ... proceed
}
```

### Layer 2: Business Logic Validation
**Purpose:** Ensure data makes sense for this operation

```csharp
public static async Task<Workspace> InitializeWorkspaceAsync(string projectDir, string sessionId)
{
    if (string.IsNullOrEmpty(projectDir))
    {
        throw new InvalidOperationException("projectDir required for workspace initialization");
    }
    
    // ... proceed
}
```

### Layer 3: Environment Guards
**Purpose:** Prevent dangerous operations in specific contexts

```csharp
public static async Task GitInitAsync(string directory)
{
    // In tests, refuse git init outside temp directories
    var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
    if (environment == "Test")
    {
        var normalized = Path.GetFullPath(directory);
        var tempPath = Path.GetFullPath(Path.GetTempPath());

        if (!normalized.StartsWith(tempPath, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Refusing git init outside temp dir during tests: {directory}");
        }
    }
    
    // ... proceed
}
```

### Layer 4: Debug Instrumentation
**Purpose:** Capture context for forensics

```csharp
public static async Task GitInitAsync(string directory)
{
    var stackTrace = Environment.StackTrace;
    
    _logger.LogDebug("About to git init: {Directory}, CWD: {CurrentDirectory}, Stack: {StackTrace}", 
        directory, 
        Environment.CurrentDirectory, 
        stackTrace);
        
    // ... proceed
}
```

## Applying the Pattern

When you find a bug:

1. **Trace the data flow** - Where does bad value originate? Where used?
2. **Map all checkpoints** - List every point data passes through
3. **Add validation at each layer** - Entry, business, environment, debug
4. **Test each layer** - Try to bypass layer 1, verify layer 2 catches it

## Example from Session

Bug: Empty `projectDir` caused `git init` in source code

**Data flow:**
1. Test setup → empty string
2. `Project.create(name, '')`
3. `WorkspaceManager.createWorkspace('')`
4. `git init` runs in `process.cwd()`

**Four layers added:**
- Layer 1: `Project.create()` validates not empty/exists/writable
- Layer 2: `WorkspaceManager` validates projectDir not empty
- Layer 3: `WorktreeManager` refuses git init outside tmpdir in tests
- Layer 4: Stack trace logging before git init

**Result:** All 1847 tests passed, bug impossible to reproduce

## Key Insight

All four layers were necessary. During testing, each layer caught bugs the others missed:
- Different code paths bypassed entry validation
- Mocks bypassed business logic checks
- Edge cases on different platforms needed environment guards
- Debug logging identified structural misuse

**Don't stop at one validation point.** Add checks at every layer.
