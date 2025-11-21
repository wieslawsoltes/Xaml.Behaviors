using System;
using ReactiveUI;
using Xaml.Behaviors.SourceGenerators;
using SourceGeneratorSample.Models;

[assembly: GenerateTypedAction(typeof(ExternalLibraryClass), nameof(ExternalLibraryClass.ExternalMethod))]
[assembly: GenerateTypedTrigger(typeof(ExternalLibraryClass), nameof(ExternalLibraryClass.ExternalEvent))]
[assembly: GenerateTypedChangePropertyAction(typeof(ExternalLibraryClass), nameof(ExternalLibraryClass.ExternalProperty))]
[assembly: GenerateTypedChangePropertyAction(typeof(Avalonia.Controls.TextBlock), "Foreground")]
[assembly: GenerateTypedChangePropertyAction(typeof(Avalonia.Controls.TextBlock), "Text")]
[assembly: GenerateTypedTrigger(typeof(SourceGeneratorSample.ViewModels.MainViewModel), "ProcessingFinished")]
[assembly: GenerateTypedDataTrigger(typeof(double))]
[assembly: GenerateTypedDataTrigger(typeof(string))]

namespace SourceGeneratorSample.ViewModels
{
    public partial class MainViewModel : ReactiveObject
    {
        private string _statusText = "Ready";
        public string StatusText
        {
            get => _statusText;
            set => this.RaiseAndSetIfChanged(ref _statusText, value);
        }

        private double _value = 0.0;
        public double Value
        {
            get => _value;
            set => this.RaiseAndSetIfChanged(ref _value, value);
        }

        private int _count = 0;
        public int Count
        {
            get => _count;
            set => this.RaiseAndSetIfChanged(ref _count, value);
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => this.RaiseAndSetIfChanged(ref _isBusy, value);
        }

        public ExternalLibraryClass ExternalObject { get; } = new ExternalLibraryClass();

        [GenerateTypedAction]
        public void Submit()
        {
            StatusText = "Submitted!";
            Console.WriteLine("Submit called");
            ProcessingFinished?.Invoke(this, EventArgs.Empty);
        }

        [GenerateTypedAction]
        public void Reset(object? sender, object? parameter)
        {
             StatusText = "Reset";
             Value = 0;
        }

        [GenerateTypedAction]
        public void UpdateMessage(string message, int count)
        {
            StatusText = $"{message} (Count: {count})";
        }

        [GenerateTypedTrigger]
        public event EventHandler? ProcessingFinished;

        [GenerateTypedChangePropertyAction]
        public string Title { get; set; } = "Sample App";
        
        public void TriggerExternal()
        {
            ExternalObject.RaiseExternalEvent();
        }

        [GenerateTypedAction]
        public async System.Threading.Tasks.Task SubmitAsync()
        {
            StatusText = "Submitting Async...";
            await System.Threading.Tasks.Task.Delay(2000);
            StatusText = "Submitted Async!";
            ProcessingFinished?.Invoke(this, EventArgs.Empty);
        }

        [GenerateTypedAction]
        public async System.Threading.Tasks.ValueTask ResetAsync()
        {
             StatusText = "Resetting Async...";
             await System.Threading.Tasks.Task.Delay(1000);
             StatusText = "Reset Async!";
             Value = 0;
        }
    }
}
