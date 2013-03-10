using CoverRetriever.ViewModel;

using NUnit.Framework;

namespace CoverRetriever.Test.ViewModel
{
    using CoverRetriever.Common.ViewModel;

    [TestFixture]
	public class ViewModelBaseTest
	{
		[Test]
		public void StartOperation_should_set_IsBusy_and_OperationName()
		{
			var target = new TestableViewModelBase();
			
			target.StartOperation("Operation1");

			Assert.IsTrue(target.IsBusy);
			Assert.That(target.OperationName, Is.EqualTo("Operation1"));
		}

		[Test]
		public void EndOperation_should_reset_IsBusy_and_OperationName()
		{
			var target = new TestableViewModelBase();

			target.StartOperation("Operation1");
			target.EndOperation();

			Assert.IsFalse(target.IsBusy);
			Assert.That(target.OperationName, Is.Empty);
		}

		[Test]
		public void EndOperation_should_raise_event_property_changed_twice()
		{
			var eventCounter = 0;
			var target = new TestableViewModelBase();
			target.PropertyChanged += (sender, args) => eventCounter++;
			target.EndOperation();

			Assert.That(eventCounter, Is.EqualTo(2));
		}

		[Test]
		public void StartOperation_should_raise_event_property_changed_twice()
		{
			var eventCounter = 0;
			var target = new TestableViewModelBase();
			target.PropertyChanged += (sender, args) => eventCounter++;
			target.StartOperation("Operation1");

			Assert.That(eventCounter, Is.EqualTo(2));
		}
	}

	class TestableViewModelBase : ViewModelBase
	{
		public void StartOperation(string operationName)
		{
			base.StartOperation(operationName);
		}

		public void EndOperation()
		{
			base.EndOperation();
		}
	}
}