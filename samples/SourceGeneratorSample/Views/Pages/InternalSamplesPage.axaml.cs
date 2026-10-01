#if UNO
using Microsoft.UI.Xaml.Controls;
using SourceGeneratorSample.ViewModels;
#else
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
#endif

namespace SourceGeneratorSample.Views.Pages
{
    public partial class InternalSamplesPage : UserControl
    {
        public InternalSamplesPage()
        {
            InitializeComponent();
        }

#if UNO
        /// <summary>
        /// Gets the view model (the inherited data context) for the compiled bindings (x:Bind) of the WinUI view.
        /// </summary>
        public MainViewModel? ViewModel => DataContext as MainViewModel;
#else
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
#endif
    }
}
