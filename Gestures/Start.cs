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
            KinectHelper.StartKinectST();

            SecurityGestureSet sgs = new SecurityGestureSet();
            sgs.record(2);
            sgs.compare();

            System.Console.WriteLine("Press ENTER to close.");
            System.Console.ReadLine();
        }
    }
}
