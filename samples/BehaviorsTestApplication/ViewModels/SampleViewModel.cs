using System;
using System.Windows.Input;
using ReactiveUI;

namespace BehaviorsTestApplication.ViewModels;

public partial class SampleViewModel : ViewModelBase
{
    public SampleViewModel()
    {
        LoadedCommand = ReactiveCommand.Create(Loaded);
        UnloadedCommand = ReactiveCommand.Create(Unloaded);
    }

    public ICommand LoadedCommand { get;  }
    
    public ICommand UnloadedCommand { get; }

    
    private void Loaded()
    {
        Console.WriteLine("Loaded");
    }

    private void Unloaded()
    {
        Console.WriteLine("Unloaded");
    }
}
