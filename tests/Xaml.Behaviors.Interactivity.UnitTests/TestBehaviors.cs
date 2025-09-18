using Avalonia;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactivity.UnitTests;

internal class TestBehavior : Behavior
{
    public int AttachedCalled { get; private set; }
    public int DetachingCalled { get; private set; }
    public int VisualAttachCalled { get; private set; }
    public int VisualDetachCalled { get; private set; }
    public int LogicalAttachCalled { get; private set; }
    public int LogicalDetachCalled { get; private set; }
    public int LoadedCalled { get; private set; }
    public int UnloadedCalled { get; private set; }
    public int InitializedCalled { get; private set; }
    public int DataContextChangedCalled { get; private set; }
    public int ResourcesChangedCalled { get; private set; }
    public int ThemeChangedCalled { get; private set; }

    protected override void OnAttached()
    {
        AttachedCalled++;
    }

    protected override void OnDetaching()
    {
        DetachingCalled++;
    }

    protected override void OnAttachedToVisualTree()
    {
        VisualAttachCalled++;
    }

    protected override void OnDetachedFromVisualTree()
    {
        VisualDetachCalled++;
    }

    protected override void OnAttachedToLogicalTree()
    {
        LogicalAttachCalled++;
    }

    protected override void OnDetachedFromLogicalTree()
    {
        LogicalDetachCalled++;
    }

    protected override void OnLoaded()
    {
        LoadedCalled++;
    }

    protected override void OnUnloaded()
    {
        UnloadedCalled++;
    }

    protected override void OnInitializedEvent()
    {
        InitializedCalled++;
    }

    protected override void OnDataContextChangedEvent()
    {
        DataContextChangedCalled++;
    }

    protected override void OnResourcesChangedEvent()
    {
        ResourcesChangedCalled++;
    }

    protected override void OnActualThemeVariantChangedEvent()
    {
        ThemeChangedCalled++;
    }
}

internal class TestBehaviorOfT : Behavior<Button>
{
    public int AttachedCalled { get; private set; }
    public int DetachingCalled { get; private set; }

    protected override void OnAttached()
    {
        base.OnAttached();
        AttachedCalled++;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        DetachingCalled++;
    }
}

