using System;

namespace CoverRetriever.ViewModel
{
	public abstract class ViewModelBase : NotifyPropertyChanged
	{
		private bool _isBusy;
		private string _operationName;
		
		/// <summary>
		/// Indicate is view model is busy
		/// </summary>
		public bool IsBusy
		{
			get
			{
				return _isBusy;
			}
			private set
			{
				_isBusy = value;
				RaisePropertyChanged("IsBusy");
			}
		}

		/// <summary>
		/// Get current operation name
		/// </summary>
		public string OperationName
		{
			get
			{
				return _operationName;
			}
			private set
			{
				_operationName = value;
				RaisePropertyChanged("OperationName");
			}
		}

		/// <summary>
		/// Set start of operation
		/// </summary>
		/// <param name="operationName">Name of started operation</param>
		protected void StartOperation(string operationName)
		{
			OperationName = operationName;
			IsBusy = true;
		}

		/// <summary>
		/// finish of operation
		/// </summary>
		protected void EndOperation()
		{
			IsBusy = false;
			OperationName = String.Empty;
		}
	}
}