using System;
using System.Reactive.Subjects;
using System.Windows.Input;
using BehaviorsTestApplication.Animations;
using ReactiveUI;

namespace BehaviorsTestApplication.ViewModels;

public class CustomAnimatorViewModel : ViewModelBase
{
    private readonly Subject<object> _animationRequests = new();

    public CustomAnimatorViewModel()
    {
        StartAnimationCommand = ReactiveCommand.Create(() => _animationRequests.OnNext(AnimationBuilder));
    }

    /// <summary>
    /// Gets the builder used by the sample to create the animation.
    /// </summary>
    public CustomStringAnimationBuilder AnimationBuilder { get; } = new();

    /// <summary>
    /// Gets the requests to start the animation, raised by <see cref="StartAnimationCommand"/>.
    /// </summary>
    public IObservable<object> AnimationRequests => _animationRequests;

    /// <summary>
    /// Gets the command that requests to start the animation.
    /// </summary>
    public ICommand StartAnimationCommand { get; }
}
