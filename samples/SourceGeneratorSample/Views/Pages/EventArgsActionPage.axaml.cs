using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace SourceGeneratorSample.Views.Pages
{
    public partial class EventArgsActionPage : UserControl
    {
        public EventArgsActionPage()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
