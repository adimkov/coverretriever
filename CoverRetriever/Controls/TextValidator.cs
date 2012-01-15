// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextValidator.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2012. All rights reserved.
// </copyright>
// <summary>
//  Validators for text box element.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CoverRetriever.Controls
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Validators for text box element.
    /// </summary>
    public class TextValidator
    {
        /// <summary>
        /// Attached property for NotifyIfEmpty.
        /// </summary>
        public static readonly DependencyProperty NotifyIfEmptyProperty = DependencyProperty.RegisterAttached(
            "NotifyIfEmpty", typeof(bool), typeof(TextValidator), new PropertyMetadata(OnNotifyIfEmptyChanged));

        /// <summary>
        /// Attached property for EmptyStringValidatorBehavior.
        /// </summary>
        public static readonly DependencyProperty EmptyStringValidatorProperty = DependencyProperty.RegisterAttached(
            "EmptyStringValidatorBehavior", typeof(EmptyStringValidatorBehavior), typeof(TextValidator), null);

        /// <summary>
        /// Sets the notify if empty.
        /// </summary>
        /// <param name="o">The host object.</param>
        /// <param name="value">The value of property.</param>
        public static void SetNotifyIfEmpty(TextBox o, bool value)
        {
            o.SetValue(NotifyIfEmptyProperty, value);
        }

        /// <summary>
        /// Gets the notify if empty.
        /// </summary>
        /// <param name="o">The host object.</param>
        /// <returns>The value of property.</returns>
        public static bool GetNotifyIfEmpty(TextBox o)
        {
            return (bool)o.GetValue(NotifyIfEmptyProperty);
        }

        /// <summary>
        /// Sets the value for EmptyStringValidatorBehavior.
        /// </summary>
        /// <param name="o">The host object.</param>
        /// <param name="value">The value of property.</param>
        public static void SetEmptyStringValidator(TextBox o, EmptyStringValidatorBehavior value)
        {
            o.SetValue(EmptyStringValidatorProperty, value);
        }

        /// <summary>
        /// Gets the value for EmptyStringValidatorBehavior.
        /// </summary>
        /// <param name="o">The host object.</param>
        /// <returns>The value of property.</returns>
        public static EmptyStringValidatorBehavior GetEmptyStringValidator(TextBox o)
        {
            return (EmptyStringValidatorBehavior)o.GetValue(EmptyStringValidatorProperty);
        }

        /// <summary>
        /// Called when value of NotifyIfEmpty changed.
        /// </summary>
        /// <param name="d">The host object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnNotifyIfEmptyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            EnsureEmptyStringValidator((TextBox)d);
            GetEmptyStringValidator((TextBox)d).IsEnableValidation = (bool)e.NewValue;
        }

        /// <summary>
        /// Ensures existence of EmptyStringValidator.
        /// </summary>
        /// <param name="host">The host.</param>
        private static void EnsureEmptyStringValidator(TextBox host)
        {
            var emptyStringValidator = GetEmptyStringValidator(host);
            if (emptyStringValidator == null)
            {
                emptyStringValidator = new EmptyStringValidatorBehavior(host);
                SetEmptyStringValidator(host, emptyStringValidator);
            }
        } 
    }
}