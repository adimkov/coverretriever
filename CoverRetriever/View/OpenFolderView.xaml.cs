//--------------------------------------------------------------------------------------------------------------------
// <copyright file="OpenFolderView.xaml.cs" author="Anton Dimkov">
//  Copyright (c) Anton Dimkov 2011. All rights reserved.
// </copyright>
// <summary>
//  Interaction logic for OpenFolderView
// </summary>
//--------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.View
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Forms;

    using CoverRetriever.Caching;
    using CoverRetriever.Resources;
    using CoverRetriever.ViewModel;

    using Microsoft.Practices.ServiceLocation;

    using Application = System.Windows.Application;

    /// <summary>
    /// Interaction logic for OpenFolderView.
    /// </summary>
    public partial class OpenFolderView
    {
        /// <summary>
        /// The key of last opened folder.
        /// </summary>
        private const string LastOpenedFolder = "LastOpenedFolder";

        /// <summary>
        /// The cache service.
        /// </summary>
        private readonly ICacheService _cacheService;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenFolderView"/> class.
        /// </summary>
        public OpenFolderView()
        {
            InitializeComponent();
            _cacheService = ServiceLocator.Current.GetInstance<ICacheService>();

            Observable.FromEventPattern<TextChangedEventArgs>(FolderPathTextBlock, "TextChanged")
                .Throttle(TimeSpan.FromMilliseconds(300))
                .ObserveOnDispatcher()
                .Subscribe(CheckFolderExistence);
        }
        
        /// <summary>
        /// Gets the view model.
        /// </summary>
        public OpenFolderViewModel ViewModel
        {
            get { return DataContext as OpenFolderViewModel; }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Window.Closing"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.ComponentModel.CancelEventArgs"/> that contains the event data.</param>
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (ViewModel != null && !ViewModel.IsCloseEnabled)
            {
                Application.Current.Shutdown(0);
            }
        }

        /// <summary>
        /// Handles the OnClick event of the Browse control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Browse_OnClick(object sender, RoutedEventArgs e)
        {
            var openDialog = new FolderBrowserDialog();
            openDialog.Description = CoverRetrieverResources.TextStepOneHeader;
            openDialog.ShowNewFolderButton = false;
            openDialog.SelectedPath = FolderPathTextBlock.Text;

            if (openDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FolderPathTextBlock.Text = openDialog.SelectedPath;
                _cacheService.Add(LastOpenedFolder, FolderPathTextBlock.Text, Caching.CacheMode.NotExpired);
            }
        }

        /// <summary>
        /// Handles the Loaded event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            FolderPathTextBlock.Text = (string)_cacheService[LastOpenedFolder];
        }

        /// <summary>
        /// Checks the folder existence.
        /// </summary>
        /// <param name="event">The @event.</param>
        private void CheckFolderExistence(EventPattern<TextChangedEventArgs> @event)
        {
            if (ViewModel != null)
            {
                var isCorrect = ViewModel.CheckForFolderExists(FolderPathTextBlock.Text);
                OkButton.IsEnabled = isCorrect;

                errorImage.Visibility = isCorrect ? Visibility.Collapsed : Visibility.Visible;
            }
        }
    }
}
