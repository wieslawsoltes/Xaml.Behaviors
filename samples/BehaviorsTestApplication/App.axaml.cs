using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using BehaviorsTestApplication.ViewModels;
using BehaviorsTestApplication.Views;

namespace BehaviorsTestApplication;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
        {
            desktopLifetime.MainWindow = new MainWindow { DataContext = new MainWindowViewModel() };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewLifetime)
        {
            singleViewLifetime.MainView = new MainView { DataContext = new MainWindowViewModel() };
        }
        base.OnFrameworkInitializationCompleted();
    }
}
