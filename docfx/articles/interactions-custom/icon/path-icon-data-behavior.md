# PathIconDataBehavior

Attaches to a `PathIcon` and sets its `Data` property to the specified `Geometry`. When detached, it restores the original `Data`.

### Properties
- `Data`: The `Geometry` to apply to the `PathIcon`.

### Usage Example

```xml
<PathIcon>
    <Interaction.Behaviors>
        <PathIconDataBehavior Data="{StaticResource MyIconGeometry}" />
    </Interaction.Behaviors>
</PathIcon>
```
