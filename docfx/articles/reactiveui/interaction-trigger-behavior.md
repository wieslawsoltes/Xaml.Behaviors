# InteractionTriggerBehavior

`InteractionTriggerBehavior<TInput, TOutput>` is a behavior that subscribes to a ReactiveUI `Interaction<TInput, TOutput>` in your ViewModel. When the interaction is raised, the behavior executes its associated actions.

## Properties

*   **`Interaction`**: The `Interaction<TInput, TOutput>` instance to observe.

## Usage

To use this behavior in XAML, you must specify the type arguments for the input and output types of the interaction.

### ViewModel

```csharp
public class MainViewModel : ReactiveObject
{
    public Interaction<string, bool> ConfirmInteraction { get; } = new Interaction<string, bool>();

    public ReactiveCommand<Unit, Unit> DeleteCommand { get; }

    public MainViewModel()
    {
        DeleteCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var result = await ConfirmInteraction.Handle("Are you sure you want to delete this?");
            if (result)
            {
                // Delete item
            }
        });
    }
}
```

### View

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:sys="clr-namespace:System;assembly=mscorlib"
             x:Class="Sample.Views.MainView">
    
    <Interaction.Behaviors>
        <InteractionTriggerBehavior x:TypeArguments="sys:String, sys:Boolean" 
                                    Interaction="{Binding ConfirmInteraction}">
            <!-- Actions to execute when interaction is raised -->
            <!-- For example, showing a dialog -->
        </InteractionTriggerBehavior>
    </Interaction.Behaviors>

    <Button Command="{Binding DeleteCommand}" Content="Delete" />
</UserControl>
```

Note that `InteractionTriggerBehavior` handles the interaction by executing its actions. However, standard actions like `InvokeCommandAction` or `ChangePropertyAction` do not return values to the interaction. If you need to return a result to the ViewModel (the `TOutput`), you might need a custom action or a different approach, as standard XAML behaviors are typically "fire and forget".

The `InteractionTriggerBehavior` automatically calls `SetOutput(default!)` on the interaction context after executing actions. This is important to ensure the `await` in the ViewModel completes.
