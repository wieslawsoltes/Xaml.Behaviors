using System;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using Avalonia.Threading;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// A trigger that executes its actions when the screen configuration changes.
/// </summary>
public class ScreensChangedTrigger : AttachedToVisualTreeTriggerBase<Visual>
{
    /// <summary>
    /// Identifies the <see cref="Screens"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Screens?> ScreensProperty =
        AvaloniaProperty.Register<ScreensChangedTrigger, Screens?>(nameof(Screens));

    /// <summary>
    /// Gets or sets the <see cref="Screens"/> instance that is observed. This is an avalonia property.
    /// If not set, the screens of the associated <see cref="TopLevel"/> will be used.
    /// </summary>
    [ResolveByName]
    public Screens? Screens
    {
        get => GetValue(ScreensProperty);
        set => SetValue(ScreensProperty, value);
    }

    private Screens? _subscribed;

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
            Subscribe(change.GetNewValue<Screens?>());
        }
    }

    private void Subscribe(Screens? screens)
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

    private Screens? GetScreens()
    {
        return Screens ?? (AssociatedObject is { } v ? TopLevel.GetTopLevel(v)?.Screens : null);
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
