using System.Collections.Generic;
using UnityEngine;

namespace RhythmPassport.Runtime
{
    public sealed class PoseFrame
    {
        private readonly Dictionary<PoseLandmarkId, PoseLandmark> landmarks = new Dictionary<PoseLandmarkId, PoseLandmark>();

        public bool IsSimulated { get; set; }
        public bool IsTrackingReliable { get; set; }
        public float Timestamp { get; set; }

        public IEnumerable<PoseLandmark> Landmarks => landmarks.Values;

        public void SetLandmark(PoseLandmark landmark)
        {
            landmarks[landmark.Id] = landmark;
        }

        public bool TryGetLandmark(PoseLandmarkId id, out PoseLandmark landmark)
        {
            return landmarks.TryGetValue(id, out landmark);
        }

        public bool HasRequiredLandmarks(float minimumConfidence)
        {
            return HasConfidence(PoseLandmarkId.Nose, minimumConfidence)
                && HasConfidence(PoseLandmarkId.LeftShoulder, minimumConfidence)
                && HasConfidence(PoseLandmarkId.RightShoulder, minimumConfidence)
                && HasConfidence(PoseLandmarkId.LeftWrist, minimumConfidence)
                && HasConfidence(PoseLandmarkId.RightWrist, minimumConfidence);
        }

        private bool HasConfidence(PoseLandmarkId id, float minimumConfidence)
        {
            return landmarks.TryGetValue(id, out var landmark) && landmark.Confidence >= minimumConfidence;
        }
    }
}
