// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;

namespace Xaml.Behaviors.Uno.Headless.Internal;

/// <summary>
/// Runs delegates on a <see cref="DispatcherQueue"/> thread and exposes their completion as tasks.
/// </summary>
/// <remarks>
/// Delegates run inline when the caller already has thread access, so calls made from the UI thread never
/// re-queue work. Queued work runs inside the caller's <see cref="ExecutionContext"/>, so async-local state
/// (for example the xUnit test context) flows to the UI thread like it does with <see cref="Task.Run(Action)"/>.
/// Exceptions and cancellation are propagated to the returned task.
/// </remarks>
internal static class DispatcherQueueInvoker
{
    /// <summary>Runs <paramref name="action"/> on the dispatcher thread.</summary>
    public static Task InvokeAsync(DispatcherQueue dispatcherQueue, Action action)
    {
        if (dispatcherQueue.HasThreadAccess)
        {
            try
            {
                action();
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        TaskCompletionSource completion = new(TaskCreationOptions.RunContinuationsAsynchronously);
        Enqueue(
            dispatcherQueue,
            () =>
            {
                action();
                completion.TrySetResult();
            },
            exception => completion.TrySetException(exception));
        return completion.Task;
    }

    /// <summary>Runs <paramref name="function"/> on the dispatcher thread and returns its result.</summary>
    public static Task<TResult> InvokeAsync<TResult>(DispatcherQueue dispatcherQueue, Func<TResult> function)
    {
        if (dispatcherQueue.HasThreadAccess)
        {
            try
            {
                return Task.FromResult(function());
            }
            catch (Exception exception)
            {
                return Task.FromException<TResult>(exception);
            }
        }

        TaskCompletionSource<TResult> completion = new(TaskCreationOptions.RunContinuationsAsynchronously);
        Enqueue(
            dispatcherQueue,
            () => completion.TrySetResult(function()),
            exception => completion.TrySetException(exception));
        return completion.Task;
    }

    /// <summary>Runs the asynchronous <paramref name="function"/> on the dispatcher thread.</summary>
    public static Task InvokeAsync(DispatcherQueue dispatcherQueue, Func<Task> function)
    {
        if (dispatcherQueue.HasThreadAccess)
        {
            try
            {
                return function();
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        TaskCompletionSource completion = new(TaskCreationOptions.RunContinuationsAsynchronously);
        Enqueue(
            dispatcherQueue,
            () => PropagateCompletion(function(), completion),
            exception => completion.TrySetException(exception));
        return completion.Task;
    }

    /// <summary>Runs the asynchronous <paramref name="function"/> on the dispatcher thread and returns its result.</summary>
    public static Task<TResult> InvokeAsync<TResult>(DispatcherQueue dispatcherQueue, Func<Task<TResult>> function)
    {
        if (dispatcherQueue.HasThreadAccess)
        {
            try
            {
                return function();
            }
            catch (Exception exception)
            {
                return Task.FromException<TResult>(exception);
            }
        }

        TaskCompletionSource<TResult> completion = new(TaskCreationOptions.RunContinuationsAsynchronously);
        Enqueue(
            dispatcherQueue,
            () => PropagateCompletion(function(), completion),
            exception => completion.TrySetException(exception));
        return completion.Task;
    }

    /// <summary>
    /// Completes once an item queued at <see cref="DispatcherQueuePriority.Low"/> priority has run, i.e. after the
    /// work queued before it at normal or higher priority has been processed.
    /// </summary>
    public static Task YieldAsync(DispatcherQueue dispatcherQueue)
    {
        TaskCompletionSource completion = new(TaskCreationOptions.RunContinuationsAsynchronously);
        if (!dispatcherQueue.TryEnqueue(DispatcherQueuePriority.Low, () => completion.TrySetResult()))
        {
            completion.TrySetException(CreateShutdownException());
        }

        return completion.Task;
    }

    private static void Enqueue(DispatcherQueue dispatcherQueue, Action work, Action<Exception> fail)
    {
        ExecutionContext? executionContext = ExecutionContext.Capture();

        bool queued = dispatcherQueue.TryEnqueue(() =>
        {
            try
            {
                if (executionContext is null)
                {
                    work();
                }
                else
                {
                    ExecutionContext.Run(executionContext, static state => ((Action)state!)(), work);
                }
            }
            catch (Exception exception)
            {
                fail(exception);
            }
        });

        if (!queued)
        {
            fail(CreateShutdownException());
        }
    }

    private static void PropagateCompletion(Task task, TaskCompletionSource completion)
        => task.ContinueWith(
            static (antecedent, state) =>
            {
                TaskCompletionSource target = (TaskCompletionSource)state!;
                if (antecedent.IsFaulted)
                {
                    target.TrySetException(antecedent.Exception!.InnerExceptions);
                }
                else if (antecedent.IsCanceled)
                {
                    target.TrySetCanceled();
                }
                else
                {
                    target.TrySetResult();
                }
            },
            completion,
            CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler.Default);

    private static void PropagateCompletion<TResult>(Task<TResult> task, TaskCompletionSource<TResult> completion)
        => task.ContinueWith(
            static (antecedent, state) =>
            {
                TaskCompletionSource<TResult> target = (TaskCompletionSource<TResult>)state!;
                if (antecedent.IsFaulted)
                {
                    target.TrySetException(antecedent.Exception!.InnerExceptions);
                }
                else if (antecedent.IsCanceled)
                {
                    target.TrySetCanceled();
                }
                else
                {
                    target.TrySetResult(antecedent.Result);
                }
            },
            completion,
            CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler.Default);

    private static InvalidOperationException CreateShutdownException()
        => new("The Uno headless dispatcher queue is shutting down and no longer accepts work.");
}
