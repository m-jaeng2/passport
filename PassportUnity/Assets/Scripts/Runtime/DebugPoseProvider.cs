using UnityEngine;

namespace RhythmPassport.Runtime
{
    public sealed class DebugPoseProvider : PoseProviderBehaviour
    {
        [Header("Debug Pose")]
        public bool animateWrists = true;
        public float animationAmplitude = 0.04f;
        public float animationSpeed = 1.5f;
        public float landmarkConfidence = 0.98f;

        private WebcamManager webcamManager;

        public override void Initialize(WebcamManager webcamManager)
        {
            this.webcamManager = webcamManager;
        }

        public override bool TryGetPoseFrame(out PoseFrame frame)
        {
            frame = null;

            if (webcamManager == null || !webcamManager.IsRunning)
            {
                return false;
            }

            var wristYOffset = animateWrists ? Mathf.Sin(Time.time * animationSpeed) * animationAmplitude : 0f;
            var wristXOffset = animateWrists ? Mathf.Cos(Time.time * animationSpeed * 0.75f) * animationAmplitude * 0.5f : 0f;

            frame = new PoseFrame
            {
                IsSimulated = true,
                IsTrackingReliable = true,
                Timestamp = Time.time,
            };

            frame.SetLandmark(new PoseLandmark(PoseLandmarkId.Nose, new Vector2(0.5f, 0.72f), landmarkConfidence));
            frame.SetLandmark(new PoseLandmark(PoseLandmarkId.LeftShoulder, new Vector2(0.42f, 0.58f), landmarkConfidence));
            frame.SetLandmark(new PoseLandmark(PoseLandmarkId.RightShoulder, new Vector2(0.58f, 0.58f), landmarkConfidence));
            frame.SetLandmark(new PoseLandmark(PoseLandmarkId.LeftWrist, new Vector2(0.34f - wristXOffset, 0.42f - wristYOffset), landmarkConfidence));
            frame.SetLandmark(new PoseLandmark(PoseLandmarkId.RightWrist, new Vector2(0.66f + wristXOffset, 0.42f + wristYOffset), landmarkConfidence));
            return true;
        }
    }
}
