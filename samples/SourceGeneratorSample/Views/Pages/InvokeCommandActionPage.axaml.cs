using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace SourceGeneratorSample.Views.Pages
{
    public partial class InvokeCommandActionPage : UserControl
    {
        public InvokeCommandActionPage()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
