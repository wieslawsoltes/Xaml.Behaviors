using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using Avalonia.Threading;

namespace Avalonia.Xaml.Interactions.Core;

/// <summary>
/// An action that requests extended screen information from <see cref="IScreens"/>.
/// </summary>
public class RequestScreenDetailsAction : Avalonia.Xaml.Interactivity.StyledElementAction
{
    /// <summary>
    /// Identifies the <see cref="Screens"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<IScreens?> ScreensProperty =
        AvaloniaProperty.Register<RequestScreenDetailsAction, IScreens?>(nameof(Screens));

    /// <summary>
    /// Gets or sets the <see cref="IScreens"/> instance used to request screen details. This is an avalonia property.
    /// If not set, the screens of the associated <see cref="TopLevel"/> will be used.
    /// </summary>
    [ResolveByName]
    public IScreens? Screens
    {
        get => GetValue(ScreensProperty);
        set => SetValue(ScreensProperty, value);
    }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (sender is not Visual visual)
        {
            return false;
        }

        Dispatcher.UIThread.InvokeAsync(async () => await RequestAsync(visual));

        return true;
    }

    private async Task RequestAsync(Visual visual)
    {
        if (IsEnabled != true)
        {
            return;
        }

        try
        {
            var screens = Screens ?? TopLevel.GetTopLevel(visual)?.Screens;
            if (screens is not null)
            {
                await screens.RequestScreenDetails();
            }
        }
        catch (Exception)
        {
            // ignored
        }
    }
}
