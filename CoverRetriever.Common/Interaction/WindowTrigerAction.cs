// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindowTrigerAction.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.  
// </copyright>
// <summary>
//   The action to trigger.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.Common.Interaction
{
    using System.Windows;
    using System.Windows.Markup;

    /// <summary>
    /// The action to trigger.
    /// </summary>
    [ContentProperty("Window")]
    public class WindowTrigerAction : PopupTrigerAction
    {
        /// <summary>
        /// Declaration of Window dependency property.
        /// </summary>
        public static readonly DependencyProperty WindowProperty = DependencyProperty.Register(
            "Window", 
            typeof(Window), 
            typeof(WindowTrigerAction), 
            new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets window for show.
        /// </summary>
        /// <value>
        /// The window.
        /// </value>
        public Window Window
        {
            get { return (Window)GetValue(WindowProperty); }
            set { SetValue(WindowProperty, value); }
        }

        /// <summary>
        /// Provides the window.
        /// </summary>
        /// <returns>
        /// The window.
        /// </returns>
        protected override Window ProvideWindow()
        {
            return Window;
        }
    }
}