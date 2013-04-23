using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestures;

namespace KinectLoginWpf
{
    public static class KinectManager
    {
        private static KinectHelper helper;
        private static SecurityGestureSet gestureSet;

        public static void setup()
        {
            if (helper == null)
            {
                helper = new KinectHelper();
                helper.StartKinectST();
            }

            if (gestureSet == null)
            {
                gestureSet = new SecurityGestureSet();
            }
        }

        public static KinectHelper getKinectHelper() {
            if (helper == null)
            {
                helper = new KinectHelper();
                helper.StartKinectST();
                return helper;
            }
            else
            {
                return helper;
            }
        }

        public static SecurityGestureSet getGestureSet()
        {
            if (gestureSet == null)
            {
                gestureSet = new SecurityGestureSet();
                return gestureSet;
            }
            else
            {
                return gestureSet;
            }
        }

        /// <summary>
        /// Records the gestures using KinectHelper. This is intended to be ran on its own thread.
        /// </summary>
        /// <param name="numGestures">The number of gestures to record</param>
        /// <param name="numSeconds">The number of seconds for each gesture</param>
        public static void recordGestures(int numGestures, float numSeconds)
        {
            gestureSet.record(numGestures, numSeconds);
        }
    }
}
