using UnityEngine;

namespace RhythmPassport.Runtime
{
    public sealed class RunnerCollisionRelay : MonoBehaviour
    {
        public CollisionManager collisionManager;

        private void OnTriggerEnter(Collider other)
        {
            if (collisionManager == null)
            {
                return;
            }

            var trackObject = other.GetComponentInParent<SpawnedTrackObject>();
            if (trackObject == null)
            {
                return;
            }

            collisionManager.HandleTrackObjectEntered(trackObject);
        }
    }
}
