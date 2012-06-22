using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Devices;
using com.google.zxing;
using com.google.zxing.oned;
using com.google.zxing.common;
using System.Windows.Threading;
using System.Threading;
using System.Diagnostics;
using System.Windows.Media.Imaging;
using System.IO;
using WebHelpers;

namespace DouMi
{
    public partial class BarCode : PhoneApplicationPage
    {
        private PhotoCamera _photoCamera;
        private PhotoCameraLuminanceSource _luminance;
        private readonly DispatcherTimer _timer;
        private EAN13Reader _reader;

        public BarCode()
        {
            InitializeComponent();

            _reader = new EAN13Reader();

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(250);
            _timer.Tick += (o, arg) => ScanPreviewBuffer();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (Microsoft.Devices.Environment.DeviceType == Microsoft.Devices.DeviceType.Emulator)
            {
                MessageBox.Show("You must deploy this sample to a device, instead of the emulator so that you can get a video stream including a barcode/QR code");
                this.IsEnabled = false;
                base.NavigationService.GoBack();
            }
            else
            {
                _photoCamera = new PhotoCamera();
                _photoCamera.Initialized += new EventHandler<CameraOperationCompletedEventArgs>(cam_Initialized);
                _videoBrush.SetSource(_photoCamera);
                BarCodeRectInitial();
                base.OnNavigatedTo(e);
            }
        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            if (_photoCamera != null)
            {
                _timer.Stop();
                _photoCamera.CancelFocus();
                _photoCamera.Dispose();
            }
            
            base.OnNavigatingFrom(e);
        }

        void cam_Initialized(object sender, CameraOperationCompletedEventArgs e)
        {
            int width = Convert.ToInt32(_photoCamera.PreviewResolution.Width);
            int height = Convert.ToInt32(_photoCamera.PreviewResolution.Height);
            _luminance = new PhotoCameraLuminanceSource(width, height);
            
            Dispatcher.BeginInvoke(() =>
            {
                _previewTransform.Rotation = _photoCamera.Orientation;
                _timer.Start();
            });
            _photoCamera.FlashMode = FlashMode.Auto;
            _photoCamera.Focus();
        }

        public void SetStillPicture()
        {
            int width = Convert.ToInt32(_photoCamera.PreviewResolution.Width);
            int height = Convert.ToInt32(_photoCamera.PreviewResolution.Height);
            int[] PreviewBuffer = new int[width * height];
            _photoCamera.GetPreviewBufferArgb32(PreviewBuffer);

            WriteableBitmap wb = new WriteableBitmap(width, height);
            PreviewBuffer.CopyTo(wb.Pixels, 0);

            MemoryStream ms = new MemoryStream();
            wb.SaveJpeg(ms, wb.PixelWidth, wb.PixelHeight, 0, 80);
            ms.Seek(0, SeekOrigin.Begin);

            BitmapImage bi = new BitmapImage();
            bi.SetSource(ms);
            ImageBrush still = new ImageBrush();
            still.ImageSource = bi;
            frame.Fill = still;
            still.RelativeTransform = new CompositeTransform() 
                { CenterX = 0.5, CenterY = 0.5, Rotation = _photoCamera.Orientation };
        }

        private void ScanPreviewBuffer()
        {
            try
            {
                _photoCamera.GetPreviewBufferY(_luminance.PreviewBufferY);
                var binarizer = new HybridBinarizer(_luminance);
                var binBitmap = new BinaryBitmap(binarizer);
                Result result = _reader.decode(binBitmap);
                if (result != null)
                {
                    _timer.Stop();
                    SetStillPicture();
                    BarCodeRectSuccess();
                    Dispatcher.BeginInvoke(() =>
                    {
                        if (WebHelper.Instance.InternetIsAvailableNotify())
                        {
                            NavigationService.Navigate(new Uri("/BookDetailPanoramaPage.xaml?isbn=" + result.Text, UriKind.Relative));
                        }
                        else
                        {
                            if (NavigationService.CanGoBack)
                                NavigationService.GoBack();
                        }
                    });
                }
                else 
                {
                    _photoCamera.Focus();
                }
            }
            catch
            {
            }
        }



        void BarCodeRectSuccess()
        {
            Dispatcher.BeginInvoke(() =>
            {
                _marker1.Fill = new SolidColorBrush(Colors.Green);
                _marker2.Fill = new SolidColorBrush(Colors.Green);
                _marker3.Fill = new SolidColorBrush(Colors.Green);
                _marker4.Fill = new SolidColorBrush(Colors.Green);
                _marker5.Fill = new SolidColorBrush(Colors.Green);
                _marker6.Fill = new SolidColorBrush(Colors.Green);
                _marker7.Fill = new SolidColorBrush(Colors.Green);
                _marker8.Fill = new SolidColorBrush(Colors.Green);
            });
        }

        void BarCodeRectInitial()
        {
            _marker1.Fill = new SolidColorBrush(Colors.Red);
            _marker2.Fill = new SolidColorBrush(Colors.Red);
            _marker3.Fill = new SolidColorBrush(Colors.Red);
            _marker4.Fill = new SolidColorBrush(Colors.Red);
            _marker5.Fill = new SolidColorBrush(Colors.Red);
            _marker6.Fill = new SolidColorBrush(Colors.Red);
            _marker7.Fill = new SolidColorBrush(Colors.Red);
            _marker8.Fill = new SolidColorBrush(Colors.Red);
        }
    }
}