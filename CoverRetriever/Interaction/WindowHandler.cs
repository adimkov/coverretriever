using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace CoverRetriever.Interaction
{
	/// <summary>
	/// This class monitor all windows in application
	/// </summary>
	public static class WindowHandler
	{
		private static List<Window> _handledWindows = new List<Window>();

		/// <summary>
		/// Add window for observe. If Window already exists - skip operation
		/// </summary>
		/// <param name="window">Window for track</param>
		public static void SafeAddWindow(Window window)
		{
			if(!_handledWindows.Contains(window))
			{
				_handledWindows.Add(window);
			}	
		}

		/// <summary>
		/// Remove window from observe. If Window already absent - skip operation
		/// </summary>
		/// <param name="window">Window stop for track</param>
		public static void SafeRemoveWindow(Window window)
		{
			if (_handledWindows.Contains(window))
			{
				_handledWindows.Remove(window);
			}
		}

		/// <summary>
		/// Close all window and clear observable windows
		/// </summary>
		public static void CloseAllWindow()
		{
			while (_handledWindows.Any())
			{
				var window = _handledWindows.First();
				WindowExtensions.SetHideOnClose(window, false);
				window.Close();	
			}
		}
	}
}