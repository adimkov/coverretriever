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

    /// <summary>
    /// Highlights an element.
    /// </summary>
    public class HighlightElementBehavior : Behavior<FrameworkElement>
    {
        /// <summary>
        /// IsTurnedOn DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty IsTurnedOnProperty = DependencyProperty.Register(
            "IsTurnedOn", typeof(bool), typeof(HighlightElementBehavior), new PropertyMetadata(false, OnIsTurnedOnChanged));

        /// <summary>
        /// The animation of highlight.
        /// </summary>
        private Storyboard _highlightAnimation;

        /// <summary>
        /// Gets or sets a value indicating whether behavior is turned on.
        /// </summary>
        /// <value>
        /// <c>true</c> if this behavior is turned on; otherwise, <c>false</c>.
        /// </value>
        public bool IsTurnedOn
        {
            get
            {
                return (bool)GetValue(IsTurnedOnProperty);
            }

            set
            {
                SetValue(IsTurnedOnProperty, value);
            }
        }

        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            _highlightAnimation = CreateHighlightAnimation();
            TurnOnOrOffHighlight(IsTurnedOn);
        }

        /// <summary>
        /// Called when turned on changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnIsTurnedOnChanged(DependencyPropertyChangedEventArgs e)
        {
            TurnOnOrOffHighlight((bool)e.NewValue);
        }

        /// <summary>
        /// Called when turned on changed.
        /// </summary>
        /// <param name="d">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIsTurnedOnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((HighlightElementBehavior)d).OnIsTurnedOnChanged(e);
        }

        /// <summary>
        /// Creates the highlight animation.
        /// </summary>
        /// <returns>The highlight animation.</returns>
        private Storyboard CreateHighlightAnimation()
        {
            var opacity = new DoubleAnimationUsingKeyFrames();
            opacity.KeyFrames.Add(new SplineDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0))));
            opacity.KeyFrames.Add(new SplineDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(100))));
            opacity.KeyFrames.Add(new EasingDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(200))));
            opacity.KeyFrames.Add(new EasingDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(300))));
            opacity.KeyFrames.Add(new EasingDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(400))));
            opacity.KeyFrames.Add(new EasingDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(2))));

            Storyboard.SetTarget(opacity, AssociatedObject);
            Storyboard.SetTargetProperty(opacity, new PropertyPath(UIElement.OpacityProperty));

            var storiboard = new Storyboard
                {
                    RepeatBehavior = RepeatBehavior.Forever
                };

            storiboard.Children.Add(opacity);
            
            return storiboard;
        }

        /// <summary>
        /// Turns the on or off highlight.
        /// </summary>
        /// <param name="isTurnOn">If set to <c>true</c> [is turn on].</param>
        private void TurnOnOrOffHighlight(bool isTurnOn)
        {
            if (isTurnOn)
            {
                AssociatedObject.Loaded += (sender, args) => _highlightAnimation.Begin(AssociatedObject);
            }
            else
            {
                _highlightAnimation.Stop();
            }
        }
    }
}