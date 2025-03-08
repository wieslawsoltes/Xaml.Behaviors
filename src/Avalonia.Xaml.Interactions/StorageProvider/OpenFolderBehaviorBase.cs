using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;

namespace Avalonia.Xaml.Interactions.Core;

/// <summary>
/// 
/// </summary>
public abstract class OpenFolderBehaviorBase : PickerBehaviorBase
{
    /// <summary>
    /// Identifies the <seealso cref="AllowMultiple"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> AllowMultipleProperty =
        AvaloniaProperty.Register<OpenFolderBehaviorBase, bool>(nameof(AllowMultiple));

    /// <summary>
    /// Gets or sets an option indicating whether open picker allows users to select multiple folders. This is an avalonia property.
    /// </summary>
    public bool AllowMultiple
    {
        get => GetValue(AllowMultipleProperty);
        set => SetValue(AllowMultipleProperty, value);
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="OpenFolderBehaviorBase"/> class.
    /// </summary>
    protected OpenFolderBehaviorBase()
    {
        PassEventArgsToCommand = true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="parameter"></param>
    protected async Task Execute(object? sender, object? parameter)
    {
        if (sender is not Visual visual)
        {
            return;
        }

        await OpenFolderPickerAsync(visual);
    }

    private async Task OpenFolderPickerAsync(Visual visual)
    {
        if (IsEnabled != true || Command is null)
        {
            return;
        }

        var storageProvider = (visual.GetVisualRoot() as TopLevel)?.StorageProvider;
        if (storageProvider is null)
        {
            return;
        }

        var folders = await storageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = Title,
            SuggestedStartLocation = SuggestedStartLocation,
            SuggestedFileName = SuggestedFileName,
            AllowMultiple = AllowMultiple
        });

        if (folders.Count <= 0)
        {
            return;
        }

        var resolvedParameter = ResolveParameter(folders);

        if (!Command.CanExecute(resolvedParameter))
        {
            return;
        }

        Command.Execute(resolvedParameter);
    }
}
