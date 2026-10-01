# Xaml.Behaviors.Uno.Headless

Headless (offscreen) UI testing for [Uno Platform](https://platform.uno) (WinUI API, Skia renderer),
modeled after `Avalonia.Headless` / `Avalonia.Headless.XUnit`.

| Package | Purpose |
| --- | --- |
| `Xaml.Behaviors.Uno.Headless` | `UnoHeadlessSession`: starts one headless Uno application per process on a dedicated UI thread and runs code on it. Test-framework agnostic. |
| `Xaml.Behaviors.Uno.Headless.XUnit` | xUnit v3 integration: `[UnoHeadlessFact]` and `[UnoHeadlessTheory]` run test methods on the Uno UI thread. |
| `Xaml.Behaviors.Uno.Headless.Host` | The headless Skia host itself (repackaged from the Uno sources, Apache-2.0). Referenced transitively. |

The host binds to Uno internals, so these packages pin an **exact** `Uno.WinUI` version (currently `6.7.135`).
Your test project must use that same Uno version.

## Test project

xUnit v3 test projects are executables, which makes them the Uno "head": the Skia runtime assemblies are
copied to the output automatically.

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <OutputType>Exe</OutputType>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="xunit.v3" Version="3.2.2" />
    <PackageReference Include="xunit.runner.visualstudio" Version="3.1.5" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="18.3.0" />
    <PackageReference Include="Xaml.Behaviors.Uno.Headless.XUnit" Version="x.y.z" />
    <!-- Your library under test (which references Uno.WinUI 6.7.135). -->
    <ProjectReference Include="..\MyLibrary\MyLibrary.csproj" />
  </ItemGroup>
</Project>
```

## Writing tests

```csharp
using Microsoft.UI.Xaml.Controls;
using Xaml.Behaviors.Uno.Headless;
using Xaml.Behaviors.Uno.Headless.XUnit;
using Xunit;

public class ButtonTests
{
    [UnoHeadlessFact]
    public async Task Button_Is_Loaded_And_Measured()
    {
        // The test runs on the Uno UI thread: create and use UI objects directly.
        UnoHeadlessSession session = UnoHeadlessSession.Current;
        Button button = new() { Content = "Click me" };

        await session.ShowAsync(button);   // sets Window.Content and waits for Loaded
        await session.WaitForIdleAsync();  // lets queued layout/dispatcher work run

        Assert.True(button.IsLoaded);
        Assert.True(button.ActualWidth > 0);
    }

    [UnoHeadlessTheory]
    [InlineData("A")]
    [InlineData("B")]
    public void TextBlock_Keeps_Text(string text)
    {
        TextBlock textBlock = new() { Text = text };
        Assert.Equal(text, textBlock.Text);
    }
}
```

The test class is constructed, initialized (`IAsyncLifetime`) and disposed on the UI thread, and awaited
expressions resume on it. All UI tests share one UI thread and one window; async tests that run in
parallel can interleave at `await` points and replace each other's window content, so consider
`[assembly: CollectionBehavior(DisableTestParallelization = true)]`.

### Synchronous tests and input

`Show` and `RunJobs` are the synchronous counterparts of `ShowAsync` and `WaitForIdleAsync` (like Avalonia's
`Dispatcher.UIThread.RunJobs()`): they run the queued UI work (layout, loaded events, bindings, queued callbacks)
on the UI thread before returning. `Keyboard` and `Mouse` simulate input:

```csharp
[UnoHeadlessFact]
public void TextBox_Receives_Typed_Text()
{
    UnoHeadlessSession session = UnoHeadlessSession.Current;
    TextBox textBox = new();
    session.Show(new StackPanel { Children = { textBox } });

    session.Mouse.Click(textBox);                     // window or element relative positions
    session.Keyboard.TypeText("abc");                  // key presses with characters, to the focused element
    session.Keyboard.Press(VirtualKey.Enter);          // PreviewKeyDown, KeyDown, KeyUp

    Assert.Equal("abc", textBox.Text);
}
```

Key events go through the regular Uno Platform keyboard pipeline to the focused element (the root element when nothing
has focus). Mouse input is injected with `InputInjector`; `Mouse` tracks the pointer position so moves are absolute.
Every injected mouse event is one frame (16 ms) after the previous one, so clicks at the same position form double taps;
`Mouse.Wait(duration)` lets time pass without input (for example to start an independent input sequence).

`Mouse.Wheel(notches)` and `Mouse.HorizontalWheel(notches)` raise `PointerWheelChanged` (a wheel delta of 120 per
notch). Mouse events carry keyboard modifiers (`PointerRoutedEventArgs.KeyModifiers`): the modifier keys held with
`Keyboard.KeyDown` (until `Keyboard.KeyUp`, see `Keyboard.Modifiers`) and the modifiers passed to the mouse members:

```csharp
session.Keyboard.KeyDown(VirtualKey.Control);     // also updates the key state (InputKeyboardSource)
session.Mouse.Click(target);                       // KeyModifiers == Control
session.Keyboard.KeyUp(VirtualKey.Control);

session.Mouse.Click(target, modifiers: VirtualKeyModifiers.Shift);
session.Mouse.Wheel(-1, VirtualKeyModifiers.Control);
```

## Configuring the session

The session starts with `UnoHeadlessSessionOptions.Default` (1024x768 raw pixels, scale 1, a minimal
application) the first time a test needs it. To change that, start it from an xUnit assembly fixture,
which runs before any test:

```csharp
[assembly: AssemblyFixture(typeof(UnoSessionFixture))]

public sealed class UnoSessionFixture : IAsyncLifetime
{
    public async ValueTask InitializeAsync() => await UnoHeadlessSession.StartAsync(new UnoHeadlessSessionOptions
    {
        Width = 1280,
        Height = 720,
        Scale = 1.5f,
        ApplicationFactory = () => new MyTestApplication(), // e.g. to add theme resources
    });

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}
```

## Without xUnit

`Xaml.Behaviors.Uno.Headless` works with any test framework (or none):

```csharp
UnoHeadlessSession session = await UnoHeadlessSession.GetOrStartAsync();

double width = await session.RunAsync(async () =>
{
    Border border = await session.ShowAsync(new Border());
    await session.WaitForIdleAsync();
    return border.ActualWidth;
});
```

`RunAsync` overloads accept `Action`, `Func<T>`, `Func<Task>` and `Func<Task<T>>`; they run inline when
called on the UI thread, flow the caller's async-local state, and propagate exceptions and cancellation.
