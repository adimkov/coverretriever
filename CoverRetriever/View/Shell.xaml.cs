using System.ComponentModel.Composition;
using CoverRetriever.ViewModel;

namespace CoverRetriever.View
{
	/// <summary>
	/// Interaction logic for Shell.xaml
	/// </summary>
	[Export]
	public partial class Shell
	{
		private readonly CoverRetrieverViewModel _viewModel;

		[ImportingConstructor]
		public Shell(CoverRetrieverViewModel viewModel)
		{
			InitializeComponent();
			_viewModel = viewModel;
			DataContext = _viewModel;
		}
	}
}
