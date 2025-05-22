using BehaviorsTestApplication.Animations;

namespace BehaviorsTestApplication.ViewModels;

public class CustomAnimatorViewModel : ViewModelBase
{
    /// <summary>
    /// Gets the builder used by the sample to create the animation.
    /// </summary>
    public CustomStringAnimationBuilder AnimationBuilder { get; } = new();
}
