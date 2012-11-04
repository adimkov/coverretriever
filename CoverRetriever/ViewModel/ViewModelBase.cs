// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelBase.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.  
// </copyright>
// <summary>
//  Base view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.ViewModel
{
    using System;

    using Microsoft.Practices.Prism.ViewModel;

    /// <summary>
    /// Base view model.
    /// </summary>
    public abstract class ViewModelBase : NotificationObject
    {
        /// <summary>
        /// Backing field for IsBusy property.
        /// </summary>
        private bool _isBusy;

        /// <summary>
        /// Backing field for OperationName property.
        /// </summary>
        private string _operationName;
        
        /// <summary>
        /// Gets a value indicating whether is view model is busy.
        /// </summary>
        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }

            private set
            {
                _isBusy = value;
                RaisePropertyChanged("IsBusy");
            }
        }

        /// <summary>
        /// Gets current operation name.
        /// </summary>
        public string OperationName
        {
            get
            {
                return _operationName;
            }

            private set
            {
                _operationName = value;
                RaisePropertyChanged("OperationName");
            }
        }

        /// <summary>
        /// Gets or sets the parent view model.
        /// </summary>
        /// <value>
        /// The parent view model.
        /// </value>
        public ViewModelBase ParentViewModel { get; set; }

        /// <summary>
        /// Set start of operation.
        /// </summary>
        /// <param name="operationName">Name of started operation.</param>
        protected void StartOperation(string operationName)
        {
            if (ParentViewModel != null)
            {
                ParentViewModel.StartOperation(operationName);    
            }
            else
            {
                OperationName = operationName;
                IsBusy = true;    
            }
        }

        /// <summary>
        /// Finish of operation.
        /// </summary>
        protected void EndOperation()
        {
            if (ParentViewModel != null)
            {
                ParentViewModel.IsBusy = false;
                ParentViewModel.OperationName = String.Empty;
            }
            else
            {
                IsBusy = false;
                OperationName = String.Empty;
            }
        }
    }
}