namespace CoverRetriever.Model
{
	/// <summary>
	/// Result of root folder selecting
	/// </summary>
	public struct RootFolderResult
	{
		public RootFolderResult(string rootFolder) : this()
		{
			HasValue = true;
			RootFolder = rootFolder;
		}

		/// <summary>
		/// indicate is user confirm selected folder
		/// </summary>
		public bool HasValue { get; private set; }

		/// <summary>
		/// Selected folder
		/// </summary>
		public string RootFolder { get; private set; }
	}
}