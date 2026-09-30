#if UNO
using Microsoft.UI.Xaml;
using SourceGeneratorSample.ViewModels;
#else
using Avalonia.Controls;
#endif

namespace SourceGeneratorSample.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
#if UNO

        /// <summary>
        /// Gets the view model for the compiled bindings (x:Bind) of the WinUI view. WinUI windows have no data
        /// context: the composition root sets the view model, the view passes it on as the data context of its content.
        /// </summary>
        public MainViewModel? ViewModel { get; init; }
#endif
    }
}
