using System.ComponentModel;

namespace CoverRetriever.ViewModel
{
	/// <summary>
	/// Base class for all notify objects 
	/// </summary>
	public abstract class NotifyPropertyChanged : INotifyPropertyChanged
	{

		/// <summary>
		/// Raise PropertyChanged
		/// </summary>
		/// <param name="propName">Name of property</param>
		protected void RaisePropertyChanged(string propName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propName));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}