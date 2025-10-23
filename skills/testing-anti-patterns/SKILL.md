---
name: testing-anti-patterns
description: Use when writing or changing tests, adding mocks, or tempted to add test-only methods to production code - prevents testing mock behavior, production pollution with test-only methods, and mocking without understanding dependencies
---

# Testing Anti-Patterns

## Overview

Tests must verify real behavior, not mock behavior. Mocks are a means to isolate, not the thing being tested.

**Core principle:** Test what the code does, not what the mocks do.

**Following strict TDD prevents these anti-patterns.**

## The Iron Laws

```
1. NEVER test mock behavior
2. NEVER add test-only methods to production classes
3. NEVER mock without understanding dependencies
```

## Anti-Pattern 1: Testing Mock Behavior

**The violation:**
```csharp
// ❌ BAD: Testing that the mock exists
[Fact]
public void RenderPage_ShouldRenderSidebar()
{
    // Arrange
    var mockSidebar = new Mock<ISidebar>();
    var page = new Page(mockSidebar.Object);

    // Act
    var result = page.Render();

    // Assert
    mockSidebar.Verify(x => x.IsVisible, Times.Once); // Testing mock!
}
```

**Why this is wrong:**
- You're verifying the mock works, not that the component works
- Test passes when mock is present, fails when it's not
- Tells you nothing about real behavior

**your human partner's correction:** "Are we testing the behavior of a mock?"

**The fix:**
```csharp
// ✅ GOOD: Test real component or don't mock it
[Fact]
public void RenderPage_ShouldIncludeSidebarContent()
{
    // Arrange
    var sidebar = new Sidebar(); // Don't mock sidebar
    var page = new Page(sidebar);

    // Act
    var result = page.Render();

    // Assert
    Assert.Contains("navigation", result.Content);
}

// OR if sidebar must be mocked for isolation:
// Don't assert on the mock - test Page's behavior with sidebar present
```

### Gate Function

```
BEFORE asserting on any mock element:
  Ask: "Am I testing real component behavior or just mock existence?"

  IF testing mock existence:
    STOP - Delete the assertion or unmock the component

  Test real behavior instead
```

## Anti-Pattern 2: Test-Only Methods in Production

**The violation:**
```csharp
// ❌ BAD: Destroy() only used in tests
public class Session
{
    public async Task DestroyAsync()  // Looks like production API!
    {
        await _workspaceManager?.DestroyWorkspaceAsync(Id);
        // ... cleanup
    }
}

// In tests
public async Task TearDown() => await _session.DestroyAsync();
```

**Why this is wrong:**
- Production class polluted with test-only code
- Dangerous if accidentally called in production
- Violates YAGNI and separation of concerns
- Confuses object lifecycle with entity lifecycle

**The fix:**
```csharp
// ✅ GOOD: Test utilities handle test cleanup
// Session has no Destroy() - it's stateless in production

// In TestUtilities/
public static class SessionTestHelpers
{
    public static async Task CleanupSessionAsync(Session session)
    {
        var workspace = session.GetWorkspaceInfo();
        if (workspace != null)
        {
            await WorkspaceManager.DestroyWorkspaceAsync(workspace.Id);
        }
    }
}

// In tests
public async Task TearDown() => await SessionTestHelpers.CleanupSessionAsync(_session);
```

### Gate Function

```
BEFORE adding any method to production class:
  Ask: "Is this only used by tests?"

  IF yes:
    STOP - Don't add it
    Put it in test utilities instead

  Ask: "Does this class own this resource's lifecycle?"

  IF no:
    STOP - Wrong class for this method
```

## Anti-Pattern 3: Mocking Without Understanding

**The violation:**
```csharp
// ❌ BAD: Mock breaks test logic
[Fact]
public async Task AddServer_ShouldDetectDuplicateServer()
{
    // Mock prevents config write that test depends on!
    var mockToolCatalog = new Mock<IToolCatalog>();
    mockToolCatalog.Setup(x => x.DiscoverAndCacheToolsAsync())
               .Returns(Task.CompletedTask);

    await AddServerAsync(config);
    await AddServerAsync(config);  // Should throw - but won't!
}
```

**Why this is wrong:**
- Mocked method had side effect test depended on (writing config)
- Over-mocking to "be safe" breaks actual behavior
- Test passes for wrong reason or fails mysteriously

**The fix:**
```csharp
// ✅ GOOD: Mock at correct level
[Fact]
public async Task AddServer_ShouldDetectDuplicateServer()
{
    // Mock the slow part, preserve behavior test needs
    var mockServerManager = new Mock<IMCPServerManager>();
    
    await AddServerAsync(config);  // Config written
    await AddServerAsync(config);  // Duplicate detected ✓
}
```

### Gate Function

```
BEFORE mocking any method:
  STOP - Don't mock yet

  1. Ask: "What side effects does the real method have?"
  2. Ask: "Does this test depend on any of those side effects?"
  3. Ask: "Do I fully understand what this test needs?"

  IF depends on side effects:
    Mock at lower level (the actual slow/external operation)
    OR use test doubles that preserve necessary behavior
    NOT the high-level method the test depends on

  IF unsure what test depends on:
    Run test with real implementation FIRST
    Observe what actually needs to happen
    THEN add minimal mocking at the right level

  Red flags:
    - "I'll mock this to be safe"
    - "This might be slow, better mock it"
    - Mocking without understanding the dependency chain
```

## Anti-Pattern 4: Incomplete Mocks

**The violation:**
```csharp
// ❌ BAD: Partial mock - only fields you think you need
var mockResponse = new ApiResponse
{
    Status = "success",
    Data = new UserData { UserId = "123", Name = "Alice" }
    // Missing: Metadata that downstream code uses
};

// Later: breaks when code accesses response.Metadata.RequestId
```

**Why this is wrong:**
- **Partial mocks hide structural assumptions** - You only mocked fields you know about
- **Downstream code may depend on fields you didn't include** - Silent failures
- **Tests pass but integration fails** - Mock incomplete, real API complete
- **False confidence** - Test proves nothing about real behavior

**The Iron Rule:** Mock the COMPLETE data structure as it exists in reality, not just fields your immediate test uses.

**The fix:**
```csharp
// ✅ GOOD: Mirror real API completeness
var mockResponse = new ApiResponse
{
    Status = "success",
    Data = new UserData { UserId = "123", Name = "Alice" },
    Metadata = new ResponseMetadata 
    { 
        RequestId = "req-789", 
        Timestamp = DateTimeOffset.UtcNow 
    }
    // All fields real API returns
};
```

### Gate Function

```
BEFORE creating mock responses:
  Check: "What fields does the real API response contain?"

  Actions:
    1. Examine actual API response from docs/examples
    2. Include ALL fields system might consume downstream
    3. Verify mock matches real response schema completely

  Critical:
    If you're creating a mock, you must understand the ENTIRE structure
    Partial mocks fail silently when code depends on omitted fields

  If uncertain: Include all documented fields
```

## Anti-Pattern 5: Integration Tests as Afterthought

**The violation:**
```
✅ Implementation complete
❌ No tests written
"Ready for testing"
```

**Why this is wrong:**
- Testing is part of implementation, not optional follow-up
- TDD would have caught this
- Can't claim complete without tests

**The fix:**
```
TDD cycle:
1. Write failing test
2. Implement to pass
3. Refactor
4. THEN claim complete
```

## When Mocks Become Too Complex

**Warning signs:**
- Mock setup longer than test logic
- Mocking everything to make test pass
- Mocks missing methods real components have
- Test breaks when mock changes

**your human partner's question:** "Do we need to be using a mock here?"

**Consider:** Integration tests with real components often simpler than complex mocks

## TDD Prevents These Anti-Patterns

**Why TDD helps:**
1. **Write test first** → Forces you to think about what you're actually testing
2. **Watch it fail** → Confirms test tests real behavior, not mocks
3. **Minimal implementation** → No test-only methods creep in
4. **Real dependencies** → You see what the test actually needs before mocking

**If you're testing mock behavior, you violated TDD** - you added mocks without watching test fail against real code first.

## Quick Reference

| Anti-Pattern | Fix |
|--------------|-----|
| Assert on mock elements | Test real component or unmock it |
| Test-only methods in production | Move to test utilities |
| Mock without understanding | Understand dependencies first, mock minimally |
| Incomplete mocks | Mirror real API completely |
| Tests as afterthought | TDD - tests first |
| Over-complex mocks | Consider integration tests |

## Red Flags

- Assertion checks for `*-mock` test IDs
- Methods only called in test files
- Mock setup is >50% of test
- Test fails when you remove mock
- Can't explain why mock is needed
- Mocking "just to be safe"

## The Bottom Line

**Mocks are tools to isolate, not things to test.**

If TDD reveals you're testing mock behavior, you've gone wrong.

Fix: Test real behavior or question why you're mocking at all.
