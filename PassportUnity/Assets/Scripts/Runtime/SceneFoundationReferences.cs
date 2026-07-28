using UnityEngine;

namespace RhythmPassport.Runtime
{
    public sealed class SceneFoundationReferences : MonoBehaviour
    {
        [Header("Environment")]
        public Transform environmentRoot;
        public Transform road;
        public Transform lanePointLeft;
        public Transform lanePointCenter;
        public Transform lanePointRight;
        public Transform destinationLandmark;
        public BoxCollider finishTrigger;

        [Header("Character")]
        public Transform characterRoot;
        public Transform playerStart;
        public Transform playerVisual;
        public Transform cameraTarget;

        [Header("Interface")]
        public Canvas uiCanvas;
        public Camera mainCamera;
    }
}
