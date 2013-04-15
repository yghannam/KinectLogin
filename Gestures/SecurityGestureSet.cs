using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace Gestures
{
    class SecurityGestureSet
    {
        Gesture g1 = new Gesture();
        Gesture g2 = new Gesture();

        List<double> errors;
       

        

        public void record(float seconds)
        {
            System.Console.Write("Press ENTER to start recording");
            System.Console.ReadLine();
          
            System.Console.WriteLine("Recording 1");            
            KinectHelper.startRecording(seconds);
            ExtensionMethods.timer(seconds);
            g1 = KinectHelper.stopRecording();
            System.Console.WriteLine("Finished Recording 1\n");

            //System.Console.Write("Press ENTER to continue");
            //System.Console.ReadLine();

           
            System.Console.WriteLine("Recording 2");
            KinectHelper.startRecording(seconds);
            ExtensionMethods.timer(seconds);
            g2 = KinectHelper.stopRecording();
            System.Console.WriteLine("Finished Recording 2\n");

        }

        double error(SkeletonPoint p1, SkeletonPoint p2)
        {
            double error = Math.Abs(p1.X - p2.X) + Math.Abs(p1.Y - p2.Y) + Math.Abs(p1.Z - p2.Z);
            return error;
        }

        public bool compare()//Gesture g1, Gesture g2)
        {
            double threshold = 10.0;
            double sum = 0.0;
            bool diff = false;
            errors = new List<double>();

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
