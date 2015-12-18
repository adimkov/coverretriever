using CoverRetriever.Common.ViewModel;
using CoverRetriever.Model;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;

namespace CoverRetriever.ViewModel
{
    [Export]
    public class SelectSearchProviderViewModel : ViewModelBase
    {
        private readonly Subject<SearchProviderResult> _selectedSearchProvider = new Subject<SearchProviderResult>();


        // <summary>
        /// Is enable to close.
        /// </summary>
        private bool _isCloseEnabled;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectSearchProviderViewModel"/> class.
        /// </summary>
        /// <param name="fileSystemService">The file system service.</param>
        [ImportingConstructor]
        public SelectSearchProviderViewModel()
        {
            ConfirmCommand = new DelegateCommand<string>(ConfirmCommandExecute);
            IsCloseEnabled = true;
        }

        public IObservable<SearchProviderResult> SelectedSearchProvider
        {
            get { return _selectedSearchProvider; }
        }

        /// <summary>
        /// Gets command for accept selected provider.
        /// </summary>
        public DelegateCommand<string> ConfirmCommand { get; private set; }

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
        /// Confirms the command execute.
        /// </summary>
        /// <param name="provider">The search provider.</param>
        private void ConfirmCommandExecute(string provider)
        {
            IsCloseEnabled = true;
            _selectedSearchProvider.OnNext(new SearchProviderResult(provider));
        }
    }
}
