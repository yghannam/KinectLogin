using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect.Toolkit.FaceTracking;

namespace KinectLogin
{
    public static class KinectManager
    {
        private static Face face;

        private static KinectHelper helper;
        private static SecurityGestureSet gestureSet;

        private static VoiceRecognition voiceRecognition;
        private static Voice voicePassword;

        public static void setup()
        {
            if (helper == null)
            {
                helper = new KinectHelper();
            }

            if (gestureSet == null)
            {
                gestureSet = new SecurityGestureSet();
            }

            if (voiceRecognition == null)
            {
                voiceRecognition = new VoiceRecognition();
            }
        }

        public static KinectHelper getKinectHelper() {
            if (helper == null)
            {
                helper = new KinectHelper();
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

        public static VoiceRecognition getVoiceRecognition()
        {
            if (voiceRecognition == null)
            {
                voiceRecognition = new VoiceRecognition();
                return voiceRecognition;
            }
            else
            {
                return voiceRecognition;
            }
        }

        /// <summary>
        /// Records the gestures using KinectHelper. This is intended to be ran on its own thread.
        /// </summary>
        /// <param name="numGestures">The number of gestures to record</param>
        /// <param name="numSeconds">The number of seconds for each gesture</param>
        public static void recordGestures(int numGestures, int numSeconds)
        {
            gestureSet.record(numGestures, numSeconds);
        }

        public static void saveVoicePassword(Voice voice)
        {
            voicePassword = voice;
        }

        public static Voice getVoicePassword()
        {
            return voicePassword;
        }

        /// <summary>
        /// This event is fired when the voice data is updated. 
        /// It checks for password matches in the voice data against the saved voice password in KinectManager.
        /// It is intended to only be executed when the Login form is active.
        /// </summary>
        public static void UpdateVoiceData(object sender, EventArgs e)
        {
            /*
            if (voiceRecognition != null)
            {
                int i;
                Voice[] voices = voiceRecognition.getVoices();

                for (i = 0; i < voices.Count(); i++)
                {
                    bool match = voiceRecognition.compare(voices[i], voicePassword);
                    if (match)
                    {
                    } // else no match
                }
            }
            */
        }

        /// <summary>
        /// Creates a new face object with the provided faceModel
        /// </summary>
        /// <param name="faceModel"></param>
        public static void SaveFace(EnumIndexableCollection<FeaturePoint, Vector3DF> faceModel)
        {
            face = new Face(faceModel);
        }

        /// <summary>
        /// Gets the face
        /// </summary>
        /// <returns>The KinectManager's current face value</returns>
        public static Face getFace()
        {
            return face;
        }

        /// <summary>
        /// Compares the current face with the faceModelCandidate
        /// </summary>
        /// <param name="faceModelCandidate"></param>
        public static bool CompareFaces(EnumIndexableCollection<FeaturePoint, Vector3DF> faceModelCandidate)
        {
            return face.compare(faceModelCandidate);
        }
    }
}
