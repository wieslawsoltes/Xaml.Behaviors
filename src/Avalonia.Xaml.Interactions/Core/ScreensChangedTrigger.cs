using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using Avalonia.Xaml.Interactivity;
using Avalonia.Threading;

namespace Avalonia.Xaml.Interactions.Core;

/// <summary>
/// A trigger that executes its actions when the screen configuration changes.
/// </summary>
public class ScreensChangedTrigger : AttachedToVisualTreeTriggerBase<Visual>
{
    /// <summary>
    /// Identifies the <see cref="Screens"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<IScreens?> ScreensProperty =
        AvaloniaProperty.Register<ScreensChangedTrigger, IScreens?>(nameof(Screens));

    /// <summary>
    /// Gets or sets the <see cref="IScreens"/> instance that is observed. This is an avalonia property.
    /// If not set, the screens of the associated <see cref="TopLevel"/> will be used.
    /// </summary>
    [ResolveByName]
    public IScreens? Screens
    {
        get => GetValue(ScreensProperty);
        set => SetValue(ScreensProperty, value);
    }

    private IScreens? _subscribed;

    /// <inheritdoc />
    protected override IDisposable OnAttachedToVisualTreeOverride()
    {
        Subscribe(GetScreens());
        return DisposableAction.Create(() => Subscribe(null));
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ScreensProperty && AssociatedObject is not null)
        {
            Subscribe(change.GetNewValue<IScreens?>());
        }
    }

    private void Subscribe(IScreens? screens)
    {
        if (_subscribed == screens)
        {
            return;
        }

        if (_subscribed is not null)
        {
            _subscribed.Changed -= OnScreensChanged;
        }

        _subscribed = screens;

        if (_subscribed is not null)
        {
            _subscribed.Changed += OnScreensChanged;
        }
    }

    private IScreens? GetScreens()
    {
        return Screens ?? (AssociatedObject is Visual v ? TopLevel.GetTopLevel(v)?.Screens : null);
    }

    private void OnScreensChanged(object? sender, EventArgs e)
    {
        if (!IsEnabled)
        {
            return;
        }

        Dispatcher.UIThread.Post(() => Interaction.ExecuteActions(AssociatedObject, Actions, e));
    }
}
