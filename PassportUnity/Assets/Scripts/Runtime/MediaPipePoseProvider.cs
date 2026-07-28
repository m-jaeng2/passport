using System.Collections;
using Mediapipe;
using Mediapipe.Tasks.Components.Containers;
using Mediapipe.Tasks.Core;
using Mediapipe.Tasks.Vision.Core;
using Mediapipe.Tasks.Vision.PoseLandmarker;
using UnityEngine;
#if UNITY_EDITOR
using Mediapipe.Unity;
#endif

namespace RhythmPassport.Runtime
{
    public sealed class MediaPipePoseProvider : PoseProviderBehaviour
    {
        private const int NoseIndex = 0;
        private const int LeftShoulderIndex = 11;
        private const int RightShoulderIndex = 12;
        private const int LeftWristIndex = 15;
        private const int RightWristIndex = 16;

        [Header("MediaPipe 모델")]
        public string modelAssetName = "pose_landmarker_full.bytes";
        [Range(1, 4)] public int numPoses = 1;
        [Range(0f, 1f)] public float minPoseDetectionConfidence = 0.5f;
        [Range(0f, 1f)] public float minPosePresenceConfidence = 0.5f;
        [Range(0f, 1f)] public float minTrackingConfidence = 0.5f;

        [Header("좌표 보정")]
        public bool mirrorToPreview = true;

        private WebcamManager webcamManager;
        private Texture2D frameTexture;
        private Color32[] pixelBuffer;
        private PoseLandmarker poseLandmarker;
        private PoseLandmarkerResult poseResult;
        private string providerStatus = "MediaPipe 초기화 대기 중";
        private bool isPreparing;
        private bool isReady;

        public override void Initialize(WebcamManager webcamManager)
        {
            this.webcamManager = webcamManager;

            if (!isPreparing && poseLandmarker == null)
            {
                StartCoroutine(PreparePoseLandmarker());
            }
        }

        private void OnDestroy()
        {
            DisposeResources();
        }

        public override bool TryGetPoseFrame(out PoseFrame frame)
        {
            frame = null;

            if (!isReady || poseLandmarker == null)
            {
                return false;
            }

            if (webcamManager == null || !webcamManager.IsRunning || webcamManager.WebcamTexture == null)
            {
                providerStatus = "웹캠 시작을 기다리는 중";
                return false;
            }

            var webcamTexture = webcamManager.WebcamTexture;
            if (webcamTexture.width <= 16 || webcamTexture.height <= 16)
            {
                providerStatus = "웹캠 프레임 안정화 중";
                return false;
            }

            EnsureFrameTexture(webcamTexture.width, webcamTexture.height);

            webcamTexture.GetPixels32(pixelBuffer);
            frameTexture.SetPixels32(pixelBuffer);
            frameTexture.Apply(false, false);

            using var image = new Image(frameTexture);
            var detected = poseLandmarker.TryDetectForVideo(
                image,
                GetTimestampMillis(),
                new ImageProcessingOptions(rotationDegrees: 0),
                ref poseResult);

            if (!detected || poseResult.poseLandmarks == null || poseResult.poseLandmarks.Count == 0)
            {
                providerStatus = "사람 포즈를 찾는 중";
                return false;
            }

            frame = BuildPoseFrame(poseResult.poseLandmarks[0]);
            providerStatus = "MediaPipe 포즈 인식 중";
            return true;
        }

        public override string GetProviderStatus()
        {
            return providerStatus;
        }

        private IEnumerator PreparePoseLandmarker()
        {
            isPreparing = true;
            providerStatus = "MediaPipe 모델을 준비하는 중";

#if UNITY_EDITOR
            IResourceManager resourceManager = new LocalResourceManager("RhythmPassport");
            yield return resourceManager.PrepareAssetAsync(modelAssetName, modelAssetName, overwriteDestination: false);

            var options = new PoseLandmarkerOptions(
                new BaseOptions(BaseOptions.Delegate.CPU, modelAssetPath: modelAssetName),
                runningMode: RunningMode.VIDEO,
                numPoses: numPoses,
                minPoseDetectionConfidence: minPoseDetectionConfidence,
                minPosePresenceConfidence: minPosePresenceConfidence,
                minTrackingConfidence: minTrackingConfidence,
                outputSegmentationMasks: false);

            poseLandmarker = PoseLandmarker.CreateFromOptions(options);
            poseResult = PoseLandmarkerResult.Alloc(numPoses, false);
            isReady = true;
            providerStatus = "MediaPipe 준비 완료";
#else
            providerStatus = "현재 MediaPipe 연동은 Unity Editor 기준으로 준비되었습니다.";
            Debug.LogWarning("MediaPipePoseProvider는 현재 Unity Editor 기준으로 설정되어 있습니다.");
#endif

            isPreparing = false;
        }

        private void EnsureFrameTexture(int width, int height)
        {
            if (frameTexture != null && frameTexture.width == width && frameTexture.height == height)
            {
                return;
            }

            if (frameTexture != null)
            {
                Destroy(frameTexture);
            }

            frameTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            pixelBuffer = new Color32[width * height];
        }

        private PoseFrame BuildPoseFrame(NormalizedLandmarks normalizedLandmarks)
        {
            var frame = new PoseFrame
            {
                IsSimulated = false,
                IsTrackingReliable = true,
                Timestamp = Time.time,
            };

            TryAppendLandmark(frame, normalizedLandmarks, NoseIndex, PoseLandmarkId.Nose);
            TryAppendLandmark(frame, normalizedLandmarks, LeftShoulderIndex, PoseLandmarkId.LeftShoulder);
            TryAppendLandmark(frame, normalizedLandmarks, RightShoulderIndex, PoseLandmarkId.RightShoulder);
            TryAppendLandmark(frame, normalizedLandmarks, LeftWristIndex, PoseLandmarkId.LeftWrist);
            TryAppendLandmark(frame, normalizedLandmarks, RightWristIndex, PoseLandmarkId.RightWrist);
            return frame;
        }

        private void TryAppendLandmark(PoseFrame frame, NormalizedLandmarks normalizedLandmarks, int sourceIndex, PoseLandmarkId targetId)
        {
            if (normalizedLandmarks.landmarks == null || sourceIndex < 0 || sourceIndex >= normalizedLandmarks.landmarks.Count)
            {
                return;
            }

            var source = normalizedLandmarks.landmarks[sourceIndex];
            var x = source.x;
            var y = source.y;

            if (mirrorToPreview && webcamManager != null && webcamManager.IsPreviewMirrored)
            {
                x = 1f - x;
            }

            if (webcamManager != null && webcamManager.IsPreviewVerticallyMirrored)
            {
                y = 1f - y;
            }

            var confidence = Mathf.Max(source.visibility ?? 0f, source.presence ?? 0f);
            if (confidence <= 0f)
            {
                confidence = 1f;
            }

            frame.SetLandmark(new PoseLandmark(targetId, new Vector2(x, y), confidence));
        }

        private static long GetTimestampMillis()
        {
            return (long)(Time.realtimeSinceStartupAsDouble * 1000d);
        }

        private void DisposeResources()
        {
            if (poseLandmarker != null)
            {
                poseLandmarker.Close();
                poseLandmarker = null;
            }

            if (frameTexture != null)
            {
                Destroy(frameTexture);
                frameTexture = null;
            }

            pixelBuffer = null;
            isReady = false;
            isPreparing = false;
        }
    }
}
