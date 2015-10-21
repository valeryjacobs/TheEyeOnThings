using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Diagnostics;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TheEyeOnThings
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private MediaCapture mediaCaptureMgr = null;
        DeviceInformationCollection devices;
        int camIndex = 2;
        int camCount = 0;

        public MainPage()
        {
            this.InitializeComponent();

            Init();

        }

        public async void Init()
        {
            devices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

            foreach(DeviceInformation cam in devices)
            {
                Debug.WriteLine(cam.Name);
            }

                
            camCount = devices.Count;

            if (camCount > 2)
            {
                Start(0);
            }
        }
        public async void Start(int camIndex)
        {
            // devices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
            if (devices.Count > 0)
            {
                // Using Windows.Media.Capture.MediaCapture APIs to stream from webcam
                mediaCaptureMgr = new MediaCapture();
                ///////////////////////////////////


                var captureInitSettings = new Windows.Media.Capture.MediaCaptureInitializationSettings();
                captureInitSettings.StreamingCaptureMode = Windows.Media.Capture.StreamingCaptureMode.Video;
                captureInitSettings.PhotoCaptureSource = Windows.Media.Capture.PhotoCaptureSource.VideoPreview;
                captureInitSettings.VideoDeviceId = devices[camIndex].Id;


                ///////////////////////////////////
                await mediaCaptureMgr.InitializeAsync(captureInitSettings);
                SetResolution();

                camCaptureElement.Source = mediaCaptureMgr;
                await mediaCaptureMgr.StartPreviewAsync();

            }
        }

        public async void SetResolution()
        {
            System.Collections.Generic.IReadOnlyList<IMediaEncodingProperties> res;
            res = mediaCaptureMgr.VideoDeviceController.GetAvailableMediaStreamProperties(MediaStreamType.VideoPreview);

            uint maxResolution = 0;
            int indexMaxResolution = 0;

            if (res.Count >= 1)
            {
                for (int i = 0; i < res.Count; i++)
                {
                    VideoEncodingProperties vp = (VideoEncodingProperties)res[i];



                    if (vp.Width > maxResolution)
                    {
                        indexMaxResolution = i;
                        maxResolution = vp.Width;

                    }
                }
                await mediaCaptureMgr.VideoDeviceController.SetMediaStreamPropertiesAsync(MediaStreamType.VideoPreview, res[indexMaxResolution]);
            }
        }

        private void switchCam_Click(object sender, RoutedEventArgs e)
        {
            camIndex = (camIndex >= camCount - 1) ? 0 : (camIndex + 1);
            Start(camIndex);
        }

        private void setCam_Click(object sender, RoutedEventArgs e)
        {
            Start(0);
        }
    }
}
