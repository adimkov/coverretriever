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
		public void DownloadCover_should_download_cover()
		{
			Stream actual = null;
			var target = new GoogleCoverRetrieverService();

			target.DownloadCover(new Uri("http://www.google.com/imgres?imgurl=http://loot-ninja.com/wp-content/uploads/2007/09/sex-pistols.jpg&imgrefurl=http://loot-ninja.com/2007/09/28/sex-pistols-re-record-anarchy-in-the-uk-for-guitar-hero-iii/&usg=__ONKUm6Tk7vWDkWRHTggvRfi5SSo=&h=303&w=520&sz=111&hl=en&start=0&sig2=7mqVvIl-n4gQSuMpsH9GSQ&zoom=1&tbnid=OXirbYPhcJFgZM:&tbnh=125&tbnw=215&ei=TMRNTcSIKoToOb_zkekP&prev=/images%3Fq%3Dsex%2Bpistols%26hl%3Den%26client%3Dfirefox-a%26hs%3DFOB%26sa%3DX%26rls%3Dorg.mozilla:en-US:official%26biw%3D1009%26bih%3D849%26tbs%3Disch:1%26prmd%3Divnsul&itbs=1&iact=hc&vpx=129&vpy=139&dur=943&hovh=171&hovw=294&tx=58&ty=193&oei=TMRNTcSIKoToOb_zkekP&esq=1&page=1&ndsp=16&ved=1t:429,r:0,s:0"))
				.Run(x => actual = x);

			Assert.That(actual, Is.Not.Null);
		}

		[Test]
		public void DownloadCover_should_throw_exception()
		{
			var target = new GoogleCoverRetrieverService();
			Assert.Throws<CoverSearchException>(() =>
			{
				target.DownloadCover(new Uri("http://localhost/fakeimage.png")).Run();
			});
		}

	}
}