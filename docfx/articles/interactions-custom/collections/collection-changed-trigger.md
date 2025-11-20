# CollectionChangedTrigger

This trigger fires whenever the `CollectionChanged` event is raised by the bound collection. Unlike `CollectionChangedBehavior`, it does not distinguish between add/remove/reset events; it simply executes its actions for *any* change.

### Properties
*   `Collection`: The `INotifyCollectionChanged` object to observe.

### Example

```xml
<ListBox ItemsSource="{Binding MyItems}">
    <Interaction.Behaviors>
        <CollectionChangedTrigger Collection="{Binding MyItems}">
            <InvokeCommandAction Command="{Binding UpdateCountCommand}" />
        </CollectionChangedTrigger>
    </Interaction.Behaviors>
</ListBox>
```
