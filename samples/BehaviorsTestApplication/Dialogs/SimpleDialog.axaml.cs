#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
#else
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
#endif

namespace BehaviorsTestApplication.Dialogs;

#if UNO
// WinUI: ShowDialogAction shows a ContentDialog; its close button (CloseButtonText) closes the dialog.
public partial class SimpleDialog : ContentDialog
{
    public SimpleDialog()
    {
        InitializeComponent();
    }
}
#else
public partial class SimpleDialog : Window
{
    public SimpleDialog()
    {
        InitializeComponent();
        var closeButton = this.FindControl<Button>("CloseButton");
        if (closeButton is not null)
        {
            closeButton.Click += (_, _) => Close();
        }
    }

#if !UNO
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
#endif
}
#endif
