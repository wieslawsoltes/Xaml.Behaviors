// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xaml.Interactivity;

namespace Xaml.Behaviors.SourceGenerators.UnitTests;

/// <summary>
/// View model whose members are turned into generated actions and triggers.
/// </summary>
public partial class CounterViewModel : INotifyPropertyChanged
{
    private int _count;

    public event PropertyChangedEventHandler? PropertyChanged;

    [GeneratePropertyTrigger]
    [GenerateTypedChangePropertyAction(UseDispatcher = true)]
    public int Count
    {
        get => _count;
        set
        {
            if (_count != value)
            {
                _count = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
            }
        }
    }

    public List<string> Log { get; } = [];

    [GenerateTypedAction]
    public void Increment(int step) => Count += step;

    [GenerateTypedAction(UseDispatcher = true)]
    public void Reset() => Count = 0;

    [GenerateTypedAction]
    public async Task LoadAsync(string item)
    {
        await Task.Yield();
        Log.Add(item);
    }
}

/// <summary>
/// Action recording the parameters it was executed with.
/// </summary>
public partial class RecordingAction : Xaml.Interactivity.Action
{
    public List<object?> Parameters { get; } = [];

    public override object? Execute(object? sender, object? parameter)
    {
        Parameters.Add(parameter);
        return true;
    }
}

/// <summary>
/// Generated invoke command action (user declared, generated properties).
/// </summary>
[GenerateTypedInvokeCommandAction]
public partial class RunCommandAction : StyledElementAction
{
    [ActionCommand]
    private ICommand? _command;

    [ActionParameter]
    private object? _parameter;
}

/// <summary>
/// Generated multi data trigger that fires when both values are positive.
/// </summary>
[GenerateTypedMultiDataTrigger]
public partial class BothPositiveTrigger : StyledElementTrigger
{
    [TriggerProperty]
    private int _first;

    [TriggerProperty]
    private int _second;

    private bool Evaluate() => _first > 0 && _second > 0;
}

/// <summary>
/// Command recording its executions.
/// </summary>
public sealed class TestCommand : ICommand
{
    public event EventHandler? CanExecuteChanged
    {
        add { }
        remove { }
    }

    public List<object?> Executed { get; } = [];

    public bool CanExecute(object? parameter) => true;

    public void Execute(object? parameter) => Executed.Add(parameter);
}
