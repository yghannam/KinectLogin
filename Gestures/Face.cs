using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.FaceTracking;

namespace Gestures
{
    public class Face
    {
        private EnumIndexableCollection<FeaturePoint, Vector3DF> faceModel;

        // Unused
        Vector3DF[] headPoints;

        public Face(EnumIndexableCollection<FeaturePoint, Vector3DF> faceModel)
        {
            this.faceModel = faceModel;
        }

        public Face(EnumIndexableCollection<FeaturePoint, Vector3DF> faceModel, Vector3DF[] headPoints)
        {
            this.faceModel = faceModel;
            this.headPoints = headPoints;
        }

        public EnumIndexableCollection<FeaturePoint, Vector3DF> getFaceModel() 
        {
            return faceModel;
        }

        public void setFaceModel(EnumIndexableCollection<FeaturePoint, Vector3DF> faceModel)
        {
            this.faceModel = faceModel;
        }

        public Vector3DF[] getHeadPoints()
        {
            return headPoints;
        }

        public void setHeadPoints(Vector3DF[] headPoints)
        {
            this.headPoints = headPoints;
        }

        /// <summary>
        /// Compares this.faceModel with faceModelCandidate
        /// </summary>
        /// <param name="faceModel2">The feature map with 3D vectors</param>
        /// <returns>true if the </returns>
        public bool compare(EnumIndexableCollection<FeaturePoint, Vector3DF> faceModelCandidate)
        {
            double threshold = 85;
            // TODO: Implement comparison
            return true;
        }
    }
}
