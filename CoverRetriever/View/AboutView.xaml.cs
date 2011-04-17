using System.Diagnostics;
using System.Windows.Navigation;

namespace CoverRetriever.View
{
	/// <summary>
	/// Interaction logic for AboutView.xaml
	/// </summary>
	public partial class AboutView
	{
		public AboutView()
		{
			InitializeComponent();
		}

		private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));

			e.Handled = true;		
		}
	}
}
