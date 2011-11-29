// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindowExtensions.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.  
// </copyright>
// <summary>
//  Hide window on closing.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.Common.Interaction
{
    using System.Windows;

    /// <summary>
    /// Hide window on closing.
    /// </summary>
    public class WindowExtensions
    {
        /// <summary>
        /// Declaration of HideOnClose attached property.
        /// </summary>
        public static readonly DependencyProperty HideOnCloseProperty = DependencyProperty.RegisterAttached(
                "HideOnClose",
                typeof(bool),
                typeof(WindowExtensions),
                new PropertyMetadata(OnHideOnCloseChanged));

        /// <summary>
        /// Declaration of Behavior attached property.
        /// </summary>
        public static readonly DependencyProperty BehaviorProperty = DependencyProperty.RegisterAttached(
            "Behavior", 
            typeof(WindowExtensionsBehavior), 
            typeof(WindowExtensions), 
            new PropertyMetadata(null));

        /// <summary>
        /// Sets the hide on close.
        /// </summary>
        /// <param name="o">The hosted object.</param>
        /// <param name="value">The value.</param>
        public static void SetHideOnClose(DependencyObject o, bool value)
        {
            o.SetValue(HideOnCloseProperty, value);
        }

        /// <summary>
        /// Gets the hide on close.
        /// </summary>
        /// <param name="o">The hosted object.</param>
        /// <returns>The value.</returns>
        public static bool GetHideOnClose(DependencyObject o)
        {
            return (bool)o.GetValue(HideOnCloseProperty);
        }

        /// <summary>
        /// Sets the behavior.
        /// </summary>
        /// <param name="o">The hosted object.</param>
        /// <param name="value">The value.</param>
        public static void SetBehavior(DependencyObject o, WindowExtensionsBehavior value)
        {
            o.SetValue(BehaviorProperty, value);
        }

        /// <summary>
        /// Gets the behavior.
        /// </summary>
        /// <param name="o">The hosted object.</param>
        /// <returns>The value.</returns>
        public static WindowExtensionsBehavior GetBehavior(DependencyObject o)
        {
            return (WindowExtensionsBehavior)o.GetValue(BehaviorProperty);
        }

        /// <summary>
        /// Called when property changed.
        /// </summary>
        /// <param name="d">The hosted object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnHideOnCloseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = d as Window;
            if (window != null)
            {
                var hideOnClose = (bool)e.NewValue;
                SmartGetBehavior(d).HideOnClose = hideOnClose;
                if (hideOnClose)
                {
                    WindowHandler.SafeAddWindow(window);
                }
                else
                {
                    WindowHandler.SafeRemoveWindow(window);
                }
            }
        }

        /// <summary>
        /// Smarts the get behavior.
        /// </summary>
        /// <param name="d">The hosted object.</param>
        /// <returns>Window extension.</returns>
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
    }
}