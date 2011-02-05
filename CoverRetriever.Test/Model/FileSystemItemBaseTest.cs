using CoverRetriever.Model;

namespace CoverRetriever.Test.Model
{
	public class FileSystemItemBaseTest
	{
		protected const string BasePath = @"C:\temp\";

		/// <summary>
		/// return base folder
		/// </summary>
		public RootFolder RootFolder
		{
			get
			{
				return new RootFolder(BasePath);
			}
			
		}
	}
}