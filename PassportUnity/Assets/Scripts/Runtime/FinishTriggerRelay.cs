using UnityEngine;

namespace RhythmPassport.Runtime
{
    public sealed class FinishTriggerRelay : MonoBehaviour
    {
        public GameplayFlowManager gameplayFlowManager;

        private void OnTriggerEnter(Collider other)
        {
            if (gameplayFlowManager == null)
            {
                return;
            }

            var relay = other.GetComponent<RunnerCollisionRelay>();
            if (relay == null)
            {
                relay = other.GetComponentInParent<RunnerCollisionRelay>();
            }

            if (relay == null)
            {
                return;
            }

            gameplayFlowManager.CompleteRun();
        }
    }
}
