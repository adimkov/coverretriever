// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmptyStringValidatorBehavior.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2012. All rights reserved.
// </copyright>
// <summary>
//   Validation behavior for TextBox
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CoverRetriever.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Validation behavior for <see cref="TextBox"/>.
    /// </summary>
    public class EmptyStringValidatorBehavior
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmptyStringValidatorBehavior"/> class.
        /// </summary>
        /// <param name="textBox">The text box.</param>
        public EmptyStringValidatorBehavior(TextBox textBox)
        {
            textBox.TextChanged += TextBoxOnTextChanged;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is enable validation.
        /// </summary>
        /// <value>
        ///  <c>true</c> if validation enable; otherwise, <c>false</c>.
        /// </value>
        public bool IsEnableValidation { get; set; }

        /// <summary>
        /// Texts the box on text changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.Controls.TextChangedEventArgs"/> instance containing the event data.</param>
        private void TextBoxOnTextChanged(object sender, TextChangedEventArgs args)
        {
            var textBox = (TextBox)sender;
            if (IsEnableValidation)
            {
                if (String.IsNullOrWhiteSpace(textBox.Text))
                {
                    VisualStateManager.GoToState(textBox, "Empty", true);
                }
                else
                {
                    VisualStateManager.GoToState(textBox, "Clean", false);
                }
            }
        }
    }
}