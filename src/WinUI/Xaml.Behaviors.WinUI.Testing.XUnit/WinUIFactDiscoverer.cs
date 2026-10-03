// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.ComponentModel;
using Xaml.Behaviors.WinUI.Testing.XUnit.Internal;
using Xunit.Internal;
using Xunit.Sdk;
using Xunit.v3;

namespace Xaml.Behaviors.WinUI.Testing.XUnit;

/// <summary>
/// Discovers <see cref="WinUIFactAttribute"/> tests. Used by xUnit; not meant to be called directly.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class WinUIFactDiscoverer : FactDiscoverer
{
    /// <inheritdoc />
    protected override IXunitTestCase CreateTestCase(ITestFrameworkDiscoveryOptions discoveryOptions, IXunitTestMethod testMethod, IFactAttribute factAttribute)
    {
        var details = TestIntrospectionHelper.GetTestCaseDetails(discoveryOptions, testMethod, factAttribute);

        return new WinUITestCase(
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
