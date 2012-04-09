// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DirectoryCoverOrganizer.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.  
// </copyright>
// <summary>
//  Manage cover in the parent folder of audio file
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.Service
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Drawing;
    using System.IO;
    using System.Linq;

    using CoverRetriever.AudioInfo;
    using CoverRetriever.AudioInfo.Helper;

    using Size = System.Windows.Size;

    /// <summary>
    /// Manage cover in the parent folder of audio file.
    /// </summary>
    [Export(typeof(DirectoryCoverOrganizer))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class DirectoryCoverOrganizer : ICoverOrganizer, IActivator
    {
        /// <summary>
        /// Default name of cover.
        /// </summary>
        private const string DefaultCoverName = "cover";

        /// <summary>
        /// Buffer size.
        /// </summary>
        private const int BufferSize = 0x4000;

        /// <summary>
        /// List of supported images format.
        /// </summary>
        private readonly IEnumerable<string> _supportedGraphicsFiles = new[] { ".jpg", ".jpeg", ".png", ".bmp" };

        /// <summary>
        /// The base path.
        /// </summary>
        private string _basePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryCoverOrganizer"/> class.
        /// </summary>
        [Obsolete("Added for MEF compatibility")]
        public DirectoryCoverOrganizer()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryCoverOrganizer"/> class.
        /// </summary>
        /// <param name="basePath">The base path.</param>
        public DirectoryCoverOrganizer(string basePath)
        {
            _basePath = basePath;
        }

        /// <summary>
        /// Gets found cover name.
        /// </summary>
        public string CoverName { get; private set; }

        /// <summary>
        /// Gets a value indicating whether can work with cover.
        /// </summary>
        public bool IsCanProcessed
        {
            get
            {
                //// todo: check for ability to read/write from disk
                return true;
            }
        }

        /// <summary>
        /// Gets cover.
        /// </summary>
        /// <returns>
        /// Cover info.
        /// </returns>
        public Cover GetCover()
        {
            if (IsCoverExists())
            {
                return PrepareCover(GetCoverPath());
            }

            throw new InvalidOperationException("File of cover doesn't exists");
        }

        /// <summary>
        ///  Gets a value indicating whether the cover existence.
        /// </summary>
        /// <returns><c>True</c> if cover exists.</returns>
        public bool IsCoverExists()
        {
            var coverFullPath = GetCoverPath();
            return File.Exists(coverFullPath) && PictureHelper.IsImageStreamValid(File.OpenRead(coverFullPath));
        }

        /// <summary>
        /// Saves stream into cover.
        /// </summary>
        /// <param name="cover">Cover to save.</param>
        /// <returns>Operation observable.</returns>
        public IObservable<Unit> SaveCover(Cover cover)
        {
            if (File.Exists(GetCoverPath()))
            {
                File.Delete(GetCoverPath());
            }

            var ext = Path.GetExtension(cover.Name);
            CoverName = DefaultCoverName + ext;
            var newCoverPath = Path.Combine(_basePath, CoverName);

            var coverSaver = cover.CoverStream
                .Do(stream =>
                {
                    try
                    {
                        PictureHelper.EnsureImageStreamValid(stream);
                        using (var newCoverStream = File.Open(newCoverPath, FileMode.CreateNew, FileAccess.Write))
                        {
                            var buffer = new byte[BufferSize];
                            int readed;

                            do
                            {
                                readed = stream.Read(buffer, 0, BufferSize);
                                newCoverStream.Write(buffer, 0, readed);
                            }
                            while (readed != 0);
                            newCoverStream.Flush();
                        }
                    }
                    finally
                    {
                        stream.Dispose();
                    }
                });

            return coverSaver.Select(x => new Unit()).Take(1);
        }

        /// <summary>
        /// Activate an object.
        /// </summary>
        /// <param name="param">Parameters to activate object.</param>
        public void Activate(params object[] param)
        {
            if (param.Length != 1)
            {
                throw new ArgumentException("Base path of cover was not found", "param");
            }

            _basePath = (string)param[0];
        }

        /// <summary>
        /// Gets cover path.
        /// </summary>
        /// <returns>Path to cover.</returns>
        private string GetCoverPath()
        {
            if (String.IsNullOrEmpty(CoverName))
            {
                var imagesFilder = Directory.GetFiles(_basePath)
                    .Where(x => _supportedGraphicsFiles.Contains(Path.GetExtension(x).ToLower()));

                var imageAsCoverFirst = imagesFilder.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x).Equals(DefaultCoverName, StringComparison.InvariantCultureIgnoreCase));

                if (imageAsCoverFirst != null)
                {
                    CoverName = Path.GetFileName(imageAsCoverFirst);
                }
            }

            return !String.IsNullOrEmpty(CoverName) ? Path.Combine(_basePath, CoverName) : String.Empty;
        }

        /// <summary>
        /// Prepares the cover.
        /// </summary>
        /// <param name="coverFullPath">The cover full path.</param>
        /// <returns>Prepared cover.</returns>
        private Cover PrepareCover(string coverFullPath)
        {
            var ms = new MemoryStream();
            Size size;
            using (var image = Image.FromFile(coverFullPath))
            {
                size = new Size(image.Width, image.Height);
            }

            using (var coverStream = File.OpenRead(coverFullPath))
            {
                ms.SetLength(coverStream.Length);
                coverStream.Read(ms.GetBuffer(), 0, (int)coverStream.Length);
                ms.Flush();
            }

            return new Cover(
                Path.GetFileName(coverFullPath),
                size, 
                ms.Length,
                Observable.Return(ms));
        }
    }
}