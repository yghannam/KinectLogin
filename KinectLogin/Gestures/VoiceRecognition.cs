using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectLogin
{
    public class VoiceRecognition
    {
        private Voice[] voices;

        /// <summary>
        /// Constructor
        /// </summary>
        public VoiceRecognition()
        {
            this.voices = new Voice[10];
        }

        /// <summary>
        /// Gets the voices
        /// </summary>
        /// <returns>An array of voices</returns>
        public Voice[] getVoices()
        {
            return this.voices;
        }

        public bool isValid(int v)
        {
            return voices[v].getVoiceData().Count >= 4 && voices[v].getVoiceData().Count <= 8;
        }

        public void record(bool startRecording, int v, EventHandler VoiceDataUpdatedEvent)
        {
            if (startRecording)
            {
                KinectHelper.startVoiceRecording();
            }
            else
            {
                voices[v] = KinectHelper.stopVoiceRecording();
                voices[v].VoiceDataUpdated += VoiceDataUpdatedEvent;
            }
        }

        public bool compare(Voice v1, Voice v2)
        {
            bool match = false;
            List<String> s1 = v1.getVoiceData();
            List<String> s2 = v2.getVoiceData();
            if (s1.Count == s2.Count)
            {
                for (int i = 0; i < s1.Count; i++)
                {
                    match = s1.ElementAt(i).Equals(s2.ElementAt(i));
                    if (!match) break;
                }
            }
            return match;
        }
    }
}
