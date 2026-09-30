// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Sdk;
using Xunit.v3;

namespace Xaml.Behaviors.Uno.Headless.XUnit.Internal;

/// <summary>
/// Runs the tests of an Uno headless test case, delegating each test to <see cref="UnoHeadlessTestRunner"/>.
/// </summary>
/// <remarks>Stateless, so a single instance is shared (the xUnit runner pattern).</remarks>
internal sealed class UnoHeadlessTestCaseRunner : XunitTestCaseRunnerBase<XunitTestCaseRunnerContext, IXunitTestCase, IXunitTest>
{
    private UnoHeadlessTestCaseRunner()
    {
    }

    /// <summary>Gets the shared runner instance.</summary>
    public static UnoHeadlessTestCaseRunner Instance { get; } = new();

    /// <summary>
    /// Creates the tests of <paramref name="testCase"/> (enumerating theory data when needed) and runs them.
    /// </summary>
    public async ValueTask<RunSummary> Run(
        IXunitTestCase testCase,
        ExplicitOption explicitOption,
        IMessageBus messageBus,
        object?[] constructorArguments,
        ExceptionAggregator aggregator,
        CancellationTokenSource cancellationTokenSource)
    {
        IReadOnlyCollection<IXunitTest> tests = await aggregator.RunAsync(testCase.CreateTests, Array.Empty<IXunitTest>());

        await using XunitTestCaseRunnerContext context = new(
            testCase,
            tests,
            messageBus,
            aggregator,
            cancellationTokenSource,
            testCase.TestCaseDisplayName,
            testCase.SkipReason,
            explicitOption,
            constructorArguments);

        await context.InitializeAsync();
        return await Run(context);
    }

    /// <inheritdoc />
    protected override ValueTask<RunSummary> RunTest(XunitTestCaseRunnerContext ctxt, IXunitTest test)
        => UnoHeadlessTestRunner.Instance.Run(
            test,
            ctxt.MessageBus,
            ctxt.ConstructorArguments,
            ctxt.ExplicitOption,
            ctxt.Aggregator.Clone(),
            ctxt.CancellationTokenSource,
            ctxt.BeforeAfterTestAttributes);
}
