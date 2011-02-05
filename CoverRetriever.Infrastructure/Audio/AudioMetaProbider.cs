
using System;
using System.ComponentModel.Composition;

namespace CoverRetriever.Infrastructure.Audio
{
	[MetadataAttribute]  
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]  
	public class AudioMetaProbider : ExportAttribute
	{
		public string FileExtension { get; private set; }

		public AudioMetaProbider(string fileExtension)
			: base(typeof(AudioMetaProbider))
		{
			FileExtension = fileExtension;
		}
	}
}