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

        public void addVoiceData(String s)
        {
            this.voiceData.Add(s);
        }

        public List<String> getVoiceData()
        {
            return ExtensionMethods.DeepClone(this.voiceData);
        }
    }
}
