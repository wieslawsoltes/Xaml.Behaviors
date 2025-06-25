using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactivity.UnitTests;

public class StubStyledElementBehavior : StyledElementBehavior
{
    public int AttachCount { get; private set; }
    public int DetachCount { get; private set; }
    public int AttachedToVisualTreeCount { get; private set; }
    public int DetachedFromVisualTreeCount { get; private set; }
    public int AttachedToLogicalTreeCount { get; private set; }
    public int DetachedFromLogicalTreeCount { get; private set; }
    public int LoadedCount { get; private set; }
    public int UnloadedCount { get; private set; }
    public int InitializedCount { get; private set; }
    public int DataContextChangedCount { get; private set; }
    public int ResourcesChangedCount { get; private set; }
    public int ActualThemeVariantChangedCount { get; private set; }

    protected override void OnAttached()
    {
        AttachCount++;
        base.OnAttached();
    }

    protected override void OnDetaching()
    {
        DetachCount++;
        base.OnDetaching();
    }

    protected override void OnAttachedToVisualTree()
    {
        AttachedToVisualTreeCount++;
        base.OnAttachedToVisualTree();
    }

    protected override void OnDetachedFromVisualTree()
    {
        DetachedFromVisualTreeCount++;
        base.OnDetachedFromVisualTree();
    }

    protected override void OnAttachedToLogicalTree()
    {
        AttachedToLogicalTreeCount++;
        base.OnAttachedToLogicalTree();
    }

    protected override void OnDetachedFromLogicalTree()
    {
        DetachedFromLogicalTreeCount++;
        base.OnDetachedFromLogicalTree();
    }

    protected override void OnLoaded()
    {
        LoadedCount++;
        base.OnLoaded();
    }

    protected override void OnUnloaded()
    {
        UnloadedCount++;
        base.OnUnloaded();
    }

    protected override void OnInitializedEvent()
    {
        InitializedCount++;
        base.OnInitializedEvent();
    }

    protected override void OnDataContextChangedEvent()
    {
        DataContextChangedCount++;
        base.OnDataContextChangedEvent();
    }

    protected override void OnResourcesChangedEvent()
    {
        ResourcesChangedCount++;
        base.OnResourcesChangedEvent();
    }

    protected override void OnActualThemeVariantChangedEvent()
    {
        ActualThemeVariantChangedCount++;
        base.OnActualThemeVariantChangedEvent();
    }
}

