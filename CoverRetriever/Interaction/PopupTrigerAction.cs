
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
		protected bool HideOnClose { get; set; }

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

			var callback = args.Callback;
			
			CancelEventHandler handler = null;
			handler =
				(o, e) =>
				{
					window.Closing -= handler;
					if (HideOnClose)
					{
						e.Cancel = true;
						window.Hide();
					}
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