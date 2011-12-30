// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OpenFolderViewModel.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.  
// </copyright>
// <summary>
//  View model for open root folder view.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;

    using CoverRetriever.Model;
    using CoverRetriever.Service;

    using Microsoft.Practices.Prism.Commands;

    /// <summary>
    /// View model for open root folder view.
    /// </summary>
    [Export]
    public class OpenFolderViewModel : ViewModelBase
    {
        /// <summary>
        /// Result publisher.
        /// </summary>
        private readonly Subject<RootFolderResult> _rootFolderSubject = new Subject<RootFolderResult>();

        /// <summary>
        /// Service to access file system.
        /// </summary>
        private readonly IFileSystemService _fileSystemService;

        /// <summary>
        /// Is enable to close.
        /// </summary>
        private bool _isCloseEnabled;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenFolderViewModel"/> class.
        /// </summary>
        /// <param name="fileSystemService">The file system service.</param>
        [ImportingConstructor]
        public OpenFolderViewModel(IFileSystemService fileSystemService)
        {
            ConfirmCommand = new DelegateCommand<string>(ConfirmCommandExecute);
            _fileSystemService = fileSystemService;
            IsCloseEnabled = true;
        }

        /// <summary>
        /// Gets command for accept selected path.
        /// </summary>
        public DelegateCommand<string> ConfirmCommand { get; private set; }
        
        /// <summary>
        /// Gets result publisher.
        /// </summary>
        public IObservable<RootFolderResult> PushRootFolder
        {
            get { return _rootFolderSubject; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance accepts close commands.
        /// </summary>
        /// <value><c>true</c> if this instance is close enable; otherwise, <c>false</c>.</value>
        public bool IsCloseEnabled
        {
            get
            {
                return _isCloseEnabled;
            } 

            set
            {
                _isCloseEnabled = value;
                RaisePropertyChanged("IsCloseEnabled");
            }
        }

        /// <summary>
        /// Checks for directory existence on client machine.
        /// </summary>
        /// <param name="testedPath">Full path to tested directory.</param>
        /// <returns><c>true</c> if directory exists; otherwise <c>false</c></returns>
        public bool CheckForFolderExists(string testedPath)
        {
            return _fileSystemService.IsDirectoryExists(testedPath);
        }

        /// <summary>
        /// Confirms the command execute.
        /// </summary>
        /// <param name="rootFolder">The root folder.</param>
        private void ConfirmCommandExecute(string rootFolder)
        {
            IsCloseEnabled = true;
            _rootFolderSubject.OnNext(new RootFolderResult(rootFolder));
        }
    }
}