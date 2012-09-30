// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PictureHelper.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.  
// </copyright>
// <summary>
//  Utility class to work with pictures in the tag
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.AudioInfo.Helper
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Reactive.Linq;

    using TagLib;

    using Size = System.Windows.Size;

    /// <summary>
    /// Utility class to work with pictures in the tag.
    /// </summary>
    public static class PictureHelper
    {
        /// <summary>
        /// Mime type to file extensions mapper.
        /// </summary>
        private static readonly Dictionary<string, string> MimeTipeDictionary = new Dictionary<string, string>
            {
                { ".jpeg", "image/jpeg" },
                { ".jpg", "image/jpeg" },
                { ".jpe", "image/jpeg" },
                { ".png", "image/png" },
                { ".bmp", "image/bmp" },
                { ".gif", "image/gif" },
            };

        /// <summary>
        /// Get MimeType by file extension.
        /// </summary>
        /// <param name="fileExtension">Extension of file.</param>
        /// <returns>The Mime type.</returns>
        public static string GetMimeTipeFromFileExtension(string fileExtension)
        {
            if (MimeTipeDictionary.ContainsKey(fileExtension))
            {
                return MimeTipeDictionary[fileExtension];
            }

            throw new MetaProviderException("MimeType for extension '{0}' was not found");
        }

        /// <summary>
        /// Replace pictures in Tag file.
        /// </summary>
        /// <param name="tag">Audio tag.</param>
        /// <param name="newPictures">The new pictures.</param>
        public static void ReplacePictures(this Tag tag, params IPicture[] newPictures)
        {
            var pictures = new List<IPicture>(tag.Pictures);
            
            foreach (var picture in newPictures)
            {
                var localPicture = picture;
                int indexOfPicture = pictures.IndexOf(tag.GetCoverSafe(localPicture.Type));
                
                if (indexOfPicture >= 0)
                {
                    pictures.RemoveAt(indexOfPicture);
                    pictures.Insert(indexOfPicture, localPicture);
                }
                else
                {
                    pictures.Add(localPicture);
                }
            }

            tag.Pictures = pictures.ToArray();
        }

        /// <summary>
        /// Prepare front cover for inject into audio file. 
        /// </summary>
        /// <param name="coverStream">Stream of cover.</param>
        /// <param name="name">Name of cover.</param>
        /// <param name="pictureType">Type of picture.</param>
        /// <returns>The <see cref="Picture"/>.</returns>
        public static IPicture PreparePicture(Stream coverStream, string name, PictureType pictureType)
        {
            var buffer = new byte[1024 * 10];
            int read;
            var downloadedCover = new MemoryStream();

            do
            {
                read = coverStream.Read(buffer, 0, buffer.Length);
                downloadedCover.Write(buffer, 0, read);
            } 
            while (read != 0);

            downloadedCover.Position = 0;

            EnsureImageStreamValid(downloadedCover);
            var frontCover = new Picture(new ByteVector(downloadedCover.ToArray(), (int)downloadedCover.Length));
            frontCover.Type = pictureType;
            frontCover.MimeType = GetMimeTipeFromFileExtension(Path.GetExtension(name));

            return frontCover;
        }

        /// <summary>
        /// Prepare cover from picture in the tag.
        /// </summary>
        /// <param name="picture">Picture from tag.</param>
        /// <returns>The <see cref="Cover"/>.</returns>
        public static Cover PrepareCover(this IPicture picture)
        {
            var name = "{0}.{1}".FormatString(picture.Type, picture.MimeType.Split('/')[1]);
            Size size;
            var coverStream = new MemoryStream(picture.Data.Count);

            coverStream.Write(picture.Data.Data, 0, picture.Data.Count);
            coverStream.Flush();
            coverStream.Position = 0;

            using (var image = Image.FromStream(coverStream))
            {
                size = new Size(image.Width, image.Height);
            }

            coverStream.Position = 0;
            return new Cover(name, size, coverStream.Length, Observable.Return(coverStream));
        }

        /// <summary>
        /// Gets cover from file if exists.
        /// </summary>
        /// <param name="tag">The <see cref="Tag"/> of audio.</param>
        /// <param name="pictureType">The type of picture.</param>
        /// <returns>The <see cref="Picture"/>.</returns>
        /// <remarks>If tag contains single image and this image not <see cref="PictureType.FrontCover"/>, will be returned this image.</remarks>
        public static IPicture GetCoverSafe(this Tag tag, PictureType pictureType)
        {
            IPicture picture = tag.Pictures.SingleOrDefault(x => x.Type == pictureType);
            if (picture == null && tag.Pictures.Any())
            {
                picture = tag.Pictures.First();
            }

            return picture;
        }

        /// <summary>
        /// Determines whether image is valid.
        /// </summary>
        /// <param name="picture">The picture.</param>
        /// <returns>
        ///   <c>true</c> if image is valid; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsImageValid(this IPicture picture)
        {
            var coverStream = new MemoryStream(picture.Data.Count);

            coverStream.Write(picture.Data.Data, 0, picture.Data.Count);
            coverStream.Flush();
            coverStream.Position = 0;

            return IsImageStreamValid(coverStream);
        }

        /// <summary>
        /// Determines whether image stream is valid.
        /// </summary>
        /// <param name="imageStream">The image stream.</param>
        /// <returns>
        ///   <c>true</c> if image stream is valid; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsImageStreamValid(Stream imageStream)
        {
            try
            {
                EnsureImageStreamValid(imageStream);
                return true;
            }
            catch (FileFormatException)
            {
                return false;
            }
        }

        /// <summary>
        /// Determines whether stream contains valid image.
        /// </summary>
        /// <param name="imageStream">The image stream.</param>
        public static void EnsureImageStreamValid(Stream imageStream)
        {
            Image image = null;
            try
            {
                image = Image.FromStream(imageStream);
                if (imageStream.CanSeek)
                {
                    imageStream.Position = 0;
                }
            }
            catch (Exception ex)
            {
                throw new FileFormatException("Invalid image format", ex);
            }
            finally
            {
                if (image != null)
                {
                    image.Dispose();
                }
            }
        }
    }
}