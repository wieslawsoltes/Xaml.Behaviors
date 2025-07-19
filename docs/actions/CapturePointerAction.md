# CapturePointerAction

Namespace: `Avalonia.Xaml.Interactions.Custom`

Captures the pointer (mouse, touch) to a target control so that subsequent pointer events are routed there.

Example: [PointerTriggersView.axaml](samples/BehaviorsTestApplication/Views/Pages/PointerTriggersView.axaml)

## Key Properties
None

## XAML Usage
```xaml
<Interaction.Triggers>
  <EventTrigger>
    <Avalonia.Xaml.Interactions.Custom:CapturePointerAction/>
  </EventTrigger>
</Interaction.Triggers>
```
