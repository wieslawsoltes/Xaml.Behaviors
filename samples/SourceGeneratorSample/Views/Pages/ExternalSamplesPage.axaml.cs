using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace SourceGeneratorSample.Views.Pages
{
    public partial class ExternalSamplesPage : UserControl
    {
        public ExternalSamplesPage()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
