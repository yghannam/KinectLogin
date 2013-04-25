using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KinectLogin
{
    /// <summary>
    /// Interaction logic for SetupLogin.xaml
    /// </summary>
    public partial class SetupLogin : Window
    {
        Login login;
        int parsedNumGestures;
        double parsedSeconds;
        FacialRecognitionWindow facialRecognitionWindow;

        public SetupLogin()
        {
            InitializeComponent();

            KinectManager.setup();

            showFacialRecognitionWindow();
        }

        private void recordGestures()
        {
            KinectManager.recordGestures(parsedNumGestures, (float)parsedSeconds);
        }

        private void showFacialRecognitionWindow()
        {
            // Show the FacialRecognitionWindow
            facialRecognitionWindow = new FacialRecognitionWindow();
            facialRecognitionWindow.ShowDialog();

            this.setupFacialRecognition.IsEnabled = false;

            // Success: Make the button green
            this.setupFacialRecognition.Foreground = Brushes.Green;
            this.setupFacialRecognition.FontWeight = FontWeights.Bold;
        }

        private void setupFacialRecognition_Click(object sender, RoutedEventArgs e)
        {
            showFacialRecognitionWindow();
        }

        private void setupGestures_Click(object sender, RoutedEventArgs e)
        {
            if (Int32.TryParse(this.numGestures.Text, out parsedNumGestures) && parsedNumGestures >= 1 && parsedNumGestures <= 10)
            {
                if (Double.TryParse(this.gestureLength.Text, out parsedSeconds) && parsedSeconds >= 1.0 && parsedSeconds <= 10.0)
                {
                    // Create a new thread for recording
                    Thread gestureRecordingThread = new Thread(new ThreadStart(recordGestures));
                    gestureRecordingThread.Start();

                    // Success: Make the button green
                    this.setupGestures.Foreground = Brushes.Green;
                    this.setupGestures.FontWeight = FontWeights.Bold;
                }
                else
                {
                    // Show an error; could not parse gestureLength.Text
                    MessageBox.Show("Could not use \"Seconds for Each Gesture\" field. Please verify this is an integer from 1 to 10.");
                }
            }
            else
            {
                // Show an error; could not parse numGestures.Text
                MessageBox.Show("Could not use \"Number of Security Gestures\" field. Please verify this is an integer from 1 to 10.");
            }
        }

        private void setupVoiceRecognition_Click(object sender, RoutedEventArgs e)
        {
            KinectHelper.StartSpeechEngine();

            VoiceRecognition voiceRecognition = new VoiceRecognition();

            for (int i = 1; i <= 2; i++)
            {
                if (i == 1)
                {
                    MessageBox.Show("Click \"OK\" to start the recording your voice password. Voice passwords must be between 4 to 8 digits and must match each other.",
                        "Begin Recording");
                }
                else if (i == 2)
                {
                    MessageBox.Show("Click \"OK\" to start the confirmation recording your voice password. This recording must match: " + voiceRecognition.getVoices()[0].ToString(),
                        "Begin Confirmation Recording");
                }

                // Start the recording
                voiceRecognition.record(true, i - 1, KinectManager.UpdateVoiceData);

                MessageBox.Show("Click \"OK\" to stop recording.", "Recording ...");

                // Stop the recording
                voiceRecognition.record(false, i - 1, KinectManager.UpdateVoiceData);

                if (voiceRecognition.isValid(i - 1))
                {
                    MessageBox.Show("Finished recording passphrase " + i.ToString());
                }
                else
                {
                    MessageBox.Show("Voice password " + i.ToString() + " did not contain between 4 to 8 digits. Please try again.");
                    i--;
                }
            }

            bool match = voiceRecognition.compare(voiceRecognition.getVoices()[0], voiceRecognition.getVoices()[1]);
            if (match)
            {
                KinectManager.saveVoicePassword(voiceRecognition.getVoices()[0].DeepClone());

                MessageBox.Show("Voice passwords match and the password has been saved.");

                // Success: Make the button green
                this.setupVoiceRecognition.Foreground = Brushes.Green;
                this.setupVoiceRecognition.FontWeight = FontWeights.Bold;
            }
            else
            {
                MessageBox.Show("Voice passwords do not match. Please try again.");
            }
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            facialRecognitionWindow.stopKinect();

            // Open the login window
            login = new Login();
            login.Show();

            // Close the settings window
            this.Close();
        }
    }
}
