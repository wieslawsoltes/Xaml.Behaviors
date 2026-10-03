// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.XamlTypeInfo;

namespace Xaml.Behaviors.WinUI.Testing;

/// <summary>
/// The default application of the test session: loads the WinUI control resources (<see cref="XamlControlsResources"/>)
/// and resolves XAML types through the WinUI controls metadata provider and additional providers.
/// </summary>
/// <remarks>
/// A code-only WinUI application must resolve the XAML types itself (the XAML compiler generates this for an
/// application defined in XAML). Derive from this class to customize the test application.
/// </remarks>
public partial class WinUITestApplication : Application, IXamlMetadataProvider
{
    private readonly List<IXamlMetadataProvider> _providers = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="WinUITestApplication"/> class.
    /// </summary>
    public WinUITestApplication()
        : this([])
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WinUITestApplication"/> class.
    /// </summary>
    /// <param name="providers">The factories of the additional XAML metadata providers.</param>
    public WinUITestApplication(IReadOnlyList<Func<IXamlMetadataProvider>> providers)
    {
        ArgumentNullException.ThrowIfNull(providers);

        _providers.Add(new XamlControlsXamlMetaDataProvider());
        foreach (var provider in providers)
        {
            _providers.Add(provider());
        }
    }

    /// <summary>
    /// Loads the WinUI control resources (the application resources are not available in the constructor of a
    /// code-only WinUI application).
    /// </summary>
    /// <param name="args">The launch arguments.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        Resources.MergedDictionaries.Add(new XamlControlsResources());
    }

    /// <inheritdoc />
    public IXamlType? GetXamlType(Type type)
    {
        foreach (var provider in _providers)
        {
            if (provider.GetXamlType(type) is { } xamlType)
            {
                return xamlType;
            }
        }

        return null;
    }

    /// <inheritdoc />
    public IXamlType? GetXamlType(string fullName)
    {
        foreach (var provider in _providers)
        {
            if (provider.GetXamlType(fullName) is { } xamlType)
            {
                return xamlType;
            }
        }

        return null;
    }

    /// <inheritdoc />
    public XmlnsDefinition[] GetXmlnsDefinitions()
    {
        List<XmlnsDefinition> definitions = [];
        foreach (var provider in _providers)
        {
            definitions.AddRange(provider.GetXmlnsDefinitions());
        }

        return [.. definitions];
    }
}
