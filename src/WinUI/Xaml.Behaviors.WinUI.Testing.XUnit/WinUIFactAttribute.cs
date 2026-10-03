// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Runtime.CompilerServices;
using Xunit;
using Xunit.v3;

namespace Xaml.Behaviors.WinUI.Testing.XUnit;

/// <summary>
/// Marks a test method that runs on the UI thread of the WinUI test session
/// (<see cref="WinUITestSession"/>), so WinUI objects can be created and used directly and awaited
/// expressions resume on the UI thread.
/// </summary>
/// <remarks>
/// The test class is constructed, initialized and disposed on the UI thread as well. The session is started with
/// <see cref="WinUITestSessionOptions.Default"/> unless it was started earlier, for example from an xUnit assembly
/// fixture that calls <see cref="WinUITestSession.StartAsync(WinUITestSessionOptions)"/>.
/// </remarks>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
[XunitTestCaseDiscoverer(typeof(WinUIFactDiscoverer))]
public sealed class WinUIFactAttribute : FactAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WinUIFactAttribute"/> class.
    /// </summary>
    /// <param name="sourceFilePath">The source file of the test (supplied by the compiler).</param>
    /// <param name="sourceLineNumber">The source line of the test (supplied by the compiler).</param>
    public WinUIFactAttribute([CallerFilePath] string? sourceFilePath = null, [CallerLineNumber] int sourceLineNumber = -1)
        : base(sourceFilePath, sourceLineNumber)
    {
    }
}
