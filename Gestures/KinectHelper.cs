using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Speech.AudioFormat;
using System.Speech.Recognition;
using System.Text;
using System.Threading;
using Microsoft.Kinect;

namespace Gestures
{
    public static class KinectHelper
    {
        static KinectSensor kinect = null;
        public static Skeleton[] skeletonData;
        public static Stream audioStream;
        //private static Thread audioRecordingThread;
        private static SpeechRecognitionEngine speechEngine;
        public static Gesture gesture;
        public static bool record = false;
        public static bool tracking = false;

        public static void StartKinectST()
        {
            //IReadOnlyCollection<RecognizerInfo> ris = SpeechRecognitionEngine.InstalledRecognizers();

            kinect = KinectSensor.KinectSensors.FirstOrDefault(s => s.Status == KinectStatus.Connected); // Get first Kinect Sensor
            if (kinect == null)
            {
                System.Console.WriteLine("Kinect not detected.");
                System.Environment.Exit(0);
            }
            kinect.SkeletonStream.Enable(); // Enable skeletal tracking

            skeletonData = new Skeleton[kinect.SkeletonStream.FrameSkeletonArrayLength]; // Allocate ST data

            // enable returning skeletons while depth is in Near Range
            kinect.DepthStream.Range = DepthRange.Near; // Depth in near range enabled
            kinect.SkeletonStream.EnableTrackingInNearRange = true;
            kinect.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated; // Use Seated Mode

            kinect.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(kinect_SkeletonFrameReady); // Get Ready for Skeleton Ready Events

            kinect.Start(); // Start Kinect sensor

            audioStream = kinect.AudioSource.Start(); // Create an audio stream

            // Find recognizer and initialize new speech engine with it.
            RecognizerInfo ri = GetKinectRecognizer();
            if (ri != null)
            {
                speechEngine = new SpeechRecognitionEngine(ri.Id);
            }

            DictationGrammar defaultDictionGrammar = new DictationGrammar();
            defaultDictionGrammar.Enabled = true;
            speechEngine.LoadGrammar(defaultDictionGrammar);
            speechEngine.SpeechRecognized += SpeechRecognized;
            speechEngine.SpeechRecognitionRejected += SpeechRejected;
            speechEngine.SetInputToAudioStream(audioStream, new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
            speechEngine.RecognizeAsync(RecognizeMode.Multiple);

        }

        private static RecognizerInfo GetKinectRecognizer()
        {
            foreach (RecognizerInfo recognizer in SpeechRecognitionEngine.InstalledRecognizers())
            {
                // System.Speech does not have a Recognizer for Kinect. Use generic Recognizer.

                //string value;
                //recognizer.AdditionalInfo.TryGetValue("Kinect", out value);
                if ("en-US".Equals(recognizer.Culture.Name, StringComparison.OrdinalIgnoreCase))// && "True".Equals(value, StringComparison.OrdinalIgnoreCase))
                {
                    return recognizer;
                }
            }
            return null;
        }


        /// <summary>
        /// Handler for recognized speech events.
        /// </summary>
        /// <param name="sender">object sending the event.</param>
        /// <param name="e">event arguments.</param>
        private static void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            // Speech utterance confidence below which we treat speech as if it hadn't been heard
            const double ConfidenceThreshold = 0.0;

            if (e.Result.Confidence >= ConfidenceThreshold)
            {
                System.Console.ReadLine();
            }

        }

        /// <summary>
        /// Handler for rejected speech events.
        /// </summary>
        /// <param name="sender">object sending the event.</param>
        /// <param name="e">event arguments.</param>
        private static void SpeechRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            System.Console.WriteLine("Speech not recognized.");
        }

        public static void startRecording(float seconds)
        {
            gesture = new Gesture();
            //gesture.skeletalData = new List<Skeleton>();
            System.Console.WriteLine("Please wait while skeleton is tracked.");
            //while (!tracking)
            //{
            //    tracking = skeletonData.Any(s => s != null && s.TrackingState == SkeletonTrackingState.Tracked);
            //}
            System.Console.WriteLine("Skeleton is now tracked.");
            ExtensionMethods.countdown();
            System.Console.WriteLine("Now Recording");
            //ExtensionMethods.timer(seconds);
            //audioRecordingThread = new Thread(AudioRecordingThread);
            //audioRecordingThread.Name = "Audio Recording Thread";
            //audioRecordingThread.Start();
            record = true;
        }

        public static Gesture stopRecording()
        {
            record = false;
            tracking = false;

            //if (audioRecordingThread != null)
            //{
            //    audioRecordingThread.Join();
            //}

            return ExtensionMethods.DeepClone(gesture);
        }

        //private static void AudioRecordingThread()
        //{

        //    while (record)
        //    {
        //        audioStream.Read(gesture.audioBuffer, 0, gesture.audioBuffer.Length);
        //    }
        //}

        private static void kinect_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame()) // Open the Skeleton frame
            {
                if (skeletonFrame != null && skeletonData != null) // check that a frame is available
                {
                    skeletonFrame.CopySkeletonDataTo(skeletonData); // get the skeletal information in this frame
                }
                if (record)
                {
                    foreach (Skeleton s in skeletonData)
                    {
                        // Save only the tracked skeleton
                        if (s.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            gesture.addSkeletalData(s);
                            break;
                        }
                    }
                }
            }
        }

        public static void DrawSkeletons()
        {
            foreach (Skeleton skeleton in skeletonData)
            {
                if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
                {
                    System.Console.WriteLine("Tracked: "+skeleton.TrackingId);
                    //foreach (Joint j in skeleton.Joints)
                    //{
                    //    System.Console.WriteLine(j.JointType + " " + j.Position.X);
                    //}
                }
                else if (skeleton.TrackingState == SkeletonTrackingState.PositionOnly)
                {
                    System.Console.WriteLine("Not Tracked: " + skeleton.TrackingId);
                }
            }
        }
    }
}
