using System.Windows;
using System.Windows.Markup;

namespace CoverRetriever.Interaction
{
	[ContentProperty("Window")]
	public class WindowTrigerAction : PopupTrigerAction
	{
		public static readonly DependencyProperty WindowProperty =
			DependencyProperty.Register("Window", typeof (Window), typeof (WindowTrigerAction),
				new PropertyMetadata(null));

		/// <summary>
		/// Get or set window for show
		/// </summary>
		public Window Window
		{
			get { return (Window)GetValue(WindowProperty); }
			set { SetValue(WindowProperty, value); }
		}

		protected override Window ProvideWindow()
		{
			return Window;
		}
	}
}