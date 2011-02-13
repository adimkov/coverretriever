using System.Windows;
using System.Windows.Forms;


namespace CoverRetriever.View
{
	/// <summary>
	/// Interaction logic for OpenFolderView.xaml
	/// </summary>
	public partial class OpenFolderView
	{
		public OpenFolderView()
		{
			InitializeComponent();
		}

		private void Browse_OnClick(object sender, RoutedEventArgs e)
		{

			var openDialog = new FolderBrowserDialog();
			openDialog.Description = "Enter description here";
			openDialog.ShowNewFolderButton = false;
			openDialog.SelectedPath = FolderPathTextBlock.Text;

			if (openDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				FolderPathTextBlock.Text = openDialog.SelectedPath;
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
