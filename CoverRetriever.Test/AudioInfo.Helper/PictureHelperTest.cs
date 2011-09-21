using System.Windows;
using CoverRetriever.AudioInfo;
using CoverRetriever.AudioInfo.Helper;
using Moq;
using NUnit.Framework;
using TagLib;

namespace CoverRetriever.Test.AudioInfo.Helper
{
    [TestFixture]
    public class PictureHelperTest
    {
        private const string ImageFileWithÄÄÒ = "ÄÄÒ - Ïîýò.mp3.jpg";

        [Test]
        public void Should_decode_jpeg_jpg_type_frome_extension()
        {
            var jpegMime = PictureHelper.GetMimeTipeFromFileExtension(".jpeg");
            var jpgMime = PictureHelper.GetMimeTipeFromFileExtension(".jpg");
            var jpeMime = PictureHelper.GetMimeTipeFromFileExtension(".jpe");

            Assert.That(jpegMime, Is.EqualTo("image/jpeg"));
            Assert.That(jpgMime, Is.EqualTo("image/jpeg"));
            Assert.That(jpeMime, Is.EqualTo("image/jpeg"));
            
        }

        [Test]
        public void Should_decode_png_type_frome_extension()
        {
            var pngMime = PictureHelper.GetMimeTipeFromFileExtension(".png");

            Assert.That(pngMime, Is.EqualTo("image/png"));
        }
        
        [Test]
        public void Should_decode_bmp_type_frome_extension()
        {
            var bmpMime = PictureHelper.GetMimeTipeFromFileExtension(".bmp");

            Assert.That(bmpMime, Is.EqualTo("image/bmp"));
        }

        [Test]
        public void Should_decode_gif_type_frome_extension()
        {
            var bmpMime = PictureHelper.GetMimeTipeFromFileExtension(".gif");

            Assert.That(bmpMime, Is.EqualTo("image/gif"));
        }

        [Test]
        public void Should_throw_type_not_found_unknown_extension()
        {
            Assert.Throws<MetaProviderException>(() => PictureHelper.GetMimeTipeFromFileExtension(".unknown"));
        }

        [Test]
        public void Should_add_picture_in_tag_if_collection_empty()
        {
            var tagMock = new Mock<Tag>();
            tagMock.SetupProperty(x => x.Pictures, new[] { new Picture() });
            
            var picture = new Mock<IPicture>();

            tagMock.Object.ReplacePictures(picture.Object);

            CollectionAssert.Contains(tagMock.Object.Pictures, picture.Object);
            Assert.That(tagMock.Object.Pictures.Length, Is.EqualTo(1));
        }

        [Test]
        public void Should_replace_picture_in_tag_images()
        {
            var tagMock = new Mock<Tag>();
            tagMock.SetupProperty(x => x.Pictures, new[] { new Picture { Type = PictureType.Artist }, new Picture { Type = PictureType.BackCover } });

            var picture = new Mock<IPicture>();
            picture.SetupGet(x => x.Type).Returns(PictureType.Artist);
            tagMock.Object.ReplacePictures(picture.Object);
            
            CollectionAssert.Contains(tagMock.Object.Pictures, picture.Object);
            Assert.That(tagMock.Object.Pictures.Length, Is.EqualTo(2));
        }

        [Test, Ignore] // todo: look how ITunes fork with couple of images
        public void Should_add_picture_in_tag_images_where_one_image_exists_with_another_type()
        {
            var tagMock = new Mock<Tag>();
            tagMock.SetupProperty(x => x.Pictures, new[] { new Picture { Type = PictureType.BackCover } });

            var picture = new Mock<IPicture>();
            picture.SetupGet(x => x.Type).Returns(PictureType.Artist);
            tagMock.Object.ReplacePictures(picture.Object);

            CollectionAssert.Contains(tagMock.Object.Pictures, picture.Object);
            Assert.That(tagMock.Object.Pictures.Length, Is.EqualTo(2));
        }

        [Test]
        public void Should_get_cover_from_tag()
        {
            var tagMock = new Mock<Tag>();
            tagMock.SetupProperty(x => x.Pictures, new[] { new Picture { Type = PictureType.BackCover } });

            Assert.IsNotNull(tagMock.Object.GetCoverSafe(PictureType.BackCover));
        }

        [Test]
        public void Should_return_null_if_cover_not_exixts_in_tag()
        {
            var tagMock = new Mock<Tag>();
            // tagMock.SetupProperty(x => x.Pictures, new[] { new Picture { Type = PictureType.BackCover } });

            Assert.IsNull(tagMock.Object.GetCoverSafe(PictureType.PublisherLogo));
        }

        [Test]
        public void Should_prepare_picture_from_stream_and_name_with_type_Front_cover_to_save_into_tag()
        {
            using (var stream = System.IO.File.OpenRead(PathUtils.BuildFullResourcePath(ImageFileWithÄÄÒ)))
            {
                var picture = PictureHelper.PreparePicture(stream, "cover.png", PictureType.FrontCover);
                Assert.That(picture.MimeType, Is.EqualTo("image/png"));
                Assert.That(picture.Type, Is.EqualTo(PictureType.FrontCover));
                Assert.That(picture.Data.Count, Is.EqualTo(113337));
            }
        }

        [Test]
        public void Should_prepare_picture_from_stream_and_name_with_type_Back_cover_to_save_into_tag()
        {
            using (var stream = System.IO.File.OpenRead(PathUtils.BuildFullResourcePath(ImageFileWithÄÄÒ)))
            {
                var picture = PictureHelper.PreparePicture(stream, "cover.jpg", PictureType.BackCover);
                Assert.That(picture.MimeType, Is.EqualTo("image/jpeg"));
                Assert.That(picture.Type, Is.EqualTo(PictureType.BackCover));
                Assert.That(picture.Data.Count, Is.EqualTo(113337));
            }
        }

        [Test]
        public void Should_prepare_cover_from_picture_of_tag()
        {
            var picture = new Picture(PathUtils.BuildFullResourcePath(ImageFileWithÄÄÒ));
            var cover = picture.PrepareCover();

            Assert.That(cover.Length, Is.EqualTo(113337));
            Assert.That(cover.CoverSize, Is.EqualTo(new Size(598, 600)));
            Assert.That(cover.Name, Is.EqualTo("FrontCover.jpeg"));
            Assert.That(cover.CoverStream, Is.Not.Null);
        }

        [Test]
        public void Should_get_image_from_tag_as_FrontCover_if_picture_in_tag_single_and_not_FrontCover()
        {
            var picture = new Picture{Type = PictureType.Other};
            var tagMock = new Mock<Tag>();
            tagMock.SetupProperty(x => x.Pictures, new[] { picture });

            var cover = tagMock.Object.GetCoverSafe(PictureType.FrontCover);

            Assert.That(picture, Is.EqualTo(cover));
        }
    }
}