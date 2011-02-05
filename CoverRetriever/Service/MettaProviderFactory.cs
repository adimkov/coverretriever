using System;
using System.ComponentModel.Composition;
using System.IO;
using CoverRetriever.AudioInfo;
using CoverRetriever.Model;

namespace CoverRetriever.Service
{
	[Export]
	public class MetaProviderFactory
	{
		/// <summary>
		/// Get meta provider for specific file
		/// </summary>
		/// <param name="fileExtension"></param>
		/// <returns></returns>
		public Lazy<IMetaProvider> GetMetaProviderForFile(string fileFullPath)
		{
			var extension = Path.GetExtension(fileFullPath).ToLower();
			switch(extension)
			{
				case ".mp3":
					return new Lazy<IMetaProvider>(() => new Mp3MetaProvider(fileFullPath));
				default:
					throw new FileSystemServiceException("Can't get meta provider for extension {0}".FormatString());

			}	
		}
	}
}