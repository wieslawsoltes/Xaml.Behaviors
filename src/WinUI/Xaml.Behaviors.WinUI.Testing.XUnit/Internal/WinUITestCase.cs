// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Sdk;
using Xunit.v3;

namespace Xaml.Behaviors.WinUI.Testing.XUnit.Internal;

/// <summary>
/// A fact (or a single theory data row) whose tests run on the UI thread of the WinUI test session.
/// </summary>
internal sealed class WinUITestCase : XunitTestCase, ISelfExecutingXunitTestCase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WinUITestCase"/> class.
    /// </summary>
    public WinUITestCase(
        IXunitTestMethod testMethod,
        string testCaseDisplayName,
        string uniqueID,
        bool @explicit,
        Type[]? skipExceptions = null,
        string? skipReason = null,
        Type? skipType = null,
        string? skipUnless = null,
        string? skipWhen = null,
        Dictionary<string, HashSet<string>>? traits = null,
        object?[]? testMethodArguments = null,
        string? sourceFilePath = null,
        int? sourceLineNumber = null,
        int? timeout = null)
        : base(testMethod, testCaseDisplayName, uniqueID, @explicit, skipExceptions, skipReason, skipType, skipUnless, skipWhen, traits, testMethodArguments, sourceFilePath, sourceLineNumber, timeout)
    {
    }

    /// <summary>
    /// Called by the xUnit de-serializer.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
    public WinUITestCase()
    {
    }

    /// <inheritdoc />
    public ValueTask<RunSummary> Run(ExplicitOption explicitOption, IMessageBus messageBus, object?[] constructorArguments, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
        => WinUITestCaseRunner.Instance.Run(this, explicitOption, messageBus, constructorArguments, aggregator, cancellationTokenSource);
}
