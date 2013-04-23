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

namespace KinectLoginWpf
{
    /// <summary>
    /// Interaction logic for SetupLogin.xaml
    /// </summary>
    public partial class SetupLogin : Window
    {
        int parsedNumGestures;
        double parsedSeconds;

        public SetupLogin()
        {
            InitializeComponent();

            KinectManager.setup();
        }

        private void recordGestures()
        {
            KinectManager.recordGestures(parsedNumGestures, (float)parsedSeconds);
        }

        private void setupFacialRecognition_Click(object sender, RoutedEventArgs e)
        {
            // Success: Make the button green
            this.setupFacialRecognition.Foreground = Brushes.Green;
            this.setupFacialRecognition.FontWeight = FontWeights.Bold;
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
            // Success: Make the button green
            this.setupVoiceRecognition.Foreground = Brushes.Green;
            this.setupVoiceRecognition.FontWeight = FontWeights.Bold;
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            // Open the login window
            Login login = new Login();
            login.Show();

            // Close the settings window
            this.Close();
        }
    }
}
