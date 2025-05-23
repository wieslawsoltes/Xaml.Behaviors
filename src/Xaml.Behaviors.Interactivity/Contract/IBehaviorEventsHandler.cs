namespace Avalonia.Xaml.Interactivity;

/// <summary>
/// Interface implemented by base behaviors.
/// </summary>
public interface IBehaviorEventsHandler
{ 
    /// <summary>
    /// Called when element is attached to the visual tree.
    /// </summary>
    void AttachedToVisualTreeEventHandler();

    /// <summary>
    /// Called when element is detached from the visual tree.
    /// </summary>
    void DetachedFromVisualTreeEventHandler();

    /// <summary>
    /// Called when element is attached to the logical tree.
    /// </summary>
    void AttachedToLogicalTreeEventHandler();

    /// <summary>
    /// Called when element is detached from the logical tree.
    /// </summary>
    void DetachedFromLogicalTreeEventHandler();

    /// <summary>
    /// Called when element is loaded.
    /// </summary>
    void LoadedEventHandler();

    /// <summary>
    /// Called when element is unloaded.
    /// </summary>
    void UnloadedEventHandler();
    
    /// <summary>
    /// Called when element is initialized.
    /// </summary>
    void InitializedEventHandler();
    
    /// <summary>
    /// Called when element DataContext changed.
    /// </summary>
    void DataContextChangedEventHandler();
    
    /// <summary>
    /// Called when element Resources changed.
    /// </summary>
    void ResourcesChangedEventHandler();
    
    /// <summary>
    /// Called when element ActualThemeVariant changed.
    /// </summary>
    void ActualThemeVariantChangedEventHandler();
}
