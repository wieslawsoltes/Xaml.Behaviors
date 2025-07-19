using System.Collections.Generic;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Platform.Storage;

namespace Avalonia.Xaml.Interactions.UnitTests.Core;

public partial class FilesDropBehavior001 : Window
{
    public ICommand DropCommand { get; }

    public IEnumerable<IStorageItem>? DroppedFiles { get; private set; }

    public FilesDropBehavior001()
    {
        InitializeComponent();

        DropCommand = new Command(parameter =>
        {
            DroppedFiles = parameter as IEnumerable<IStorageItem>;
        });

        DataContext = this;
    }
}
