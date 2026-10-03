#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Avalonia.Controls;
#endif

#if UNO
namespace Xaml.Interactions.UnitTests.Core;
#else
namespace Avalonia.Xaml.Interactions.UnitTests.Core;
#endif

public partial class ListReorderDragBehaviorWindow : Window
{
    public ListReorderDragBehaviorWindow()
    {
        InitializeComponent();
    }
}
