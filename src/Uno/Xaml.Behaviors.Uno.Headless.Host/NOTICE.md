# NOTICE

This package (`Xaml.Behaviors.Uno.Headless.Host`) redistributes source code from the
[Uno Platform](https://github.com/unoplatform/uno) project:

- Source: `src/Uno.UI.Runtime.Skia.Headless` (branch `release/stable/6.7`)
- Copyright © Uno Platform Inc. and contributors
- License: Apache License, Version 2.0 — https://www.apache.org/licenses/LICENSE-2.0

The `buildTransitive` props/targets are adapted from the same project
(`buildTransitive/Uno.WinUI.Runtime.Skia.Headless.props/.targets`) and renamed to match this package id.

Modifications made for this redistribution:

- The project file was rewritten to build outside the Uno repository against the published
  `Uno.WinUI` packages: it compiles against the `uno-runtime/net10.0/skia` implementation assemblies.
- The C# sources are kept as close to upstream as possible. They are the `release/stable/6.7` version
  of the host (null-surface renderer), not the one on Uno's `master` branch.
- The host runs its UI thread on its own event loop (`Hosting/HeadlessEventLoop.cs`) instead of the Skia runtime's
  internal `EventLoop`, and exposes `HeadlessHost.RunJobs()` to drain the queued UI work synchronously.
- The host registers a keyboard input source (`Hosting/HeadlessKeyboardInputSource.cs`) and exposes
  `HeadlessHost.RaiseKey()`/`RaiseCharacter()`, so keyboard input reaches the focused element like on other hosts.
- The host exposes `HeadlessHost.InjectMouseInput()` (`Hosting/HeadlessMouseInput.cs`): mouse input injected with
  keyboard modifiers (the public `InputInjector.InjectMouseInput` raises pointer events without modifiers) and with the
  WinUI wheel rotation in `MouseData` (Uno Platform reads it from `DeltaY`/`DeltaX`).

Unless required by applicable law or agreed to in writing, software distributed under the Apache
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either
express or implied. See the License for the specific language governing permissions and limitations
under the License.

The rest of the Xaml.Behaviors project is licensed under the MIT license.
