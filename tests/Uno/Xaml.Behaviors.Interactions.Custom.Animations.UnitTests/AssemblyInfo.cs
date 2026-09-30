// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Xunit;

// All [UnoHeadlessFact] tests share one UI thread and one window; run them one at a time for determinism.
[assembly: CollectionBehavior(DisableTestParallelization = true)]
