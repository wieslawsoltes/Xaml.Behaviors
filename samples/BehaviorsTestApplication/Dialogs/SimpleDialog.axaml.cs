using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace BehaviorsTestApplication.Dialogs;

public partial class SimpleDialog : Window
{
    public SimpleDialog()
    {
        InitializeComponent();
        this.FindControl<Button>("CloseButton").Click += (_, _) => Close();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
