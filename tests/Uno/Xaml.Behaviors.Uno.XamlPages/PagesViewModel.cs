// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Xaml.Behaviors.Uno.XamlPages;

public sealed class PagesViewModel : INotifyPropertyChanged
{
    private int _count;
    private string _status = "idle";

    public PagesViewModel()
    {
        IncrementCommand = new RelayCommand(p =>
        {
            Parameters.Add(p);
            Count++;
        });
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ICommand IncrementCommand { get; }

    public List<object?> Parameters { get; } = [];

    public int Count
    {
        get => _count;
        set
        {
            _count = value;
            OnPropertyChanged();
        }
    }

    public string Status
    {
        get => _status;
        set
        {
            _status = value;
            OnPropertyChanged();
        }
    }

    public void Reset() => Count = 0;

    private void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    private sealed class RelayCommand(Action<object?> execute) : ICommand
    {
        public event EventHandler? CanExecuteChanged { add { } remove { } }

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter) => execute(parameter);
    }
}
