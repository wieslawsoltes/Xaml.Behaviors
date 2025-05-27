// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Threading.Tasks;
using Avalonia.Xaml.Interactivity;
using ReactiveUI;

namespace Avalonia.Xaml.Interactions.ReactiveUI;

/// <summary>
/// A behavior that registers a handler for a <see cref="Interaction{TInput,TOutput}"/> and executes its actions when the interaction is triggered.
/// </summary>
public class InteractionTriggerBehavior<TInput, TOutput> : StyledElementTrigger<Visual>
{
    private IDisposable? _disposable;

    /// <summary>
    /// Identifies the <see cref="Interaction"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Interaction<TInput, TOutput>?> InteractionProperty =
#pragma warning disable AVP1002
        AvaloniaProperty.Register<InteractionTriggerBehavior<TInput, TOutput>, Interaction<TInput, TOutput>?>(nameof(Interaction));
#pragma warning restore AVP1002

    /// <summary>
    /// Gets or sets the interaction to register the handler for. This is an avalonia property.
    /// </summary>
    public Interaction<TInput, TOutput>? Interaction
    {
        get => GetValue(InteractionProperty);
        set => SetValue(InteractionProperty, value);
    }

    /// <inheritdoc/>
    protected override void OnAttachedToVisualTree()
    {
        base.OnAttachedToVisualTree();
        
        if (Interaction is null)
        {
            return;
        }

        _disposable = Interaction.RegisterHandler(context =>
        {
            Avalonia.Xaml.Interactivity.Interaction.ExecuteActions(AssociatedObject, Actions, context.Input);
            context.SetOutput(default!);
            return Task.CompletedTask;
        });
    }

    /// <inheritdoc/>
    protected override void OnDetachedFromVisualTree()
    {
        base.OnDetachedFromVisualTree();
        
        _disposable?.Dispose();
    }
}
