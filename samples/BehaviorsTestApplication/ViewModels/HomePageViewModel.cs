using ReactiveUI;

namespace BehaviorsTestApplication.ViewModels;

public class HomePageViewModel : ViewModelBase, IRoutableViewModel
{
    public HomePageViewModel(IScreen screen)
    {
        HostScreen = screen;
    }

    public string UrlPathSegment => "home";

    public IScreen HostScreen { get; }
}
