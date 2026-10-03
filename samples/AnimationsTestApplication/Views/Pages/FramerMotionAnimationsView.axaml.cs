#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Avalonia.Controls;
#endif

namespace AnimationsTestApplication.Views.Pages;

public partial class FramerMotionAnimationsView : UserControl
{
    public FramerMotionAnimationsView()
    {
        InitializeComponent();
    }
}
