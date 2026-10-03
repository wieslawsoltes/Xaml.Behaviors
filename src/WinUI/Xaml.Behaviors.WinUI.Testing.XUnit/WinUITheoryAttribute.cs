// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Runtime.CompilerServices;
using Xunit;
using Xunit.v3;

namespace Xaml.Behaviors.WinUI.Testing.XUnit;

/// <summary>
/// Marks a data-driven test method whose rows run on the UI thread of the WinUI test session
/// (<see cref="WinUITestSession"/>), so WinUI objects can be created and used directly and awaited
/// expressions resume on the UI thread.
/// </summary>
/// <remarks>
/// Theory data is enumerated by xUnit (not on the UI thread); only the test class and the test method run on it.
/// See <see cref="WinUIFactAttribute"/> for how the session is started.
/// </remarks>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
[XunitTestCaseDiscoverer(typeof(WinUITheoryDiscoverer))]
public sealed class WinUITheoryAttribute : TheoryAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WinUITheoryAttribute"/> class.
    /// </summary>
    /// <param name="sourceFilePath">The source file of the test (supplied by the compiler).</param>
    /// <param name="sourceLineNumber">The source line of the test (supplied by the compiler).</param>
    public WinUITheoryAttribute([CallerFilePath] string? sourceFilePath = null, [CallerLineNumber] int sourceLineNumber = -1)
        : base(sourceFilePath, sourceLineNumber)
    {
    }
}
