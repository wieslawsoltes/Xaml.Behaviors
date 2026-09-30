// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace Avalonia;

/// <summary>
/// The Avalonia <c>Vector</c> used (fully qualified, to avoid the clash with <c>System.Numerics.Vector</c>) by the
/// shared tests. The Uno Platform port maps <c>Vector</c> to <see cref="Windows.Foundation.Point"/>.
/// </summary>
/// <param name="X">The X component.</param>
/// <param name="Y">The Y component.</param>
internal readonly record struct Vector(double X, double Y)
{
    /// <summary>Converts the vector to the <see cref="Windows.Foundation.Point"/> of the Uno Platform port.</summary>
    /// <param name="vector">The vector.</param>
    public static implicit operator Windows.Foundation.Point(Vector vector) => new(vector.X, vector.Y);
}
