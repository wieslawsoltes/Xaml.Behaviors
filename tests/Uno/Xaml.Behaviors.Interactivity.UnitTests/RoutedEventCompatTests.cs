// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.UI.Input.Preview.Injection;
using Xaml.Behaviors.Uno.Headless;
using Xaml.Behaviors.Uno.Headless.XUnit;
using Xunit;

namespace Xaml.Interactivity.UnitTests;

public class RoutedEventCompatTests
{
    private static UnoHeadlessSession Session => UnoHeadlessSession.Current;

    [UnoHeadlessFact]
    public async Task Plain_RoutedEventArgs_Handler_Receives_Pointer_Events()
    {
        var injector = InputInjector.TryCreate();
        Assert.SkipWhen(injector is null, "Input injection is not available.");

        // Fills the window, so the press hits it wherever the pointer currently is.
        var border = new Border { Background = new SolidColorBrush(Microsoft.UI.Colors.Red) };
        var received = 0;
        EventHandler<RoutedEventArgs> handler = (_, _) => received++;
        border.AddHandler(UIElement.PointerPressedEvent, handler, RoutingStrategies.Bubble);
        await Session.ShowAsync(border);

        injector!.InjectMouseInput([new InjectedInputMouseInfo { MouseOptions = InjectedInputMouseOptions.LeftDown }]);
        injector.InjectMouseInput([new InjectedInputMouseInfo { MouseOptions = InjectedInputMouseOptions.LeftUp }]);
        await Session.WaitForIdleAsync();

        Assert.Equal(1, received);

        border.RemoveRoutedEventHandler(UIElement.PointerPressedEvent, handler);
        injector.InjectMouseInput([new InjectedInputMouseInfo { MouseOptions = InjectedInputMouseOptions.LeftDown }]);
        injector.InjectMouseInput([new InjectedInputMouseInfo { MouseOptions = InjectedInputMouseOptions.LeftUp }]);
        await Session.WaitForIdleAsync();

        Assert.Equal(1, received);
    }
}
