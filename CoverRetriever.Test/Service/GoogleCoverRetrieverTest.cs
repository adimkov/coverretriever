using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using CoverRetriever.Model;
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
			IEnumerable<RemoteCover> actual = null;
			var target = new GoogleCoverRetrieverService();
			target.GetCoverFor("Sex Pistols", "Pretty Vacant", 3).Run(x => actual = x);
			
			Assert.That(actual, Is.Not.Null);
			Assert.That(actual.Count(), Is.EqualTo(3));
		}

		[Test]
		public void Should_get_stream_of_thumb_cover()
		{
			IEnumerable<RemoteCover> actual = null;
			Stream firstThumbStream = null;
			var target = new GoogleCoverRetrieverService();
			target.GetCoverFor("Sex Pistols", "Pretty Vacant", 1).Run(x => actual = x);

			Assert.That(actual, Is.Not.Null);
			actual.First().ThumbStream
				.Timeout(TimeSpan.FromMinutes(1))
				.Run(x => firstThumbStream = x);

			Assert.That(firstThumbStream, Is.Not.Null);
			Assert.That(firstThumbStream.CanRead, Is.True);
		}
	}
}