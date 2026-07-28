using System;
using UnityEngine;

namespace RhythmPassport.Runtime
{
    public sealed class MotionRecognitionManager : MonoBehaviour
    {
        [Header("Dependencies")]
        public PoseDetectionManager poseDetectionManager;
        public WebcamUiReferences webcamUi;

        [Header("Thresholds")]
        [Min(0f)] public float handRaiseShoulderOffset = 0.08f;
        [Min(0f)] public float handsTogetherDistanceThreshold = 0.12f;
        [Min(0f)] public float neutralWristDropOffset = 0.04f;

        [Header("Hold Times")]
        [Min(0f)] public float handsTogetherHoldDuration = 0.7f;

        [Header("Cooldowns")]
        [Min(0f)] public float sideGestureCooldown = 0.25f;
        [Min(0f)] public float jumpCooldown = 0.6f;
        [Min(0f)] public float pauseCooldown = 1f;

        private float handsTogetherTimer;
        private float sideGestureReadyTime;
        private float jumpReadyTime;
        private float pauseReadyTime;
        private bool requiresNeutralReset;

        public event Action<MotionGesture> GestureTriggered;

        public MotionGesture CurrentGesture { get; private set; } = MotionGesture.None;
        public MotionGesture LastTriggeredGesture { get; private set; } = MotionGesture.None;
        public bool IsInputLocked => requiresNeutralReset;
        public string DebugSummary { get; private set; } = "제스처 대기 중";

        private void Awake()
        {
            if (poseDetectionManager == null)
            {
                poseDetectionManager = FindAnyObjectByType<PoseDetectionManager>();
            }
        }

        private void Update()
        {
            if (poseDetectionManager == null || poseDetectionManager.CurrentFrame == null)
            {
                CurrentGesture = MotionGesture.None;
                handsTogetherTimer = 0f;
                UpdateUi("제스처 대기 중");
                return;
            }

            var frame = poseDetectionManager.CurrentFrame;
            if (!TryReadPose(frame, out var pose))
            {
                CurrentGesture = MotionGesture.None;
                handsTogetherTimer = 0f;
                UpdateUi("랜드마크 부족");
                return;
            }

            var leftHandUp = pose.LeftWrist.y <= pose.LeftShoulder.y - handRaiseShoulderOffset;
            var rightHandUp = pose.RightWrist.y <= pose.RightShoulder.y - handRaiseShoulderOffset;
            var wristsTogetherDistance = Vector2.Distance(pose.LeftWrist, pose.RightWrist);
            var handsTogether = wristsTogetherDistance <= handsTogetherDistanceThreshold;
            var neutral = IsNeutralPose(pose);

            if (handsTogether)
            {
                handsTogetherTimer += Time.deltaTime;
            }
            else
            {
                handsTogetherTimer = 0f;
            }

            var candidateGesture = DetermineGesture(leftHandUp, rightHandUp, handsTogether);
            CurrentGesture = candidateGesture;

            if (requiresNeutralReset)
            {
                if (neutral)
                {
                    requiresNeutralReset = false;
                }

                UpdateUi(BuildDebugSummary(candidateGesture, wristsTogetherDistance, neutral, true));
                return;
            }

            if (!poseDetectionManager.IsReady)
            {
                UpdateUi("포즈 준비 중");
                return;
            }

            if (TryTriggerGesture(candidateGesture))
            {
                requiresNeutralReset = true;
            }

            UpdateUi(BuildDebugSummary(candidateGesture, wristsTogetherDistance, neutral, false));
        }

        private bool TryTriggerGesture(MotionGesture candidateGesture)
        {
            var now = Time.time;

            switch (candidateGesture)
            {
                case MotionGesture.LeftHandUp:
                    if (now < sideGestureReadyTime)
                    {
                        return false;
                    }

                    sideGestureReadyTime = now + sideGestureCooldown;
                    PublishGesture(MotionGesture.LeftHandUp);
                    return true;

                case MotionGesture.RightHandUp:
                    if (now < sideGestureReadyTime)
                    {
                        return false;
                    }

                    sideGestureReadyTime = now + sideGestureCooldown;
                    PublishGesture(MotionGesture.RightHandUp);
                    return true;

                case MotionGesture.BothHandsUp:
                    if (now < jumpReadyTime)
                    {
                        return false;
                    }

                    jumpReadyTime = now + jumpCooldown;
                    PublishGesture(MotionGesture.BothHandsUp);
                    return true;

                case MotionGesture.HandsTogether:
                    if (handsTogetherTimer < handsTogetherHoldDuration || now < pauseReadyTime)
                    {
                        return false;
                    }

                    pauseReadyTime = now + pauseCooldown;
                    PublishGesture(MotionGesture.HandsTogether);
                    return true;

                default:
                    return false;
            }
        }

        private void PublishGesture(MotionGesture gesture)
        {
            LastTriggeredGesture = gesture;
            GestureTriggered?.Invoke(gesture);
        }

        private MotionGesture DetermineGesture(bool leftHandUp, bool rightHandUp, bool handsTogether)
        {
            if (handsTogether)
            {
                return MotionGesture.HandsTogether;
            }

            if (leftHandUp && rightHandUp)
            {
                return MotionGesture.BothHandsUp;
            }

            if (leftHandUp)
            {
                return MotionGesture.LeftHandUp;
            }

            if (rightHandUp)
            {
                return MotionGesture.RightHandUp;
            }

            return MotionGesture.Neutral;
        }

        private bool IsNeutralPose(in PoseSnapshot pose)
        {
            return pose.LeftWrist.y >= pose.LeftShoulder.y + neutralWristDropOffset
                && pose.RightWrist.y >= pose.RightShoulder.y + neutralWristDropOffset;
        }

        private bool TryReadPose(PoseFrame frame, out PoseSnapshot pose)
        {
            pose = default;

            return frame.TryGetLandmark(PoseLandmarkId.Nose, out var nose)
                && frame.TryGetLandmark(PoseLandmarkId.LeftShoulder, out var leftShoulder)
                && frame.TryGetLandmark(PoseLandmarkId.RightShoulder, out var rightShoulder)
                && frame.TryGetLandmark(PoseLandmarkId.LeftWrist, out var leftWrist)
                && frame.TryGetLandmark(PoseLandmarkId.RightWrist, out var rightWrist)
                && PoseSnapshot.TryCreate(nose, leftShoulder, rightShoulder, leftWrist, rightWrist, out pose);
        }

        private string BuildDebugSummary(MotionGesture gesture, float wristDistance, bool neutral, bool locked)
        {
            var gestureText = gesture switch
            {
                MotionGesture.Neutral => "기본 자세",
                MotionGesture.LeftHandUp => "왼손 들기",
                MotionGesture.RightHandUp => "오른손 들기",
                MotionGesture.BothHandsUp => "양손 들기",
                MotionGesture.HandsTogether => "양손 모으기",
                _ => "대기",
            };

            var lockText = locked ? "입력 잠금" : "입력 가능";
            DebugSummary = $"현재 제스처: {gestureText}\n손목 거리: {wristDistance:0.00}\n기본 자세 복귀: {(neutral ? "완료" : "필요")}\n상태: {lockText}";
            return DebugSummary;
        }

        private void UpdateUi(string text)
        {
            DebugSummary = text;

            if (webcamUi != null && webcamUi.gestureStatusText != null)
            {
                webcamUi.gestureStatusText.text = text;
            }
        }

        private readonly struct PoseSnapshot
        {
            public PoseSnapshot(Vector2 nose, Vector2 leftShoulder, Vector2 rightShoulder, Vector2 leftWrist, Vector2 rightWrist)
            {
                Nose = nose;
                LeftShoulder = leftShoulder;
                RightShoulder = rightShoulder;
                LeftWrist = leftWrist;
                RightWrist = rightWrist;
            }

            public Vector2 Nose { get; }
            public Vector2 LeftShoulder { get; }
            public Vector2 RightShoulder { get; }
            public Vector2 LeftWrist { get; }
            public Vector2 RightWrist { get; }

            public static bool TryCreate(
                PoseLandmark nose,
                PoseLandmark leftShoulder,
                PoseLandmark rightShoulder,
                PoseLandmark leftWrist,
                PoseLandmark rightWrist,
                out PoseSnapshot pose)
            {
                pose = new PoseSnapshot(
                    nose.NormalizedPosition,
                    leftShoulder.NormalizedPosition,
                    rightShoulder.NormalizedPosition,
                    leftWrist.NormalizedPosition,
                    rightWrist.NormalizedPosition);

                return nose.Confidence > 0f
                    && leftShoulder.Confidence > 0f
                    && rightShoulder.Confidence > 0f
                    && leftWrist.Confidence > 0f
                    && rightWrist.Confidence > 0f;
            }
        }
    }
}
