using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
namespace Gestures
{
    public static class KinectHelper
    {
        static KinectSensor kinect = null;
        public static Skeleton[] skeletonData;
        public static List<Skeleton> skeleton;
        public static bool record = false;
        public static bool tracking = false;

        public static void StartKinectST()
        {
            kinect = KinectSensor.KinectSensors.FirstOrDefault(s => s.Status == KinectStatus.Connected); // Get first Kinect Sensor
            if (kinect == null)
            {
                System.Console.WriteLine("Kinect not detected.");
                System.Environment.Exit(0);
            }
            kinect.SkeletonStream.Enable(); // Enable skeletal tracking

            skeletonData = new Skeleton[kinect.SkeletonStream.FrameSkeletonArrayLength]; // Allocate ST data

            // enable returning skeletons while depth is in Near Range
            kinect.DepthStream.Range = DepthRange.Near; // Depth in near range enabled
            kinect.SkeletonStream.EnableTrackingInNearRange = true;
            kinect.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated; // Use Seated Mode

            kinect.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(kinect_SkeletonFrameReady); // Get Ready for Skeleton Ready Events

            kinect.Start(); // Start Kinect sensor
        }

        public static void startRecording(float seconds)
        {
            skeleton = new List<Skeleton>();
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

        public static List<Skeleton> stopRecording()
        {
            record = false;
            tracking = false;
            return ExtensionMethods.DeepClone(skeleton);
        }

        private static void kinect_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
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
                            skeleton.Add(s);
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
    }
}
