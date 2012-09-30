// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CoverPreviewViewModel.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.  
// </copyright>
// <summary>
//  ViewModel for preview cover.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.ViewModel
{
    using System;
    using System.IO;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Windows;

    using CoverRetriever.AudioInfo;
    using CoverRetriever.Resources;

    using Microsoft.Practices.Prism.Commands;

    /// <summary>
    /// ViewModel for preview cover.
    /// </summary>
    public class CoverPreviewViewModel : ViewModelBase
    {
        /// <summary>
        /// The observer of saving cover.
        /// </summary>
        private readonly Subject<Cover> _saveCoverSubject = new Subject<Cover>();

        /// <summary>
        /// The remote cover.
        /// </summary>
        private readonly Cover _remoteCover;

        /// <summary>
        /// The cover stream.
        /// </summary>
        private IObservable<Stream> _previewCoverSource;

        /// <summary>
        /// An error message.
        /// </summary>
        private string _errorMessage;

        /// <summary>
        /// Progress of downloading.
        /// </summary>
        private double _downloadProgress;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoverPreviewViewModel"/> class.
        /// </summary>
        /// <param name="remoteCover">The remote cover.</param>
        public CoverPreviewViewModel(Cover remoteCover)
        {
            _remoteCover = remoteCover;
            StartOperation(CoverRetrieverResources.MessageCoverPreview);
            SaveCoverCommand = new DelegateCommand(SaveCoverCommandExecute, () => String.IsNullOrEmpty(ErrorMessage));
            CloseCommand = new DelegateCommand(CloseCommandExecute);

            CoverAsyncSource = remoteCover.CoverStream
                .Finally(EndOperation)
                .Do(
                    x => { },
                    ex =>
                    {
                        ErrorMessage = ex.Message;
                    },
                    () =>
                    {
                        ErrorMessage = null;
                    });
        }

        /// <summary>
        /// Gets cover size.
        /// </summary>
        public Size CoverSize
        {
            get
            {
                return _remoteCover.CoverSize;
            }
        }

        /// <summary>
        /// Gets cover Image.
        /// </summary>
        public IObservable<Stream> CoverAsyncSource
        {
            get
            {
                return _previewCoverSource;
            }

            private set
            {
                _previewCoverSource = value;
                RaisePropertyChanged("CoverAsyncSource");
            }
        }

        /// <summary>
        /// Gets reason of error.
        /// </summary>
        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }

            private set
            {
                _errorMessage = value;
                SaveCoverCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged("ErrorMessage");
            }
        }
        
        /// <summary>
        /// Gets the progress of image downloading.
        /// </summary>
        /// <remarks>
        /// Property does not work at this moment.
        /// </remarks>
        public double DownloadProgress
        {
            get
            {
                return _downloadProgress;
            }

            private set
            {
                _downloadProgress = value;
                RaisePropertyChanged("DownloadProgress");
            }
        }

        /// <summary>
        /// Gets command to perform cover save.
        /// </summary>
        public DelegateCommand SaveCoverCommand { get; private set; }

        /// <summary>
        /// Gets command to release all resources.
        /// </summary>
        public DelegateCommand CloseCommand { get; private set; }

        /// <summary>
        /// Gets observer of click Save cover button.
        /// </summary>
        public IObservable<Cover> SaveCover
        {
            get
            {
                return _saveCoverSubject;
            }
        }

        /// <summary>
        /// Set busy indicator state.
        /// </summary>
        /// <param name="isBusy">If set to <c>true</c>busy.</param>
        /// <param name="operationName">Name of the operation.</param>
        public void SetBusy(bool isBusy, string operationName)
        {
            if (isBusy)
            {
                StartOperation(operationName);
            }
            else
            {
                EndOperation();
            }
        }

        /// <summary>
        /// Saves the cover command execute.
        /// </summary>
        private void SaveCoverCommandExecute()
        {
            _saveCoverSubject.OnNext(_remoteCover);
        }

        /// <summary>
        /// Closes the command execute.
        /// </summary>
        private void CloseCommandExecute()
        {
            _saveCoverSubject.OnCompleted();
        }
    }
}