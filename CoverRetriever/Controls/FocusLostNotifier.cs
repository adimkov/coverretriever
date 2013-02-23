// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FocusLostNotifier.cs" author="Anton Dimkov">
//     Copyright (c) Anton Dimkov 2013. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace CoverRetriever.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// Declaration of the <see cref="FocusLostNotifier" /> class.
    /// </summary>
    public class FocusLostNotifier : DependencyObject
    {
        /// <summary>
        /// The focusable elements property.
        /// </summary>
        public static readonly DependencyProperty FocusableElementsProperty = DependencyProperty.Register(
            "FocusableElements",
            typeof(ObservableCollection<UIElementItem>), 
            typeof(FocusLostNotifier),
            new PropertyMetadata(FocusableElementsPropertyChangedCallback));

        /// <summary>
        /// The focus lost event listener.
        /// </summary>
        private readonly FocusLostEventListener focusLostEventListener;

        /// <summary>
        /// Initializes a new instance of the <see cref="FocusLostNotifier" /> class.
        /// </summary>
        public FocusLostNotifier()
        {
            FocusableElements = new ObservableCollection<UIElementItem>();
            focusLostEventListener = new FocusLostEventListener(this);
        }

        /// <summary>
        /// Gets or sets the lost focus command.
        /// </summary>
        /// <value>
        /// The lost focus command.
        /// </value>
        public ICommand LostFocusCommand
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets list of focusable elements.
        /// </summary>
        public ObservableCollection<UIElementItem> FocusableElements
        {
            get
            {
                return (ObservableCollection<UIElementItem>)GetValue(FocusableElementsProperty);
            }

            set
            {
                SetValue(FocusableElementsProperty, value);
            }
        }

        /// <summary>
        /// Focusable the elements property changed callback.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void FocusableElementsPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((FocusLostNotifier)d).FocusableElementsPropertyChanged(e);
        }

        /// <summary>
        /// Focusable the elements property changed.
        /// </summary>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private void FocusableElementsPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            var newCollection = e.NewValue as ObservableCollection<UIElementItem>;
            var oldCollection = e.OldValue as ObservableCollection<UIElementItem>;
 
            if (newCollection != null)
            {
                SubscribeElementsChanged(newCollection);
                newCollection.CollectionChanged += ElementCollectionChanged;
            }

            if (oldCollection != null)
            {
                UnsubscribeElementsChanged(oldCollection);
                oldCollection.CollectionChanged -= ElementCollectionChanged;
            }
        }

        /// <summary>
        /// Unsubscribes the elements changed.
        /// </summary>
        /// <param name="oldCollection">The old collection.</param>
        private void UnsubscribeElementsChanged(IEnumerable<DependencyObject> oldCollection)
        {
            foreach (var element in oldCollection)
            {
                LostFocusEventManager.RemoveListener(element, focusLostEventListener);
            }
        }

        /// <summary>
        /// Subscribes the element changed.
        /// </summary>
        /// <param name="newCollection">The new collection.</param>
        private void SubscribeElementsChanged(IEnumerable<UIElementItem> newCollection)
        {
            foreach (var element in newCollection)
            {
                LostFocusEventManager.AddListener(element.Element, focusLostEventListener);
            }
        }

        /// <summary>
        /// Elements the collection changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="NotifyCollectionChangedEventArgs" /> instance containing the event data.</param>
        private void ElementCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Add)
            {
                var items = args.NewItems.OfType<UIElementItem>();

                SubscribeElementsChanged(items);
            }

            if (args.Action == NotifyCollectionChangedAction.Remove)
            {
                var items = args.OldItems.OfType<UIElementItem>();

                UnsubscribeElementsChanged(items);
            }
        }

        /// <summary>
        /// Declaration of the <see cref="FocusLostEventListener" /> class.
        /// </summary>
        private class FocusLostEventListener : IWeakEventListener
        {
            /// <summary>
            /// The focus lost notifier.
            /// </summary>
            private readonly FocusLostNotifier focusLostNotifier;

            /// <summary>
            /// Initializes a new instance of the <see cref="FocusLostEventListener" /> class.
            /// </summary>
            /// <param name="focusLostNotifier">The focus lost notifier.</param>
            public FocusLostEventListener(FocusLostNotifier focusLostNotifier)
            {
                this.focusLostNotifier = focusLostNotifier;
            }

            /// <summary>
            /// Receives events from the centralized event manager.
            /// </summary>
            /// <param name="managerType">The type of the <see cref="T:System.Windows.WeakEventManager" /> calling this method.</param>
            /// <param name="sender">Object that originated the event.</param>
            /// <param name="e">Event data.</param>
            /// <returns>
            /// True if the listener handled the event. It is considered an error by the <see cref="T:System.Windows.WeakEventManager" /> handling in WPF to register a listener for an event that the listener does not handle. Regardless, the method should return false if it receives an event that it does not recognize or handle.
            /// </returns>
            public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
            {
                var elements = focusLostNotifier.FocusableElements.Select(x => x.Element).OfType<UIElement>();
                if (elements.All(x => x.IsFocused))
                {
                    var command = focusLostNotifier.LostFocusCommand;
                    if (command != null && command.CanExecute(null))
                    {
                        command.Execute(null);
                    }
                }
                return true;
            }
        }
    }
}