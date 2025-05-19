using System;
using ReactiveUI;

namespace BehaviorsTestApplication.ViewModels;

public class ReactiveNavigationViewModel : ViewModelBase, IScreen
{
    public ReactiveNavigationViewModel()
    {
        Router = new RoutingState();
        Home = new HomePageViewModel(this);
        Second = new DetailPageViewModel(this);
        Router.Navigate.Execute(Home).Subscribe();
    }

    public RoutingState Router { get; }

    public HomePageViewModel Home { get; }

    public DetailPageViewModel Second { get; }
}
