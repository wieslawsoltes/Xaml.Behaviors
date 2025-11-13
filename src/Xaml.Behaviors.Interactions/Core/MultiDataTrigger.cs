// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Diagnostics.CodeAnalysis;

namespace Avalonia.Xaml.Interactions.Core;

/// <summary>
/// A behavior that performs actions when all bound data conditions are satisfied.
/// </summary>
[RequiresUnreferencedCode("This functionality is not compatible with trimming.")]
public class MultiDataTrigger : MultiDataTriggerBehavior;
