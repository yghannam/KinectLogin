using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace Gestures
{
    class Start
    {
        static void Main()
        {
            System.Console.WriteLine("Hello!\n");
            KinectHelper helper = new KinectHelper();
            helper.StartKinectST();

            SecurityGestureSet gestureSet = new SecurityGestureSet();

            gestureSet.record(2, 2);

            if (gestureSet.getGestures() != null && gestureSet.getGestures().Length >= 2)
            {
                gestureSet.compare(gestureSet.getGestures()[0], gestureSet.getGestures()[1]);
            }

            System.Console.WriteLine("Press ENTER to close.");
            System.Console.ReadLine();
        }
    }
}
