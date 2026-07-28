using UnityEngine;

namespace RhythmPassport.Runtime
{
    public abstract class PoseProviderBehaviour : MonoBehaviour
    {
        public abstract void Initialize(WebcamManager webcamManager);
        public abstract bool TryGetPoseFrame(out PoseFrame frame);
    }
}
