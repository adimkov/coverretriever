// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UIElementItem.cs" author="Anton Dimkov">
//     Copyright (c) Anton Dimkov 2013. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace CoverRetriever.Controls
{
    using System.Windows;

    /// <summary>
    /// Declaration of the <see cref="UIElementItem" /> class.
    /// </summary>
    public class UIElementItem : DependencyObject
    {
        /// <summary>
        /// The element property
        /// </summary>
        public static readonly DependencyProperty ElementProperty = DependencyProperty.Register(
            "Element", typeof(DependencyObject), typeof(UIElementItem), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the dependency object.
        /// </summary>
        /// <value>
        /// The dependency object.
        /// </value>
        public DependencyObject Element
        {
            get
            {
                return (DependencyObject)GetValue(ElementProperty);
            }

            set
            {
                SetValue(ElementProperty, value);
            }
        }

    }
}