using System.Windows;
using System.Windows.Markup;

namespace CoverRetriever.Interaction
{
	/// <summary>
	/// shows custom control in window
	/// </summary>
	[ContentProperty("Content")]
	public class ContentChildWindow : PopupTrigerAction
	{
		private Window _customContentWindow = new Window();

		public static readonly DependencyProperty ContentProperty =
			DependencyProperty.Register("Content", typeof (DependencyObject), typeof (ContentChildWindow),
				new PropertyMetadata(null));

		public static readonly DependencyProperty MinWidthProperty =
			DependencyProperty.Register("MinWidth", typeof (double), typeof (ContentChildWindow),
				new PropertyMetadata(0d));

		public static readonly DependencyProperty MinHeightProperty =
			DependencyProperty.Register("MinHeight", typeof (double), typeof (ContentChildWindow),
				new PropertyMetadata(0d));

		public static readonly DependencyProperty WidthProperty =
			DependencyProperty.Register("Width", typeof (double), typeof (ContentChildWindow),
				new PropertyMetadata(double.NaN));

		public static readonly DependencyProperty HeightProperty =
			DependencyProperty.Register("Height", typeof (double), typeof (ContentChildWindow),
				new PropertyMetadata(double.NaN));

		public static readonly DependencyProperty WindowStartupLocationProperty =
			DependencyProperty.Register("WindowStartupLocation", typeof(WindowStartupLocation), typeof(ContentChildWindow),
				new PropertyMetadata(WindowStartupLocation.CenterScreen));

		/// <summary>
		/// Get or set WindowStartupLocation of window
		/// </summary>
		public WindowStartupLocation WindowStartupLocation
		{
			get { return (WindowStartupLocation)GetValue(WindowStartupLocationProperty); }
			set { SetValue(WindowStartupLocationProperty, value); }
		}


		public ContentChildWindow()
		{
			HideOnClose = true;
			/*
			 MinWidth="400" 
			 MinHeight="400"
			 Width="600"
			 Height="600"
			 
			 */
		}

		/// <summary>
		/// Window Content
		/// </summary>
		public DependencyObject Content
		{
			get { return (DependencyObject)GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}
		
		/// <summary>
		/// Set min	Height of window
		/// </summary>
		public double MinHeight
		{
			get { return (double)GetValue(MinHeightProperty); }
			set { SetValue(MinHeightProperty, value); }
		}

		/// <summary>
		/// set Min width of window
		/// </summary>
		public double MinWidth
		{
			get { return (double)GetValue(MinWidthProperty); }
			set { SetValue(MinWidthProperty, value); }
		}
		
		/// <summary>
		/// Height of window
		/// </summary>
		public double Height
		{
			get { return (double)GetValue(HeightProperty); }
			set { SetValue(HeightProperty, value); }
		}


		/// <summary>
		/// Width of element
		/// </summary>
		public double Width
		{
			get { return (double)GetValue(WidthProperty); }
			set { SetValue(WidthProperty, value); }
		}


		#region Overrides of PopupTrigerAction

		protected override Window ProvideWindow()
		{
			_customContentWindow.Content = Content;
			_customContentWindow.MinWidth = MinWidth;
			_customContentWindow.MinHeight = MinHeight;
			_customContentWindow.WindowStartupLocation = WindowStartupLocation;
			
			if (!double.IsNaN(Width))
			{
				_customContentWindow.Width = Width;
			}
			if (!double.IsNaN(Height))
			{
				_customContentWindow.Height = Height;
			}

			_customContentWindow.HorizontalContentAlignment = HorizontalAlignment.Stretch;
			_customContentWindow.VerticalContentAlignment = VerticalAlignment.Stretch;
			return _customContentWindow;
		}

		#endregion
	}
}