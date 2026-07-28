using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace RhythmPassport.Runtime
{
    public sealed class PoseDetectionManager : MonoBehaviour
    {
        [Header("Dependencies")]
        public WebcamManager webcamManager;
        public WebcamUiReferences webcamUi;
        public PoseProviderBehaviour poseProvider;

        [Header("Thresholds")]
        [Range(0f, 1f)] public float minimumConfidence = 0.6f;
        public float readyHoldDuration = 2f;

        private float readyTimer;

        public PoseFrame CurrentFrame { get; private set; }
        public bool IsReady { get; private set; }
        public bool HasReliableTracking => CurrentFrame != null && CurrentFrame.IsTrackingReliable;

        private void Awake()
        {
            if (webcamManager == null)
            {
                webcamManager = FindAnyObjectByType<WebcamManager>();
            }

            if (poseProvider == null)
            {
                poseProvider = GetComponent<PoseProviderBehaviour>();
            }

            if (poseProvider != null && webcamManager != null)
            {
                poseProvider.Initialize(webcamManager);
            }
        }

        private void Update()
        {
            if (webcamManager == null || poseProvider == null)
            {
                UpdateUi("포즈 공급자 없음", "랜드마크 없음");
                CurrentFrame = null;
                readyTimer = 0f;
                IsReady = false;
                return;
            }

            if (poseProvider.TryGetPoseFrame(out var frame))
            {
                CurrentFrame = frame;
                var hasRequiredLandmarks = frame.HasRequiredLandmarks(minimumConfidence);
                readyTimer = hasRequiredLandmarks ? readyTimer + Time.deltaTime : 0f;
                IsReady = readyTimer >= readyHoldDuration;
                UpdateUi(
                    IsReady ? "준비 완료" : "인식 준비 중",
                    BuildLandmarkSummary(frame));
            }
            else
            {
                CurrentFrame = null;
                readyTimer = 0f;
                IsReady = false;
                UpdateUi("플레이어 인식 대기", "랜드마크 없음");
            }
        }

        public bool TryGetLandmark(PoseLandmarkId id, out PoseLandmark landmark)
        {
            landmark = default;
            return CurrentFrame != null && CurrentFrame.TryGetLandmark(id, out landmark);
        }

        private void UpdateUi(string recognitionStatus, string landmarkSummary)
        {
            if (webcamUi == null)
            {
                return;
            }

            if (webcamUi.recognitionStatusText != null)
            {
                webcamUi.recognitionStatusText.text = recognitionStatus;
            }

            if (webcamUi.landmarkStatusText != null)
            {
                webcamUi.landmarkStatusText.text = landmarkSummary;
            }
        }

        private static string BuildLandmarkSummary(PoseFrame frame)
        {
            var builder = new StringBuilder();
            builder.Append(frame.IsSimulated ? "디버그 포즈" : "실시간 포즈");

            AppendLandmark(builder, frame, PoseLandmarkId.Nose, "코");
            AppendLandmark(builder, frame, PoseLandmarkId.LeftShoulder, "왼어깨");
            AppendLandmark(builder, frame, PoseLandmarkId.RightShoulder, "오른어깨");
            AppendLandmark(builder, frame, PoseLandmarkId.LeftWrist, "왼손목");
            AppendLandmark(builder, frame, PoseLandmarkId.RightWrist, "오른손목");
            return builder.ToString();
        }

        private static void AppendLandmark(StringBuilder builder, PoseFrame frame, PoseLandmarkId id, string label)
        {
            if (!frame.TryGetLandmark(id, out var landmark))
            {
                return;
            }

            builder.AppendLine();
            builder.Append(label);
            builder.Append(": ");
            builder.Append(landmark.NormalizedPosition.x.ToString("0.00"));
            builder.Append(", ");
            builder.Append(landmark.NormalizedPosition.y.ToString("0.00"));
        }
    }
}
