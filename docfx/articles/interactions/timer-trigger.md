# TimerTrigger

`TimerTrigger` is a trigger that invokes its actions after a specified interval. It can be configured to fire once, a specific number of times, or indefinitely.

## Properties

*   **`MillisecondsPerTick`**: The time, in milliseconds, between timer ticks. Default is 1000ms.
*   **`TotalTicks`**: The number of ticks after which the trigger stops firing. Default is 1.
*   **`RepeatForever`**: A value indicating whether the timer repeats indefinitely. If set to `True`, `TotalTicks` is ignored.

## Usage

```xml
<TextBlock Text="Tick Tock">
    <Interaction.Behaviors>
        <TimerTrigger MillisecondsPerTick="1000" RepeatForever="True">
            <CallMethodAction TargetObject="{Binding}" MethodName="OnTick" />
        </TimerTrigger>
    </Interaction.Behaviors>
</TextBlock>
```
