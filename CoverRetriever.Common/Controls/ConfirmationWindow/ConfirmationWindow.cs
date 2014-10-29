// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfirmationWindow.cs" author="Anton Dimkov">
//     Copyright (c) Anton Dimkov 2013. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace CoverRetriever.Common.Controls
{
    using System.Windows;
    using System.Windows.Controls.Primitives;

    /// <summary>
    /// Declaration of the <see cref="ConfirmationWindow"/> class.
    /// </summary>
    [TemplatePart(Name = PartOkButton, Type = typeof(ButtonBase))]
    public class ConfirmationWindow : Window
    {
        /// <summary>
        /// The text property.
        /// </summary>
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(
            "Message", typeof(string), typeof(ConfirmationWindow), new PropertyMetadata(string.Empty));

        /// <summary>
        /// The part ok button.
        /// </summary>
        private const string PartOkButton = "PART_OKButton";

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfirmationWindow"/> class.
        /// </summary>
        public ConfirmationWindow()
        {
            DefaultStyleKey = typeof(ConfirmationWindow);
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            this.Owner = Application.Current.MainWindow;
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public string Message
        {
            get
            {
                return (string)GetValue(MessageProperty);
            }

            set
            {
                SetValue(MessageProperty, value);
            }
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var okButton = GetTemplateChild(PartOkButton) as ButtonBase;
            
            if (okButton != null)
            {
                okButton.Click += ConfirmButtonOnClick;    
            }
        }

        /// <summary>
        /// Confirms the button on click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="routedEventArgs">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void ConfirmButtonOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            DialogResult = true;
            Close();
        }
    }
}