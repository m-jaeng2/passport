using UnityEngine;

namespace RhythmPassport.Runtime
{
    public sealed class SpawnedTrackObject : MonoBehaviour
    {
        public TrackObjectType trackObjectType;
        public int laneIndex;
        public float spawnZ;
        public int scoreValue;
        public int comboValue;
        public int damageValue;
        public int healValue;
        public bool isObstacle;
        public bool awardsAvoidBonus;
        public bool wasResolved;

        public void Configure(
            TrackObjectType type,
            int assignedLaneIndex,
            float assignedSpawnZ,
            int assignedScoreValue,
            int assignedComboValue,
            int assignedDamageValue,
            int assignedHealValue,
            bool obstacle,
            bool avoidBonus)
        {
            trackObjectType = type;
            laneIndex = assignedLaneIndex;
            spawnZ = assignedSpawnZ;
            scoreValue = assignedScoreValue;
            comboValue = assignedComboValue;
            damageValue = assignedDamageValue;
            healValue = assignedHealValue;
            isObstacle = obstacle;
            awardsAvoidBonus = avoidBonus;
            wasResolved = false;
            gameObject.SetActive(true);
        }

        public void MarkResolved()
        {
            wasResolved = true;
        }
    }
}
