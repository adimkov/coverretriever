// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HighlightElementBehavior.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2012. All rights reserved.
// </copyright>
// <summary>
//  Highlights an element.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CoverRetriever.Behaviors
{
    using System;
    using System.Windows;
    using System.Windows.Interactivity;
    using System.Windows.Media.Animation;

    using Microsoft.Practices.Prism.Interactivity.InteractionRequest;

    /// <summary>
    /// Highlights an element.
    /// </summary>
    public class HighlightElementBehavior : Behavior<FrameworkElement>
    {
        /// <summary>
        /// HighlightRequest DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty HighlightRequestProperty = DependencyProperty.Register(
            "HighlightRequest", typeof(IInteractionRequest), typeof(HighlightElementBehavior), new PropertyMetadata(null, OnHighlightRequestChanged));

        /// <summary>
        /// Pause time in milliseconds.
        /// </summary>
        private const int PauseTime = 1500;

        /// <summary>
        /// The animation of highlight.
        /// </summary>
        private Storyboard _highlightAnimation;

        /// <summary>
        /// Gets or sets request to highlight element.
        /// </summary>
        public IInteractionRequest HighlightRequest
        {
            get
            {
                return (IInteractionRequest)GetValue(HighlightRequestProperty);
            }

            set
            {
                SetValue(HighlightRequestProperty, value);
            }
        }

        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            _highlightAnimation = CreateHighlightAnimation();
        }

        /// <summary>
        /// Raises the HighlightRequest changed event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnHighlightRequestChanged(DependencyPropertyChangedEventArgs e)
        {
            var newRequest = (IInteractionRequest)e.NewValue;
            var oldRequest = (IInteractionRequest)e.OldValue;
            
            if (newRequest != null)
            {
                newRequest.Raised += HighlightRequestOnRaised;
            }

            if (oldRequest != null)
            {
                oldRequest.Raised -= HighlightRequestOnRaised;
            }
        }

        /// <summary>
        /// Called when highlight request has been changed.
        /// </summary>
        /// <param name="d">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnHighlightRequestChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((HighlightElementBehavior)d).OnHighlightRequestChanged(e);
        }

        /// <summary>
        /// Highlights the request on raised.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="Microsoft.Practices.Prism.Interactivity.InteractionRequest.InteractionRequestedEventArgs"/> instance containing the event data.</param>
        private void HighlightRequestOnRaised(object sender, InteractionRequestedEventArgs args)
        {
            _highlightAnimation.Begin(AssociatedObject);
        }

        /// <summary>
        /// Creates the highlight animation.
        /// </summary>
        /// <returns>The highlight animation.</returns>
        private Storyboard CreateHighlightAnimation()
        {
            var opacity = new DoubleAnimationUsingKeyFrames();
            opacity.KeyFrames.Add(new SplineDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(PauseTime))));
            opacity.KeyFrames.Add(new SplineDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(PauseTime + 150))));
            opacity.KeyFrames.Add(new EasingDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(PauseTime + 250))));
            opacity.KeyFrames.Add(new EasingDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(PauseTime + 350))));
            opacity.KeyFrames.Add(new EasingDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(PauseTime + 450))));
            opacity.KeyFrames.Add(new EasingDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(PauseTime + 550))));
            opacity.KeyFrames.Add(new EasingDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(PauseTime + 650))));
                                                                                                              
            Storyboard.SetTarget(opacity, AssociatedObject);
            Storyboard.SetTargetProperty(opacity, new PropertyPath(UIElement.OpacityProperty));

            var storiboard = new Storyboard();
            storiboard.Children.Add(opacity);
            
            return storiboard;
        }
    }
}