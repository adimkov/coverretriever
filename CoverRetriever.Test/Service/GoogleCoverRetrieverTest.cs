using System.Linq;

using CoverRetriever.Service;

using NUnit.Framework;

namespace CoverRetriever.Test.Service
{
	//TODO: finish test
	[TestFixture]
	public class GoogleCoverRetrieverTest
	{
		[Test]
		public void GetCoverFor_should_download_and_parse_responce_from_google()
		{
			var target = new GoogleCoverRetrieverService();
			target.GetCoverFor("Sex Pistols", "Pretty Vacant", 3).Run();
		}
	}
}