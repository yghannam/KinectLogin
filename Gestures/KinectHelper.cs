using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Microsoft.Kinect;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;

namespace Gestures
{
    public class KinectHelper
    {
        public static KinectSensor kinect = null;

        public static KinectSensor getKinectSensor()
        {
            return kinect;
        }

        public static Skeleton[] skeletonData;
        private static SpeechRecognitionEngine speechEngine;
        public static Gesture gesture;
        public static Voice voice;
        public static bool record = false;
        public static bool tracking = false;
		
		public static bool voiceRecord = false;
        
        // Depth image fields
        private static readonly int Bgr32BytesPerPixel = (PixelFormats.Bgr32.BitsPerPixel + 7) / 8;

        private DepthColorizer colorizer = new DepthColorizer();
        private KinectDepthTreatment depthTreatment = KinectDepthTreatment.ClampUnreliableDepths;

        private DepthImageFormat lastImageFormat;
        private DepthImagePixel[] pixelData;

        // We want to control how depth data gets converted into false-color data
        // for more intuitive visualization, so we keep 32-bit color frame buffer versions of
        // these, to be updated whenever we receive and process a 16-bit frame.
        private byte[] depthFrame32;
        public Image depthPictureBoxImage;

        public event EventHandler DepthImageUpdated;
        public WriteableBitmap outputBitmap;
        
        public void StartKinectST()
        {
            //IReadOnlyCollection<RecognizerInfo> ris = SpeechRecognitionEngine.InstalledRecognizers();

            kinect = KinectSensor.KinectSensors.FirstOrDefault(s => s.Status == KinectStatus.Connected); // Get first Kinect Sensor
            if (kinect == null)
            {
                System.Console.WriteLine("Kinect not detected.");
                System.Environment.Exit(0);
            }
            kinect.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30); // Enable the depth stream
            kinect.SkeletonStream.Enable(); // Enable skeletal tracking

            skeletonData = new Skeleton[kinect.SkeletonStream.FrameSkeletonArrayLength]; // Allocate ST data

            // enable returning skeletons while depth is in Near Range
            //kinect.DepthStream.Range = DepthRange.Near; // Depth in near range enabled
            kinect.SkeletonStream.EnableTrackingInNearRange = true;
            kinect.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated; // Use Seated Mode

            kinect.DepthFrameReady += new EventHandler<DepthImageFrameReadyEventArgs>(kinect_DepthFrameReady); // Get Ready for Skeleton Ready Events
            kinect.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(kinect_SkeletonFrameReady); // Get Ready for Skeleton Ready Events

            kinect.Start(); // Start Kinect sensor

            // Find recognizer and initialize new speech engine with it.
            RecognizerInfo ri = GetKinectRecognizer();
            if (ri != null)
            {
                speechEngine = new SpeechRecognitionEngine(ri.Id);

                var choices = new Choices();
                choices.Add(new SemanticResultValue("zero", "ZERO"));
                choices.Add(new SemanticResultValue("one", "ONE"));
                choices.Add(new SemanticResultValue("two", "TWO"));
                choices.Add(new SemanticResultValue("three", "THREE"));
                choices.Add(new SemanticResultValue("four", "FOUR"));
                choices.Add(new SemanticResultValue("five", "FIVE"));
                choices.Add(new SemanticResultValue("six", "SIX"));
                choices.Add(new SemanticResultValue("seven", "SEVEN"));
                choices.Add(new SemanticResultValue("eight", "EIGHT"));
                choices.Add(new SemanticResultValue("nine", "NINE"));

                var gb = new GrammarBuilder { Culture = ri.Culture };
                gb.Append(choices);
                var g = new Grammar(gb);
                speechEngine.LoadGrammar(g);
            }

           
            speechEngine.SpeechRecognized += SpeechRecognized;
            speechEngine.SpeechRecognitionRejected += SpeechRejected;
            speechEngine.SetInputToAudioStream(kinect.AudioSource.Start(), new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
            speechEngine.UpdateRecognizerSetting("AdaptationOn", 0);
            speechEngine.RecognizeAsync(RecognizeMode.Multiple);

        }

        private static RecognizerInfo GetKinectRecognizer()
        {
            foreach (RecognizerInfo recognizer in SpeechRecognitionEngine.InstalledRecognizers())
            {
                // System.Speech does not have a Recognizer for Kinect. Use generic Recognizer.

                string value;
                recognizer.AdditionalInfo.TryGetValue("Kinect", out value);
                if ("en-US".Equals(recognizer.Culture.Name, StringComparison.OrdinalIgnoreCase) && "True".Equals(value, StringComparison.OrdinalIgnoreCase))
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
            const double ConfidenceThreshold = 0.3;

            if (voiceRecord && e.Result.Confidence >= ConfidenceThreshold)
            {
                voice.addVoiceData(e.Result.Semantics.Value.ToString());
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

        public static void startRecording()
        {
            gesture = new Gesture();
            System.Console.WriteLine("Please wait while skeleton is tracked.");
            while (!tracking)
            {
                tracking = skeletonData.Any(s => s != null && s.TrackingState == SkeletonTrackingState.Tracked);
            }
            System.Console.WriteLine("Skeleton is now tracked.");
            ExtensionMethods.countdown();
            System.Console.WriteLine("Now Recording");
            record = true;
        }

        public static Gesture stopRecording()
        {
            record = false;
            tracking = false;

            return ExtensionMethods.DeepClone(gesture);
        }

        public static void startVoiceRecording()
        {
            voice = new Voice();
            System.Console.WriteLine("Now Recording Voice");
            voiceRecord = true;
        }

        public static Voice stopVoiceRecording()
        {
            voiceRecord = false;
            return ExtensionMethods.DeepClone(voice);
        }
       
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


        private void kinect_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            int imageWidth = 0;
            int imageHeight = 0;
            bool haveNewFormat = false;
            int minDepth = 0;
            int maxDepth = 0;

            using (DepthImageFrame imageFrame = e.OpenDepthImageFrame())
            {
                if (imageFrame != null)
                {
                    imageWidth = imageFrame.Width;
                    imageHeight = imageFrame.Height;

                    // We need to detect if the format has changed.
                    haveNewFormat = this.lastImageFormat != imageFrame.Format;

                    if (haveNewFormat)
                    {
                        this.pixelData = new DepthImagePixel[imageFrame.PixelDataLength];
                        this.depthFrame32 = new byte[imageFrame.Width * imageFrame.Height * Bgr32BytesPerPixel];
                        this.lastImageFormat = imageFrame.Format;
                    }

                    imageFrame.CopyDepthImagePixelDataTo(this.pixelData);
                    minDepth = imageFrame.MinDepth;
                    maxDepth = imageFrame.MaxDepth;
                }
            }

            // Did we get a depth frame?
            if (imageWidth != 0)
            {
                colorizer.ConvertDepthFrame(this.pixelData, minDepth, maxDepth, this.depthTreatment, this.depthFrame32);

                if (haveNewFormat)
                {
                    // A WriteableBitmap is a WPF construct that enables resetting the Bits of the image.
                    // This is more efficient than creating a new Bitmap every frame.
                    this.outputBitmap = new WriteableBitmap(
                        imageWidth,
                        imageHeight,
                        96, // DpiX
                        96, // DpiY
                        PixelFormats.Bgr32,
                        null);
                }

                this.outputBitmap.WritePixels(
                    new Int32Rect(0, 0, imageWidth, imageHeight),
                    this.depthFrame32,
                    imageWidth * Bgr32BytesPerPixel,
                    0);

                if (this.DepthImageUpdated != null)
                {
                    this.DepthImageUpdated(this, new EventArgs());
                }
            }
        }
    }
}
