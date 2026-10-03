#nullable enable

using System;
using System.Collections.Concurrent;
using System.Threading;
using Uno.Foundation.Logging;

namespace Uno.UI.Runtime.Skia.Headless;

/// <summary>
/// The UI thread of the headless host: runs the scheduled dispatcher work in order on a dedicated thread.
/// </summary>
/// <remarks>
/// Not part of the upstream host (which uses the Skia runtime's internal <c>EventLoop</c>): the queue can also be
/// drained synchronously from the UI thread (<see cref="RunPending"/>), which lets tests process layout, loaded
/// events and queued callbacks without awaiting, like Avalonia's <c>Dispatcher.UIThread.RunJobs()</c>.
/// </remarks>
internal sealed class HeadlessEventLoop
{
	private readonly BlockingCollection<Action> _queue = new();
	private readonly Thread _thread;

	public HeadlessEventLoop()
	{
		_thread = new Thread(Run)
		{
			IsBackground = true,
			Name = "Uno headless UI",
		};
		_thread.Start();
	}

	public bool HasThreadAccess => Thread.CurrentThread == _thread;

	public void Schedule(Action action) => _queue.Add(action);

	/// <summary>
	/// Runs the queued work, including work queued while draining, until the queue is empty or
	/// <paramref name="maxItems"/> items ran. Must be called on the UI thread.
	/// </summary>
	/// <returns>The number of items that ran.</returns>
	public int RunPending(int maxItems)
	{
		if (!HasThreadAccess)
		{
			throw new InvalidOperationException("The headless event loop can only be drained from its own thread.");
		}

		var count = 0;
		while (count < maxItems && _queue.TryTake(out var action))
		{
			Execute(action);
			count++;
		}

		return count;
	}

	private void Run()
	{
		foreach (var action in _queue.GetConsumingEnumerable())
		{
			Execute(action);
		}
	}

	private void Execute(Action action)
	{
		try
		{
			action();
		}
		catch (Exception exception)
		{
			if (this.Log().IsEnabled(LogLevel.Error))
			{
				this.Log().Error("Unhandled exception in the headless event loop.", exception);
			}
		}
	}
}
