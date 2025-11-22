using System;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using Xaml.Behaviors.SourceGenerators;
using Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;

[assembly: GenerateTypedAction(typeof(TestControl), "TestMethod")]
[assembly: GenerateTypedAction(typeof(TestControl), "TestMethodWithParameter")]
[assembly: GenerateTypedTrigger(typeof(TestControl), "TestEvent")]
[assembly: GenerateTypedChangePropertyAction(typeof(TestControl), "Tag")]
[assembly: GenerateTypedDataTrigger(typeof(string))]

namespace Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;

[GenerateTypedMultiDataTrigger]
public partial class TypedMultiDataTrigger : StyledElementTrigger
{
    [TriggerProperty]
    public string? _value1;

    [TriggerProperty]
    public string? _value2;

    public bool Evaluate()
    {
        return _value1 == "A" && _value2 == "B";
    }
}

[GenerateTypedInvokeCommandAction]
public partial class TypedInvokeCommandAction : StyledElementAction
{
    [ActionCommand]
    public System.Windows.Input.ICommand? _command;

    [ActionParameter]
    public object? _commandParameter;
}
