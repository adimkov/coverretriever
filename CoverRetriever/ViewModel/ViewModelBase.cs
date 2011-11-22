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

    /// <summary>
    /// Base view model.
    /// </summary>
    public abstract class ViewModelBase : NotifyPropertyChanged
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
        /// Set start of operation.
        /// </summary>
        /// <param name="operationName">Name of started operation.</param>
        protected void StartOperation(string operationName)
        {
            OperationName = operationName;
            IsBusy = true;
        }

        /// <summary>
        /// Finish of operation.
        /// </summary>
        protected void EndOperation()
        {
            IsBusy = false;
            OperationName = String.Empty;
        }
    }
}