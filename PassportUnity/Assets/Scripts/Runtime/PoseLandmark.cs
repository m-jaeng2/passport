using UnityEngine;

namespace RhythmPassport.Runtime
{
    public enum PoseLandmarkId
    {
        Nose = 0,
        LeftShoulder = 1,
        RightShoulder = 2,
        LeftWrist = 3,
        RightWrist = 4,
    }

    [System.Serializable]
    public struct PoseLandmark
    {
        public PoseLandmark(PoseLandmarkId id, Vector2 normalizedPosition, float confidence)
        {
            Id = id;
            NormalizedPosition = normalizedPosition;
            Confidence = confidence;
        }

        public PoseLandmarkId Id;
        public Vector2 NormalizedPosition;
        public float Confidence;
    }
}
