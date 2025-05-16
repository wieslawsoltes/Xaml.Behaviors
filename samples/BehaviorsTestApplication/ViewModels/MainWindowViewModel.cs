using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Windows.Input;
using Avalonia.Platform.Storage;
using ReactiveUI;

namespace BehaviorsTestApplication.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private int _value;

    public MainWindowViewModel()
    {
        PointerTriggersViewModel = new PointerTriggersViewModel();
        
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

        FileItems = new ObservableCollection<Uri>();

        Values = Observable.Interval(TimeSpan.FromSeconds(1)).Select(_ => _value++);

        MyString = "";

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
    }

    [Reactive]
    public partial PointerTriggersViewModel PointerTriggersViewModel { get; set; }

    [Reactive]
    public partial int Count { get; set; }

    [Reactive]
    public partial double Position { get; set; }

    [Reactive]
    public partial ObservableCollection<ItemViewModel>? Items { get; set; }

    [Reactive]
    public partial ObservableCollection<Uri>? FileItems { get; set; }

    [Reactive] internal partial string MyString { get; set; }
    
    [Reactive] public partial bool FocusFlag { get; set; }
    
    [Reactive] public partial bool IsLoading { get; set; }

    [Reactive] public partial double Progress { get; set; }

    public IObservable<int> Values { get; }

    public ICommand DataContextChangedCommand { get; set; }

    public ICommand InitializeCommand { get; set; }

    public ICommand MoveLeftCommand { get; set; }

    public ICommand MoveRightCommand { get; set; }

    public ICommand ResetMoveCommand { get; set; }

    public ICommand OpenItemCommand { get; set; }

    public ICommand OpenFilesCommand { get; set; }

    public ICommand SaveFileCommand { get; set; }
    
    public ICommand OpenFoldersCommand { get; set; }
    
    public ICommand GetClipboardTextCommand { get; set; }

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
        }
    }

    private void GetClipboardText(string? text)
    {
        Console.WriteLine($"GetClipboardTextCommand: {text}");
    }

    public void IncrementCount() => Count++;

    public void DecrementCount(object? sender, object parameter) => Count--;
}
