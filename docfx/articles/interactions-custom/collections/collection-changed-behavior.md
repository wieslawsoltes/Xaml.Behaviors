# CollectionChangedBehavior

This behavior allows you to define different sets of actions to execute depending on the type of change that occurred in an `INotifyCollectionChanged` collection (e.g., `ObservableCollection<T>`).

### Properties
*   `Collection`: The collection to observe.
*   `AddedActions`: Actions to execute when items are added.
*   `RemovedActions`: Actions to execute when items are removed.
*   `ResetActions`: Actions to execute when the collection is reset (cleared).

### Example

```xml
<UserControl.Resources>
    <CollectionChangedBehavior x:Key="LogChangesBehavior" Collection="{Binding MyItems}">
        <CollectionChangedBehavior.AddedActions>
            <CallMethodAction TargetObject="{Binding}" MethodName="OnItemsAdded" />
        </CollectionChangedBehavior.AddedActions>
        
        <CollectionChangedBehavior.RemovedActions>
            <CallMethodAction TargetObject="{Binding}" MethodName="OnItemsRemoved" />
        </CollectionChangedBehavior.RemovedActions>
        
        <CollectionChangedBehavior.ResetActions>
            <CallMethodAction TargetObject="{Binding}" MethodName="OnCollectionReset" />
        </CollectionChangedBehavior.ResetActions>
    </CollectionChangedBehavior>
</UserControl.Resources>

<Grid>
    <Interaction.Behaviors>
        <Binding Source="{StaticResource LogChangesBehavior}" />
    </Interaction.Behaviors>
</Grid>
```
