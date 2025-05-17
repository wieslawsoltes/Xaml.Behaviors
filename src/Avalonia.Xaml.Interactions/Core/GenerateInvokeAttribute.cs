#pragma warning disable MA0048 // File name must match type name
using System;

namespace Avalonia.Xaml.Interactions.Core;

/// <summary>
/// Marks a method for which an <see cref="InvokeMethodAction"/> delegate should be generated.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class GenerateInvokeAttribute : Attribute
{
}
#pragma warning restore MA0048
