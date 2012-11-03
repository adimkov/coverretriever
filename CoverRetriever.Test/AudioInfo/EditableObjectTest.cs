namespace CoverRetriever.Test.AudioInfo
{
    using CoverRetriever.AudioInfo;

    using FluentAssertions;

    using NUnit.Framework;

    [TestFixture]
    public class EditableObjectTest
    {
        [Test]
        public void Should_cancel_changes()
        {
            var revertableObject = new MockEditableObject("Test", true);
            revertableObject.BeginEdit();

            revertableObject.StringValue = "Data";
            revertableObject.BoolValue = false;

            revertableObject.CancelEdit();

            revertableObject.StringValue.Should().Be("Test");
            revertableObject.BoolValue.Should().Be(true);
        }

        [Test]
        public void Should_commit_changes()
        {
            var revertableObject = new MockEditableObject("Test", true);
            revertableObject.BeginEdit();

            revertableObject.StringValue = "Data";
            revertableObject.BoolValue = false;

            revertableObject.EndEdit();

            revertableObject.StringValue.Should().Be("Data");
            revertableObject.BoolValue.Should().Be(false);
        }

        [Test]
        public void Should_indicate_not_changed()
        {
            var revertableObject = new MockEditableObject("Test", true);
            revertableObject.BeginEdit();

            revertableObject.IsChanged().Should().BeFalse();
        }

        [Test]
        public void Should_indicate_changed()
        {
            var revertableObject = new MockEditableObject("Test", true);
            revertableObject.BeginEdit();
            
            revertableObject.StringValue = "Data";

            revertableObject.IsChanged().Should().BeTrue();
        }

        [Test]
        public void Should_indicate_changed_if_title_reset_null()
        {
            var revertableObject = new MockEditableObject("Test", true);
            revertableObject.BeginEdit();

            revertableObject.StringValue = null;

            revertableObject.IsChanged().Should().BeTrue();
    
        }

        [Test]
        public void Should_indicate_changed_if_set_title_from_null_to_value()
        {
            var revertableObject = new MockEditableObject(null, null, true);
            revertableObject.BeginEdit();

            revertableObject.StringValue = "Test";

            revertableObject.IsChanged().Should().BeTrue();
        }
    }

    class MockEditableObject : EditableObject
    {
        public MockEditableObject(string stringValue, bool boolValue)
        {
            StringValue = stringValue;
            BoolValue = boolValue;
        }

        public MockEditableObject(string stringValue, object data, bool boolValue)
        {
            StringValue = stringValue;
            AData = data;
            BoolValue = boolValue;
        }

        public string StringValue { get; set; }
        public object AData { get; set; }
        public bool BoolValue { get; set; }
    }
}