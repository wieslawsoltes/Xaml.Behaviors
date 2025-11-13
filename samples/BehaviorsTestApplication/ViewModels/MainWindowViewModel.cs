using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;
using System.Net.Http;
using System.Reactive.Subjects;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using Avalonia.Xaml.Interactions.Custom;
using ReactiveUI;

namespace BehaviorsTestApplication.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private int _value;
    private int _containerSampleCounter = 1;
    private readonly Subject<object> _scrollToItemSubject;
    private readonly Subject<int> _scrollToIndexSubject;

    public MainWindowViewModel()
    {
        Count = 0;
        Position = 100.0;

        Items =
        [
            new("First Item", "Red")
            {
                Items =
                [
                    new("First Item Sub Item 1"), new("First Item Sub Item 2"), new("First Item Sub Item 3")
                ]
            },
            new("Second Item", "Green")
            {
                Items =
                [
                    new("Second Item Sub Item 1"), new("Second Item Sub Item 2"), new("Second Item Sub Item 3")
                ]
            },
            new("Third Item", "Blue")
            {
                Items =
                [
                    new("Third Item Sub Item 1"), new("Third Item Sub Item 2"), new("Third Item Sub Item 3")
                ]
            },
            new("Fourth Item", "Orange")
            {
                Items =
                [
                    new("Fourth Item Sub Item 1"), new("Fourth Item Sub Item 2"), new("Fourth Item Sub Item 3")
                ]
            },
            new("Fifth Item", "Purple")
            {
                Items =
                [
                    new("Fifth Item Sub Item 1"), new("Fifth Item Sub Item 2"), new("Fifth Item Sub Item 3")
                ]
            },
            new("Sixth Item", "Pink")
            {
                Items =
                [
                    new("Sixth Item Sub Item 1"), new("Sixth Item Sub Item 2"), new("Sixth Item Sub Item 3")
                ]
            }
        ];

        Suggestions =
        [
            "Apple",
            "Banana",
            "Cherry",
            "Date",
            "Elderberry",
            "Fig",
            "Grape"
        ];

        FileItems = new ObservableCollection<Uri>();
        DocumentsFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        NewDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BehaviorsTestDirectory");

        // DocumentsFolder is set to null initially. In real applications, you would obtain
        // an IStorageFolder from a previous picker operation or storage provider.

        Values = Observable.Interval(TimeSpan.FromSeconds(1)).Select(_ => _value++);

        MyString = "";
        ValidatedText = "";
        ValidatedNumber = 0m;
        SelectedItem = null;
        ValidatedSlider = 0.0;
        ValidatedDate = DateTimeOffset.Now;
        ValidatedItem = null;

        IsLoading = true;
        Progress = 30;

        DataContextChangedCommand = ReactiveCommand.Create(DataContextChanged);

        InitializeCommand = ReactiveCommand.Create(Initialize);

        MoveLeftCommand = ReactiveCommand.Create(() => Position -= 5.0);
        MoveLeftCommand = ReactiveCommand.Create(() => Position -= 5.0);
        MoveRightCommand = ReactiveCommand.Create(() => Position += 5.0);
        ResetMoveCommand = ReactiveCommand.Create(() => Position = 100.0);

        OpenItemCommand = ReactiveCommand.Create<ItemViewModel>(OpenItem);

        OpenFilesCommand = ReactiveCommand.Create<IEnumerable<IStorageItem>>(OpenFiles);
        SaveFileCommand = ReactiveCommand.Create<Uri>(SaveFile);
        OpenFoldersCommand = ReactiveCommand.Create<IEnumerable<IStorageFolder>>(OpenFolders);

        GetClipboardTextCommand = ReactiveCommand.Create<string?>(GetClipboardText);
        GetClipboardDataCommand = ReactiveCommand.Create<object?>(GetClipboardData);
        GetClipboardFormatsCommand = ReactiveCommand.Create<IEnumerable<string>?>(GetClipboardFormats);

        UploadFilePath = string.Empty;
        UploadUrl = string.Empty;
        UploadCompleted = false;
        UploadCompletedCommand = ReactiveCommand.Create<System.Net.Http.HttpResponseMessage>(OnUploadCompleted);

        UploadStatusMessage = "Provide a file path and upload URL, then choose an upload strategy.";
        BeginUploadCommand = ReactiveCommand.Create(BeginUpload);

        ActiveScreenSummary = "Active screen info will appear once the window is visible.";
        ActiveScreenDiagnostics = string.Empty;

        TryPrepareSampleUploadDefaults();

        Greeting = "Entered text will appear here";
        TextChangedCommand = ReactiveCommand.Create<TextChangedEventArgs>(OnTextChanged);

        ScrollDemoItems = new ObservableCollection<string>
        {
            "Alpha",
            "Bravo",
            "Charlie",
            "Delta",
            "Echo",
            "Foxtrot",
            "Golf",
            "Hotel",
            "India",
            "Juliet",
            "Kilo",
            "Lima",
            "Mike",
            "November",
            "Oscar",
            "Papa",
            "Quebec",
            "Romeo",
            "Sierra",
            "Tango",
            "Uniform",
            "Victor",
            "Whiskey",
            "X-ray",
            "Yankee",
            "Zulu"
        };

        ScrollSelectedItem = ScrollDemoItems.FirstOrDefault();
        ScrollTargetIndex = 0;
        MaxScrollIndex = Math.Max(0, ScrollDemoItems.Count - 1);

        ScrollChangeSizeOptions = new List<HorizontalScrollViewerBehavior.ChangeSize>
        {
            HorizontalScrollViewerBehavior.ChangeSize.Line,
            HorizontalScrollViewerBehavior.ChangeSize.Page
        };

        SelectedScrollChangeSize = HorizontalScrollViewerBehavior.ChangeSize.Line;
        RequireShiftForHorizontalScroll = false;
        IsRenderTriggerEnabled = true;

        _scrollToItemSubject = new Subject<object>();
        _scrollToIndexSubject = new Subject<int>();

        ScrollToItemCommand = ReactiveCommand.Create<object?>(item =>
        {
            if (item is not null)
            {
                _scrollToItemSubject.OnNext(item);
            }
        });

        ScrollToIndexCommand = ReactiveCommand.Create<int>(index =>
        {
            var clamped = Math.Clamp(index, 0, MaxScrollIndex);
            ScrollTargetIndex = clamped;
            _scrollToIndexSubject.OnNext(clamped);
        });

        ScreenEvents = new ObservableCollection<string>();
        ContainerEventLog = new ObservableCollection<string>();

        TaskStatusMessage = "Press Start to run a demo task.";
        WriteableBitmapStatusMessage = "Render not requested yet.";
        RenderTriggerMessage = "Waiting for trigger...";
        WriteableBitmapTimerMessage = "Timer disabled.";
        IsWriteableBitmapTimerEnabled = false;
        WriteableBitmapTimerInterval = 500;

        StartSampleTaskCommand = ReactiveCommand.CreateFromTask(StartSampleTaskAsync);
        UpdateRenderTimestampCommand = ReactiveCommand.Create(() =>
        {
            RenderTriggerMessage = $"Last trigger tick at {DateTime.Now:T}";
        });

        UpdateManualWriteableBitmapMessageCommand = ReactiveCommand.Create(() =>
        {
            WriteableBitmapStatusMessage = $"Manual trigger fired at {DateTime.Now:T}";
        });

        UpdateWriteableBitmapTimerMessageCommand = ReactiveCommand.Create(() =>
        {
            WriteableBitmapTimerMessage = $"Timer tick at {DateTime.Now:T}";
        });
    }

    [Reactive]
    public partial int Count { get; set; }

    [Reactive]
    public partial int TimerCount { get; set; }

    [Reactive]
    public partial double Position { get; set; }

    [Reactive]
    public partial ObservableCollection<ItemViewModel>? Items { get; set; }

    [Reactive]
    public partial ObservableCollection<string>? Suggestions { get; set; }

    [Reactive]
    public partial ObservableCollection<Uri>? FileItems { get; set; }

    [Reactive]
    public partial string? DocumentsFolderPath { get; set; }

    [Reactive]
    public partial string? NewDirectoryPath { get; set; }

    [Reactive]
    public partial IStorageFolder? DocumentsFolder { get; set; }

    [Reactive]
    public partial string UploadFilePath { get; set; }

    [Reactive]
    public partial string UploadUrl { get; set; }

    [Reactive]
    public partial bool UploadCompleted { get; set; }

    [Reactive]
    public partial string UploadStatusMessage { get; set; }

    [Reactive]
    public partial Screen? ActiveScreen { get; set; }

    [Reactive]
    public partial string ActiveScreenSummary { get; set; }

    [Reactive]
    public partial string ActiveScreenDiagnostics { get; set; }

    [Reactive]
    public partial ObservableCollection<string> ScrollDemoItems { get; set; }

    [Reactive]
    public partial string? ScrollSelectedItem { get; set; }

    [Reactive]
    public partial int ScrollTargetIndex { get; set; }

    [Reactive]
    public partial int MaxScrollIndex { get; set; }

    public IObservable<object> ScrollToItemObservable => _scrollToItemSubject;

    public IObservable<int> ScrollToIndexObservable => _scrollToIndexSubject;

    [Reactive]
    public partial bool RequireShiftForHorizontalScroll { get; set; }

    [Reactive]
    public partial HorizontalScrollViewerBehavior.ChangeSize SelectedScrollChangeSize { get; set; }

    public IReadOnlyList<HorizontalScrollViewerBehavior.ChangeSize> ScrollChangeSizeOptions { get; }

    [Reactive]
    public partial string? TextValidationError { get; set; }

    [Reactive]
    public partial bool IsRenderTriggerEnabled { get; set; }

    [Reactive]
    public partial string TaskStatusMessage { get; set; }

    [Reactive]
    public partial bool TaskCompletedIndicator { get; set; }

    [Reactive]
    public partial Task? SampleTask { get; set; }

    [Reactive]
    public partial string RenderTriggerMessage { get; set; }

    [Reactive]
    public partial string WriteableBitmapStatusMessage { get; set; }

    [Reactive]
    public partial bool IsWriteableBitmapTimerEnabled { get; set; }

    [Reactive]
    public partial int WriteableBitmapTimerInterval { get; set; }

    [Reactive]
    public partial string WriteableBitmapTimerMessage { get; set; }

    public ObservableCollection<string> ContainerEventLog { get; }

    public ObservableCollection<string> ScreenEvents { get; }

    [Reactive] internal partial string MyString { get; set; }
    
    [Reactive] public partial bool FocusFlag { get; set; }
    
    [Reactive] public partial bool IsLoading { get; set; }

    [Reactive] public partial double Progress { get; set; }

    [Reactive] public partial string Greeting { get; set; }

    [Reactive] public partial string ValidatedText { get; set; }

    [Reactive] public partial bool IsTextValid { get; set; }

    [Reactive] public partial decimal ValidatedNumber { get; set; }

    [Reactive] public partial bool IsNumberValid { get; set; }

    [Reactive] public partial ItemViewModel? SelectedItem { get; set; }

    [Reactive] public partial double ValidatedSlider { get; set; }

    [Reactive] public partial bool IsSliderValid { get; set; }

    [Reactive] public partial DateTimeOffset? ValidatedDate { get; set; }

    [Reactive] public partial bool IsDateValid { get; set; }

    [Reactive] public partial ItemViewModel? ValidatedItem { get; set; }

    [Reactive] public partial bool IsItemValid { get; set; }

    public IObservable<int> Values { get; }

    public ICommand DataContextChangedCommand { get; set; }

    public ICommand InitializeCommand { get; set; }

    public ICommand MoveLeftCommand { get; set; }

    public ICommand MoveRightCommand { get; set; }

    public ICommand ResetMoveCommand { get; set; }

    public ICommand OpenItemCommand { get; set; }

    public ICommand OpenFilesCommand { get; set; }

    public ICommand SaveFileCommand { get; set; }

    public ICommand BeginUploadCommand { get; set; }

    public ICommand OpenFoldersCommand { get; set; }

    public ICommand GetClipboardTextCommand { get; set; }

    public ICommand GetClipboardDataCommand { get; set; }

    public ICommand GetClipboardFormatsCommand { get; set; }

    public ICommand UploadCompletedCommand { get; set; }

    public ICommand TextChangedCommand { get; }

    public ICommand ScrollToItemCommand { get; }

    public ICommand ScrollToIndexCommand { get; }

    public ICommand StartSampleTaskCommand { get; }

    public ICommand UpdateRenderTimestampCommand { get; }

    public ICommand UpdateManualWriteableBitmapMessageCommand { get; }

    public ICommand UpdateWriteableBitmapTimerMessageCommand { get; }

    private void DataContextChanged()
    {
        Console.WriteLine("DataContextChanged");
    }

    private void Initialize()
    {
        Console.WriteLine("InitializeCommand");
    }

    private void OpenItem(ItemViewModel item)
    {
        Console.WriteLine($"OpenItemCommand: {item.Value}");
    }

    private void OpenFiles(IEnumerable<IStorageItem> files)
    {
        foreach (var file in files)
        {
            Console.WriteLine($"OpenFilesCommand: {file.Name}, {file.Path}");

            FileItems.Add(file.Path);
        }
    }

    private void SaveFile(Uri file)
    {
        Console.WriteLine($"SaveFileCommand: {file}");

        FileItems.Add(file);
    }

    private void OpenFolders(IEnumerable<IStorageFolder> folders)
    {
        foreach (var folder in folders)
        {
            Console.WriteLine($"OpenFoldersCommand: {folder.Name}, {folder.Path}");

            FileItems.Add(folder.Path);

            // Set the first folder as DocumentsFolder to demonstrate SuggestedStartLocation usage
            if (DocumentsFolder is null)
            {
                DocumentsFolder = folder;
            }
        }
    }

    private void GetClipboardText(string? text)
    {
        Console.WriteLine($"GetClipboardTextCommand: {text}");
    }

    private void GetClipboardData(object? data)
    {
        Console.WriteLine($"GetClipboardDataCommand: {data}");
    }

    private void GetClipboardFormats(IEnumerable<string>? formats)
    {
        if (formats is not null)
        {
            Console.WriteLine($"GetClipboardFormatsCommand: {string.Join(',', formats)}");
        }
    }

    private void OnUploadCompleted(System.Net.Http.HttpResponseMessage response)
    {
        Console.WriteLine($"UploadCompleted: {response.StatusCode}");
        UploadCompleted = true;
        var reason = response.ReasonPhrase ?? string.Empty;
        UploadStatusMessage = $"Upload completed with status {(int)response.StatusCode} {reason}".Trim();
    }

    public void IncrementCount() => Count++;

    public void DecrementCount(object? sender, object parameter) => Count--;

    public void IncrementTimerCount() => TimerCount++;

    public void DecrementTimerCount(object? sender, object parameter) => TimerCount--;

    private void OnTextChanged(TextChangedEventArgs args)
    {
        if (args.Source is TextBox control)
        {
            Greeting = control.Text;
        }
    }

    private async Task StartSampleTaskAsync()
    {
        TaskCompletedIndicator = false;
        TaskStatusMessage = "Task running...";

        var work = Task.Delay(1500);
        SampleTask = work;

        try
        {
            await work;
        }
        finally
        {
            // TaskCompletedTrigger will update state via actions when the task finishes.
        }
    }

    private void BeginUpload()
    {
        UploadCompleted = false;
        UploadStatusMessage = "Uploading... (check console output for HTTP response)";
    }

    private void TryPrepareSampleUploadDefaults()
    {
        try
        {
            var tempPath = Path.Combine(Path.GetTempPath(), "AvaloniaBehaviorUploadSample.txt");
            if (!File.Exists(tempPath))
            {
                File.WriteAllText(tempPath, "Sample upload content generated for the ButtonUploadFileBehavior demo.");
            }

            UploadFilePath = tempPath;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unable to provision sample upload file: {ex.Message}");
            UploadFilePath = string.Empty;
        }

        if (string.IsNullOrWhiteSpace(UploadUrl))
        {
            UploadUrl = "https://httpbin.org/post";
        }
    }

    public void OnContainerPreparing(object? sender, object? parameter) => AppendContainerLog("PreparingContainer", parameter);

    public void OnContainerPrepared(object? sender, object? parameter) => AppendContainerLog("ContainerPrepared", parameter);

    public void OnContainerClearing(object? sender, object? parameter) => AppendContainerLog("ContainerClearing", parameter);

    public void OnContainerIndexChanged(object? sender, object? parameter) => AppendContainerLog("ContainerIndexChanged", parameter);

    public void ClearContainerLog() => ContainerEventLog.Clear();

    public void AddSampleItem()
    {
        Items ??= new ObservableCollection<ItemViewModel>();

        var name = $"Generated {_containerSampleCounter++}";
        var color = "Generated";
        Items.Add(new ItemViewModel(name, color));
        SelectedItem = Items.LastOrDefault();
        MaxScrollIndex = Math.Max(0, ScrollDemoItems.Count - 1);
        ScrollTargetIndex = Math.Clamp(ScrollTargetIndex, 0, MaxScrollIndex);
    }

    public void RemoveSelectedItem()
    {
        if (Items is null || SelectedItem is null)
        {
            return;
        }

        var index = Items.IndexOf(SelectedItem);
        Items.Remove(SelectedItem);
        SelectedItem = Items.Count > 0
            ? Items[Math.Clamp(index, 0, Items.Count - 1)]
            : null;
        MaxScrollIndex = Math.Max(0, ScrollDemoItems.Count - 1);
        ScrollTargetIndex = Math.Clamp(ScrollTargetIndex, 0, MaxScrollIndex);
    }

    private void AppendContainerLog(string category, object? parameter)
    {
        var details = DescribeContainerParameter(parameter);
        var message = $"{DateTime.Now:HH:mm:ss} - {category}{(string.IsNullOrWhiteSpace(details) ? string.Empty : $": {details}")}";
        ContainerEventLog.Insert(0, message);

        if (ContainerEventLog.Count > 50)
        {
            ContainerEventLog.RemoveAt(ContainerEventLog.Count - 1);
        }
    }

    private static string DescribeContainerParameter(object? parameter)
    {
        if (parameter is null)
        {
            return string.Empty;
        }

        var type = parameter.GetType();
        var parts = new List<string>();

        void Append(string propertyName)
        {
            var property = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            if (property is null)
            {
                return;
            }

            var value = property.GetValue(parameter);
            if (value is Control control)
            {
                var context = control.DataContext ?? control;
                parts.Add($"{propertyName}={context}");
            }
            else
            {
                parts.Add($"{propertyName}={value}");
            }
        }

        Append("Index");
        Append("OldIndex");
        Append("NewIndex");
        Append("Item");
        Append("Container");

        return string.Join(", ", parts);
    }

    public void OnScreensChanged(object? sender, object? parameter)
    {
        AppendScreenLog("ScreensChanged");
    }

    public void ClearScreenEvents() => ScreenEvents.Clear();

    private void AppendScreenLog(string category)
    {
        var entry = $"{DateTime.Now:T} - {category}";
        ScreenEvents.Insert(0, entry);
        if (ScreenEvents.Count > 50)
        {
            ScreenEvents.RemoveAt(ScreenEvents.Count - 1);
        }
    }

    partial void OnActiveScreenChanged(Screen? value);

    partial void OnActiveScreenChanged(Screen? value)
    {
        if (value is null)
        {
            ActiveScreenSummary = "Active screen info will appear once the window is visible.";
            ActiveScreenDiagnostics = string.Empty;
            return;
        }

        var label = value.IsPrimary ? "Primary screen" : "Secondary screen";
        ActiveScreenSummary = label;

        ActiveScreenDiagnostics =
            $"Bounds: {value.Bounds}{Environment.NewLine}" +
            $"Working area: {value.WorkingArea}{Environment.NewLine}" +
            $"Scaling: {value.Scaling:0.##}";
    }

    public async Task LoadDataAsync()
    {
        IsLoading = true;
        Progress = 0;

        for (var i = 0; i <= 100; i += 20)
        {
            await Task.Delay(100);
            Progress = i;
        }

        IsLoading = false;
    }
}
