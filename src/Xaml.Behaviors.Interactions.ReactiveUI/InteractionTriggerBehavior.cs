// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Threading.Tasks;
#if UNO
using Xaml.Interactivity;
#else
using Avalonia.Xaml.Interactivity;
#endif
using ReactiveUI;

#if UNO
namespace Xaml.Interactions.ReactiveUI;
#else
namespace Avalonia.Xaml.Interactions.ReactiveUI;
#endif

/// <summary>
/// A behavior that registers a handler for a <see cref="Interaction{TInput,TOutput}"/> and executes its actions when the interaction is triggered.
/// </summary>
public partial class InteractionTriggerBehavior<TInput, TOutput> : StyledElementTrigger<Visual>
{
    private IDisposable? _disposable;

    /// <summary>
    /// Gets or sets the interaction to register the handler for. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial Interaction<TInput, TOutput>? Interaction { get; set; }

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
#if UNO
            global::Xaml.Interactivity.Interaction.ExecuteActions(AssociatedObject, Actions, context.Input);
#else
            Avalonia.Xaml.Interactivity.Interaction.ExecuteActions(AssociatedObject, Actions, context.Input);
#endif
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
