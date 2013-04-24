using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestures
{
    [Serializable()]
    public class Voice
    {

        private List<String> voiceData = new List<String>();

        public event EventHandler VoiceDataUpdated;

        public void addVoiceData(String s)
        {
            this.voiceData.Add(s);

            if (this.VoiceDataUpdated != null)
            {
                this.VoiceDataUpdated(this, new EventArgs());
            }
        }

        public List<String> getVoiceData()
        {
            return ExtensionMethods.DeepClone(this.voiceData);
        }

        public override string ToString()
        {
            if(voiceData != null)
            {
                string tokenizedString = "";
                int i;

                for(i = 0; i < voiceData.Count; i++)
                {
                    tokenizedString += voiceData[i];

                    if(i < voiceData.Count - 1)
                    {
                        tokenizedString += ", ";
                    }
                }

                return tokenizedString;
            }
            else
            {
                return "";
            }
        }
    }
}
