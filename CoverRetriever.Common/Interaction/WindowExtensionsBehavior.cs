// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindowExtensionsBehavior.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.  
// </copyright>
// <summary>
//  The behavior of WindowExtensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.Common.Interaction
{
    using System.ComponentModel;
    using System.Windows;

    /// <summary>
    /// The behavior of WindowExtensions.
    /// </summary>
    public class WindowExtensionsBehavior
    {
        /// <summary>
        /// The window.
        /// </summary>
        private readonly Window _window;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowExtensionsBehavior"/> class.
        /// </summary>
        /// <param name="window">The window.</param>
        public WindowExtensionsBehavior(Window window)
        {
            _window = window;
            _window.Closing += WindowOnClosing;
            HideOnClose = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether window will hide on close action.
        /// </summary>
        public bool HideOnClose { get; set; }

        /// <summary>
        /// Windows the on closing.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="cancelEventArgs">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
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