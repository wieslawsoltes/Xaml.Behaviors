using Avalonia.Markup.Xaml.Templates;
using Avalonia.Metadata;
using Avalonia.Styling;

namespace Avalonia.Xaml.Interactivity;

/// <summary>
/// A template for creating custom objects.
/// </summary>
public class ObjectTemplate : ITemplate
{
    /// <summary>
    /// Gets or sets the content of the template.
    /// </summary>
    [Content]
    [TemplateContent(TemplateResultType = typeof(object))]
    public object? Content { get; set; }

    /// <summary>
    /// Builds the collection of behaviors from the template.
    /// </summary>
    /// <returns>The collection of behaviors created from the template.</returns>
    object? ITemplate.Build() => TemplateContent.Load<object>(Content)?.Result;
}
