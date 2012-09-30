using System;
using System.IO;
using CoverRetriever.Model;
using CoverRetriever.ViewModel;
using NUnit.Framework;

namespace CoverRetriever.Test.ViewModel
{
    using System.Reactive.Linq;

    using CoverRetriever.AudioInfo;

    using Microsoft.Reactive.Testing;

    [TestFixture]
    public class CoverPreviewViewModelTest : ViewModelMock
    {
        [Test]
        public void Ctr_should_create_instance_and_not_throw_exceptions()
        {
            RemoteCover remoteCover = GetRemoteCoverStub(Observable.Empty<Stream>());

            var target = new CoverPreviewViewModel(remoteCover);
            Assert.That(target.CoverSize, Is.EqualTo(remoteCover.CoverSize));
            Assert.That(target.SaveCoverCommand, Is.Not.Null);
            Assert.That(target.CloseCommand, Is.Not.Null);
            Assert.That(target.SaveCover, Is.Not.Null);
            Assert.That(target.ErrorMessage, Is.Null);
        }

        [Test]
        public void Should_begin_load_an_image_on_subscrible()
        {
            var coverStream = Observable.Empty<Stream>();
            
            var remoteCover = GetRemoteCoverStub(coverStream);
            var target = new CoverPreviewViewModel(remoteCover);
            
            target.SetBusy(true, "test");
            Assert.That(target.IsBusy, Is.True);
            
            target.CoverAsyncSource.Subscribe();

            Assert.That(target.IsBusy, Is.False);
            Assert.That(target.OperationName, Is.Not.Null);
            Assert.That(target.ErrorMessage, Is.Null);
        }

        [Test]
        public void SaveCoverCommand_shoult_push_save_command()
        {
            var remoteCover = GetRemoteCoverStub(Observable.Empty<Stream>());
            
            var target = new CoverPreviewViewModel(remoteCover);
            
            var testScheduler = new TestScheduler();
            var mockObservable = new MockObserver<Cover>(testScheduler);

            target.SaveCover.Subscribe(mockObservable);

            target.SaveCoverCommand.Execute();

            Assert.That(mockObservable.Messages[0].Value.Value, Is.EqualTo(remoteCover));
        }
        
        [Test]
        public void Should_set_error_message_of_unable_to_load_image()
        {
            var remoteCover = GetRemoteCoverStub(Observable.Throw<Stream>(new Exception("UnitTest")));
            var target = new CoverPreviewViewModel(remoteCover);
            
            Assert.Throws<Exception>(() => target.CoverAsyncSource.Subscribe());
            
            Assert.That(target.ErrorMessage, Is.EqualTo("UnitTest"));	
        }
    }
}