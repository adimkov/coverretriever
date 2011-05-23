using System.ComponentModel.Composition;

namespace CoverRetriever.AudioInfo
{
	[Export("flac", typeof(IMetaProvider))]
	[PartCreationPolicy(CreationPolicy.NonShared)]
	public class FlacMetaProvider : AudioFileMetaProvider
	{
		public FlacMetaProvider()
		{
		}

		public FlacMetaProvider(string filePath)
		{
			Activate(filePath);
		}
	}
}