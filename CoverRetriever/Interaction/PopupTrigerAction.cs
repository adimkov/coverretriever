
using System.ComponentModel;
using System.Windows;
using System.Windows.Interactivity;

using Microsoft.Practices.Prism.Interactivity.InteractionRequest;

namespace CoverRetriever.Interaction
{
	/// <summary>
	/// Base class for trigger actions that handle an interaction request by popping up a child window.
	/// </summary>
	public abstract class PopupTrigerAction : TriggerAction<FrameworkElement>
	{
		/// <summary>
		/// Displays the child window and collects results for <see cref="IInteractionRequest"/>.
		/// </summary>
		/// <param name="parameter">The parameter to the action. If the action does not require a parameter, the parameter may be set to a null reference.</param>
		protected override void Invoke(object parameter)
		{
			var args = parameter as InteractionRequestedEventArgs;
			var window = ProvideWindow();
			if (args == null || window == null)
			{
				return;
			}

			var closeRequest = args.Context as CloseNotification;
			if (closeRequest != null)
			{
				window.Close();
				return;
			}

			var callback = args.Callback;
			
			CancelEventHandler handler = null;
			handler =
				(o, e) =>
				{
					window.Closing -= handler;
					callback();
				};

			window.Closing += handler;

			if (!string.IsNullOrEmpty(args.Context.Title))
			{
				window.Title = args.Context.Title;
			}

			window.DataContext = args.Context.Content;
			window.ShowDialog();
		}

		protected abstract Window ProvideWindow();
	}
}