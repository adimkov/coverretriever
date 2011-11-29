using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Data;

using CoverRetriever.Model;
using CoverRetriever.Resources;
using CoverRetriever.ViewModel;

namespace CoverRetriever.View
{
    using System.Linq;

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
			Title = CoverRetrieverResources.TextProduct;
			_viewModel = viewModel;
			_viewModel.SavingCoverResult
				.ObserveOnDispatcher()
				.Subscribe(OnNextCoverSaveResult);
			
			DataContext = _viewModel;
		}


		private void OnNextCoverSaveResult(ProcessResult processResult)
		{
			if (processResult == ProcessResult.Begin)
			{
				AnimationGrid.Visibility = Visibility.Visible;

				var animatedBinding = new Binding("SelectedItem");
				animatedBinding.ElementName = "elementFlow";
				AnimationGrid.SetBinding(DataContextProperty, animatedBinding);
			}
			else if (processResult == ProcessResult.Done)
			{
				AnimationGrid.Visibility = Visibility.Collapsed;
				AnimationGrid.ClearValue(DataContextProperty);
			}
		}
	}
}
