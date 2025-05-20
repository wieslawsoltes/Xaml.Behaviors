using ReactiveUI;

namespace BehaviorsTestApplication.ViewModels;

public class HomePageViewModel(IScreen screen) : ViewModelBase, IRoutableViewModel
{
    public string UrlPathSegment => "home";

    public IScreen HostScreen { get; } = screen;
}
