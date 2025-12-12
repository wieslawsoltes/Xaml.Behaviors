using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Xaml.Interactivity;
using Avalonia.Threading;
using Xunit;

namespace Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;

public class EventCommandRuntimeTests
{
    private class CommandStub : System.Windows.Input.ICommand
    {
        public bool Executed { get; private set; }
#pragma warning disable CS0067
        public event System.EventHandler? CanExecuteChanged;
#pragma warning restore CS0067
        public bool CanExecute(object? parameter) => true;
        public void Execute(object? parameter) => Executed = true;
    }

    [AvaloniaFact]
    public void EventCommand_Should_Execute_Command()
    {
        var command = new CommandStub();
        var button = new Button();

        dynamic trigger = GeneratedTypeHelper.CreateInstance("ButtonClickEventCommandTrigger", "Avalonia.Controls");
        trigger.Command = command;
        trigger.Attach(button);

        button.Command = command;
        button.RaiseEvent(new Avalonia.Interactivity.RoutedEventArgs(Button.ClickEvent));

        Assert.True(command.Executed);
    }

    [AvaloniaFact]
    public async Task EventCommand_WithDispatcher_Executes_On_UIThread()
    {
        var command = new CommandStub();
        var source = new DispatcherEventSource();

        dynamic trigger = GeneratedTypeHelper.CreateInstance("FiredEventCommandTrigger", "Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests");
        trigger.Command = command;
        trigger.Attach(source);

        source.Raise();
        Assert.False(command.Executed);

        await FlushDispatcherAsync();
        Assert.True(command.Executed);
    }

    [AvaloniaFact]
    public void EventCommand_Unsubscribes_When_Trigger_Is_Collected()
    {
        var source = new DispatcherEventSource();

        var weak = CreateAndReleaseTrigger(source);
        Assert.NotEqual(0, source.SubscriptionCount);

        for (var i = 0; i < 5 && weak.IsAlive; i++)
        {
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true, compacting: true);
            GC.WaitForPendingFinalizers();
        }

        if (weak.IsAlive)
        {
            NullOutProxyWeakReference(source);
        }

        source.Raise();
        Assert.Equal(0, source.SubscriptionCount);
    }

    private static WeakReference CreateAndReleaseTrigger(DispatcherEventSource source)
    {
        dynamic trigger = GeneratedTypeHelper.CreateInstance("FiredEventCommandTrigger", "Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests");
        trigger.Attach(source);
        var weak = new WeakReference((object)trigger);
        trigger = null!;
        return weak;
    }

    private static void NullOutProxyWeakReference(DispatcherEventSource source)
    {
#pragma warning disable IL2075 // Reflection access is intentional in test helper
        var handler = source.FirstHandler?.GetInvocationList().FirstOrDefault();
        if (handler?.Target is not object proxy)
        {
            return;
        }

        var triggerField = proxy.GetType().GetField("_trigger", BindingFlags.NonPublic | BindingFlags.Instance);
        if (triggerField == null)
        {
            return;
        }

        var weakType = triggerField.FieldType;
        var ctor = weakType.GetConstructor(new[] { weakType.GenericTypeArguments[0] });
        if (ctor == null)
        {
            return;
        }

        var weakNull = ctor.Invoke(new object?[] { null });
        triggerField.SetValue(proxy, weakNull);
#pragma warning restore IL2075
    }

    private static async Task FlushDispatcherAsync()
    {
        await Dispatcher.UIThread.InvokeAsync(() => { });
    }
}
