using System.ComponentModel.Composition.Hosting;
using System.Windows;

using CoverRetriever.AudioInfo;
using CoverRetriever.View;

using Microsoft.Practices.Prism.MefExtensions;

namespace CoverRetriever
{
	/// <summary>
	/// Install cover retriever to start
	/// </summary>
	public class CoverRetreiverBootstrapper : MefBootstrapper
	{
		/// <summary>
		/// Creates the shell or main window of the application.
		/// </summary>
		/// <returns>The shell of the application.</returns>
		/// <remarks>
		/// If the returned instance is a <see cref="DependencyObject"/>, the
		/// <see cref="MefBootstrapper"/> will attach the default <seealso cref="Microsoft.Practices.Composite.Regions.IRegionManager"/> of
		/// the application in its <see cref="Microsoft.Practices.Composite.Presentation.Regions.RegionManager.RegionManagerProperty"/> attached property
		/// in order to be able to add regions by using the <seealso cref=""Microsoft.Practices.Composite.Presentation.Regions.RegionManager.RegionNameProperty"/>
		/// attached property from XAML.
		/// </remarks>
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
			this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(CoverRetreiverBootstrapper).Assembly));
			this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(Mp3MetaProvider).Assembly));
		}
	}
}