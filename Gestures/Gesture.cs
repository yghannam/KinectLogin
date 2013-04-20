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
        /// <summary>
        /// Number of milliseconds between each read of audio data from the stream.
        /// Faster polling (few tens of ms) ensures a smoother audio stream visualization.
        /// </summary>
        private const int AudioPollingInterval = 50;

        /// <summary>
        /// Number of samples captured from Kinect audio stream each millisecond.
        /// </summary>
        private const int SamplesPerMillisecond = 16;

        /// <summary>
        /// Number of bytes in each Kinect audio stream sample.
        /// </summary>
        private const int BytesPerSample = 2;

        private List<Skeleton> skeletalData = new List<Skeleton>();
        public byte[] audioBuffer = new byte[AudioPollingInterval * SamplesPerMillisecond * BytesPerSample];
        
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
