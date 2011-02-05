using CoverRetriever.ViewModel;

using NUnit.Framework;

namespace CoverRetriever.Test.ViewModel
{
	[TestFixture]
	public class NotifyPropertyChangedTest
	{
		[Test]
		public void RaisePropertyChanged_should_raise_event()
		{
			var target = new TextableNotifyPropertyChanged();
			
			target.PropertyChanged += (sender, args) => Assert.That(args.PropertyName, Is.EqualTo("UnitTest"));
			target.RaisePropertyChanged("UnitTest");
		}
	}

	class TextableNotifyPropertyChanged : NotifyPropertyChanged
	{
		public void RaisePropertyChanged(string propName)
		{
			base.RaisePropertyChanged(propName);
		}
	}
}