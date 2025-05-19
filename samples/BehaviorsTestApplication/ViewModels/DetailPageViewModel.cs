using ReactiveUI;

namespace BehaviorsTestApplication.ViewModels;

public class DetailPageViewModel : ViewModelBase, IRoutableViewModel
{
    public DetailPageViewModel(IScreen screen)
    {
        HostScreen = screen;
    }

    public string UrlPathSegment => "detail";

    public IScreen HostScreen { get; }
}
