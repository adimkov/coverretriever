using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace CoverRetriever.Controls
{
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;

    public class ImageAsyncSource
    {
        public static readonly DependencyProperty AsyncSourceProperty =
            DependencyProperty.RegisterAttached("AsyncSource", typeof (IObservable<Stream>), typeof (ImageAsyncSource),
                                                new PropertyMetadata(new PropertyChangedCallback(OnAsyncSourceChanged)));

        public static readonly DependencyProperty DispatcherProperty = DependencyProperty.RegisterAttached(
            "Dispatcher", typeof(IScheduler), typeof(ImageAsyncSource), new PropertyMetadata(DispatcherScheduler.Instance));

        public static void SetAsyncSource(Image o, IObservable<Stream> value)
        {
            o.SetValue(AsyncSourceProperty, value);
        }

        public static IObservable<Stream> GetAsyncSource(DependencyObject o)
        {
            return (IObservable<Stream>) o.GetValue(AsyncSourceProperty);
        }

        public static void SetDispatcher(DependencyObject o, IScheduler value)
        {
            o.SetValue(DispatcherProperty, value);
        }

        public static IScheduler GetDispatcher(DependencyObject o)
        {
            return (IScheduler)o.GetValue(DispatcherProperty);
        }

        private static void OnAsyncSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var image = d as Image;
            var source = e.NewValue as IObservable<Stream>;
            if (image != null && source != null)
            {
                image.Source = null;
                AddSource(image, source, GetDispatcher(d));
            }
        }

        private static void AddSource(Image image, IObservable<Stream> source, IScheduler scheduler)
        {
            source
                .ObserveOn(scheduler)
                .Subscribe(stream =>
                                {
                                    var bitmapImage = new BitmapImage();
                                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                                    bitmapImage.BeginInit();
                                    bitmapImage.CreateOptions = BitmapCreateOptions.IgnoreColorProfile; 
                                    bitmapImage.StreamSource = stream;
                                    bitmapImage.EndInit();

                                    image.Source = bitmapImage;
                                },
                                ex =>
                                    {
                                        // mute exception
                                        image.Source = null;
                                    });
        }
    }
}