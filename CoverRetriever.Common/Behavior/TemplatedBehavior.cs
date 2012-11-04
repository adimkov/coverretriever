// -------------------------------------------------------------------------------------------------
// <copyright file="TemplatedBehavior.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2012. All rights reserved.  
// </copyright>
// <summary>
//  Base behavior for template control
// </summary>
// -------------------------------------------------------------------------------------------------

namespace CoverRetriever.Common.Behavior
{
    using System.Windows;

    /// <summary>
    /// Base behavior for template control.
    /// </summary>
    public class TemplatedBehavior : FrameworkElement
    {
        /// <summary>
        /// Dependency property for AssociatedObject.
        /// </summary>
        public static readonly DependencyProperty AssociatedObjectProperty = DependencyProperty.Register(
            "AssociatedObject", typeof(DependencyObject), typeof(TemplatedBehavior), new PropertyMetadata(null));

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplatedBehavior" /> class.
        /// </summary>
        public TemplatedBehavior()
        {
            Loaded += (sender, args) => OnAttached();
            Unloaded += (sender, args) => OnDetached();

            IsHitTestVisible = false;
        }

        /// <summary>
        /// Called when behavior attached.
        /// </summary>
        public virtual void OnAttached()
        {
        }

        /// <summary>
        /// Called when behavior detached.
        /// </summary>
        public virtual void OnDetached()
        {
        }
    }
}