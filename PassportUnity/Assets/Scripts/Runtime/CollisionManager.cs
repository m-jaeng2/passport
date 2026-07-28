using UnityEngine;

namespace RhythmPassport.Runtime
{
    public sealed class CollisionManager : MonoBehaviour
    {
        [Header("Dependencies")]
        public CharacterLaneRunner runner;
        public ScoreManager scoreManager;
        public HealthManager healthManager;
        public SpawnManager spawnManager;

        public void HandleTrackObjectEntered(SpawnedTrackObject trackObject)
        {
            if (trackObject == null || trackObject.wasResolved || runner == null || runner.IsFinished)
            {
                return;
            }

            if (trackObject.isObstacle)
            {
                if (ShouldIgnoreObstacle(trackObject))
                {
                    return;
                }

                trackObject.MarkResolved();
                healthManager?.ApplyDamage(trackObject.damageValue);
                scoreManager?.RegisterDamage();
                spawnManager?.RecycleTrackObject(trackObject);
                return;
            }

            trackObject.MarkResolved();
            if (trackObject.healValue > 0)
            {
                healthManager?.Heal(trackObject.healValue);
            }

            scoreManager?.RegisterItemPickup(trackObject.scoreValue, trackObject.comboValue);
            spawnManager?.RecycleTrackObject(trackObject);
        }

        private bool ShouldIgnoreObstacle(SpawnedTrackObject trackObject)
        {
            return trackObject.trackObjectType switch
            {
                TrackObjectType.Fence => runner.IsAirborne,
                TrackObjectType.Barrier => runner.IsAirborne,
                _ => false,
            };
        }
    }
}
