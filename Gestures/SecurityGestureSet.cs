using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace Gestures
{
    public class SecurityGestureSet
    {
        private Gesture[] gestures;
        private List<double> errors;

        /// <summary>
        /// Constructor
        /// </summary>
        public SecurityGestureSet()
        {
            this.gestures = new Gesture[10];
            this.errors = new List<double>();
        }

        /// <summary>
        /// Gets the gestures
        /// </summary>
        /// <returns>An array of gestures</returns>
        public Gesture[] getGestures()
        {
            return this.gestures;
        }
       
        /// <summary>
        /// Records gestures
        /// </summary>
        /// <param name="numGestures">The number of gestures to record</param>
        /// <param name="seconds">The length of each record in seconds</param>
        public void record(int numGestures, float seconds)
        {
            for (int i = 1; i <= numGestures; i++)
            {
                System.Console.WriteLine("Recording " + i);
                KinectHelper.startRecording();
                ExtensionMethods.timer(seconds);
                gestures[i - 1] = KinectHelper.stopRecording();
                gestures[i - 1].extractGestures();
                System.Console.WriteLine("Finished Recording " + i + "\n");
            }
        }
        
        /// <summary>
        /// Computes the error between two 3D skeletal points p1 and p2
        /// </summary>
        /// <param name="p1">The first 3D skeletal point</param>
        /// <param name="p2">The second 3D skeletal point</param>
        /// <returns></returns>
        double error(SkeletonPoint p1, SkeletonPoint p2)
        {
            double error = Math.Abs(p1.X - p2.X) + Math.Abs(p1.Y - p2.Y) + Math.Abs(p1.Z - p2.Z);
            return error;
        }

        /// <summary>
        /// Compares two Gestures g1 and g2 and prints "No Match" if they are not similar enough for some threshold or "Match" if they are
        /// </summary>
        /// <param name="g1">The first Gesture to compare</param>
        /// <param name="g2">The second Gesture to compare</param>
        /// <returns>true if g1 and g2 are a match; false otherwise</returns>
        public bool compare(Gesture g1, Gesture g2)
        {
            double threshold = 10.0;
            double sum = 0.0;
            bool diff = false;

            List<Skeleton> s1 = g1.getSkeletalData();
            List<Skeleton> s2 = g2.getSkeletalData();
            double[] jointErrors = new double[20];

            for (int i = 0; i < Math.Min(s1.Count, s2.Count); i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    SkeletonPoint p1 = s1[i].Joints.ElementAt(j).Position;
                    SkeletonPoint p2 = s2[i].Joints.ElementAt(j).Position;
                    double err = error(p1, p2);
                    errors.Add(err);
                    sum += err;
                    jointErrors[j] += err;
                    if (jointErrors[j] > threshold)
                    {
                        diff = true;
                        break;
                    }
                    //if(g1[i].Joints.ElementAt(j).JointType == JointType.HandLeft || g1[i].Joints.ElementAt(j).JointType == JointType.HandRight)
                    //    System.Console.WriteLine("{0}: ({1}, {2}, {3}) ({4}, {5}, {6}), {7}", g1[i].Joints.ElementAt(j).JointType, p1.X, p1.Y, p1.Z, p2.X, p2.Y, p2.Z, err);
                }
            }

            String a = diff ? "No " : "";
            System.Console.WriteLine(a + "Match");
            System.Console.WriteLine(sum);
            return diff;
        }
    }
}
