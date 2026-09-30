// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.PropertyGenerator;

namespace Xaml.Interactivity.UnitTests;

public partial class RecordingBehavior : Behavior
{
    public List<string> Events { get; } = [];

    [StyledProperty]
    public partial string? Text { get; set; }

    protected override void OnAttached() => Events.Add("Attached");

    protected override void OnDetaching() => Events.Add("Detaching");

    protected override void OnInitializedEvent() => Events.Add("Initialized");

    protected override void OnAttachedToLogicalTree() => Events.Add("AttachedToLogicalTree");

    protected override void OnAttachedToVisualTree() => Events.Add("AttachedToVisualTree");

    protected override void OnLoaded() => Events.Add("Loaded");

    protected override void OnUnloaded() => Events.Add("Unloaded");

    protected override void OnDetachedFromVisualTree() => Events.Add("DetachedFromVisualTree");

    protected override void OnDetachedFromLogicalTree() => Events.Add("DetachedFromLogicalTree");

    protected override void OnDataContextChangedEvent() => Events.Add("DataContextChanged");

    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        Events.Add("PropertyChanged:" + (change.Property == TextProperty ? nameof(Text) : change.Property == IsEnabledProperty ? nameof(IsEnabled) : "?"));
    }
}

public partial class ButtonOnlyBehavior : Behavior<Button>
{
}

public partial class RecordingAction : Action
{
    public List<(object? Sender, object? Parameter)> Calls { get; } = [];

    public override object? Execute(object? sender, object? parameter)
    {
        Calls.Add((sender, parameter));
        return "executed";
    }
}

public partial class ManualTrigger : Trigger
{
    public IEnumerable<object> Fire(object? parameter) => Interaction.ExecuteActions(AssociatedObject, Actions, parameter);
}

public partial class TestInvokeCommandAction : InvokeCommandActionBase
{
    public override object? Execute(object? sender, object? parameter)
    {
        if (Command?.CanExecute(ResolveParameter(parameter)) == true)
        {
            Command.Execute(ResolveParameter(parameter));
            return true;
        }

        return false;
    }
}

public partial class ClickTrigger : EventTriggerBase
{
}

public sealed class TestCommand(Func<object?, bool>? canExecute = null) : ICommand
{
    private bool _enabled = true;

    public List<object?> Executed { get; } = [];

    public event EventHandler? CanExecuteChanged;

    public bool Enabled
    {
        get => _enabled;
        set
        {
            _enabled = value;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public bool CanExecute(object? parameter) => _enabled && (canExecute?.Invoke(parameter) ?? true);

    public void Execute(object? parameter) => Executed.Add(parameter);
}

public sealed class TestViewModel
{
    public string Name { get; set; } = "vm";
}
