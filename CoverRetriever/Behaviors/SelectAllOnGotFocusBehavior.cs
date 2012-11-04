// -----------------------------------------------------------------------------------------------
// <copyright file="SelectAllOnGotFocusBehavior.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2012. All rights reserved.
// </copyright>
// <summary>
//  Selects all text in textBox on control got focus.
// </summary>
// -----------------------------------------------------------------------------------------------
namespace CoverRetriever.Behaviors
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Interactivity;

    /// <summary>
    /// Selects all text in textBox on control got focus.
    /// </summary>
    public class SelectAllOnGotFocusBehavior : Behavior<TextBox>
    {
        /// <summary>
        /// Called when behavior is attached.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.GotFocus += AssociatedObjectOnGotFocus;
        }

        /// <summary>
        /// Called when behavior is detached.
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.GotFocus -= AssociatedObjectOnGotFocus;
        }

        /// <summary>
        /// Associated object on got focus.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="routedEventArgs">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void AssociatedObjectOnGotFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            AssociatedObject.SelectAll();
        }
    }
}