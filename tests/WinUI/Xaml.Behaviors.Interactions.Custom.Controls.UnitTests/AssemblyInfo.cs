// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Xunit;

// The default WinUI test application loads the control resources (the Uno twin starts the session with an application
// that does): no assembly fixture is needed.
[assembly: CollectionBehavior(DisableTestParallelization = true)]
