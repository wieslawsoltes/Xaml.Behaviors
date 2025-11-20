# ExecuteScriptAction

`ExecuteScriptAction` allows you to write and execute C# code snippets directly within your XAML. This is useful for quick prototyping, debugging, or simple logic that doesn't warrant a full ViewModel command.

## Properties

*   **`Script`**: The C# code to execute.
*   **`UseDispatcher`**: If `True` (default), the script runs on the UI thread. If `False`, it runs on a background thread via `Task.Run`.

## Script Context (Globals)

The script has access to a global context object (`ExecuteScriptActionGlobals`) which exposes:

*   **`Sender`**: The object that triggered the action (e.g., the Button).
*   **`Parameter`**: The command parameter (if any).

## Default Imports

The following namespaces are automatically imported for your script:
*   `System`
*   `System.Collections.Generic`
*   `System.Linq`
*   `Avalonia`
*   `Avalonia.Collections`
*   `Avalonia.Controls`
*   `Avalonia.Interactivity`
*   `Avalonia.Metadata`
*   `Avalonia.LogicalTree`
*   `Avalonia.Reactive`
*   `Avalonia.Input`
*   `Avalonia.Markup.Xaml`

## Usage

```xml
<Button Content="Click Me">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <ExecuteScriptAction>
                <ExecuteScriptAction.Script>
                    <![CDATA[
                        var button = Sender as Avalonia.Controls.Button;
                        if (button != null)
                        {
                            button.Content = "Clicked!";
                        }
                        System.Diagnostics.Debug.WriteLine("Script executed!");
                    ]]>
                </ExecuteScriptAction.Script>
            </ExecuteScriptAction>
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```

### Accessing Parameter

```xml
<Button Content="Log Parameter" CommandParameter="Hello World">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <ExecuteScriptAction>
                <ExecuteScriptAction.Script>
                    <![CDATA[
                        System.Diagnostics.Debug.WriteLine("Parameter: " + Parameter);
                    ]]>
                </ExecuteScriptAction.Script>
            </ExecuteScriptAction>
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```
