using System.Windows.Controls;

namespace CoverRetriever.View
{
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
	/// Interaction logic for FileConductorView.xaml
	/// </summary>
	public partial class FileConductorView : UserControl
	{
		public FileConductorView()
		{
			InitializeComponent();
		}

        private void FileConductorView_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((UIElement)sender).Focus();
        }
	}
}
