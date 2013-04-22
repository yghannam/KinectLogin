using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Gestures;

namespace KinectLogin
{
    public partial class SetupLogin : Form
    {
        public SetupLogin()
        {
            InitializeComponent();

            KinectHelper.StartKinectST();
        }

        private void setupGestures_Click(object sender, EventArgs e)
        {
            int parsedNumGestures;
            double parsedSeconds;
            if (Int32.TryParse(this.numGestures.Text, out parsedNumGestures) && parsedNumGestures >= 1 && parsedNumGestures <= 10)
            {
                if (Double.TryParse(this.gestureLength.Text, out parsedSeconds) && parsedSeconds >= 1.0 && parsedSeconds <= 10.0)
                {
                    SecurityGestureSet gestureSet = new SecurityGestureSet();
                    gestureSet.record(parsedNumGestures, (float)parsedSeconds);

                    if (gestureSet.getGestures() != null && gestureSet.getGestures().Length >= 2)
                    {
                        gestureSet.compare(gestureSet.getGestures()[0], gestureSet.getGestures()[1]);
                    }
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

        private void setupVocalCommand_Click(object sender, EventArgs e)
        {
            VoiceRecognition voiceRecognition = new VoiceRecognition();

            for (int i = 1; i <= 2; i++)
            {
                MessageBox.Show("Click OK to record passphrase " + i.ToString());
                voiceRecognition.record(true, i-1);
                MessageBox.Show("Click OK to stop recording passphrase " + i.ToString());
                voiceRecognition.record(false, i-1);

                if (voiceRecognition.isValid(i-1))
                    MessageBox.Show("Finished recording passphrase  " + i.ToString());
                else
                {
                    MessageBox.Show("Phrase " + i.ToString() + " was not between 4 and 8 values. Please try again.");
                    i--;
                }
            }

            bool match = voiceRecognition.compare(voiceRecognition.getVoices()[0], voiceRecognition.getVoices()[1]);
            if (match)
                MessageBox.Show("Phrases match");
            else
                MessageBox.Show("Phrases do not match.");
        }

        private void setupFacialRecognition_Click(object sender, EventArgs e)
        {
            
        }
    }
}
