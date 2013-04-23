using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace Gestures
{
    [Serializable()]
    public class Gesture
    {
        
        private List<Skeleton> skeletalData = new List<Skeleton>();
        
        public void addSkeletalData(Skeleton s)
        {
            this.skeletalData.Add(s);
        }

        public List<Skeleton> getSkeletalData()
        {
            return ExtensionMethods.DeepClone(this.skeletalData);
        }
    }
}
