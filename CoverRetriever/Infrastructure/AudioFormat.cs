using System.Collections.Generic;

namespace CoverRetriever.Infrastructure
{
	public static class AudioFormat
	{
		private static IEnumerable<string> _audioFileExt = new [] {".mp3", ".flac"};

		/// <summary>
		/// Get supported audio files extensions
		/// </summary>
		public static IEnumerable<string> AudioFileExtensions
		{
			get
			{
				return _audioFileExt;
			}
		}
	}
}