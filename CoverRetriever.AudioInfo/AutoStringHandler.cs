using System;
using System.Text;

using TagLib;
using TagLib.Id3v1;

namespace CoverRetriever.AudioInfo
{
	public class AutoStringHandler : StringHandler
	{
		public override string Parse(ByteVector data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}

			var str = Encoding.Default.GetString(data.Data).Trim();
			int index = str.IndexOf('\0');
			return ((index < 0) ? str : str.Substring(0, index));
		}

		public override ByteVector Render(string text)
		{
			return ByteVector.FromString(text, StringType.Latin1);
		}
	}
}