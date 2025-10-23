// Complete implementation of condition-based waiting utilities
// From: Test infrastructure improvements (2025-10-03)
// Context: Fixed 15 flaky tests by replacing arbitrary timeouts

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ZenPowers.Testing;

public enum EventType
{
    ToolResult,
    AgentMessage,
    ToolCall
}

public class Event
{
    public EventType Type { get; set; }
    public object Data { get; set; } = default!;
    public string Id { get; set; } = string.Empty;
}

public interface IThreadManager
{
    IEnumerable<Event> GetEvents(string threadId);
}

/// <summary>
/// Wait for a specific event type to appear in thread
/// </summary>
/// <param name="threadManager">The thread manager to query</param>
/// <param name="threadId">Thread to check for events</param>
/// <param name="eventType">Type of event to wait for</param>
/// <param name="timeoutMs">Maximum time to wait (default 5000ms)</param>
/// <param name="cancellationToken">Cancellation token</param>
/// <returns>Task resolving to the first matching event</returns>
/// <example>
/// await WaitForEventAsync(threadManager, agentThreadId, EventType.ToolResult);
/// </example>
public static class ConditionWaiting
{
    public static async Task<Event> WaitForEventAsync(
        IThreadManager threadManager,
        string threadId,
        EventType eventType,
        int timeoutMs = 5000,
        CancellationToken cancellationToken = default)
    {
        return await WaitForAsync(
            () => threadManager.GetEvents(threadId).FirstOrDefault(e => e.Type == eventType),
            $"{eventType} event",
            timeoutMs,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Wait for a specific number of events of a given type
    /// </summary>
    /// <param name="threadManager">The thread manager to query</param>
    /// <param name="threadId">Thread to check for events</param>
    /// <param name="eventType">Type of event to wait for</param>
    /// <param name="count">Number of events to wait for</param>
    /// <param name="timeoutMs">Maximum time to wait (default 5000ms)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task resolving to all matching events once count is reached</returns>
    /// <example>
    /// // Wait for 2 AGENT_MESSAGE events (initial response + continuation)
    /// await WaitForEventCountAsync(threadManager, agentThreadId, EventType.AgentMessage, 2);
    /// </example>
    public static async Task<IEnumerable<Event>> WaitForEventCountAsync(
        IThreadManager threadManager,
        string threadId,
        EventType eventType,
        int count,
        int timeoutMs = 5000,
        CancellationToken cancellationToken = default)
    {
        return await WaitForAsync(
            () =>
            {
                var matchingEvents = threadManager.GetEvents(threadId)
                    .Where(e => e.Type == eventType)
                    .ToList();
                return matchingEvents.Count >= count ? matchingEvents : null;
            },
            $"{count} {eventType} events",
            timeoutMs,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Wait for an event matching a custom predicate
    /// Useful when you need to check event data, not just type
    /// </summary>
    /// <param name="threadManager">The thread manager to query</param>
    /// <param name="threadId">Thread to check for events</param>
    /// <param name="predicate">Function that returns true when event matches</param>
    /// <param name="description">Human-readable description for error messages</param>
    /// <param name="timeoutMs">Maximum time to wait (default 5000ms)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task resolving to the first matching event</returns>
    /// <example>
    /// // Wait for TOOL_RESULT with specific ID
    /// await WaitForEventMatchAsync(
    ///     threadManager,
    ///     agentThreadId,
    ///     e => e.Type == EventType.ToolResult && e.Id == "call_123",
    ///     "TOOL_RESULT with id=call_123"
    /// );
    /// </example>
    public static async Task<Event> WaitForEventMatchAsync(
        IThreadManager threadManager,
        string threadId,
        Func<Event, bool> predicate,
        string description,
        int timeoutMs = 5000,
        CancellationToken cancellationToken = default)
    {
        return await WaitForAsync(
            () => threadManager.GetEvents(threadId).FirstOrDefault(predicate),
            description,
            timeoutMs,
            cancellationToken: cancellationToken);
    }

    private static async Task<T> WaitForAsync<T>(
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

// Usage example from actual debugging session:
//
// BEFORE (flaky):
// ---------------
// var messageTask = agent.SendMessageAsync("Execute tools");
// await Task.Delay(300); // Hope tools start in 300ms
// agent.Abort();
// await messageTask;
// await Task.Delay(50);  // Hope results arrive in 50ms
// Assert.Equal(2, toolResults.Count);         // Fails randomly
//
// AFTER (reliable):
// ----------------
// var messageTask = agent.SendMessageAsync("Execute tools");
// await WaitForEventCountAsync(threadManager, threadId, EventType.ToolCall, 2); // Wait for tools to start
// agent.Abort();
// await messageTask;
// await WaitForEventCountAsync(threadManager, threadId, EventType.ToolResult, 2); // Wait for results
// Assert.Equal(2, toolResults.Count); // Always succeeds
//
// Result: 60% pass rate → 100%, 40% faster execution
