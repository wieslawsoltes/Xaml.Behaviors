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

public partial class InvokeCommandAction003 : Window
{
    public ICommand TestCommand { get; set; }

    public InvokeCommandAction003()
    {
        InitializeComponent();

        TestCommand = new Command(parameter =>
        {
            TargetTextBox.Text = $"Command {parameter}";
        });

        DataContext = this;
    }
}
