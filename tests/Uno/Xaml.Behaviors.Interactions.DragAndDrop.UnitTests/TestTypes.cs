// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Windows.ApplicationModel.DataTransfer;

namespace Xaml.Interactions.DragAndDrop.UnitTests;

public sealed class RecordingDropHandler : DropHandlerBase
{
    public List<string> Calls { get; } = [];

    public object? SourceContext { get; private set; }

    public object? TargetContext { get; private set; }

    public DataPackageOperation DropEffects { get; private set; }

    public bool IsValid { get; set; } = true;

    public override void Enter(object? sender, DragEventArgs e, object? sourceContext, object? targetContext)
    {
        Calls.Add("Enter");
        base.Enter(sender, e, sourceContext, targetContext);
    }

    public override void Over(object? sender, DragEventArgs e, object? sourceContext, object? targetContext)
    {
        if (Calls.Count == 0 || Calls[^1] != "Over")
        {
            Calls.Add("Over");
        }

        base.Over(sender, e, sourceContext, targetContext);
    }

    public override void Drop(object? sender, DragEventArgs e, object? sourceContext, object? targetContext)
    {
        Calls.Add("Drop");
        SourceContext = sourceContext;
        TargetContext = targetContext;
        base.Drop(sender, e, sourceContext, targetContext);
    }

    public override void Leave(object? sender, RoutedEventArgs e)
    {
        Calls.Add("Leave");
        base.Leave(sender, e);
    }

    public override bool Validate(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
        => IsValid;

    public override bool Execute(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
    {
        DropEffects = e.AllowedOperations;
        return IsValid;
    }
}

public sealed class RecordingDragHandler : IDragHandler
{
    public List<(string Name, object? Context)> Calls { get; } = [];

    public void BeforeDragDrop(object? sender, PointerRoutedEventArgs e, object? context) => Calls.Add(("Before", context));

    public void AfterDragDrop(object? sender, PointerRoutedEventArgs e, object? context) => Calls.Add(("After", context));
}

public sealed class RecordingCommand(Func<object?, bool>? canExecute = null) : ICommand
{
    public List<object?> Parameters { get; } = [];

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => canExecute?.Invoke(parameter) ?? true;

    public void Execute(object? parameter) => Parameters.Add(parameter);

    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}

public sealed class ExposedDropHandler : DropHandlerBase
{
    public void Move<T>(IList<T> items, int source, int target) => MoveItem(items, source, target);

    public void Move<T>(IList<T> sourceItems, IList<T> targetItems, int source, int target) => MoveItem(sourceItems, targetItems, source, target);

    public void Swap<T>(IList<T> items, int source, int target) => SwapItem(items, source, target);

    public void Swap<T>(IList<T> sourceItems, IList<T> targetItems, int source, int target) => SwapItem(sourceItems, targetItems, source, target);

    public void Insert<T>(IList<T> items, T item, int index) => InsertItem(items, item, index);
}
