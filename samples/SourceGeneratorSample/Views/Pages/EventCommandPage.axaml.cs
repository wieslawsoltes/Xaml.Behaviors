using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace SourceGeneratorSample.Views.Pages
{
    public partial class EventCommandPage : UserControl
    {
        public EventCommandPage()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
