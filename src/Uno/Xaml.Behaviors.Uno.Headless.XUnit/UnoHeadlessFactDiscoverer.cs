// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.ComponentModel;
using Xaml.Behaviors.Uno.Headless.XUnit.Internal;
using Xunit.Internal;
using Xunit.Sdk;
using Xunit.v3;

namespace Xaml.Behaviors.Uno.Headless.XUnit;

/// <summary>
/// Discovers <see cref="UnoHeadlessFactAttribute"/> tests. Used by xUnit; not meant to be called directly.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class UnoHeadlessFactDiscoverer : FactDiscoverer
{
    /// <inheritdoc />
    protected override IXunitTestCase CreateTestCase(ITestFrameworkDiscoveryOptions discoveryOptions, IXunitTestMethod testMethod, IFactAttribute factAttribute)
    {
        var details = TestIntrospectionHelper.GetTestCaseDetails(discoveryOptions, testMethod, factAttribute);

        return new UnoHeadlessTestCase(
            details.ResolvedTestMethod,
            details.TestCaseDisplayName,
            details.UniqueID,
            details.Explicit,
            details.SkipExceptions,
            details.SkipReason,
            details.SkipType,
            details.SkipUnless,
            details.SkipWhen,
            testMethod.Traits.ToReadWrite(StringComparer.OrdinalIgnoreCase),
            testMethodArguments: null,
            details.SourceFilePath,
            details.SourceLineNumber,
            details.Timeout);
    }
}
