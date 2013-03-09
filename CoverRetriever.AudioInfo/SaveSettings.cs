// -------------------------------------------------------------------------------------------------
// <copyright file="SaveSettings.cs" author="Anton Dimkov">
//     Copyright (c) Anton Dimkov 2013. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------------------------------
namespace CoverRetriever.AudioInfo
{
    using Microsoft.Practices.Prism.ViewModel;

    /// <summary>
    /// Declaration of the <see cref="SaveSettings"/> class.
    /// </summary>
    public class SaveSettings : NotificationObject
    {
        /// <summary>
        /// The save track name.
        /// </summary>
        private bool trackName;

        /// <summary>
        /// The save album.
        /// </summary>
        private bool album;

        /// <summary>
        /// The save artist.
        /// </summary>
        private bool artist;

        /// <summary>
        /// The save year.
        /// </summary>
        private bool year;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SaveSettings"/> is title.
        /// </summary>
        /// <value>
        ///   <c>true</c> if title; otherwise, <c>false</c>.
        /// </value>
        public bool TrackName
        {
            get
            {
                return trackName;
            }

            set
            {
                if (value.Equals(trackName))
                {
                    return;
                }

                trackName = value;
                RaisePropertyChanged(() => TrackName);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SaveSettings" /> is album.
        /// </summary>
        /// <value>
        ///   <c>true</c> if album; otherwise, <c>false</c>.
        /// </value>
        public bool Album
        {
            get
            {
                return album;
            }

            set
            {
                if (value.Equals(album))
                {
                    return;
                }

                album = value;
                RaisePropertyChanged(() => Album);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SaveSettings"/> is artist.
        /// </summary>
        /// <value>
        ///   <c>true</c> if artist; otherwise, <c>false</c>.
        /// </value>
        public bool Artist
        {
            get
            {
                return artist;
            }

            set
            {
                if (value.Equals(artist))
                {
                    return;
                }

                artist = value;
                RaisePropertyChanged(() => Artist);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SaveSettings"/> is year.
        /// </summary>
        /// <value>
        ///   <c>true</c> if year; otherwise, <c>false</c>.
        /// </value>
        public bool Year
        {
            get
            {
                return year;
            }

            set
            {
                if (value.Equals(year))
                {
                    return;
                }

                year = value;
                RaisePropertyChanged(() => Year);
            }
        }
    }
}