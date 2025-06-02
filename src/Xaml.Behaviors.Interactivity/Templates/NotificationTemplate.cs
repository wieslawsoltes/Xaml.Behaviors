using Avalonia.Controls.Notifications;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Metadata;
using Avalonia.Styling;

namespace Avalonia.Xaml.Interactivity;

/// <summary>
/// A template for creating notifications.
/// </summary>
public class NotificationTemplate : ITemplate
{
    /// <summary>
    /// Gets or sets the content of the template.
    /// </summary>
    [Content]
    [TemplateContent(TemplateResultType = typeof(INotification))]
    public object? Content { get; set; }

    /// <summary>
    /// Builds the collection of behaviors from the template.
    /// </summary>
    /// <returns>The collection of behaviors created from the template.</returns>
    object? ITemplate.Build() => TemplateContent.Load<INotification>(Content)?.Result;
}
