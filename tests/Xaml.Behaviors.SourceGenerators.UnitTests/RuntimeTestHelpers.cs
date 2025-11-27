using System;
using System.Collections.Generic;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;

internal sealed class RecordingAction : StyledElementAction
{
    public List<object?> SeenParameters { get; } = new();

    public override object? Execute(object? sender, object? parameter)
    {
        SeenParameters.Add(parameter);
        return parameter;
    }
}

internal sealed class TestObservable<T> : IObservable<T>
{
    private readonly List<IObserver<T>> _observers = new();
    private bool _completed;

    public IDisposable Subscribe(IObserver<T> observer)
    {
        if (_completed)
        {
            observer.OnCompleted();
            return Disposable.Empty;
        }

        _observers.Add(observer);
        return new Subscription(_observers, observer);
    }

    public void OnNext(T value)
    {
        foreach (var observer in _observers.ToArray())
        {
            observer.OnNext(value);
        }
    }

    public void OnError(Exception exception)
    {
        foreach (var observer in _observers.ToArray())
        {
            observer.OnError(exception);
        }
    }

    public void OnCompleted()
    {
        _completed = true;
        foreach (var observer in _observers.ToArray())
        {
            observer.OnCompleted();
        }
    }

    private sealed class Subscription : IDisposable
    {
        private readonly List<IObserver<T>> _observers;
        private readonly IObserver<T> _observer;

        public Subscription(List<IObserver<T>> observers, IObserver<T> observer)
        {
            _observers = observers;
            _observer = observer;
        }

        public void Dispose()
        {
            _observers.Remove(_observer);
        }
    }

    private sealed class Disposable : IDisposable
    {
        public static readonly IDisposable Empty = new Disposable();
        public void Dispose()
        {
        }
    }
}
