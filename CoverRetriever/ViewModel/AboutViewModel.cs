using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Reflection;
using CoverRetriever.Infrastructure;
using CoverRetriever.Resources;
using CoverRetriever.Service;
using Microsoft.Practices.Prism.Commands;

namespace CoverRetriever.ViewModel
{
	[Export(typeof(AboutViewModel))]
	public partial class AboutViewModel : ViewModelBase
	{
		private string _newVersionText;
		private bool _isNewVersionAvailable;
		private IEnumerable<string> _changesInNewVersion;
		private readonly IVersionControlService _versionControl;
		private readonly Version _currentVersion;
		
		[ImportingConstructor]
		public AboutViewModel(
			IVersionControlService versionControl,
			[Import(ConfigurationKeys.GetNewVersionUri)]Uri downloadNewVersionUri,
			[Import(ConfigurationKeys.ProjectHomeUri)]Uri projectHomeUri,
			[Import(ConfigurationKeys.BlogUri)]Uri blogUri)
		{
			_versionControl = versionControl;
			DownloadNewVersionUri = downloadNewVersionUri;
			ProjectHomeUri = projectHomeUri;
			BlogUri = blogUri;

			LoadedCommand = new DelegateCommand(LoadedCommandExecute);
			_currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
		}

		/// <summary>
		/// Get number of new version
		/// </summary>
		public string NewVersionText
		{
			get
			{
				return _newVersionText;
			}
			private set
			{
				_newVersionText = value;
				RaisePropertyChanged("NewVersionText");
			}
		}

		/// <summary>
		/// <see cref="bool">True</see> if new version of application available
		/// </summary>
		public bool IsNewVersionAvailable
		{
			get
			{
				return _isNewVersionAvailable;
			}
			private set
			{
				_isNewVersionAvailable = value;
				RaisePropertyChanged("IsNewVersionAvailable");
			}
		}

		/// <summary>
		/// Download new version uri
		/// </summary>
		public Uri DownloadNewVersionUri { get; private set; }

		/// <summary>
		/// Get Uri of project home
		/// </summary>
		public Uri ProjectHomeUri { get; private set; }

		/// <summary>
		/// Get uri of blog
		/// </summary>
		public Uri BlogUri { get; private set; }

		/// <summary>
		/// Get or set list of changes in new version
		/// </summary>
		public IEnumerable<string> ChangesInNewVersion
		{
			get { return _changesInNewVersion; }
			private set
			{
				_changesInNewVersion = value;
				RaisePropertyChanged("ChangesInNewVersion");
			}
		}

		/// <summary>
		/// Component has been loaded
		/// </summary>
		public DelegateCommand LoadedCommand { get; private set; }

		private void LoadedCommandExecute()
		{
			_versionControl.GetLatestVersion().Subscribe(
				x =>
				{
					ChangesInNewVersion = x.Comment;
					NewVersionText = CoverRetrieverResources.TextNewVersion.FormatUIString(x.Version);
					IsNewVersionAvailable = _currentVersion < x.Version;
				},
				ex =>
				{
					IsNewVersionAvailable = false;
				});
		}
	}
}