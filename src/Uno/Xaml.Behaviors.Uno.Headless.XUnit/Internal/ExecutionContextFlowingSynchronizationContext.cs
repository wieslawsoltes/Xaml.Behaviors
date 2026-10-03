// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Threading;

namespace Xaml.Behaviors.Uno.Headless.XUnit.Internal;

/// <summary>
/// Wraps a synchronization context so that <see cref="Post"/> runs the callback inside the poster's
/// <see cref="ExecutionContext"/>.
/// </summary>
/// <remarks>
/// Uno's UI thread synchronization context does not flow the execution context through <c>Post</c>. xUnit posts
/// tests that have a timeout to the current synchronization context and relies on the execution context to carry
/// the test context (<c>TestContext.Current</c>), so the runner wraps the UI thread context for that call.
/// </remarks>
internal sealed class ExecutionContextFlowingSynchronizationContext : SynchronizationContext
{
    private readonly SynchronizationContext _inner;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExecutionContextFlowingSynchronizationContext"/> class.
    /// </summary>
    /// <param name="inner">The context that executes the callbacks.</param>
    public ExecutionContextFlowingSynchronizationContext(SynchronizationContext inner)
    {
        _inner = inner;
    }

    /// <inheritdoc />
    public override void Post(SendOrPostCallback d, object? state)
    {
        ExecutionContext? executionContext = ExecutionContext.Capture();
        if (executionContext is null)
        {
            _inner.Post(d, state);
            return;
        }

        _inner.Post(
            static packed =>
            {
                PostedWork work = (PostedWork)packed!;
                ExecutionContext.Run(work.ExecutionContext, static s => ((PostedWork)s!).Invoke(), work);
            },
            new PostedWork(executionContext, d, state));
    }

    /// <inheritdoc />
    public override void Send(SendOrPostCallback d, object? state) => _inner.Send(d, state);

    /// <inheritdoc />
    public override SynchronizationContext CreateCopy() => new ExecutionContextFlowingSynchronizationContext(_inner.CreateCopy());

    private sealed class PostedWork(ExecutionContext executionContext, SendOrPostCallback callback, object? state)
    {
        public ExecutionContext ExecutionContext { get; } = executionContext;

        public void Invoke() => callback(state);
    }
}
