#nullable enable

using Windows.System;
using Windows.UI.Input.Preview.Injection;

namespace Uno.UI.Runtime.Skia.Headless;

/// <summary>
/// Mouse input injection with keyboard modifiers and WinUI wheel semantics.
/// </summary>
/// <remarks>
/// Not part of the upstream host. The public <see cref="InputInjector.InjectMouseInput(System.Collections.Generic.IEnumerable{InjectedInputMouseInfo})"/>
/// of Uno Platform raises every pointer event without keyboard modifiers and reads the wheel rotation from
/// <see cref="InjectedInputMouseInfo.DeltaY"/>/<see cref="InjectedInputMouseInfo.DeltaX"/> instead of
/// <see cref="InjectedInputMouseInfo.MouseData"/> (WinUI), so a wheel injected the WinUI way is not delivered.
/// This injects through the internal overload of the injector that takes the modifiers.
/// </remarks>
internal static class HeadlessMouseInput
{
	private const InjectedInputMouseOptions ButtonDown =
		InjectedInputMouseOptions.LeftDown | InjectedInputMouseOptions.RightDown | InjectedInputMouseOptions.MiddleDown | InjectedInputMouseOptions.XDown;

	public static void Inject(InputInjector injector, InjectedInputMouseInfo info, VirtualKeyModifiers modifiers)
	{
		// WinUI passes the wheel rotation in MouseData; Uno Platform reads it from DeltaY (DeltaX for a horizontal wheel).
		if ((info.MouseOptions & InjectedInputMouseOptions.Wheel) != 0 && info.DeltaY == 0)
		{
			info = Copy(info, info.DeltaX, unchecked((int)info.MouseData));
		}
		else if ((info.MouseOptions & InjectedInputMouseOptions.HWheel) != 0 && info.DeltaX == 0)
		{
			info = Copy(info, unchecked((int)info.MouseData), info.DeltaY);
		}

		// Like the public InjectMouseInput: the first button press of a sequence starts a new pointer sequence.
		if ((info.MouseOptions & ButtonDown) != 0 && !injector.Mouse.Properties.HasPressedButton)
		{
			injector.Mouse.StartNewSequence();
		}

		injector.InjectMouseInput(new[] { (info, modifiers) });
	}

	private static InjectedInputMouseInfo Copy(InjectedInputMouseInfo info, int deltaX, int deltaY) => new()
	{
		TimeOffsetInMilliseconds = info.TimeOffsetInMilliseconds,
		MouseOptions = info.MouseOptions,
		MouseData = info.MouseData,
		DeltaX = deltaX,
		DeltaY = deltaY,
	};
}
