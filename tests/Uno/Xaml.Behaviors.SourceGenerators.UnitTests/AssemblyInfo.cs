// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Microsoft.UI.Xaml.Controls;
using Xaml.Behaviors.SourceGenerators;
using Xunit;

// All [UnoHeadlessFact] tests share one UI thread and one window; run them one at a time for determinism.
[assembly: CollectionBehavior(DisableTestParallelization = true)]

// Generated for WinUI framework types (the generated classes live in the namespace of the declaring type).
[assembly: GenerateTypedTrigger(typeof(Button), "Click")]
[assembly: GenerateTypedChangePropertyAction(typeof(TextBlock), "Text")]
[assembly: GenerateTypedDataTrigger(typeof(int))]
[assembly: GeneratePropertyTrigger(typeof(TextBox), "TextProperty")]
[assembly: GenerateEventCommand(typeof(Button), "Click", ParameterPath = "OriginalSource")]
