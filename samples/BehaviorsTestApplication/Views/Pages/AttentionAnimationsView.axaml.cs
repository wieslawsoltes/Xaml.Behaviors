#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Avalonia.Controls;
#endif

namespace BehaviorsTestApplication.Views.Pages;

public partial class AttentionAnimationsView : UserControl
{
    public AttentionAnimationsView()
    {
        InitializeComponent();
    }
}
