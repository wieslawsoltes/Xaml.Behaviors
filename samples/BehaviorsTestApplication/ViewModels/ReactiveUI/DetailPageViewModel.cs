using ReactiveUI;
using ReactiveUI.Reactive;

namespace BehaviorsTestApplication.ViewModels;

public class DetailPageViewModel(IScreen screen) : ViewModelBase, IRoutableViewModel
{
    public string UrlPathSegment => "detail";

    public IScreen HostScreen { get; } = screen;
}
