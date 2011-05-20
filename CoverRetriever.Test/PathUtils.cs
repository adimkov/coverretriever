using System;
using System.IO;

namespace CoverRetriever.Test
{
	public class PathUtils
	{
		/// <summary>
		/// build path to file in content folder
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public static string BuildFullResourcePath(string fileName)
		{
			string resourceFolder = "Input.AudioInfo";
			return Path.Combine(Environment.CurrentDirectory, resourceFolder, fileName);
		}
	}
}