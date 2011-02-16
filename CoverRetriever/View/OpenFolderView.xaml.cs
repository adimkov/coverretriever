using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

using CoverRetriever.Resources;
using CoverRetriever.ViewModel;

using Application = System.Windows.Application;

namespace CoverRetriever.View
{
	/// <summary>
	/// Interaction logic for OpenFolderView.xaml
	/// </summary>
	public partial class OpenFolderView
	{
		public OpenFolderView()
		{
			InitializeComponent();
			Observable.FromEvent<TextChangedEventArgs>(FolderPathTextBlock, "TextChanged")
				.Throttle(TimeSpan.FromMilliseconds(500))
				.ObserveOnDispatcher()
				.Subscribe(CheckFolderExistence);
		}
		
		/// <summary>
		/// Get view model
		/// </summary>
		public OpenFolderViewModel ViewModel
		{
			get { return DataContext as OpenFolderViewModel; }
		}

		private void Browse_OnClick(object sender, RoutedEventArgs e)
		{
			var openDialog = new FolderBrowserDialog();
			openDialog.Description = CoverRetrieverResources.TextStepOneHeader;
			openDialog.ShowNewFolderButton = false;
			openDialog.SelectedPath = FolderPathTextBlock.Text;

			if (openDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				FolderPathTextBlock.Text = openDialog.SelectedPath;
			}
		}

		private void CheckFolderExistence(IEvent<TextChangedEventArgs> @event)
		{
			if (ViewModel != null)
			{
				var isCorrect =  ViewModel.CheckForFolderExists(FolderPathTextBlock.Text);
				OkButton.IsEnabled = isCorrect;

				errorImage.Visibility = isCorrect?Visibility.Collapsed:Visibility.Visible;
			} 	
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Window.Closing"/> event.
		/// </summary>
		/// <param name="e">A <see cref="T:System.ComponentModel.CancelEventArgs"/> that contains the event data.</param>
		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);
			if (ViewModel != null && !ViewModel.IsCloseEnable)
			{
				Application.Current.Shutdown(0);
			}
		}
	}
}
