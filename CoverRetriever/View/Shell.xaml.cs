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
		private readonly CoverRetreiverViewModel _viewModel;

		[ImportingConstructor]
		public Shell(CoverRetreiverViewModel viewModel)
		{
			InitializeComponent();
			_viewModel = viewModel;
			DataContext = _viewModel;
		}
	}
}
