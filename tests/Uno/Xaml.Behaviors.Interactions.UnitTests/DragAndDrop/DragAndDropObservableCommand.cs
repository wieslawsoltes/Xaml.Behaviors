// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Windows.Input;

namespace Xaml.Interactions.UnitTests.DragAndDrop;

/// <summary>
/// The <c>ObservableCommand</c> of the shared <c>Core/ObservableCommand.cs</c> used by
/// <c>DragDropCommandsBehaviorTests</c>, while that file is not shared yet (see SharedTests.props, "Core" block).
/// </summary>
/// <remarks>
/// Declared in the namespace of the drag and drop tests, it takes precedence over the Core one imported with
/// <c>using Xaml.Interactions.UnitTests.Core;</c>, so both can coexist; remove it once the Core file is shared.
/// </remarks>
internal sealed class ObservableCommand : ICommand
{
    private EventHandler? _canExecuteChanged;

    public bool CanExecuteResult { get; set; } = true;

    public Func<object?, bool>? CanExecuteCallback { get; set; }

    public int SubscriptionCount { get; private set; }

    public object? LastCanExecuteParameter { get; private set; }

    public int CanExecuteCallCount { get; private set; }

    public event EventHandler? CanExecuteChanged
    {
        add
        {
            _canExecuteChanged += value;
            SubscriptionCount++;
        }
        remove
        {
            _canExecuteChanged -= value;
            SubscriptionCount--;
        }
    }

    public bool CanExecute(object? parameter)
    {
        CanExecuteCallCount++;
        LastCanExecuteParameter = parameter;
        return CanExecuteCallback?.Invoke(parameter) ?? CanExecuteResult;
    }

    public void Execute(object? parameter)
    {
    }

    public void RaiseCanExecuteChanged() => _canExecuteChanged?.Invoke(this, EventArgs.Empty);
}
