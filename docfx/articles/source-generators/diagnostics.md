# Source Generator Diagnostics

This page lists the analyzer diagnostics emitted by `Xaml.Behaviors.SourceGenerators` (11.3.9.1) with quick fixes and minimal examples you can copy into a failing project to see the error disappear.

## Quick reference

| Rule | Applies to | When it fires | Typical fix |
| --- | --- | --- | --- |
| XBG001 | Typed Trigger | Event delegate cannot be inspected | Use a normal delegate type with an `Invoke` method (e.g., `EventHandler`) |
| XBG002 | Typed Trigger | Event delegate returns non-`void` | Change the delegate to return `void` |
| XBG003 | Typed Trigger | Event delegate has an `out` parameter | Remove the `out` parameter |
| XBG004 | Typed Trigger | Event name pattern did not match an event | Point the attribute at an existing event |
| XBG005 | ChangePropertyAction | Property name pattern did not match a property | Point the attribute at an existing property with a setter |
| XBG006 | Typed Action | Method name pattern did not match a method | Point the attribute at an existing method |
| XBG007 | Typed Action | Literal method name has multiple overloads | Rename/adjust methods to make the target unambiguous |
| XBG008 | All generators | Generic target/member/type parameter found | Use non-generic types/members for generation |
| XBG009 | Typed Action | Method parameter uses `ref`/`out`/`in` | Pass parameters by value |
| XBG010 | Actions/Triggers/ChangeProperty | Static member targeted | Use instance members |
| XBG011 | Typed MultiDataTrigger | Target type not derived from `StyledElementTrigger` | Derive from `Avalonia.Xaml.Interactivity.StyledElementTrigger` |
| XBG012 | Typed InvokeCommandAction | Target type not derived from `StyledElementAction` | Derive from `Avalonia.Xaml.Interactivity.StyledElementAction` |
| XBG013 | Typed MultiDataTrigger | Missing non-static `bool Evaluate()` | Add `bool Evaluate()` |
| XBG014 | All generators | Target or its types are not accessible | Make members/types public or grant `InternalsVisibleTo` access |
| XBG015 | ChangePropertyAction | Property setter is inaccessible | Expose a public/internal setter |
| XBG016 | MultiDataTrigger/InvokeCommand | Target type is not `partial` | Mark the class `partial` |
| XBG017 | ChangePropertyAction | Property uses an `init`-only setter | Use a settable property |
| XBG018 | MultiDataTrigger/InvokeCommand | Target type is nested | Move the class to the top level |

## Trigger diagnostics (XBG001-XBG004)

### XBG001 Unsupported trigger delegate
The event delegate type cannot be inspected (no usable `Invoke` method was found).

```csharp
using Xaml.Behaviors.SourceGenerators;

public class Sender
{
    // Delegates such as System.Delegate/MulticastDelegate cannot be inspected.
    public event Delegate? Broken;
}

[assembly: GenerateTypedTrigger(typeof(Sender), "Broken")] // XBG001
```

**Fix**: Use a delegate type with a normal `void`-returning `Invoke`.

```csharp
public delegate void ReadyHandler(object? sender, EventArgs args);
public event ReadyHandler? Ready; // OK
```

### XBG002 Unsupported trigger delegate return type
The event delegate returns a value; typed triggers require `void`.

```csharp
public delegate int ReturningHandler(object? sender, EventArgs args);

public class Sender
{
    public event ReturningHandler? Completed;
}

[assembly: GenerateTypedTrigger(typeof(Sender), "Completed")] // XBG002
```

**Fix**: Change the delegate to `void`.

```csharp
public delegate void CompletedHandler(object? sender, EventArgs args);
public event CompletedHandler? Completed; // OK
```

### XBG003 Unsupported trigger delegate parameter
The event delegate has an `out` parameter, which cannot be forwarded by the generated trigger.

```csharp
public delegate void RequestHandler(out string payload);

public class Sender
{
    public event RequestHandler? Requested;
}

[assembly: GenerateTypedTrigger(typeof(Sender), "Requested")] // XBG003
```

**Fix**: Remove the `out` parameter or redesign the event arguments.

```csharp
public delegate void RequestHandler(object? sender, RequestEventArgs args);
```

### XBG004 Trigger event not found
No event matched the name/pattern supplied to `GenerateTypedTrigger`.

```csharp
using Avalonia.Controls;
using Xaml.Behaviors.SourceGenerators;

[assembly: GenerateTypedTrigger(typeof(Button), "Tapped")] // XBG004, Button has Click not Tapped
```

**Fix**: Point the attribute at an existing event or correct the pattern.

```csharp
[assembly: GenerateTypedTrigger(typeof(Button), "Click")] // OK
```

## ChangePropertyAction diagnostics (XBG005, XBG015, XBG017)

### XBG005 Change property target not found
The supplied property name/pattern did not match a writable property.

```csharp
using Avalonia.Controls;
using Xaml.Behaviors.SourceGenerators;

[assembly: GenerateTypedChangePropertyAction(typeof(TextBox), "Forecolor")] // XBG005, property is Foreground
```

**Fix**: Use the correct property name.

```csharp
[assembly: GenerateTypedChangePropertyAction(typeof(TextBox), "Foreground")] // OK
```

### XBG015 Property setter not accessible
The property exists but its setter is not accessible to the generator.

```csharp
public class ViewModel
{
    public string Name { get; private set; } = string.Empty;
}

[assembly: GenerateTypedChangePropertyAction(typeof(ViewModel), "Name")] // XBG015
```

**Fix**: Expose a public/internal setter that the generator can call.

```csharp
public string Name { get; set; } = string.Empty; // OK
```

### XBG017 Init-only setter not supported
The property uses an `init`-only setter, which cannot be set at runtime by the generated action.

```csharp
public class ViewModel
{
    public string Name { get; init; } = string.Empty;
}

[assembly: GenerateTypedChangePropertyAction(typeof(ViewModel), "Name")] // XBG017
```

**Fix**: Switch to a settable property.

```csharp
public string Name { get; set; } = string.Empty; // OK
```

## Action diagnostics (XBG006, XBG007, XBG009)

### XBG006 Action method not found
No method matched the name/pattern supplied to `GenerateTypedAction`.

```csharp
using Xaml.Behaviors.SourceGenerators;

public class ViewModel
{
    public void Submit() { }
}

[assembly: GenerateTypedAction(typeof(ViewModel), "Save*")] // XBG006
```

**Fix**: Point the attribute at an existing method or correct the pattern.

```csharp
[assembly: GenerateTypedAction(typeof(ViewModel), "Submit")] // OK
```

### XBG007 Action method ambiguous
A literal method name matched multiple overloads; the generator cannot choose one.

```csharp
public class ViewModel
{
    [GenerateTypedAction]
    public void Save() { }

    public void Save(string value) { }
}
// XBG007 for Save
```

**Fix**: Remove/rename overloads or split the methods so the target is unique.

### XBG009 Unsupported parameter modifier
The action method uses `ref`, `out`, or `in` parameters, which cannot be passed through the generated action.

```csharp
public class ViewModel
{
    [GenerateTypedAction]
    public void Increment(ref int value) { value++; }
}
// XBG009
```

**Fix**: Pass by value instead.

```csharp
[GenerateTypedAction]
public void Increment(int value) { /* ... */ } // OK
```

## Shape and accessibility diagnostics (XBG008, XBG010, XBG014)

### XBG008 Generic members not supported
The target type or member uses type parameters or generic members.

```csharp
public class ViewModel<T>
{
    [GenerateTypedAction]
    public void Save(T value) { }
}
// XBG008
```

**Fix**: Move generation to non-generic types/members.

### XBG010 Static member not supported
Static methods/properties/events cannot be used by the generators.

```csharp
public class ViewModel
{
    [GenerateTypedAction]
    public static void Submit() { }
}
// XBG010
```

**Fix**: Make the member an instance member.

### XBG014 Member not accessible
The target member or one of its types is not accessible to the generator (non-public and not visible via `InternalsVisibleTo`).

```csharp
internal class ViewModel
{
    [GenerateTypedAction]
    internal void Save() { }
}
// XBG014 when the generator does not have internal visibility
```

**Fix**: Make members/types public, or expose internals to `Xaml.Behaviors.SourceGenerators`.

```csharp
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Xaml.Behaviors.SourceGenerators")]
```

## MultiDataTrigger and InvokeCommand diagnostics (XBG011-XBG013, XBG016, XBG018)

### XBG011 Invalid multi data trigger target
`[GenerateTypedMultiDataTrigger]` must be applied to a type deriving from `Avalonia.Xaml.Interactivity.StyledElementTrigger`.

```csharp
using Xaml.Behaviors.SourceGenerators;

[GenerateTypedMultiDataTrigger]
public partial class InvalidTrigger { } // XBG011
```

**Fix**: Derive from `StyledElementTrigger`.

```csharp
using Avalonia.Xaml.Interactivity;

[GenerateTypedMultiDataTrigger]
public partial class ValidationTrigger : StyledElementTrigger { } // OK
```

### XBG012 Invalid invoke command action target
`[GenerateTypedInvokeCommandAction]` must be applied to a type deriving from `Avalonia.Xaml.Interactivity.StyledElementAction`.

```csharp
[GenerateTypedInvokeCommandAction]
public partial class InvokeFromControl // XBG012
{
}
```

**Fix**: Derive from `StyledElementAction`.

```csharp
using Avalonia.Xaml.Interactivity;

[GenerateTypedInvokeCommandAction]
public partial class InvokeSave : StyledElementAction { } // OK
```

### XBG013 Evaluate method required
`[GenerateTypedMultiDataTrigger]` requires a non-static `bool Evaluate()` method.

```csharp
[GenerateTypedMultiDataTrigger]
public partial class ValidationTrigger : Avalonia.Xaml.Interactivity.StyledElementTrigger
{
    [TriggerProperty] private bool _isValid;
}
// XBG013
```

**Fix**: Add `Evaluate`.

```csharp
private bool Evaluate() => _isValid;
```

### XBG016 Type must be partial
`[GenerateTypedMultiDataTrigger]` and `[GenerateTypedInvokeCommandAction]` target classes must be declared `partial`.

```csharp
[GenerateTypedInvokeCommandAction]
public class SubmitCommandAction : Avalonia.Xaml.Interactivity.StyledElementAction // XBG016
{
}
```

**Fix**: Mark the class `partial`.

```csharp
public partial class SubmitCommandAction : Avalonia.Xaml.Interactivity.StyledElementAction { } // OK
```

### XBG018 Nested types not supported
The target type is nested inside another class/struct.

```csharp
public class Container
{
    [GenerateTypedInvokeCommandAction]
    public partial class NestedAction : Avalonia.Xaml.Interactivity.StyledElementAction { } // XBG018
}
```

**Fix**: Move the type to the top level.

```csharp
[GenerateTypedInvokeCommandAction]
public partial class NestedAction : Avalonia.Xaml.Interactivity.StyledElementAction { } // OK
```
