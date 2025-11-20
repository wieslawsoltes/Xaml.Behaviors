# CarouselSelectionChangedTrigger

The `CarouselSelectionChangedTrigger` is a specialized trigger that executes actions when the `SelectionChanged` event is raised by a `Carousel` (or any `SelectingItemsControl`).

### Usage Example

```xml
<Carousel>
    <Interaction.Behaviors>
        <CarouselSelectionChangedTrigger>
            <InvokeCommandAction Command="{Binding PageChangedCommand}" />
        </CarouselSelectionChangedTrigger>
    </Interaction.Behaviors>
    <!-- Items -->
</Carousel>
```
