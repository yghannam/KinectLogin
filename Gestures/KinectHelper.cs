using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Microsoft.Kinect;

namespace Gestures
{
    public class KinectHelper
    {
        public static KinectSensor kinect = null;
        public static Skeleton[] skeletonData;
        public static Gesture gesture;
        public static bool record = false;
        public static bool tracking = false;
        
        // Depth image fields
        private static readonly int Bgr32BytesPerPixel = (PixelFormats.Bgr32.BitsPerPixel + 7) / 8;

        private DepthColorizer colorizer = new DepthColorizer();
        private KinectDepthTreatment depthTreatment = KinectDepthTreatment.ClampUnreliableDepths;

        private DepthImageFormat lastImageFormat;
        private DepthImagePixel[] pixelData;

        // We want to control how depth data gets converted into false-color data
        // for more intuitive visualization, so we keep 32-bit color frame buffer versions of
        // these, to be updated whenever we receive and process a 16-bit frame.
        private byte[] depthFrame32;
        public Image depthPictureBoxImage;

        public event EventHandler DepthImageUpdated;
        public WriteableBitmap outputBitmap;
        
        public void StartKinectST()
        {
            kinect = KinectSensor.KinectSensors.FirstOrDefault(s => s.Status == KinectStatus.Connected); // Get first Kinect Sensor
            if (kinect == null)
            {
                System.Console.WriteLine("Kinect not detected.");
                System.Environment.Exit(0);
            }
            kinect.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30); // Enable the depth stream
            kinect.SkeletonStream.Enable(); // Enable skeletal tracking

            skeletonData = new Skeleton[kinect.SkeletonStream.FrameSkeletonArrayLength]; // Allocate ST data

            // enable returning skeletons while depth is in Near Range
            //kinect.DepthStream.Range = DepthRange.Near; // Depth in near range enabled
            kinect.SkeletonStream.EnableTrackingInNearRange = true;
            kinect.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated; // Use Seated Mode

            kinect.DepthFrameReady += new EventHandler<DepthImageFrameReadyEventArgs>(kinect_DepthFrameReady); // Get Ready for Skeleton Ready Events
            kinect.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(kinect_SkeletonFrameReady); // Get Ready for Skeleton Ready Events

            kinect.Start(); // Start Kinect sensor
        }

        public static void startRecording(float seconds)
        {
            gesture = new Gesture();
            //gesture.skeletalData = new List<Skeleton>();
            System.Console.WriteLine("Please wait while skeleton is tracked.");
            while (!tracking)
            {
                tracking = skeletonData.Any(s => s != null && s.TrackingState == SkeletonTrackingState.Tracked);
            }
            System.Console.WriteLine("Skeleton is now tracked.");
            ExtensionMethods.countdown();
            System.Console.WriteLine("Now Recording");
            //ExtensionMethods.timer(seconds);
            record = true;
        }

        public static Gesture stopRecording()
        {
            record = false;
            tracking = false;
            return ExtensionMethods.DeepClone(gesture);
        }

        private void kinect_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame()) // Open the Skeleton frame
            {
                if (skeletonFrame != null && skeletonData != null) // check that a frame is available
                {
                    skeletonFrame.CopySkeletonDataTo(skeletonData); // get the skeletal information in this frame
                }
                if (record)
                {
                    foreach (Skeleton s in skeletonData)
                    {
                        // Save only the tracked skeleton
                        if (s.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            gesture.addSkeletalData(s);
                            break;
                        }
                    }
                }
            }
        }

        public static void DrawSkeletons()
        {
            foreach (Skeleton skeleton in skeletonData)
            {
                if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
                {
                    System.Console.WriteLine("Tracked: "+skeleton.TrackingId);
                    //foreach (Joint j in skeleton.Joints)
                    //{
                    //    System.Console.WriteLine(j.JointType + " " + j.Position.X);
                    //}
                }
                else if (skeleton.TrackingState == SkeletonTrackingState.PositionOnly)
                {
                    System.Console.WriteLine("Not Tracked: " + skeleton.TrackingId);
                }
            }
        }


        private void kinect_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            int imageWidth = 0;
            int imageHeight = 0;
            bool haveNewFormat = false;
            int minDepth = 0;
            int maxDepth = 0;

            using (DepthImageFrame imageFrame = e.OpenDepthImageFrame())
            {
                if (imageFrame != null)
                {
                    imageWidth = imageFrame.Width;
                    imageHeight = imageFrame.Height;

                    // We need to detect if the format has changed.
                    haveNewFormat = this.lastImageFormat != imageFrame.Format;

                    if (haveNewFormat)
                    {
                        this.pixelData = new DepthImagePixel[imageFrame.PixelDataLength];
                        this.depthFrame32 = new byte[imageFrame.Width * imageFrame.Height * Bgr32BytesPerPixel];
                        this.lastImageFormat = imageFrame.Format;
                    }

                    imageFrame.CopyDepthImagePixelDataTo(this.pixelData);
                    minDepth = imageFrame.MinDepth;
                    maxDepth = imageFrame.MaxDepth;
                }
            }

            // Did we get a depth frame?
            if (imageWidth != 0)
            {
                colorizer.ConvertDepthFrame(this.pixelData, minDepth, maxDepth, this.depthTreatment, this.depthFrame32);

                if (haveNewFormat)
                {
                    // A WriteableBitmap is a WPF construct that enables resetting the Bits of the image.
                    // This is more efficient than creating a new Bitmap every frame.
                    this.outputBitmap = new WriteableBitmap(
                        imageWidth,
                        imageHeight,
                        96, // DpiX
                        96, // DpiY
                        PixelFormats.Bgr32,
                        null);
                }

                this.outputBitmap.WritePixels(
                    new Int32Rect(0, 0, imageWidth, imageHeight),
                    this.depthFrame32,
                    imageWidth * Bgr32BytesPerPixel,
                    0);

                if (this.DepthImageUpdated != null)
                {
                    this.DepthImageUpdated(this, new EventArgs());
                }
            }
        }

        public static Bitmap ByteToImage(byte[] blob)
        {
            MemoryStream mStream = new MemoryStream();
            byte[] pData = blob;
            mStream.Write(pData, 0, Convert.ToInt32(pData.Length));
            Bitmap bm = new Bitmap(mStream, false);
            mStream.Dispose();
            return bm;
        }
    }
}
