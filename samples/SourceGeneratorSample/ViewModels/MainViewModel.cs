using System;
using ReactiveUI;
using System.Windows.Input;
using System.Reactive;
using Xaml.Behaviors.SourceGenerators;
using SourceGeneratorSample.Models;

[assembly: GenerateTypedChangePropertyAction(typeof(Avalonia.Controls.TextBlock), "Foreground")]
[assembly: GenerateTypedChangePropertyAction(typeof(Avalonia.Controls.TextBlock), "Text")]
[assembly: GenerateTypedTrigger(typeof(Avalonia.Controls.Button), "Click")]
[assembly: GenerateTypedTrigger(typeof(SourceGeneratorSample.ViewModels.MainViewModel), "ProcessingFinished")]
[assembly: GenerateTypedDataTrigger(typeof(double))]
[assembly: GenerateTypedDataTrigger(typeof(string))]

// Wildcard examples, you can also use regex patterns.
[assembly: GenerateTypedChangePropertyAction(typeof(Avalonia.Input.InputElement), "*")]
[assembly: GenerateTypedTrigger(typeof(Avalonia.Input.InputElement), "*")]

namespace SourceGeneratorSample.ViewModels
{
    public partial class MainViewModel : ReactiveObject
    {
        public MainViewModel()
        {
            TriggerExternalCommand = ReactiveCommand.Create<string?>(_ => TriggerExternal());
        }

        private string _statusText = "Ready";
        public string StatusText
        {
            get => _statusText;
            set => this.RaiseAndSetIfChanged(ref _statusText, value);
        }

        private string _externalTriggerStatus = "Waiting...";
        public string ExternalTriggerStatus
        {
            get => _externalTriggerStatus;
            set => this.RaiseAndSetIfChanged(ref _externalTriggerStatus, value);
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
        public ICommand TriggerExternalCommand { get; }

        [GenerateTypedAction]
        public void Submit()
        {
            StatusText = "Submitted!";
            InternalSubmitMessage = "Submitted successfully!";
            Console.WriteLine("Submit called");
            ProcessingFinished?.Invoke(this, EventArgs.Empty);
        }

        [GenerateTypedAction]
        public void Reset(object? sender, object? parameter)
        {
             StatusText = "Reset";
             InternalSubmitMessage = "Reset";
             Value = 0;
        }

        [GenerateTypedAction]
        public void UpdateMessage(string message, int count)
        {
            StatusText = $"{message} (Count: {count})";
        }

        [GenerateTypedTrigger]
        public event EventHandler? ProcessingFinished;

        private string _title = "Sample App";
        [GenerateTypedChangePropertyAction]
        public string Title
        {
            get => _title;
            set => this.RaiseAndSetIfChanged(ref _title, value);
        }

        private string _externalEventMessage = "Waiting for event...";
        [GenerateTypedChangePropertyAction]
        public string ExternalEventMessage
        {
            get => _externalEventMessage;
            set => this.RaiseAndSetIfChanged(ref _externalEventMessage, value);
        }

        private string _processingMessage = "Waiting for process...";
        [GenerateTypedChangePropertyAction]
        public string ProcessingMessage
        {
            get => _processingMessage;
            set => this.RaiseAndSetIfChanged(ref _processingMessage, value);
        }

        private string _internalSubmitMessage = "Waiting for submit...";
        public string InternalSubmitMessage
        {
            get => _internalSubmitMessage;
            set => this.RaiseAndSetIfChanged(ref _internalSubmitMessage, value);
        }
        
        public void TriggerExternal()
        {
            ExternalObject.RaiseExternalEvent();
            ExternalTriggerStatus = $"External triggered at {DateTime.Now:T}";
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
