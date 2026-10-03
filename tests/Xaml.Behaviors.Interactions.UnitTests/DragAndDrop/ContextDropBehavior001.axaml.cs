#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Avalonia.Controls;
#endif

#if UNO
namespace Xaml.Interactions.UnitTests.DragAndDrop;
#else
namespace Avalonia.Xaml.Interactions.UnitTests.DragAndDrop;
#endif

public partial class ContextDropBehavior001 : Window
{
    public ContextDropBehavior001()
    {
        InitializeComponent();
    }
}
