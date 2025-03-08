using Avalonia.Platform.Storage;

namespace Avalonia.Xaml.Interactions.Core;

/// <summary>
/// 
/// </summary>
public abstract class PickerBehaviorBase : InvokeCommandBehaviorBase
{
    /// <summary>
    /// Identifies the <seealso cref="Title"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<PickerBehaviorBase, string?>(nameof(Title));
    
    /// <summary>
    /// Identifies the <seealso cref="SuggestedStartLocation"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<IStorageFolder?> SuggestedStartLocationProperty =
        AvaloniaProperty.Register<PickerBehaviorBase, IStorageFolder?>(nameof(SuggestedStartLocation));
    
    /// <summary>
    /// Identifies the <seealso cref="SuggestedFileName"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> SuggestedFileNameProperty =
        AvaloniaProperty.Register<PickerBehaviorBase, string?>(nameof(SuggestedFileName));

    /// <summary>
    /// Gets or sets the text that appears in the title bar of a picker. This is an avalonia property.
    /// </summary>
    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    } 

    /// <summary>
    /// Gets or sets the initial location where the file open picker looks for files to present to the user. This is an avalonia property.
    /// </summary>
    public IStorageFolder? SuggestedStartLocation
    {
        get => GetValue(SuggestedStartLocationProperty);
        set => SetValue(SuggestedStartLocationProperty, value);
    } 
   
    /// <summary>
    /// Gets or sets the file name that the file picker suggests to the user. This is an avalonia property.
    /// </summary>
    public string? SuggestedFileName
    {
        get => GetValue(SuggestedFileNameProperty);
        set => SetValue(SuggestedFileNameProperty, value);
    } 
}
