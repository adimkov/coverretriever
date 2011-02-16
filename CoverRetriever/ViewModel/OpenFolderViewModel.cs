using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

using CoverRetriever.Model;
using CoverRetriever.Service;

using Microsoft.Practices.Prism.Commands;

namespace CoverRetriever.ViewModel
{
	[Export]
	public class OpenFolderViewModel : ViewModelBase
	{
		private readonly Subject<RootFolderResult> _rootFolderSubject = new Subject<RootFolderResult>();
		private readonly IFileSystemService _fileSystemService;

		[ImportingConstructor]
		public OpenFolderViewModel(IFileSystemService fileSystemService)
		{
			ConfirmCommand = new DelegateCommand<string>(ConfirmCommandExecute);
			_fileSystemService = fileSystemService;
			IsCloseEnable = true;
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

		/// <summary>
		/// Get or set close command availability
		/// </summary>
		public bool IsCloseEnable { get; set; }

		/// <summary>
		/// Check for directory existence on client machine
		/// </summary>
		/// <param name="testedPath">full path</param>
		/// <returns></returns>
		public bool CheckForFolderExists(string testedPath)
		{
			return _fileSystemService.IsDirectoryExists(testedPath);
		}

		private void ConfirmCommandExecute(string rootFolder)
		{
			IsCloseEnable = true;
			_rootFolderSubject.OnNext(new RootFolderResult(rootFolder));	
		}
	}
}