// -------------------------------------------------------------------------------------------------
// <copyright file="TemplatedBehavior{TElement}.cs" author="Anton Dimkov">
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
    /// <typeparam name="TElement">The type of the element.</typeparam>
    public class TemplatedBehavior<TElement> : TemplatedBehavior
        where TElement : DependencyObject
    {
        /// <summary>
        /// Gets or sets target element for behavior.
        /// </summary>
        public TElement AssociatedObject
        {
            get
            {
                return (TElement)GetValue(AssociatedObjectProperty);
            }

            set
            {
                SetValue(AssociatedObjectProperty, value);
            }
        }
    }
}