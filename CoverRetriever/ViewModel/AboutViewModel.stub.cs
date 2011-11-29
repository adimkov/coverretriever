using System.Collections.Generic;
using System.Diagnostics;

using CoverRetriever.Resources;

namespace CoverRetriever.ViewModel
{
    using CoverRetriever.Common.Extensions;

    public partial class AboutViewModel
	{
		public AboutViewModel()
		{
			InitStub();
		}

		[Conditional("DEBUG")]
		private void InitStub()
		{
			IsNewVersionAvailable = true;
			NewVersionText = CoverRetrieverResources.TextNewVersion.FormatString("1.0.1.0");

			
			var changesList = new List<string>();
			changesList.Add("Minor change #1");
			changesList.Add("Major very huge change #2");
			changesList.Add("Major very huge change #3");
			changesList.Add("Minor change WWWWWWWWWWW wwwwwwwwwwww WWWWWWWWWWWW#4");
			ChangesInNewVersion = changesList;
		}
	}
}