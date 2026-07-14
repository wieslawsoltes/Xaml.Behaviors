using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Xaml.Interactions.UnitTests.Core;

namespace Avalonia.Xaml.Interactions.UnitTests.Custom;

public partial class DetachedFromVisualTreeTrigger001 : Window
{
    public DetachedFromVisualTreeTrigger001()
    {
        InitializeComponent();
    }
}

public sealed class DetachedTriggerBindingSource
{
    public DetachedTriggerBindingSource()
    {
        DetachedCommand = new Command(_ => DetachedCount++);
    }

    public ICommand DetachedCommand { get; }

    public int DetachedCount { get; private set; }
}
