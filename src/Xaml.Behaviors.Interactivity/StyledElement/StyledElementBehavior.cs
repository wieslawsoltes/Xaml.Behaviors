// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Diagnostics;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Reactive;

namespace Avalonia.Xaml.Interactivity;

/// <summary>
/// A base class for behaviors, implementing the basic plumbing of <see cref="IBehavior"/>.
/// </summary>
public abstract class StyledElementBehavior : StyledElement, IBehavior, IBehaviorEventsHandler
{
    private IDisposable? _dataContextDisposable;
    private bool _isAttachedToLogicalTree;
    private bool _isAttachedToVisualTree;
    private bool _isInitializedNotified;
    private bool _isLoaded;

    /// <summary>
    /// Identifies the <seealso cref="IsEnabled"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> IsEnabledProperty =
        AvaloniaProperty.Register<StyledElementBehavior, bool>(nameof(IsEnabled), defaultValue: true);

    /// <summary>
    /// Gets the <see cref="AvaloniaObject"/> to which the behavior is attached.
    /// </summary>
    public AvaloniaObject? AssociatedObject { get; private set; }

    /// <summary>
    /// Gets the <see cref="StyledElement"/> to which this behavior is attached.
    /// </summary>
    public StyledElement? AssociatedStyledElement => AssociatedObject as StyledElement;

    /// <summary>
    /// Gets or sets a value indicating whether this instance is enabled.
    /// </summary>
    /// <value><c>true</c> if this instance is enabled; otherwise, <c>false</c>.</value>
    public bool IsEnabled
    {
        get => GetValue(IsEnabledProperty);
        set => SetValue(IsEnabledProperty, value);
    }

    /// <summary>
    /// Attaches the behavior to the specified <see cref="AvaloniaObject"/>.
    /// </summary>
    /// <param name="associatedObject">The <see cref="AvaloniaObject"/> to which to attach.</param>
    /// <exception cref="ArgumentNullException"><paramref name="associatedObject"/> is null.</exception>
    public void Attach(AvaloniaObject? associatedObject)
    {
        if (Equals(associatedObject, AssociatedObject))
        {
            return;
        }

        if (AssociatedObject is not null)
        {
            throw new InvalidOperationException(string.Format(
                CultureInfo.CurrentCulture,
                "An instance of a behavior cannot be attached to more than one object at a time."));
        }

        Debug.Assert(associatedObject is not null, "Cannot attach the behavior to a null object.");
        AssociatedObject = associatedObject ?? throw new ArgumentNullException(nameof(associatedObject));
        _dataContextDisposable = SynchronizeDataContext(associatedObject);
        
        // NOTE: Special case handling for TopLevel as it does not trigger attached to logical or visual tree events.
        if (AssociatedObject is TopLevel)
        {
            AttachBehaviorToLogicalTree();
        }

        OnAttached();
    }

    /// <summary>
    /// Detaches the behaviors from the <see cref="AssociatedObject"/>.
    /// </summary>
    public void Detach()
    {
        OnDetaching();

        if (Parent is not null || TemplatedParent is not null)
        {
            DetachBehaviorFromLogicalTree();
        }

        _dataContextDisposable?.Dispose();
        _isAttachedToLogicalTree = false;
        _isAttachedToVisualTree = false;
        _isInitializedNotified = false;
        _isLoaded = false;
        AssociatedObject = null;
    }

    /// <summary>
    /// Called after the behavior is attached to the <see cref="AssociatedObject"/>.
    /// </summary>
    /// <remarks>
    /// Override this to hook up functionality to the <see cref="AssociatedObject"/>
    /// </remarks>
    protected virtual void OnAttached()
    {
    }

    /// <summary>
    /// Called when the behavior is being detached from its <see cref="AssociatedObject"/>.
    /// </summary>
    /// <remarks>
    /// Override this to unhook functionality from the <see cref="AssociatedObject"/>
    /// </remarks>
    protected virtual void OnDetaching()
    {
    }

    void IBehaviorEventsHandler.AttachedToVisualTreeEventHandler()
    {
        if (_isAttachedToVisualTree)
        {
            return;
        }

        _isAttachedToVisualTree = true;
        AttachBehaviorToLogicalTree();

        OnAttachedToVisualTree();
    }

    void IBehaviorEventsHandler.DetachedFromVisualTreeEventHandler()
    {
        if (!_isAttachedToVisualTree)
        {
            return;
        }

        _isAttachedToVisualTree = false;
        try
        {
            OnDetachedFromVisualTree();
        }
        finally
        {
            if (AssociatedObject is not TopLevel)
            {
                DetachBehaviorFromLogicalTree();
            }
        }
    }

    void IBehaviorEventsHandler.AttachedToLogicalTreeEventHandler()
    {
        if (_isAttachedToLogicalTree)
        {
            return;
        }

        _isAttachedToLogicalTree = true;
        AttachBehaviorToLogicalTree();

        OnAttachedToLogicalTree();
    }

    void IBehaviorEventsHandler.DetachedFromLogicalTreeEventHandler()
    {
        if (!_isAttachedToLogicalTree)
        {
            return;
        }

        _isAttachedToLogicalTree = false;
        try
        {
            OnDetachedFromLogicalTree();
        }
        finally
        {
            if (AssociatedObject is not TopLevel)
            {
                DetachBehaviorFromLogicalTree();
            }
        }
    }

    void IBehaviorEventsHandler.LoadedEventHandler()
    {
        if (_isLoaded)
        {
            return;
        }

        _isLoaded = true;
        OnLoaded();
    }

    void IBehaviorEventsHandler.UnloadedEventHandler()
    {
        if (!_isLoaded)
        {
            return;
        }

        _isLoaded = false;
        OnUnloaded();
    }

    void IBehaviorEventsHandler.InitializedEventHandler()
    {
        if (_isInitializedNotified)
        {
            return;
        }

        _isInitializedNotified = true;
        Initialize();

        OnInitializedEvent();
    }

    void IBehaviorEventsHandler.DataContextChangedEventHandler() => OnDataContextChangedEvent();

    void IBehaviorEventsHandler.ResourcesChangedEventHandler() => OnResourcesChangedEvent();

    void IBehaviorEventsHandler.ActualThemeVariantChangedEventHandler() => OnActualThemeVariantChangedEvent();

    /// <summary>
    /// Called after the <see cref="AssociatedObject"/> is attached to the visual tree.
    /// </summary>
    /// <remarks>
    /// Invoked only when the <see cref="AssociatedObject"/> is of type <see cref="Visual"/>.
    /// </remarks>
    protected virtual void OnAttachedToVisualTree()
    {
    }

    /// <summary>
    /// Called when the <see cref="AssociatedObject"/> is being detached from the visual tree.
    /// </summary>
    /// <remarks>
    /// Invoked only when the <see cref="AssociatedObject"/> is of type <see cref="Visual"/>.
    /// </remarks>
    protected virtual void OnDetachedFromVisualTree()
    {
    }

    /// <summary>
    /// Called after the <see cref="AssociatedObject"/> is attached to the logical tree.
    /// </summary>
    /// <remarks>
    /// Invoked only when the <see cref="AssociatedObject"/> is of type <see cref="StyledElement"/>.
    /// </remarks>
    protected virtual void OnAttachedToLogicalTree()
    {
    }

    /// <summary>
    /// Called when the <see cref="AssociatedObject"/> is being detached from the logical tree.
    /// </summary>
    /// <remarks>
    /// Invoked only when the <see cref="AssociatedObject"/> is of type <see cref="StyledElement"/>.
    /// </remarks>
    protected virtual void OnDetachedFromLogicalTree()
    {
    }

    /// <summary>
    /// Called after the <see cref="AssociatedObject"/> is loaded.
    /// </summary>
    /// <remarks>
    /// Invoked only when the <see cref="AssociatedObject"/> is of type <see cref="Control"/>.
    /// </remarks>
    protected virtual void OnLoaded()
    {
    }

    /// <summary>
    /// Called when the <see cref="AssociatedObject"/> is unloaded.
    /// </summary>
    /// <remarks>
    /// Invoked only when the <see cref="AssociatedObject"/> is of type <see cref="Control"/>.
    /// </remarks>
    protected virtual void OnUnloaded()
    {
    }

    /// <summary>
    /// Called when the <see cref="AssociatedObject"/> is initialized.
    /// </summary>
    /// <remarks>
    /// Invoked only when the <see cref="AssociatedObject"/> is of type <see cref="StyledElement"/>.
    /// </remarks>
    protected virtual void OnInitializedEvent()
    {
    }

    /// <summary>
    /// Called when the <see cref="AssociatedObject"/> DataContext changed.
    /// </summary>
    /// <remarks>
    /// Invoked only when the <see cref="AssociatedObject"/> is of type <see cref="StyledElement"/>.
    /// </remarks>
    protected virtual void OnDataContextChangedEvent()
    {
    }

    /// <summary>
    /// Called when the <see cref="AssociatedObject"/> Resources changed.
    /// </summary>
    /// <remarks>
    /// Invoked only when the <see cref="AssociatedObject"/> is of type <see cref="StyledElement"/>.
    /// </remarks>
    protected virtual void OnResourcesChangedEvent()
    {
    }

    /// <summary>
    /// Called when the <see cref="AssociatedObject"/> ActualThemeVariant changed.
    /// </summary>
    /// <remarks>
    /// Invoked only when the <see cref="AssociatedObject"/> is of type <see cref="StyledElement"/>.
    /// </remarks>
    protected virtual void OnActualThemeVariantChangedEvent()
    {
    }

    internal virtual void Initialize()
    {
        InitializeIfNeeded();
    }

    internal virtual void AttachBehaviorToLogicalTree()
    {
        StyledElement? parent = null;
        AvaloniaObject? templatedParent = null;

        if (AssociatedObject is TopLevel topLevel)
        {
            parent = topLevel;
            templatedParent = topLevel.TemplatedParent;
        }
        else if (AssociatedObject is FlyoutBase { Target: { } target })
        {
            parent = target;
            templatedParent = target.TemplatedParent;
        }
        else if (AssociatedObject is StyledElement styledElement && styledElement.Parent is not null)
        {
            parent = styledElement;
            templatedParent = styledElement.TemplatedParent;
        }

        if (parent is null)
        {
            return;
        }

        // Required for $parent binding in XAML
        ((ISetLogicalParent)this).SetParent(parent);

        // Required for TemplateBinding in XAML
        if (templatedParent is not null)
        {
            TemplatedParentHelper.SetTemplatedParent(this, templatedParent);
        }
    }

    internal virtual void DetachBehaviorFromLogicalTree()
    {
        ((ISetLogicalParent)this).SetParent(null);

        if (TemplatedParent is not null)
        {
            TemplatedParentHelper.SetTemplatedParent(this, null);
        }
    }

    private IDisposable? SynchronizeDataContext(AvaloniaObject associatedObject)
    {
        if (associatedObject is StyledElement styledElement)
        {
#if false
            // Set initial data context value immediately so bindings are
            // available before the first DataContextChanged event is raised.
            SetCurrentValue(DataContextProperty, styledElement.DataContext);
#endif
            // Required for data context binding in XAML
            return styledElement
                .GetObservable(DataContextProperty)
                .Subscribe(new AnonymousObserver<object?>(x =>
                {
                    SetCurrentValue(DataContextProperty, x);
                }));
        }

        if (associatedObject is FlyoutBase flyout)
        {
            IDisposable? targetDataContextSubscription = null;

            void TargetChanged(Control? target)
            {
                targetDataContextSubscription?.Dispose();
                targetDataContextSubscription = null;

                if (Parent is not null || TemplatedParent is not null)
                {
                    DetachBehaviorFromLogicalTree();
                }

                if (target is null)
                {
                    SetCurrentValue(DataContextProperty, null);
                    return;
                }

                AttachBehaviorToLogicalTree();
                targetDataContextSubscription = target
                    .GetObservable(DataContextProperty)
                    .Subscribe(new AnonymousObserver<object?>(x =>
                    {
                        SetCurrentValue(DataContextProperty, x);
                    }));
            }

            var targetSubscription = flyout
                .GetObservable(FlyoutBase.TargetProperty)
                .Subscribe(new AnonymousObserver<Control?>(TargetChanged));

            return DisposableAction.Create(() =>
            {
                targetSubscription.Dispose();
                targetDataContextSubscription?.Dispose();
            });
        }

        return default;
    }
}
