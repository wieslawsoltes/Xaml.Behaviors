// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Runtime.CompilerServices;
using Xunit;
using Xunit.v3;

namespace Xaml.Behaviors.Uno.Headless.XUnit;

/// <summary>
/// Marks a test method that runs on the UI thread of the headless Uno session
/// (<see cref="UnoHeadlessSession"/>), so Uno UI objects can be created and used directly and awaited
/// expressions resume on the UI thread.
/// </summary>
/// <remarks>
/// The test class is constructed, initialized and disposed on the UI thread as well. The session is started with
/// <see cref="UnoHeadlessSessionOptions.Default"/> unless it was started earlier, for example from an xUnit assembly
/// fixture that calls <see cref="UnoHeadlessSession.StartAsync(UnoHeadlessSessionOptions)"/>.
/// </remarks>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
[XunitTestCaseDiscoverer(typeof(UnoHeadlessFactDiscoverer))]
public sealed class UnoHeadlessFactAttribute : FactAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnoHeadlessFactAttribute"/> class.
    /// </summary>
    /// <param name="sourceFilePath">The source file of the test (supplied by the compiler).</param>
    /// <param name="sourceLineNumber">The source line of the test (supplied by the compiler).</param>
    public UnoHeadlessFactAttribute([CallerFilePath] string? sourceFilePath = null, [CallerLineNumber] int sourceLineNumber = -1)
        : base(sourceFilePath, sourceLineNumber)
    {
    }
}
