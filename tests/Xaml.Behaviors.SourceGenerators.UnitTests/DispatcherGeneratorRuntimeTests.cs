using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
using Xaml.Behaviors.SourceGenerators;
using Xunit;

[assembly: GenerateTypedChangePropertyAction(typeof(Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests.DispatcherHost), "Message", UseDispatcher = true)]

namespace Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;

public partial class DispatcherHost : Control
{
    public int Calls { get; private set; }
    public string? Message { get; set; }

    [GenerateTypedAction(UseDispatcher = true)]
    public void Increment() => Calls++;

    [GenerateTypedAction(UseDispatcher = true)]
    public async Task FaultingAsync()
    {
        await Task.Yield();
        throw new InvalidOperationException("boom");
    }

    [GenerateTypedAction(UseDispatcher = true)]
    public async Task CanceledAsync()
    {
        await Task.FromCanceled(new CancellationToken(true));
    }
}

[GenerateTypedInvokeCommandAction(UseDispatcher = true)]
public partial class DispatcherInvokeAction : StyledElementAction
{
    [ActionCommand] private ICommand? _command;
    [ActionParameter] private object? _parameter;
}

public sealed class TestRelayCommand : ICommand
{
    public bool Executed { get; private set; }
    public object? LastParameter { get; private set; }

#pragma warning disable CS0067
    public event EventHandler? CanExecuteChanged;
#pragma warning restore CS0067

    public bool CanExecute(object? parameter) => true;

    public void Execute(object? parameter)
    {
        Executed = true;
        LastParameter = parameter;
    }
}

public class DispatcherGeneratorRuntimeTests
{
    [AvaloniaFact]
    public async Task TypedAction_UseDispatcher_RunsOnDispatcher()
    {
        var host = new DispatcherHost();
        dynamic action = GeneratedTypeHelper.CreateInstance("IncrementAction", "Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests");

        action.TargetObject = host;
        action.Execute(host, null);

        await FlushDispatcherAsync();
        Assert.Equal(1, host.Calls);
    }

    [AvaloniaFact]
    public async Task ChangePropertyAction_UseDispatcher_SetsValue()
    {
        var host = new DispatcherHost();
        dynamic action = GeneratedTypeHelper.CreateInstance("DispatcherHostSetMessageAction", "Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests");

        action.Value = "Hello";
        action.Execute(host, null);

        await FlushDispatcherAsync();
        Assert.Equal("Hello", host.Message);
    }

    [AvaloniaFact]
    public async Task InvokeCommandAction_UseDispatcher_ExecutesCommand()
    {
        var host = new DispatcherHost();
        dynamic action = GeneratedTypeHelper.CreateInstance("DispatcherInvokeAction", "Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests");
        var command = new TestRelayCommand();

        action.Command = command;
        action.Parameter = "Param";

        action.Execute(host, null);

        await FlushDispatcherAsync();
        Assert.True(command.Executed);
        Assert.Equal("Param", command.LastParameter);
    }

    [AvaloniaFact]
    public async Task TypedAction_UseDispatcher_Faulted_Task_Sets_LastError()
    {
        var host = new DispatcherHost();
        dynamic action = GeneratedTypeHelper.CreateInstance("FaultingAsyncAction", "Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests");

        action.TargetObject = host;
        action.Execute(host, null);

        await WaitForExecutionToCompleteAsync(action);
        Assert.IsType<InvalidOperationException>(action.LastError);
    }

    [AvaloniaFact]
    public async Task TypedAction_UseDispatcher_Canceled_Task_Sets_LastError()
    {
        var host = new DispatcherHost();
        dynamic action = GeneratedTypeHelper.CreateInstance("CanceledAsyncAction", "Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests");

        action.TargetObject = host;
        action.Execute(host, null);

        await WaitForExecutionToCompleteAsync(action);
        Assert.IsType<OperationCanceledException>(action.LastError);
    }

    private static async Task FlushDispatcherAsync()
    {
        await Dispatcher.UIThread.InvokeAsync(() => { });
    }

    private static async Task WaitForExecutionToCompleteAsync(dynamic action)
    {
        for (var i = 0; i < 80; i++)
        {
            await FlushDispatcherAsync();
            await Task.Delay(5);
            if (!(bool)action.IsExecuting && action.LastError != null)
            {
                break;
            }
        }
        await FlushDispatcherAsync();
    }
}
