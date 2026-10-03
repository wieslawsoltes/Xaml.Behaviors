// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xaml.Behaviors.Uno.Headless.XUnit.Internal;
using Xunit.Sdk;
using Xunit.v3;

namespace Xaml.Behaviors.WinUI.Testing.XUnit.Internal;

/// <summary>
/// Runs a single test on the UI thread of the WinUI test session: test class construction, <c>IAsyncLifetime</c>,
/// before/after attributes, the test method and disposal all execute there, and the UI thread's
/// synchronization context is captured so awaited expressions resume on it.
/// </summary>
/// <remarks>Stateless, so a single instance is shared (the xUnit runner pattern).</remarks>
internal sealed class WinUITestRunner : XunitTestRunnerBase<XunitTestRunnerContext, IXunitTest>
{
    private WinUITestRunner()
    {
    }

    /// <summary>Gets the shared runner instance.</summary>
    public static WinUITestRunner Instance { get; } = new();

    /// <summary>
    /// Runs <paramref name="test"/> on the UI thread of the WinUI test session, starting the session if needed.
    /// </summary>
    public async ValueTask<RunSummary> Run(
        IXunitTest test,
        IMessageBus messageBus,
        object?[] constructorArguments,
        ExplicitOption explicitOption,
        ExceptionAggregator aggregator,
        CancellationTokenSource cancellationTokenSource,
        IReadOnlyCollection<IBeforeAfterTestAttribute> beforeAfterAttributes)
    {
        await using XunitTestRunnerContext context = new(
            test,
            messageBus,
            explicitOption,
            aggregator,
            cancellationTokenSource,
            beforeAfterAttributes,
            constructorArguments);

        await context.InitializeAsync();

        WinUITestSession session;
        try
        {
            session = await WinUITestSession.GetOrStartAsync();
        }
        catch (Exception exception)
        {
            // Report the start-up failure as the test's failure instead of crashing the run.
            context.Aggregator.Add(exception);
            return await Run(context);
        }

        return await session.RunAsync(() => Run(context).AsTask());
    }

    /// <inheritdoc />
    protected override async ValueTask<TimeSpan> RunTest(XunitTestRunnerContext ctxt)
    {
        // A WinUI application terminates on an unhandled UI thread exception: the session handles them, and the ones
        // raised while the test runs fail it.
        WinUITestSession? session = WinUITestSession.CurrentOrNull;
        session?.TakeUnhandledExceptions();
        TimeSpan elapsed = await RunTestCore(ctxt);
        if (session is not null)
        {
            foreach (Exception exception in session.TakeUnhandledExceptions())
            {
                ctxt.Aggregator.Add(exception);
            }
        }

        return elapsed;
    }

    private ValueTask<TimeSpan> RunTestCore(XunitTestRunnerContext ctxt)
    {
        // Tests with a timeout are posted by xUnit to SynchronizationContext.Current (read synchronously by the base
        // implementation). Make that post flow the execution context so TestContext.Current stays correct.
        SynchronizationContext? current = SynchronizationContext.Current;
        if (ctxt.Test.Timeout <= 0 || current is null or ExecutionContextFlowingSynchronizationContext)
        {
            return base.RunTest(ctxt);
        }

        SynchronizationContext.SetSynchronizationContext(new ExecutionContextFlowingSynchronizationContext(current));
        try
        {
            return base.RunTest(ctxt);
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(current);
        }
    }
}
