// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Collections.ObjectModel;
using ReactiveUI.Primitives;
using ReactiveUI;

namespace BehaviorsTestApplication.ViewModels;

/// <summary>
/// View model of the sample page.
/// </summary>
public sealed class MainViewModel : ReactiveObject
{
    private int _count;
    private string _lastKey = "none";

    /// <summary>
    /// Initializes a new instance of the <see cref="MainViewModel"/> class.
    /// </summary>
    public MainViewModel()
    {
        IncrementCommand = ReactiveCommand.Create(() => { Count++; });
        ResetCommand = ReactiveCommand.Create(() => { Count = 0; });
        LogCommand = ReactiveCommand.Create<object?>(parameter => Log.Insert(0, parameter?.ToString() ?? "(null)"));
        KeyCommand = ReactiveCommand.Create<object?>(parameter => { LastKey = parameter?.ToString() ?? "none"; });
    }

    /// <summary>Gets the click counter.</summary>
    public int Count
    {
        get => _count;
        set => this.RaiseAndSetIfChanged(ref _count, value);
    }

    /// <summary>Gets the last key reported by the key trigger.</summary>
    public string LastKey
    {
        get => _lastKey;
        set => this.RaiseAndSetIfChanged(ref _lastKey, value);
    }

    /// <summary>Gets the log of command parameters.</summary>
    public ObservableCollection<string> Log { get; } = [];

    /// <summary>Gets the command incrementing <see cref="Count"/>.</summary>
    public ReactiveCommand<RxVoid, RxVoid> IncrementCommand { get; }

    /// <summary>Gets the command resetting <see cref="Count"/>.</summary>
    public ReactiveCommand<RxVoid, RxVoid> ResetCommand { get; }

    /// <summary>Gets the command logging its parameter.</summary>
    public ReactiveCommand<object?, RxVoid> LogCommand { get; }

    /// <summary>Gets the command receiving key events.</summary>
    public ReactiveCommand<object?, RxVoid> KeyCommand { get; }
}
