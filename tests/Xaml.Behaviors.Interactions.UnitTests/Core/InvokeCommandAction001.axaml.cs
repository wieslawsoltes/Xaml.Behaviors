using System.Windows.Input;
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

public partial class InvokeCommandAction001 : Window
{
    public ICommand TestCommand { get; set; }

    public InvokeCommandAction001()
    {
        InitializeComponent();

        TestCommand = new Command(_ =>
        {
            TargetTextBox.Text = "Command Text";
        });

        DataContext = this;
    }
}
