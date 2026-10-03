// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Xaml.Behaviors.WinUI.Testing.XUnit.Internal;
using Xunit;
using Xunit.Internal;
using Xunit.Sdk;
using Xunit.v3;

namespace Xaml.Behaviors.WinUI.Testing.XUnit;

/// <summary>
/// Discovers <see cref="WinUITheoryAttribute"/> tests. Used by xUnit; not meant to be called directly.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class WinUITheoryDiscoverer : TheoryDiscoverer
{
    /// <inheritdoc />
    protected override ValueTask<IReadOnlyCollection<IXunitTestCase>> CreateTestCasesForDataRow(ITestFrameworkDiscoveryOptions discoveryOptions, IXunitTestMethod testMethod, ITheoryAttribute theoryAttribute, ITheoryDataRow dataRow, object?[] testMethodArguments)
    {
        var details = TestIntrospectionHelper.GetTestCaseDetailsForTheoryDataRow(discoveryOptions, testMethod, theoryAttribute, dataRow, testMethodArguments);
        Dictionary<string, HashSet<string>> traits = TestIntrospectionHelper.GetTraits(testMethod, dataRow);

        IXunitTestCase testCase = new WinUITestCase(
            details.ResolvedTestMethod,
            details.TestCaseDisplayName,
            details.UniqueID,
            details.Explicit,
            details.SkipExceptions,
            details.SkipReason,
            details.SkipType,
            details.SkipUnless,
            details.SkipWhen,
            traits,
            testMethodArguments,
            details.SourceFilePath,
            details.SourceLineNumber,
            details.Timeout);

        return new ValueTask<IReadOnlyCollection<IXunitTestCase>>([testCase]);
    }

    /// <inheritdoc />
    protected override ValueTask<IReadOnlyCollection<IXunitTestCase>> CreateTestCasesForTheory(ITestFrameworkDiscoveryOptions discoveryOptions, IXunitTestMethod testMethod, ITheoryAttribute theoryAttribute)
    {
        var details = TestIntrospectionHelper.GetTestCaseDetails(discoveryOptions, testMethod, theoryAttribute);
        Dictionary<string, HashSet<string>> traits = testMethod.Traits.ToReadWrite(StringComparer.OrdinalIgnoreCase);

        // Mirrors TheoryDiscoverer: a statically skipped theory runs as a single skipped test case; otherwise the
        // data is enumerated when the test case runs.
        bool enumerateAtRuntime = details.SkipReason is null || details.SkipUnless is not null || details.SkipWhen is not null;

        IXunitTestCase testCase = enumerateAtRuntime
            ? new WinUIDelayEnumeratedTheoryTestCase(
                details.ResolvedTestMethod,
                details.TestCaseDisplayName,
                details.UniqueID,
                details.Explicit,
                theoryAttribute.SkipTestWithoutData,
                details.SkipExceptions,
                details.SkipReason,
                details.SkipType,
                details.SkipUnless,
                details.SkipWhen,
                traits,
                details.SourceFilePath,
                details.SourceLineNumber,
                details.Timeout)
            : new WinUITestCase(
                details.ResolvedTestMethod,
                details.TestCaseDisplayName,
                details.UniqueID,
                details.Explicit,
                details.SkipExceptions,
                details.SkipReason,
                details.SkipType,
                details.SkipUnless,
                details.SkipWhen,
                traits,
                testMethodArguments: null,
                details.SourceFilePath,
                details.SourceLineNumber,
                details.Timeout);

        return new ValueTask<IReadOnlyCollection<IXunitTestCase>>([testCase]);
    }
}
