// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetFocusOnMouseDown.cs" author="Anton Dimkov">
//     Copyright (c) Anton Dimkov 2013. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace CoverRetriever.Behaviors
{
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Interactivity;

    /// <summary>
    /// Declaration of the <see cref="SetFocusOnMouseDown"/> class.
    /// </summary>
    public class SetFocusOnMouseDown : Behavior<FrameworkElement>
    {
        /// <summary>
        /// Called when behavior has been attached.
        /// </summary>
        protected override void OnAttached()
        {
            AssociatedObject.Focusable = true;
            AssociatedObject.MouseDown += AssociatedObjectOnMouseDown;
        }

        /// <summary>
        /// Called when behavior is detaching.
        /// </summary>
        protected override void OnDetaching()
        {
            AssociatedObject.MouseDown -= AssociatedObjectOnMouseDown;
        }

        /// <summary>
        /// Associated the object on mouse down.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="mouseButtonEventArgs">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void AssociatedObjectOnMouseDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            AssociatedObject.Focus();
        }
    }
}