using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CoverRetriever.Controls
{
	/// <summary>
	/// Display an error message.
	/// Control will shows up when the error message not null
	/// </summary>
	[TemplateVisualState(GroupName = VisualStateGroupErrorStates, Name = VisualStateError)]
	[TemplateVisualState(GroupName = VisualStateGroupErrorStates, Name = VisualStateNoError)]
	public class ErrorIndicator : ContentControl
	{
		private const string VisualStateError = "Error";
		private const string VisualStateNoError = "NoError";
		private const string VisualStateGroupErrorStates = "ErrorStates";

		private const string CategoryCommonProperties = "Common Properties";
		private const string CategoryBrushes = "Brushes";
		private const string CategoryMiscellaneous = "Miscellaneous";

		public static readonly DependencyProperty ErrorContentProperty =
			DependencyProperty.Register("ErrorContent", typeof(object), typeof(ErrorIndicator),
				new PropertyMetadata(null, OnErrorContentChanged));

		public static readonly DependencyProperty ErrorContentTemplateProperty =
			DependencyProperty.Register("ErrorContentTemplate", typeof(DataTemplate), typeof(ErrorIndicator),
				new PropertyMetadata(null));

		public static readonly DependencyProperty OverlayBrushProperty =
			DependencyProperty.Register("OverlayBrush", typeof(Brush), typeof(ErrorIndicator),
				new PropertyMetadata(null));

		public static readonly DependencyProperty IsErroredProperty =
			DependencyProperty.Register("IsErrored", typeof(bool), typeof(ErrorIndicator),
				new PropertyMetadata(false, OnIsErroredChanged));

		public ErrorIndicator()
		{
			DefaultStyleKey = typeof(ErrorIndicator);
		}

		/// <summary>
		/// Get or set error content
		/// </summary>
		[Category(CategoryCommonProperties)]
		public object ErrorContent
		{
			get { return GetValue(ErrorContentProperty); }
			set { SetValue(ErrorContentProperty, value); }
		}

		/// <summary>
		/// Get or set template of ErrorContent
		/// </summary>
		[Category(CategoryMiscellaneous)]
		public DataTemplate ErrorContentTemplate
		{
			get { return (DataTemplate)GetValue(ErrorContentTemplateProperty); }
			set { SetValue(ErrorContentTemplateProperty, value); }
		}

		/// <summary>
		/// Get or set brush of content Overlay on error occur
		/// </summary>
		[Category(CategoryBrushes)]
		public Brush OverlayBrush
		{
			get { return (Brush)GetValue(OverlayBrushProperty); }
			set { SetValue(OverlayBrushProperty, value); }
		}

		/// <summary>
		/// Get or set error sate of error indicator
		/// <remarks>
		/// if IsErrored is true - ErrorIndicator activate
		/// </remarks>
		/// </summary>
		[Category(CategoryCommonProperties)]
		public bool IsErrored
		{
			get { return (bool)GetValue(IsErroredProperty); }
			set { SetValue(IsErroredProperty, value); }
		}

		/// <summary>
		/// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
		/// </summary>
		public override void OnApplyTemplate()
		{
			GoToState(IsErrored);
		}

		protected virtual void OnIsErroredChanged(DependencyPropertyChangedEventArgs e)
		{
			var isErrored = (bool)e.NewValue;
			GoToState(isErrored);
		}

		protected virtual void OnErrorContentChanged(DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue is string)
			{
				IsErrored = !String.IsNullOrEmpty((string)e.NewValue);
			}
			else
			{
				IsErrored = e.NewValue != null;
			}
		}

		private static void OnIsErroredChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((ErrorIndicator)d).OnIsErroredChanged(e);
		}

		private static void OnErrorContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((ErrorIndicator)d).OnErrorContentChanged(e);
		}

		/// <summary>
		/// Go to state "Error" if parameter is <see cref="bool">True</see>. Otherwise go to "NoError" state
		/// </summary>
		/// <param name="isErrored">indicate is error occur</param>
		private void GoToState(bool isErrored)
		{
			if (isErrored)
			{
				VisualStateManager.GoToState(this, VisualStateError, true);
			}
			else
			{
				VisualStateManager.GoToState(this, VisualStateNoError, true);
			}
		}
	}
}
