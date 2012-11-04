// -------------------------------------------------------------------------------------------------
// <copyright file="FocusableBusyBehavior.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2012. All rights reserved.  
// </copyright>
// <summary>
//  Sets the focus on element that was before busy event
// </summary>
// -----------------------------------------------------------------------------------------------

namespace CoverRetriever.Behaviors
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Threading;

    using CoverRetriever.Common.Behavior;

    using Xceed.Wpf.Toolkit;

    /// <summary>
    /// Sets the focus on element that was before busy event.
    /// </summary>
    public class FocusableBusyBehavior : TemplatedBehavior<BusyIndicator>
    {
        /// <summary>
        /// Dependency property for IsBusy.
        /// </summary>
        public static readonly DependencyProperty IsBusyProperty = DependencyProperty.Register(
            "IsBusy", typeof(bool), typeof(FocusableBusyBehavior), new PropertyMetadata(false, OnIsBusyChanged));

        /// <summary>
        /// Focused element before busy indicator shown.
        /// </summary>
        private UIElement focusedElement;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is busy.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is busy; otherwise, <c>false</c>.
        /// </value>
        public bool IsBusy
        {
            get
            {
                return (bool)GetValue(IsBusyProperty);
            }

            set
            {
                SetValue(IsBusyProperty, value);
            }
        }

        /// <summary>
        /// Called when id busy changed.
        /// </summary>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        protected virtual void OnIsBusyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (Application.Current.MainWindow == null)
            {
                return;
            }

            if (IsBusy)
            {
                focusedElement = (UIElement)FocusManager.GetFocusedElement(Application.Current.MainWindow);
            }
            else
            {
                Dispatcher.BeginInvoke(
                    DispatcherPriority.Input,
                    new Action(() => focusedElement.With(x => x.Focus())));
            }
        }

        /// <summary>
        /// Called when id busy changed.
        /// </summary>
        /// <param name="d">The host.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void OnIsBusyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((FocusableBusyBehavior)d).OnIsBusyChanged(e);
        }
    }
}