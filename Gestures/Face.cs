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
            // +/- 5% error for distance
            float distanceErrorThreshold = 0.05F;
            float lowDistance, highDistance;

            // threshold for the number of wrong distances
            int numWrongPointsThreshold = 20;
            int numWrongPoints = 0;

            int numFaceFeatures = faceModel.Count;
            int numFaceCandidateFeatures = faceModelCandidate.Count;

            // Build a distance matrix for each point that corresponds to every other point
            float[,] faceModelDistanceMatrix = new float[numFaceFeatures, numFaceFeatures];

            float[,] faceModelCandidateDistanceMatrix = new float[numFaceCandidateFeatures, numFaceCandidateFeatures];

            // calculate the distance

            // For each row of the distanceMatrix
            foreach (int i in Enum.GetValues(typeof(FeaturePoint)))
            {
                // For each column of the distanceMatrix
                foreach (int j in Enum.GetValues(typeof(FeaturePoint)))
                {
                    // Add the distances to the matrices
                    faceModelDistanceMatrix[i, j] = pointDistance3D(faceModel[i], faceModel[j]);

                    faceModelCandidateDistanceMatrix[i, j] = pointDistance3D(faceModelCandidate[i], faceModelCandidate[j]);

                    // Verify these distances are not 0
                    if (faceModelDistanceMatrix[i, j] != 0 && faceModelCandidateDistanceMatrix[i, j] != 0)
                    {
                        // Compare these using the threshold; candidate must be within low and high distances
                        lowDistance = faceModelDistanceMatrix[i, j] * (1 - distanceErrorThreshold);
                        highDistance = faceModelDistanceMatrix[i, j] * (1 + distanceErrorThreshold);

                        // Check that lowDistance <= faceModelCandidateDistanceMatrix[i, j] <= highDistance
                        if (lowDistance >= faceModelCandidateDistanceMatrix[i, j] || highDistance <= faceModelCandidateDistanceMatrix[i, j])
                        {
                            numWrongPoints++;
                        }
                    }
                }
            }

            if (numWrongPoints < numWrongPointsThreshold)
            {
                // Match
                return true;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Calculates the 3D euclidean distance between the two points
        /// </summary>
        /// <param name="vector1">A 3D point</param>
        /// <param name="vector2">A 3D point</param>
        /// <returns></returns>
        public float pointDistance3D(Vector3DF point1, Vector3DF point2)
        {
            float diffX, diffY, diffZ;
            double distance;

            // calculate the difference between the two points
            diffX = point1.X - point2.X;
            diffY = point1.Y - point2.Y;
            diffZ = point1.Z - point2.Z;

            // calculate the Euclidean distance between the two points
            distance = Math.Sqrt(Math.Pow(diffX, 2) + Math.Pow(diffY, 2) + Math.Pow(diffZ, 2));

            return (float) distance;  // return the distance as a float
        }
    }
}
