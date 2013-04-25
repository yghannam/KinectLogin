using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace KinectLogin
{
    [Serializable()]
    public class Gesture
    {
        
        private List<Skeleton> skeletalData = new List<Skeleton>();
        private List<String> gesturePhrase = new List<String>();
        
        public void addSkeletalData(Skeleton s)
        {
            this.skeletalData.Add(s);
        }

        public List<Skeleton> getSkeletalData()
        {
            return ExtensionMethods.DeepClone(this.skeletalData);
        }

        // Loop through each skeleton and match to gesture template.
        // Add gesture to list of gestures if different than previous skeleton's gesture.
        // Return false if no valid gestures found.
        public bool extractGestures()
        {
            String gesture = null;
            foreach(Skeleton s in skeletalData)
            {
                gesture = matchGestureTemplates(s);
                if (gesture != null)
                {
                    //if (gesturePhrase.Count == 0)
                        gesturePhrase.Add(gesture);
                    //else if (!gesturePhrase.Last().Equals(gesture))
                    //    gesturePhrase.Add(gesture);
                }
            }
            return gesturePhrase.Count > 0;
        }


        private String matchGestureTemplates(Skeleton s)
        {
            float threshold = 0.1f;
            String gesture = null;

            Joints joints = new Joints();
            for (int i = 0; i < s.Joints.Count; i++ )
            {
                joints.addJoints(s.Joints.ElementAt(i), i);
            }

            bool rightHandtoHead = Vector3.Dot(Vector3.Normalize(joints.HAND_RIGHT - joints.HEAD), Vector3.Right) > threshold;
            bool rightForearm_vertical = Vector3.Dot(Vector3.Normalize(joints.HAND_RIGHT - joints.ELBOW_RIGHT), Vector3.Up) > threshold;
            bool rightForearm_horizontal = Vector3.Dot(Vector3.Normalize(joints.HAND_RIGHT - joints.ELBOW_RIGHT), Vector3.Right) > threshold;
            bool rightUpperarm_horizontal = Vector3.Dot(Vector3.Normalize(joints.ELBOW_RIGHT - joints.SHOULDER_RIGHT), Vector3.Right) > threshold;

            bool leftHandtoHead = Vector3.Dot(Vector3.Normalize(joints.HAND_LEFT - joints.HEAD), Vector3.Left) > threshold;
            bool leftForearm_vertical = Vector3.Dot(Vector3.Normalize(joints.HAND_LEFT - joints.ELBOW_LEFT), Vector3.Up) > threshold;
            bool leftForearm_horizontal = Vector3.Dot(Vector3.Normalize(joints.HAND_LEFT - joints.ELBOW_LEFT), Vector3.Left) > threshold;
            bool leftUpperarm_horizontal = Vector3.Dot(Vector3.Normalize(joints.ELBOW_LEFT - joints.SHOULDER_LEFT), Vector3.Left) > threshold;

            // Elbow Right Angles
            if (rightHandtoHead && rightForearm_vertical)
            {
                if (leftHandtoHead && leftForearm_vertical)
                {
                    gesture = "ElbowRightAngle_Both";
                    System.Console.WriteLine("ElbowRightAngle_Both");
                }
                else
                {
                    gesture = "ElbowRightAngle_Right";
                    System.Console.WriteLine("ElbowRightAngle_Right");
                }
            }
            else if (leftHandtoHead && leftForearm_vertical)
            {
                gesture = "ElbowRightAngle_Left";
                System.Console.WriteLine("ElbowRightAngle_Left");
            }


            // Straight Arms
            else if (rightForearm_horizontal && rightUpperarm_horizontal)
            {
                if (leftForearm_horizontal && leftUpperarm_horizontal)
                {
                    gesture = "StraightArm_Both";
                    System.Console.WriteLine("StraightArm_Both");
                }
                else
                {
                    gesture = "StraightArm_Right";
                    System.Console.WriteLine("StraightArm_Right");
                }
            }
            else if (leftForearm_horizontal && leftUpperarm_horizontal)
            {
                gesture = "StraightArm_Left";
                System.Console.WriteLine("StraightArm_Left");
            }
          

            return gesture;

        }

        private class Joints
        {
            public Vector3 HIP_CENTER,
            SPINE,
            SHOULDER_CENTER,
            HEAD,
            SHOULDER_LEFT,
            ELBOW_LEFT,
            WRIST_LEFT,
            HAND_LEFT,
            SHOULDER_RIGHT,
            ELBOW_RIGHT,
            WRIST_RIGHT,
            HAND_RIGHT,
            HIP_LEFT,
            KNEE_LEFT,
            ANKLE_LEFT,
            FOOT_LEFT,
            HIP_RIGHT,
            KNEE_RIGHT,
            ANKLE_RIGHT,
            FOOT_RIGHT;

            public void addJoints(Joint j, int type)
            {
                switch (type)
                {
                    case 0:
                        this.HIP_CENTER = new Vector3(j);
                        break;
                    case 1:
                        this.SPINE = new Vector3(j);
                        break;
                    case 2:
                        this.SHOULDER_CENTER = new Vector3(j);
                        break;
                    case 3:
                        this.HEAD = new Vector3(j);
                        break;
                    case 4:
                        this.SHOULDER_LEFT = new Vector3(j);
                        break;
                    case 5:
                        this.ELBOW_LEFT = new Vector3(j);
                        break;
                    case 6:
                        this.WRIST_LEFT = new Vector3(j);
                        break;
                    case 7:
                        this.HAND_LEFT = new Vector3(j);
                        break;
                    case 8:
                        this.SHOULDER_RIGHT = new Vector3(j);
                        break;
                    case 9:
                        this.ELBOW_RIGHT = new Vector3(j);
                        break;
                    case 10:
                        this.WRIST_RIGHT = new Vector3(j);
                        break;
                    case 11:
                        this.HAND_RIGHT = new Vector3(j);
                        break;
                    case 12:
                        this.HIP_LEFT = new Vector3(j);
                        break;
                    case 13:
                        this.KNEE_LEFT = new Vector3(j);
                        break;
                    case 14:
                        this.ANKLE_LEFT = new Vector3(j);
                        break;
                    case 15:
                        this.FOOT_LEFT = new Vector3(j);
                        break;
                    case 16:
                        this.HIP_RIGHT = new Vector3(j);
                        break;
                    case 17:
                        this.KNEE_RIGHT = new Vector3(j);
                        break;
                    case 18:
                        this.ANKLE_RIGHT = new Vector3(j);
                        break;
                    case 19:
                        this.FOOT_RIGHT = new Vector3(j);
                        break;
                    default:
                        break;
                }
            }
        }

        private class Vector3
        {
            float x;
            float y;
            float z;

            public static Vector3 Up = new Vector3(0.0f, 1.0f, 0.0f);
            public static Vector3 Down = new Vector3(0.0f, -1.0f, 0.0f);
            public static Vector3 Right = new Vector3(1.0f, 0.0f, 0.0f);
            public static Vector3 Left = new Vector3(-1.0f, 0.0f, 0.0f);
            public static Vector3 Forward = new Vector3(0.0f, 0.0f, 1.0f);
            public static Vector3 Back = new Vector3(0.0f, 0.0f, -1.0f);

            public Vector3(Joint j)
            {
                this.x = j.Position.X;
                this.y = j.Position.Y;
                this.z = j.Position.Z;
            }

            public Vector3(float x, float y, float z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }

            public static Vector3 operator +(Vector3 v1, Vector3 v2)
            {
                return new Vector3(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
            }

            public static Vector3 operator -(Vector3 v1, Vector3 v2)
            {
                return new Vector3(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
            }

            public static float Dot(Vector3 v1, Vector3 v2)
            {
                return v1.x * v2.x + v1.y * v2.y + v1.z * v2.z;
            }

            public static float Magnitude(Vector3 v)
            {
                return (float)Math.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
            }

            public static Vector3 Normalize(Vector3 v)
            {
                float magnitude = Magnitude(v);
                return new Vector3(v.x / magnitude, v.y / magnitude, v.z / magnitude);
            }
        }
    }
}
