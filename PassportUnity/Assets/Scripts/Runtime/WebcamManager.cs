using System;
using UnityEngine;

namespace RhythmPassport.Runtime
{
    public sealed class WebcamManager : MonoBehaviour
    {
        [Header("Device")]
        public string preferredDeviceName;
        public bool mirrorHorizontally = true;
        public bool playOnStart = true;

        [Header("UI")]
        public WebcamUiReferences webcamUi;

        private WebCamTexture webcamTexture;
        private bool startupFailed;

        public event Action<bool> RunningStateChanged;

        public WebCamTexture WebcamTexture => webcamTexture;
        public bool IsRunning => webcamTexture != null && webcamTexture.isPlaying;
        public bool HasCameraPermissionIssue => startupFailed;
        public string ActiveDeviceName { get; private set; } = string.Empty;
        public bool IsPreviewMirrored => mirrorHorizontally;
        public bool IsPreviewVerticallyMirrored => webcamTexture != null && webcamTexture.videoVerticallyMirrored;

        private void Start()
        {
            if (playOnStart)
            {
                StartCamera();
            }
        }

        private void Update()
        {
            if (!IsRunning || webcamUi == null || webcamUi.webcamPreviewImage == null)
            {
                return;
            }

            UpdatePreviewTransform();
            UpdateStatusText();
        }

        private void OnDisable()
        {
            StopCamera();
        }

        public bool StartCamera()
        {
            if (IsRunning)
            {
                return true;
            }

            startupFailed = false;

            var devices = WebCamTexture.devices;
            if (devices == null || devices.Length == 0)
            {
                ActiveDeviceName = string.Empty;
                startupFailed = true;
                UpdateStatusText();
                return false;
            }

            var selectedDevice = SelectDevice(devices);
            ActiveDeviceName = selectedDevice.name;
            webcamTexture = new WebCamTexture(ActiveDeviceName, 1280, 720, 30);
            webcamTexture.Play();

            if (webcamUi != null && webcamUi.webcamPreviewImage != null)
            {
                webcamUi.webcamPreviewImage.texture = webcamTexture;
                webcamUi.webcamPreviewImage.color = Color.white;
            }

            UpdatePreviewTransform();
            UpdateStatusText();
            RunningStateChanged?.Invoke(true);
            return true;
        }

        public void StopCamera()
        {
            if (webcamTexture == null)
            {
                return;
            }

            if (webcamTexture.isPlaying)
            {
                webcamTexture.Stop();
            }

            if (webcamUi != null && webcamUi.webcamPreviewImage != null)
            {
                webcamUi.webcamPreviewImage.texture = null;
                webcamUi.webcamPreviewImage.color = new Color(1f, 1f, 1f, 0.15f);
            }

            Destroy(webcamTexture);
            webcamTexture = null;
            UpdateStatusText();
            RunningStateChanged?.Invoke(false);
        }

        private WebCamDevice SelectDevice(WebCamDevice[] devices)
        {
            if (!string.IsNullOrWhiteSpace(preferredDeviceName))
            {
                foreach (var device in devices)
                {
                    if (string.Equals(device.name, preferredDeviceName, StringComparison.OrdinalIgnoreCase))
                    {
                        return device;
                    }
                }
            }

            return devices[0];
        }

        private void UpdatePreviewTransform()
        {
            if (webcamTexture == null || webcamUi == null || webcamUi.webcamPreviewImage == null)
            {
                return;
            }

            var previewImage = webcamUi.webcamPreviewImage;
            var scale = previewImage.rectTransform.localScale;
            scale.x = Mathf.Abs(scale.x) * (mirrorHorizontally ? -1f : 1f);
            scale.y = Mathf.Abs(scale.y) * (webcamTexture.videoVerticallyMirrored ? -1f : 1f);
            previewImage.rectTransform.localScale = scale;
            previewImage.rectTransform.localEulerAngles = new Vector3(0f, 0f, -webcamTexture.videoRotationAngle);

            if (webcamUi.webcamPreviewFitter != null && webcamTexture.width > 16 && webcamTexture.height > 16)
            {
                webcamUi.webcamPreviewFitter.aspectRatio = (float)webcamTexture.width / webcamTexture.height;
            }
        }

        private void UpdateStatusText()
        {
            if (webcamUi == null || webcamUi.deviceStatusText == null)
            {
                return;
            }

            if (startupFailed)
            {
                webcamUi.deviceStatusText.text = "카메라를 찾을 수 없습니다.";
                return;
            }

            if (!IsRunning)
            {
                webcamUi.deviceStatusText.text = "웹캠 준비 중";
                return;
            }

            webcamUi.deviceStatusText.text = $"웹캠 연결됨: {ActiveDeviceName}";
        }
    }
}
