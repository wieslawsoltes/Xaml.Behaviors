using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace SourceGeneratorSample.Views.Pages
{
    public partial class DataTriggerPage : UserControl
    {
        public DataTriggerPage()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
