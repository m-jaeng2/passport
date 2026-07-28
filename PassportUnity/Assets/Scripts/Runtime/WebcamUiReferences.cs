using UnityEngine;
using UnityEngine.UI;

namespace RhythmPassport.Runtime
{
    public sealed class WebcamUiReferences : MonoBehaviour
    {
        public RawImage webcamPreviewImage;
        public AspectRatioFitter webcamPreviewFitter;
        public Text deviceStatusText;
        public Text recognitionStatusText;
        public Text landmarkStatusText;
        public Text gestureStatusText;
    }
}
