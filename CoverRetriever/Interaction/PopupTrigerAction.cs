
using System.ComponentModel;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Markup;

using Microsoft.Practices.Prism.Interactivity.InteractionRequest;

namespace CoverRetriever.Interaction
{
	/// <summary>
	/// Base class for trigger actions that handle an interaction request by popping up a child window.
	/// </summary>
	[ContentProperty("Window")]
	public class PopupTrigerAction : TriggerAction<FrameworkElement>
	{
		public static readonly DependencyProperty WindowProperty =
			DependencyProperty.Register("Window", typeof (Window), typeof (PopupTrigerAction),
				new PropertyMetadata(null));

		/// <summary>
		/// Window to display
		/// </summary>
		public Window Window
		{
			get { return (Window)GetValue(WindowProperty); }
			set { SetValue(WindowProperty, value); }
		}

		/// Displays the child window and collects results for <see cref="IInteractionRequest"/>.
		/// </summary>
		/// <param name="parameter">The parameter to the action. If the action does not require a parameter, the parameter may be set to a null reference.</param>
		protected override void Invoke(object parameter)
		{
			var args = parameter as InteractionRequestedEventArgs;
			if (args == null || Window == null)
			{
				return;
			}

			var callback = args.Callback;
			
			CancelEventHandler handler = null;
			handler =
				(o, e) =>
				{
					Window.Closing -= handler;
					e.Cancel = true;
					Window.Hide();
					callback();
				};

			Window.Closing += handler;

			if (!string.IsNullOrEmpty(args.Context.Title))
			{
				Window.Title = args.Context.Title;
			}

			Window.DataContext = args.Context.Content;
			Window.ShowDialog();
		}

	}
}