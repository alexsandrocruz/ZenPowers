using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ZenPowers.Core;

/// <summary>
/// Categories of ZenPowers skills
/// </summary>
public enum SkillCategory
{
    Testing,
    Debugging,
    Collaboration,
    Meta,
    CodeReview,
    GitWorkflow,
    Security
}

/// <summary>
/// Test execution results for TDD workflows
/// </summary>
public enum TestResult
{
    NotRun,
    Passed,
    Failed,
    Error
}

/// <summary>
/// Debugging investigation phases
/// </summary>
public enum DebuggingPhase
{
    RootCauseInvestigation,
    PatternAnalysis,
    HypothesisTesting,
    Implementation
}

/// <summary>
/// Core utility functions for condition-based waiting patterns
/// </summary>
public static class ConditionWaiting
{
    /// <summary>
    /// Wait for a condition to become true with timeout and cancellation support
    /// </summary>
    public static async Task WaitForAsync(
        Func<bool> condition,
        string description,
        int timeoutMs = 5000,
        int pollIntervalMs = 10,
        CancellationToken cancellationToken = default)
    {
        var startTime = DateTime.UtcNow;

        while (!cancellationToken.IsCancellationRequested)
        {
            if (condition())
                return;

            if ((DateTime.UtcNow - startTime).TotalMilliseconds > timeoutMs)
            {
                throw new TimeoutException($"Timeout waiting for {description} after {timeoutMs}ms");
            }

            await Task.Delay(pollIntervalMs, cancellationToken);
        }

        throw new OperationCanceledException();
    }

    /// <summary>
    /// Wait for a condition to return a non-null result
    /// </summary>
    public static async Task<T> WaitForAsync<T>(
        Func<T?> condition,
        string description,
        int timeoutMs = 5000,
        int pollIntervalMs = 10,
        CancellationToken cancellationToken = default)
        where T : class
    {
        var startTime = DateTime.UtcNow;

        while (!cancellationToken.IsCancellationRequested)
        {
            var result = condition();
            if (result != null) return result;

            if ((DateTime.UtcNow - startTime).TotalMilliseconds > timeoutMs)
            {
                throw new TimeoutException($"Timeout waiting for {description} after {timeoutMs}ms");
            }

            await Task.Delay(pollIntervalMs, cancellationToken);
        }

        throw new OperationCanceledException();
    }
}

/// <summary>
/// Registry and utilities for ZenPowers skills system
/// </summary>
public static class SkillRegistry
{
    /// <summary>
    /// Available skill categories in ZenPowers
    /// </summary>
    public static SkillCategory[] ListCategories() => new[]
    {
        SkillCategory.Testing,
        SkillCategory.Debugging,
        SkillCategory.Collaboration,
        SkillCategory.Meta,
        SkillCategory.CodeReview,
        SkillCategory.GitWorkflow,
        SkillCategory.Security
    };

    /// <summary>
    /// Validates that required parameters are not null or empty
    /// </summary>
    public static void ValidateRequired(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{parameterName} cannot be null or empty", parameterName);
        }
    }

    /// <summary>
    /// Validates that a directory exists and is accessible
    /// </summary>
    public static void ValidateDirectory(string path, string parameterName)
    {
        ValidateRequired(path, parameterName);
        
        if (!System.IO.Directory.Exists(path))
        {
            throw new System.IO.DirectoryNotFoundException($"Directory not found: {path}");
        }
    }
}
