// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Bootstrapper.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.  
// </copyright>
// <summary>
//  Class initializer of cover retriever application
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever
{
    using System;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Windows;

    using CoverRetriever.AudioInfo;
    using CoverRetriever.Common.Infrastructure;
    using CoverRetriever.Properties;
    using CoverRetriever.View;

    using Microsoft.Practices.Prism.MefExtensions;

    /// <summary>
    /// Class initializer of cover retriever application.
    /// </summary>
    public class Bootstrapper : MefBootstrapper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Bootstrapper"/> class.
        /// </summary>
        public Bootstrapper()
        {
            VersionControlConnectionString = "http://adimkov.users.sourceforge.net/CoverRetrieverAssets/Versions.xml";
            ProjectHomeUri = new Uri("http://dimkov.org");
            BlogUri = new Uri("http://anton-dimkov.blogspot.com/");
            GetNewVersionUri = new Uri("http://sourceforge.net/projects/coverretriever/files/");
            CacheFilePath = Settings.Default.CacheFilePath;
            LastFmApiKey = Settings.Default.LastFmApiKey;
            LastFmFingerprintClientPath = Settings.Default.LastFmFingerprintClientPath;
            LastFmServiceBaseAddress = Settings.Default.LastFmServiceBaseAddress;
        }

        /// <summary>
        /// Gets uri to file that describe versions history.
        /// </summary>
        [Export(ConfigurationKeys.VersionControlConnectionString)]
        public string VersionControlConnectionString { get; private set; }

        /// <summary>
        /// Gets address of project home.
        /// </summary>
        [Export(ConfigurationKeys.ProjectHomeUri)]
        public Uri ProjectHomeUri { get; private set; }

        /// <summary>
        /// Gets address of my blog.
        /// </summary>
        [Export(ConfigurationKeys.BlogUri)]
        public Uri BlogUri { get; private set; }

        /// <summary>
        /// Gets address where available new product version.
        /// </summary>
        [Export(ConfigurationKeys.GetNewVersionUri)]
        public Uri GetNewVersionUri { get; private set; }

        /// <summary>
        /// Gets the cache file path.
        /// </summary>
        [Export(ConfigurationKeys.CacheFilePath)]
        public string CacheFilePath { get; private set; }

        /// <summary>
        /// Gets the last fm API key.
        /// </summary>
        [Export(ConfigurationKeys.LastFmApiKey)]
        public string LastFmApiKey { get; private set; }
        
        /// <summary>
        /// Gets the last fm service base address.
        /// </summary>
        [Export(ConfigurationKeys.LastFmServiceBaseAddress)]
        public string LastFmServiceBaseAddress { get; private set; }

        /// <summary>
        /// Gets the last fm fingerprint client path.
        /// </summary>
        [Export(ConfigurationKeys.LastFmFingerprintClientPath)]
        public string LastFmFingerprintClientPath { get; private set; }

        /// <summary>
        /// Creates the shell or main window of the application.
        /// </summary>
        /// <remarks>
        /// If the returned instance is a <see cref="DependencyObject"/>, the
        /// <see cref="MefBootstrapper"/> will attach the default <seealso cref="Microsoft.Practices.Prism.Regions.IRegionManager"/> of
        /// the application in its <see cref="Microsoft.Practices.Prism.Regions.RegionManager.RegionManagerProperty"/> attached property
        /// in order to be able to add regions by using the <seealso cref="Microsoft.Practices.Prism.Regions.RegionManager.RegionNameProperty"/>
        /// attached property from XAML.
        /// </remarks>
        /// <returns>
        /// The shell of the application.
        /// </returns>
        protected override DependencyObject CreateShell()
        {
            return Container.GetExportedValue<Shell>();
        }

        /// <summary>
        /// Initializes the shell.
        /// </summary>
        /// <remarks>
        /// The base implemention ensures the shell is composed in the container.
        /// </remarks>
        protected override void InitializeShell()
        {
            base.InitializeShell();

            Application.Current.MainWindow = (Shell)Shell;
            Application.Current.MainWindow.Show();
        }

        /// <summary>
        /// Configures the <see cref="AggregateCatalog"/> used by MEF.
        /// </summary>
        /// <remarks>
        /// The base implementation does nothing.
        /// </remarks>
        protected override void ConfigureAggregateCatalog()
        {
            base.ConfigureAggregateCatalog();

            // Add this assembly to export ModuleTracker
            AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(Bootstrapper).Assembly));
            AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(Mp3MetaProvider).Assembly));
        }
    }
}
