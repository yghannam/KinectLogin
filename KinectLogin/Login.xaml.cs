using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;

namespace KinectLogin
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private static readonly int Bgr32BytesPerPixel = (PixelFormats.Bgr32.BitsPerPixel + 7) / 8;
        private readonly KinectSensorChooser sensorChooser = new KinectSensorChooser();
        private WriteableBitmap colorImageWritableBitmap;
        private byte[] colorImageData;
        private ColorImageFormat currentColorImageFormat = ColorImageFormat.Undefined;
        private bool gesturesProvided, voiceProvided;
        private bool faceAuthenticated, gesturesAuthenticated, voiceAuthenticated;
        private bool successDialogShown;

        public Login()
        {
            InitializeComponent();

            // Show message for modes where no data was recorded
            if (KinectManager.getGestureSet() == null || 
                KinectManager.getGestureSet().getGestures() == null ||
                KinectManager.getGestureSet().getGestures().Count(g => g != null) == 0)
            {
                gesturesProvided = false;
                gesturesAuthenticated = true;
                this.gestureRecognitionAuthenticationStatus.Text = "Not Provided";
                this.gestureRecognitionAuthenticationStatus.Foreground = Brushes.Black;
            }
            else
            {
                gesturesProvided = true;
            }

            if (KinectManager.getVoicePassword() == null)
            {
                voiceProvided = false;
                voiceAuthenticated = true;
                this.voiceRecognitionAuthenticationStatus.Text = "Not Provided";
                this.voiceRecognitionAuthenticationStatus.Foreground = Brushes.Black;
            }
            else
            {
                voiceProvided = true;
            }

            var faceTrackingViewerBinding = new Binding("Kinect") { Source = sensorChooser };
            faceTrackingViewer.SetBinding(FaceTrackingViewer.KinectProperty, faceTrackingViewerBinding);

            sensorChooser.KinectChanged += SensorChooserOnKinectChanged;

            sensorChooser.Start();
        }

        private void SensorChooserOnKinectChanged(object sender, KinectChangedEventArgs kinectChangedEventArgs)
        {
            KinectSensor oldSensor = kinectChangedEventArgs.OldSensor;
            KinectSensor newSensor = kinectChangedEventArgs.NewSensor;

            if (oldSensor != null)
            {
                oldSensor.AllFramesReady -= KinectSensorOnAllFramesReady;
                oldSensor.ColorStream.Disable();
                oldSensor.DepthStream.Disable();
                oldSensor.DepthStream.Range = DepthRange.Default;
                oldSensor.SkeletonStream.Disable();
                oldSensor.SkeletonStream.EnableTrackingInNearRange = false;
                oldSensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Default;
            }

            if (newSensor != null)
            {
                try
                {
                    newSensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                    newSensor.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30);
                    try
                    {
                        // This will throw on non Kinect For Windows devices.
                        newSensor.DepthStream.Range = DepthRange.Near;
                        newSensor.SkeletonStream.EnableTrackingInNearRange = true;
                    }
                    catch (InvalidOperationException)
                    {
                        newSensor.DepthStream.Range = DepthRange.Default;
                        newSensor.SkeletonStream.EnableTrackingInNearRange = false;
                    }

                    newSensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
                    newSensor.SkeletonStream.Enable();
                    newSensor.AllFramesReady += KinectSensorOnAllFramesReady;
                }
                catch (InvalidOperationException)
                {
                    // This exception can be thrown when we are trying to
                    // enable streams on a device that has gone away.  This
                    // can occur, say, in app shutdown scenarios when the sensor
                    // goes away between the time it changed status and the
                    // time we get the sensor changed notification.
                    //
                    // Behavior here is to just eat the exception and assume
                    // another notification will come along if a sensor
                    // comes back.
                }
            }
        }

        private void WindowClosed(object sender, EventArgs e)
        {
            sensorChooser.Stop();
            faceTrackingViewer.Dispose();
            System.Environment.Exit(0);
        }

        public void stopKinect()
        {
            sensorChooser.Stop();
            faceTrackingViewer.Dispose();
        }

        private void KinectSensorOnAllFramesReady(object sender, AllFramesReadyEventArgs allFramesReadyEventArgs)
        {
            using (var colorImageFrame = allFramesReadyEventArgs.OpenColorImageFrame())
            {
                if (colorImageFrame == null)
                {
                    return;
                }

                // Make a copy of the color frame for displaying.
                var haveNewFormat = this.currentColorImageFormat != colorImageFrame.Format;
                if (haveNewFormat)
                {
                    this.currentColorImageFormat = colorImageFrame.Format;
                    this.colorImageData = new byte[colorImageFrame.PixelDataLength];
                    this.colorImageWritableBitmap = new WriteableBitmap(
                        colorImageFrame.Width, colorImageFrame.Height, 96, 96, PixelFormats.Bgr32, null);
                    ColorImage.Source = this.colorImageWritableBitmap;
                }

                colorImageFrame.CopyPixelDataTo(this.colorImageData);
                this.colorImageWritableBitmap.WritePixels(
                    new Int32Rect(0, 0, colorImageFrame.Width, colorImageFrame.Height),
                    this.colorImageData,
                    colorImageFrame.Width * Bgr32BytesPerPixel,
                    0);
            }

            // Check the facial recognition
            if (!faceAuthenticated && faceTrackingViewer.getFaceModel() != null)
            {
                bool matched = KinectManager.CompareFaces(faceTrackingViewer.getFaceModel());

                if (matched)
                {
                    faceAuthenticated = true;
                    this.facialRecognitionAuthenticationStatus.Text = "Authenticated";
                    this.facialRecognitionAuthenticationStatus.Foreground = Brushes.Green;
                } // else do nothing...
            }

            // Optional:
            // Check the gesture recognition AFTER facial recognition
            if (!gesturesAuthenticated && gesturesProvided && faceAuthenticated)
            {
                bool matched = true;
                int i;
                for (i = 0; i < KinectManager.getGestureSet().getGestures().Length; i++)
                {
                    // TODO: Compare gestures
                    //matched = gestureSet.compare(gestureSet.getGestures()[0], gestureSet.getGestures()[1]);
                }

                if (matched)
                {
                    gesturesAuthenticated = true;
                    this.gestureRecognitionAuthenticationStatus.Text = "Authenticated";
                    this.gestureRecognitionAuthenticationStatus.Foreground = Brushes.Green;
                } // else do nothing...
            }

            // Optional:
            // Check the voice recognition all the time
            if (!voiceAuthenticated && voiceProvided)
            {
                bool matched = true;
                int i;
                //TODO: Compare voices

                if (matched)
                {
                    voiceAuthenticated = true;
                    this.voiceRecognitionAuthenticationStatus.Text = "Authenticated";
                    this.voiceRecognitionAuthenticationStatus.Foreground = Brushes.Green;
                } // else do nothing...
            }

            // Check that all modes are authenticated
            if (faceAuthenticated && gesturesAuthenticated && voiceAuthenticated)
            {
                this.stopKinect();

                if (!successDialogShown)
                {
                    successDialogShown = true;
                    MessageBox.Show("Access Granted.", "Notice");
                }
            }
        }
    }
}
