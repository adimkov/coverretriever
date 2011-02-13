
using System.Windows;

namespace CoverRetriever.Interaction
{
	/// <summary>
	/// Hide window on closing
	/// </summary>
	public class WindowExtensions
	{
		public static readonly DependencyProperty HideOnCloseProperty =
			DependencyProperty.RegisterAttached("HideOnClose", typeof (bool), typeof (WindowExtensions),
				new PropertyMetadata(new PropertyChangedCallback(OnHideOnCloseChanged)));

		public static readonly DependencyProperty BehaviorProperty =
			DependencyProperty.RegisterAttached("Behavior", typeof (WindowExtensionsBehavior), typeof (WindowExtensions),
				new PropertyMetadata(null));

		public static void SetHideOnClose(DependencyObject o, bool value)
		{
			o.SetValue(HideOnCloseProperty, value);
		}

		public static bool GetHideOnClose(DependencyObject o)
		{
			return (bool)o.GetValue(HideOnCloseProperty);
		}

		private static void OnHideOnCloseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var hideOnClose = (bool)e.NewValue;
			SmartGetBehavior(d).HideOnClose = hideOnClose;
			if(hideOnClose)
			{
				WindowHandler.SafeAddWindow((Window)d);
			}
			else
			{
				WindowHandler.SafeRemoveWindow((Window)d);	
			}
		}

		private static WindowExtensionsBehavior SmartGetBehavior(DependencyObject d)
		{
			var behaviour = GetBehavior(d);
			if (behaviour == null)
			{	
				behaviour = new WindowExtensionsBehavior((Window)d);
				SetBehavior(d, behaviour);
			}
			return behaviour;
		}

		public static void SetBehavior(DependencyObject o, WindowExtensionsBehavior value)
		{
			o.SetValue(BehaviorProperty, value);
		}

		public static WindowExtensionsBehavior GetBehavior(DependencyObject o)
		{
			return (WindowExtensionsBehavior)o.GetValue(BehaviorProperty);
		}
	}
}