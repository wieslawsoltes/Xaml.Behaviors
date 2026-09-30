using System.Windows.Input;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactions.UnitTests.Core;
#else
using Avalonia.Controls;
using Avalonia.Xaml.Interactions.UnitTests.Core;
#endif

#if UNO
namespace Xaml.Interactions.UnitTests.Custom;
#else
namespace Avalonia.Xaml.Interactions.UnitTests.Custom;
#endif

public partial class DetachedFromVisualTreeTrigger001 : Window
{
    public DetachedFromVisualTreeTrigger001()
    {
        InitializeComponent();
    }
#if UNO

    // WinUI clears the inherited DataContext of an element leaving the tree before it raises Unloaded (when the
    // detached trigger runs), so the page binds the command with x:Bind.
    internal DetachedTriggerBindingSource Source => (DetachedTriggerBindingSource)DataContext;
#endif
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
