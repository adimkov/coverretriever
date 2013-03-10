// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PopupTrigerAction.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.  
// </copyright>
// <summary>
//   Base class for trigger actions that handle an interaction request by popping up a child window.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.Common.Interaction
{
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Interactivity;

    using Microsoft.Practices.Prism.Interactivity.InteractionRequest;

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

            var confirmation = args.Context as Confirmation;

            var callback = args.Callback;
            
            CancelEventHandler handler = null;
            handler =
                (o, e) =>
                {
                    window.Closing -= handler;
                    if (confirmation != null)
                    {
                        confirmation.Confirmed = window.DialogResult ?? false;
                    }

                    callback();
                };

            window.Closing += handler;

            if (!string.IsNullOrEmpty(args.Context.Title))
            {
                window.Title = args.Context.Title;
            }

            ConfigureWindow(window, args.Context);
            window.ShowDialog();
        }

        /// <summary>
        /// Provides the window.
        /// </summary>
        /// <returns>The window.</returns>
        protected abstract Window ProvideWindow();

        /// <summary>
        /// Configures the window.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <param name="context">The context.</param>
        protected virtual void ConfigureWindow(Window window, Notification context)
        {
            window.DataContext = context.Content;
        }
    }
}