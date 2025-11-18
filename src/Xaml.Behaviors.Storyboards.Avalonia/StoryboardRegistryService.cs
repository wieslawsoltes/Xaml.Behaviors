// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.LogicalTree;

namespace Avalonia.Xaml.Behaviors.Storyboards;

/// <summary>
/// Maintains a lookup table of running storyboard instances keyed by host element and storyboard key.
/// </summary>
public sealed class StoryboardRegistryService
{
    private readonly object _gate = new();
    private readonly Dictionary<StyledElement, HostRegistration> _hosts =
        new(ReferenceEqualityComparer.Instance);

    /// <summary>
    /// Gets the shared singleton instance used by storyboard actions.
    /// </summary>
    public static StoryboardRegistryService Instance { get; } = new();

    /// <summary>
    /// Registers a storyboard instance for the specified host and key.
    /// </summary>
    /// <param name="host">The host element that owns the storyboard.</param>
    /// <param name="key">The storyboard key.</param>
    /// <param name="instance">The storyboard instance to store.</param>
    /// <returns>The registered instance.</returns>
    public StoryboardInstance Register(StyledElement host, string key, StoryboardInstance instance)
    {
        if (host is null)
        {
            throw new ArgumentNullException(nameof(host));
        }

        if (instance is null)
        {
            throw new ArgumentNullException(nameof(instance));
        }

        if (!ReferenceEquals(host, instance.Host))
        {
            throw new InvalidOperationException("StoryboardInstance.Host must match the registration host.");
        }

        key = NormalizeKey(key);

        lock (_gate)
        {
            if (!_hosts.TryGetValue(host, out var registration))
            {
                registration = new HostRegistration(this, host);
                _hosts.Add(host, registration);
            }

            registration.Register(key, instance);
            return instance;
        }
    }

    /// <summary>
    /// Attempts to retrieve the most recent storyboard instance for the specified host and key.
    /// </summary>
    /// <param name="host">The host element.</param>
    /// <param name="key">The storyboard key.</param>
    /// <param name="instance">When this method returns, contains the resolved instance.</param>
    /// <returns><c>true</c> if an instance was found; otherwise, <c>false</c>.</returns>
    public bool TryGet(StyledElement host, string key, out StoryboardInstance? instance)
    {
        if (host is null)
        {
            throw new ArgumentNullException(nameof(host));
        }

        key = NormalizeKey(key);

        lock (_gate)
        {
            if (_hosts.TryGetValue(host, out var registration))
            {
                return registration.TryGet(key, out instance);
            }
        }

        instance = null;
        return false;
    }

    /// <summary>
    /// Removes storyboard instances for the specified host and key.
    /// </summary>
    /// <param name="host">The host element.</param>
    /// <param name="key">The storyboard key.</param>
    /// <param name="instance">
    /// The specific instance to remove. When <c>null</c>, all instances for the key are removed.
    /// </param>
    /// <returns><c>true</c> if an entry was removed; otherwise, <c>false</c>.</returns>
    public bool Remove(StyledElement host, string key, StoryboardInstance? instance = null)
    {
        if (host is null)
        {
            throw new ArgumentNullException(nameof(host));
        }

        key = NormalizeKey(key);

        lock (_gate)
        {
            if (!_hosts.TryGetValue(host, out var registration))
            {
                return false;
            }

            var removed = registration.Remove(key, instance);
            if (registration.IsEmpty)
            {
                registration.Dispose();
                _hosts.Remove(host);
            }

            return removed;
        }
    }

    /// <summary>
    /// Clears all storyboard registrations for the specified host.
    /// </summary>
    /// <param name="host">The host element.</param>
    public void CleanupHost(StyledElement host)
    {
        if (host is null)
        {
            throw new ArgumentNullException(nameof(host));
        }

        lock (_gate)
        {
            if (_hosts.TryGetValue(host, out var registration))
            {
                _hosts.Remove(host);
                registration.Dispose();
            }
        }
    }

    /// <summary>
    /// Clears every registered host and disposes their instances.
    /// </summary>
    public void Clear()
    {
        lock (_gate)
        {
            foreach (var registration in _hosts.Values)
            {
                registration.Dispose();
            }

            _hosts.Clear();
        }
    }

    private static string NormalizeKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("Storyboard keys cannot be null or whitespace.", nameof(key));
        }

        return key.Trim();
    }

    private void OnHostDetached(object? sender, LogicalTreeAttachmentEventArgs e)
    {
        if (sender is StyledElement host)
        {
            CleanupHost(host);
        }
    }

    private sealed class HostRegistration : IDisposable
    {
        private readonly StoryboardRegistryService _owner;
        private readonly StyledElement _host;
        private readonly Dictionary<string, List<StoryboardInstance>> _registrations = new(StringComparer.Ordinal);
        private int _nextLayer;
        private bool _isDisposed;

        public HostRegistration(StoryboardRegistryService owner, StyledElement host)
        {
            _owner = owner;
            _host = host;
            _host.DetachedFromLogicalTree += owner.OnHostDetached;
        }

        public bool IsEmpty => _registrations.Count == 0;

        public void Register(string key, StoryboardInstance instance)
        {
            if (!_registrations.TryGetValue(key, out var list))
            {
                list = new List<StoryboardInstance>();
                _registrations.Add(key, list);
            }

            if (instance.HandoffBehavior == StoryboardHandoffBehavior.SnapshotAndReplace && list.Count > 0)
            {
                DisposeList(list);
                list.Clear();
            }

            _nextLayer++;
            instance.Layer = _nextLayer;
            list.Add(instance);

        }

        public bool TryGet(string key, out StoryboardInstance? instance)
        {
            if (_registrations.TryGetValue(key, out var list) && list.Count > 0)
            {
                instance = list[list.Count - 1];
                return true;
            }

            instance = null;
            return false;
        }

        public bool Remove(string key, StoryboardInstance? instance)
        {
            if (!_registrations.TryGetValue(key, out var list) || list.Count == 0)
            {
                return false;
            }

            if (instance is null)
            {
                DisposeList(list);
                _registrations.Remove(key);
                return true;
            }

            for (var i = list.Count - 1; i >= 0; i--)
            {
                if (!ReferenceEquals(list[i], instance))
                {
                    continue;
                }

                list.RemoveAt(i);
                instance.Dispose();

                if (list.Count == 0)
                {
                    _registrations.Remove(key);
                }

                return true;
            }

            return false;
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            _host.DetachedFromLogicalTree -= _owner.OnHostDetached;

            foreach (var list in _registrations.Values)
            {
                DisposeList(list);
            }

            _registrations.Clear();
        }

        private static void DisposeList(List<StoryboardInstance> list)
        {
            foreach (var instance in list)
            {
                instance.Dispose();
            }
        }
    }

    private sealed class ReferenceEqualityComparer : IEqualityComparer<StyledElement>
    {
        public static ReferenceEqualityComparer Instance { get; } = new();

        public bool Equals(StyledElement? x, StyledElement? y) => ReferenceEquals(x, y);

        public int GetHashCode(StyledElement obj) => RuntimeHelpers.GetHashCode(obj);
    }
}
