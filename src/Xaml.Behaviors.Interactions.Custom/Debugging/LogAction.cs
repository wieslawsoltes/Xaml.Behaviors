using System;
using System.Diagnostics;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Specifies the severity level of the log message.
/// </summary>
public enum LogActionLevel
{
    /// <summary>
    /// Information level.
    /// </summary>
    Info,

    /// <summary>
    /// Warning level.
    /// </summary>
    Warning,

    /// <summary>
    /// Error level.
    /// </summary>
    Error,

    /// <summary>
    /// Debug level.
    /// </summary>
    Debug
}

/// <summary>
/// An action that logs a message to the debug output.
/// </summary>
public partial class LogAction : StyledElementAction
{

    /// <summary>
    /// Gets or sets the message format string.
    /// </summary>
    [StyledProperty]
    public partial string? Message { get; set; }

    /// <summary>
    /// Gets or sets an optional argument to format the message with.
    /// </summary>
    [StyledProperty]
    public partial object? Argument { get; set; }

    /// <summary>
    /// Gets or sets the log level.
    /// </summary>
    [StyledProperty(DefaultValue = LogActionLevel.Info)]
    public partial LogActionLevel Level { get; set; }

    /// <inheritdoc />
    public override object? Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return null;
        }

        var message = Message;
        var argument = Argument;
        string output;

        if (message is null)
        {
            output = argument?.ToString() ?? "LogAction invoked with no message or argument.";
        }
        else
        {
            try
            {
                output = argument != null ? string.Format(message, argument) : message;
            }
            catch (FormatException)
            {
                output = message;
            }
        }

        var level = Level;
        var category = "LogAction";

        switch (level)
        {
            case LogActionLevel.Info:
                Trace.TraceInformation($"{category}: {output}");
                break;
            case LogActionLevel.Warning:
                Trace.TraceWarning($"{category}: {output}");
                break;
            case LogActionLevel.Error:
                Trace.TraceError($"{category}: {output}");
                break;
            case LogActionLevel.Debug:
                Debug.WriteLine($"{category}: {output}");
                break;
        }

        // Also write to Console for visibility in some environments
        Console.WriteLine($"[{level}] {category}: {output}");

        return true;
    }
}
