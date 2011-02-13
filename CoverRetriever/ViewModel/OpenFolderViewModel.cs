using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

using CoverRetriever.Model;

using Microsoft.Practices.Prism.Commands;

namespace CoverRetriever.ViewModel
{
	[Export]
	public class OpenFolderViewModel : ViewModelBase
	{
		private readonly Subject<RootFolderResult> _rootFolderSubject = new Subject<RootFolderResult>();
		public OpenFolderViewModel()
		{
			ConfirmCommand = new DelegateCommand<string>(ConfirmCommandExecute);
		}

		/// <summary>
		/// Accept selected path
		/// </summary>
		public DelegateCommand<string> ConfirmCommand { get; private set; }
		
		/// <summary>
		/// Push root folder on get it
		/// </summary>
		public IObservable<RootFolderResult> PushRootFolder
		{
			get { return _rootFolderSubject; }
		}

		private void ConfirmCommandExecute(string rootFolder)
		{
			_rootFolderSubject.OnNext(new RootFolderResult(rootFolder));	
		}
	}
}