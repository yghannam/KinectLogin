using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Kinect;

namespace KinectLogin
{
    public class SecurityGestureSet
    {
        private Gesture[] gestures;
        private List<double> errors;
        private Thread AuthenticationThread;
        private bool authenticationStatus = false;

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

        public bool getAuthenticationStatus()
        {
            return this.authenticationStatus;
        }
       
        /// <summary>
        /// Records gestures
        /// </summary>
        /// <param name="numGestures">The number of gestures to record</param>
        /// <param name="seconds">The length of each record in seconds</param>
        public void record(int numGestures, int seconds)
        {
            bool match = false;
            while (!match)
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

                match = compare(gestures[0], gestures[1]);
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
            bool match = false;

            List<String> p1 = g1.getGesturePhrase();
            List<String> p2 = g2.getGesturePhrase();

            if (p1.Count == p2.Count)
            {
                for (int i = 0; i < p1.Count; i++)
                {
                    match = p1.ElementAt(i).Equals(p2.ElementAt(i));
                    if (!match)
                        break;
                }
            }

            if (!match)
                System.Console.WriteLine("Gestures do not match. Please try again.");
            else
                System.Console.WriteLine("Gestures match.");
            return match;
        }

        public void Authenticate()
        {
            System.Console.WriteLine("Recording " + 3);
            KinectHelper.startRecording();
            ExtensionMethods.timer(10);
            gestures[2] = KinectHelper.stopRecording();
            gestures[2].extractGestures();
            System.Console.WriteLine("Finished Recording " + 3 + "\n");

            authenticationStatus = compare(gestures[0], gestures[2]);
        }
    }
}
