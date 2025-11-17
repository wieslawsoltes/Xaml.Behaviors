// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;

namespace Avalonia.Xaml.Behaviors.Storyboards;

/// <summary>
/// Represents a parsed storyboard target property path.
/// </summary>
public sealed class StoryboardPropertyPath
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StoryboardPropertyPath"/> class.
    /// </summary>
    /// <param name="originalPath">The original string that was parsed.</param>
    /// <param name="segments">The parsed path segments.</param>
    public StoryboardPropertyPath(string originalPath, IReadOnlyList<StoryboardPropertyPathSegment> segments)
    {
        OriginalPath = originalPath ?? throw new ArgumentNullException(nameof(originalPath));
        Segments = segments ?? throw new ArgumentNullException(nameof(segments));

        if (segments.Count == 0)
        {
            throw new ArgumentException("A property path must contain at least one segment.", nameof(segments));
        }
    }

    /// <summary>
    /// Gets the original string representation of the property path.
    /// </summary>
    public string OriginalPath { get; }

    /// <summary>
    /// Gets the segments that compose the property path.
    /// </summary>
    public IReadOnlyList<StoryboardPropertyPathSegment> Segments { get; }

    /// <summary>
    /// Parses the provided string into a <see cref="StoryboardPropertyPath"/>.
    /// </summary>
    /// <param name="path">The storyboard target property path string.</param>
    /// <returns>The parsed <see cref="StoryboardPropertyPath"/>.</returns>
    public static StoryboardPropertyPath Parse(string path)
    {
        var segments = StoryboardPropertyPathParser.Parse(path);
        return new StoryboardPropertyPath(path, segments);
    }
}

/// <summary>
/// Represents a single segment inside a storyboard target property path.
/// </summary>
public sealed class StoryboardPropertyPathSegment
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StoryboardPropertyPathSegment"/> class.
    /// </summary>
    /// <param name="propertyName">The property or member name.</param>
    /// <param name="ownerTypeName">An optional owner type (used for attached properties).</param>
    /// <param name="indexerArguments">Indexer arguments applied to the segment.</param>
    public StoryboardPropertyPathSegment(string propertyName, string? ownerTypeName, IReadOnlyList<string> indexerArguments)
    {
        PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
        OwnerTypeName = ownerTypeName;
        IndexerArguments = indexerArguments ?? throw new ArgumentNullException(nameof(indexerArguments));
    }

    /// <summary>
    /// Gets the property or member name for the segment.
    /// </summary>
    public string PropertyName { get; }

    /// <summary>
    /// Gets the optional owner type name for attached properties.
    /// </summary>
    public string? OwnerTypeName { get; }

    /// <summary>
    /// Gets the indexer arguments applied to the segment.
    /// </summary>
    public IReadOnlyList<string> IndexerArguments { get; }

    /// <summary>
    /// Gets a value indicating whether the segment refers to an attached property.
    /// </summary>
    public bool IsAttached => !string.IsNullOrWhiteSpace(OwnerTypeName);

    /// <summary>
    /// Gets a value indicating whether the segment includes indexer arguments.
    /// </summary>
    public bool HasIndexers => IndexerArguments.Count > 0;
}
