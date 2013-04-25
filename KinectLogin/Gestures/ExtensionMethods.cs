using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.IO;


namespace KinectLogin
{
    public static class ExtensionMethods
    {
        // Deep clone
        public static T DeepClone<T>(this T a)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                //XmlSerializer formatter = new XmlSerializer(typeof(Gesture));
                formatter.Serialize(stream, a);
                stream.Position = 0;
                return (T)formatter.Deserialize(stream);
            }
        }

        public static void timer(int seconds)
        {
            Thread.Sleep(seconds * 1000);
            //DateTime startTime = DateTime.Now;
            //DateTime currentTime = DateTime.Now;
            //while (currentTime.Second - startTime.Second < seconds)
            //{
            //    //if (currentTime.Second - startTime.Second % 1 == 0)
            //    //    System.Console.WriteLine(currentTime.Second - startTime.Second);
            //    currentTime = DateTime.Now;
            //}
        }

       public static void countdown()
        {
            for (int i = 5; i > 0; i--)
            {
                System.Console.Write(i);
                System.Threading.Thread.Sleep(250);
                System.Console.Write(".");
                System.Threading.Thread.Sleep(250);
                System.Console.Write(".");
                System.Threading.Thread.Sleep(250);
                System.Console.Write(".");
                System.Threading.Thread.Sleep(250);
            }
            System.Console.WriteLine("GO!");
        }
    }
}
