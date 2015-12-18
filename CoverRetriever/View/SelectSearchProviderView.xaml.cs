using CoverRetriever.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Application = System.Windows.Application;

namespace CoverRetriever.View
{
    /// <summary>
    /// Interaction logic for SelectSearchProviderView.xaml
    /// </summary>
    public partial class SelectSearchProviderView : System.Windows.Window, System.Windows.Markup.IComponentConnector
    {
        public SelectSearchProviderView()
        {
            InitializeComponent();
        }

        public SelectSearchProviderViewModel ViewModel
        {
            get { return DataContext as SelectSearchProviderViewModel; }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Window.Closing"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.ComponentModel.CancelEventArgs"/> that contains the event data.</param>
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (ViewModel != null && !ViewModel.IsCloseEnabled)
            {
                Application.Current.Shutdown(0);
            }
        }
    }
}
