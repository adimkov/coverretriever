// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfirmationWindowTrigger.cs" author="Anton Dimkov">
//     Copyright (c) Anton Dimkov 2013. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace CoverRetriever.Common.Interaction
{
    using System.Windows;

    using CoverRetriever.Common.Controls;

    using Microsoft.Practices.Prism.Interactivity.InteractionRequest;

    /// <summary>
    /// Declaration of the <see cref="ConfirmationWindowTrigger"/> class.
    /// </summary>
    public class ConfirmationWindowTrigger : PopupTrigerAction
    {
        /// <summary>
        /// Provides the window.
        /// </summary>
        /// <returns>
        /// The window.
        /// </returns>
        protected override Window ProvideWindow()
        {
            return new ConfirmationWindow();
        }

        /// <summary>
        /// Configures the window.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <param name="context">The context.</param>
        protected override void ConfigureWindow(Window window, Notification context)
        {
            ((ConfirmationWindow)window).Message = context.Content.ToString();
        }
    }
}