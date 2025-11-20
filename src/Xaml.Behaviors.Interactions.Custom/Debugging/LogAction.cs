using System;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

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
public class LogAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <seealso cref="Message"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> MessageProperty =
        AvaloniaProperty.Register<LogAction, string?>(nameof(Message));

    /// <summary>
    /// Identifies the <seealso cref="Argument"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<object?> ArgumentProperty =
        AvaloniaProperty.Register<LogAction, object?>(nameof(Argument));

    /// <summary>
    /// Identifies the <seealso cref="Level"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<LogActionLevel> LevelProperty =
        AvaloniaProperty.Register<LogAction, LogActionLevel>(nameof(Level), LogActionLevel.Info);

    /// <summary>
    /// Gets or sets the message format string.
    /// </summary>
    public string? Message
    {
        get => GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    /// <summary>
    /// Gets or sets an optional argument to format the message with.
    /// </summary>
    public object? Argument
    {
        get => GetValue(ArgumentProperty);
        set => SetValue(ArgumentProperty, value);
    }

    /// <summary>
    /// Gets or sets the log level.
    /// </summary>
    public LogActionLevel Level
    {
        get => GetValue(LevelProperty);
        set => SetValue(LevelProperty, value);
    }

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
