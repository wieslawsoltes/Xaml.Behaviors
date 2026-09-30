#nullable enable

using System.Collections.Generic;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;

namespace Uno.UI.Runtime.Skia.Headless;

/// <summary>
/// The keyboard of the headless host: key presses are delivered through the regular Uno Platform keyboard pipeline
/// (preview and bubbling key events and character input on the focused element).
/// </summary>
/// <remarks>
/// Not part of the upstream host, which registers no keyboard input source (keyboard injection raises nothing).
/// </remarks>
internal sealed class HeadlessKeyboardInputSource : IUnoKeyboardInputSource
{
	private const string DeviceId = "headless-keyboard";

	public event TypedEventHandler<object, KeyEventArgs>? KeyDown;

	public event TypedEventHandler<object, KeyEventArgs>? KeyUp;

	public event TypedEventHandler<object, CharacterReceivedEventArgs>? CharacterReceived;

	public bool RaiseKey(VirtualKey key, VirtualKeyModifiers modifiers, bool down, char? character)
	{
		var status = new CorePhysicalKeyStatus { IsKeyReleased = !down, RepeatCount = 1 };
		var args = new KeyEventArgs(DeviceId, key, modifiers, status, down ? character : null);
		(down ? KeyDown : KeyUp)?.Invoke(this, args);
		return args.Handled;
	}

	public bool RaiseCharacter(char character)
	{
		var args = new CharacterReceivedEventArgs(character, new CorePhysicalKeyStatus { RepeatCount = 1 });
		CharacterReceived?.Invoke(this, args);
		return args.Handled;
	}
}

/// <summary>
/// The keyboard input sources the Uno Platform input managers created for the host windows.
/// </summary>
internal sealed class HeadlessKeyboard
{
	private readonly List<HeadlessKeyboardInputSource> _sources = [];

	public HeadlessKeyboardInputSource Create()
	{
		var source = new HeadlessKeyboardInputSource();
		_sources.Add(source);
		return source;
	}

	public bool RaiseKey(VirtualKey key, VirtualKeyModifiers modifiers, bool down, char? character)
	{
		var handled = false;
		foreach (var source in _sources)
		{
			handled |= source.RaiseKey(key, modifiers, down, character);
		}

		return handled;
	}

	public bool RaiseCharacter(char character)
	{
		var handled = false;
		foreach (var source in _sources)
		{
			handled |= source.RaiseCharacter(character);
		}

		return handled;
	}
}
