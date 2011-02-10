using System.Windows;
using System.Windows.Controls;

using CoverRetriever.ViewModel;

namespace CoverRetriever.View
{
	/// <summary>
	/// Interaction logic for CoverPreviewView.xaml
	/// </summary>
	public partial class CoverPreviewView : UserControl
	{
		public CoverPreviewView()
		{
			InitializeComponent();
		}

		private void CoverPreviewView_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if(e.OldValue != null)
			{
				((CoverPreviewViewModel)e.OldValue).FinishCommand.Execute();
			}
		}
	}
}
