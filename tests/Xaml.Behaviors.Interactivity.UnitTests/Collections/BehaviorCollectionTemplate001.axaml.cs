#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Avalonia.Controls;
#endif

#if UNO
namespace Xaml.Interactivity.UnitTests;
#else
namespace Avalonia.Xaml.Interactivity.UnitTests;
#endif

public partial class BehaviorCollectionTemplate001 : Window
{
    public BehaviorCollectionTemplate001()
    {
        InitializeComponent();
    }
}
