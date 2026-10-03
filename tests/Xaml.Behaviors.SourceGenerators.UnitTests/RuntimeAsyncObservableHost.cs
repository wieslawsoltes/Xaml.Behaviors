using System;
using System.Threading.Tasks;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Avalonia.Controls;
#endif
using Xaml.Behaviors.SourceGenerators;

#if UNO
[assembly: GenerateAsyncTrigger(typeof(Xaml.Behaviors.SourceGenerators.UnitTests.AssemblyAsyncHost), "BackgroundTask")]
#else
[assembly: GenerateAsyncTrigger(typeof(Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests.AssemblyAsyncHost), "BackgroundTask")]
#endif

#if UNO
namespace Xaml.Behaviors.SourceGenerators.UnitTests;
#else
namespace Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;
#endif

public class RuntimeAsyncObservableHost : Control
{
    [GenerateAsyncTrigger]
    public Task<int>? SuccessfulTask { get; set; }

    [GenerateAsyncTrigger]
    public Task? FaultedTask { get; set; }

    [GenerateAsyncTrigger]
    public Task? CanceledTask { get; set; }

    [GenerateAsyncTrigger(UseDispatcher = false)]
    public Task<int>? BackgroundTask { get; set; }

    [GenerateAsyncTrigger(FireOnAttach = false)]
    public Task<int>? DeferredTask { get; set; }

    [GenerateObservableTrigger]
    public IObservable<int>? IntStream { get; set; }

    [GenerateObservableTrigger(UseDispatcher = false)]
    public IObservable<string>? BackgroundStream { get; set; }

    [GenerateObservableTrigger(FireOnAttach = false)]
    public IObservable<int>? DeferredStream { get; set; }
}

public class AssemblyAsyncHost : Control
{
    public Task? BackgroundTask { get; set; }
}
