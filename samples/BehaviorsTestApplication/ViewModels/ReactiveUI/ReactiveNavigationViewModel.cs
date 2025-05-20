using System;
using BehaviorsTestApplication.Views.Pages;
using ReactiveUI;
using Splat;

namespace BehaviorsTestApplication.ViewModels;

public class ReactiveNavigationViewModel : ViewModelBase, IScreen
{
    public ReactiveNavigationViewModel()
    {
        Router = new RoutingState();

        Locator.CurrentMutable.Register<HomePageViewModel>(() => new HomePageViewModel(this));
        Locator.CurrentMutable.Register<DetailPageViewModel>(() => new DetailPageViewModel(this));

        Locator.CurrentMutable.Register(() => new HomePageView(), typeof(IViewFor<HomePageViewModel>));
        Locator.CurrentMutable.Register(() => new DetailPageView(), typeof(IViewFor<DetailPageViewModel>));

        HomePageViewModel = new HomePageViewModel(this);
        DetailsViewModel = new DetailPageViewModel(this);

        Router.Navigate.Execute(HomePageViewModel).Subscribe();
    }

    public RoutingState Router { get; }

    public HomePageViewModel HomePageViewModel { get; }

    public DetailPageViewModel DetailsViewModel { get; }
}
