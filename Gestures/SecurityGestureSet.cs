using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace Gestures
{
    class SecurityGestureSet
    {
        List<Skeleton> g1 = new List<Skeleton>();
        List<Skeleton> g2 = new List<Skeleton>();

        void countdown()
        {
            for (int i = 5; i > 0; i--)
            {
                System.Console.Write(i);
                System.Threading.Thread.Sleep(250);
                System.Console.Write(".");
                System.Threading.Thread.Sleep(250);
                System.Console.Write(".");
                System.Threading.Thread.Sleep(250);
                System.Console.Write(".");
                System.Threading.Thread.Sleep(250);
            }
            System.Console.WriteLine("GO!");
        }

        void timer(float seconds)
        {
            DateTime startTime = DateTime.Now;
            DateTime currentTime = DateTime.Now;
            while (currentTime.Second - startTime.Second < seconds)
            {
                if (currentTime.Second - startTime.Second % 1 == 0)
                    System.Console.WriteLine(currentTime.Second - startTime.Second);
                currentTime = DateTime.Now;
            }
        }

        public void record(float seconds)
        {
            System.Console.Write("Press ENTER to start recording");
            System.Console.ReadLine();
            
            countdown();

            System.Console.WriteLine("Recording 1");            
            KinectHelper.startRecording();
            timer(seconds);
            g1 = KinectHelper.stopRecording();
            System.Console.WriteLine("Finished Recording 1");

            //System.Console.Write("Press ENTER to continue");
            //System.Console.ReadLine();

            countdown();

            System.Console.WriteLine("Recording 2");
            KinectHelper.startRecording();
            timer(seconds);
            g2 = KinectHelper.stopRecording();
            System.Console.WriteLine("Finished Recording 2");

        }

        double error(SkeletonPoint p1, SkeletonPoint p2)
        {
            double error = Math.Abs(p1.X - p2.X) + Math.Abs(p1.Y - p2.Y) + Math.Abs(p1.Z - p2.Z);
            return error;
        }

        public bool compare()//Gesture g1, Gesture g2)
        {
            double threshold = 0.50;
            double sum = 0.0;
            bool diff = false;

            for (int i = 0; i < Math.Min(g1.Count, g2.Count); i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    SkeletonPoint p1 = g1[i].Joints.ElementAt(j).Position;
                    SkeletonPoint p2 = g2[i].Joints.ElementAt(j).Position;
                    double err = error(p1, p2);
                    sum += err;
                    if (err > threshold)
                    {
                        diff = true;
                        break;
                    }
                    if(g1[i].Joints.ElementAt(j).JointType == JointType.HandLeft || g1[i].Joints.ElementAt(j).JointType == JointType.HandRight)
                        System.Console.WriteLine("{0}: ({1}, {2}, {3}) ({4}, {5}, {6}), {7}", g1[i].Joints.ElementAt(j).JointType, p1.X, p1.Y, p1.Z, p2.X, p2.Y, p2.Z, err);
                }
            }

            String a = diff ? "No " : "";
            System.Console.WriteLine(a + "Match");
            System.Console.WriteLine(sum);
            return g1 == g2;
        }
    }
}
