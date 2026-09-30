// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Xaml.Behaviors.Uno.Headless;
using Xaml.Behaviors.Uno.Headless.XUnit;
using Xunit;

namespace Xaml.Interactivity.UnitTests;

/// <summary>
/// Verifies the headless Uno test infrastructure this project's behavior tests run on.
/// </summary>
public class HeadlessSmokeTests
{
    [UnoHeadlessFact]
    public async Task Element_Loads_In_Headless_Window()
    {
        UnoHeadlessSession session = UnoHeadlessSession.Current;
        Border border = new();

        await session.ShowAsync(border);

        Assert.True(session.HasThreadAccess);
        Assert.True(border.IsLoaded);
    }
}
