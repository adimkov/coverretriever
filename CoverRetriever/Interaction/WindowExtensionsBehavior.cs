using System.ComponentModel;
using System.Windows;

namespace CoverRetriever.Interaction
{
	public class WindowExtensionsBehavior
	{
		private readonly Window _window;

		public WindowExtensionsBehavior(Window window)
		{
			_window = window;
			_window.Closing += WindowOnClosing;
			HideOnClose = true;
		}

		public bool HideOnClose { get; set; }

		private void WindowOnClosing(object sender, CancelEventArgs cancelEventArgs)
		{
			if (HideOnClose)
			{
				cancelEventArgs.Cancel = true;
				_window.Hide();
			}
		}
	}
}