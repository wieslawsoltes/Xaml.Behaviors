// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Windows.Foundation;
using Windows.UI.Input.Preview.Injection;
using Xaml.Behaviors.Uno.Headless;

namespace Xaml.Interactions.Draggable.UnitTests;

/// <summary>
/// Injects mouse input in window coordinates and waits for the UI thread to process it.
/// </summary>
/// <remarks>
/// Uno Platform applies mouse moves as deltas from the injector's current position (starting at 0,0), whatever the
/// <see cref="InjectedInputMouseOptions.Absolute"/> flag says, so the helper tracks the position.
/// </remarks>
internal sealed class MouseInput(InputInjector injector)
{
    private int _x;
    private int _y;

    private static UnoHeadlessSession Session => UnoHeadlessSession.Current;

    public static MouseInput? TryCreate()
    {
        var injector = InputInjector.TryCreate();
        if (injector is null)
        {
            return null;
        }

        injector.InitializeTouchInjection(InjectedInputVisualizationMode.None);
        return new MouseInput(injector);
    }

    public static Point Center(FrameworkElement element)
        => element.TransformToVisual(null).TransformPoint(new Point(element.ActualWidth / 2, element.ActualHeight / 2));

    public Task MoveAsync(Point position)
    {
        var x = (int)System.Math.Round(position.X);
        var y = (int)System.Math.Round(position.Y);
        injector.InjectMouseInput([new InjectedInputMouseInfo
        {
            DeltaX = x - _x,
            DeltaY = y - _y,
            MouseOptions = InjectedInputMouseOptions.Move,
            TimeOffsetInMilliseconds = 1,
        }]);
        _x = x;
        _y = y;
        return Session.WaitForIdleAsync();
    }

    public Task DownAsync()
    {
        injector.InjectMouseInput([new InjectedInputMouseInfo { MouseOptions = InjectedInputMouseOptions.LeftDown, TimeOffsetInMilliseconds = 1 }]);
        return Session.WaitForIdleAsync();
    }

    public Task UpAsync()
    {
        injector.InjectMouseInput([new InjectedInputMouseInfo { MouseOptions = InjectedInputMouseOptions.LeftUp, TimeOffsetInMilliseconds = 1 }]);
        return Session.WaitForIdleAsync();
    }

    /// <summary>Presses at <paramref name="from"/>, moves in <paramref name="steps"/> steps to <paramref name="to"/>.</summary>
    public async Task PressAndMoveAsync(Point from, Point to, int steps = 4)
    {
        await MoveAsync(from);
        await DownAsync();
        for (var i = 1; i <= steps; i++)
        {
            await MoveAsync(new Point(from.X + (to.X - from.X) * i / steps, from.Y + (to.Y - from.Y) * i / steps));
        }
    }

    public async Task DragAsync(Point from, Point to, int steps = 4)
    {
        await PressAndMoveAsync(from, to, steps);
        await UpAsync();
    }
}
