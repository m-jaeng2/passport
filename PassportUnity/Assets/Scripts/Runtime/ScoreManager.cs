using UnityEngine;

namespace RhythmPassport.Runtime
{
    public sealed class ScoreManager : MonoBehaviour
    {
        [Header("Dependencies")]
        public GameplayHudReferences gameplayHud;

        [Header("Scoring")]
        [Min(0)] public int avoidObstacleScore = 10;
        [Min(0)] public int damagePenalty = 5;

        public int Score { get; private set; }
        public int Combo { get; private set; }
        public string DebugSummary { get; private set; } = "점수 0 | 콤보 x0";

        private void Start()
        {
            UpdateHud();
        }

        public void RegisterAvoidObstacle()
        {
            Score += avoidObstacleScore;
            Combo += 1;
            UpdateHud();
        }

        public void RegisterItemPickup(int scoreValue, int comboValue)
        {
            Score += scoreValue;
            Combo = Mathf.Max(0, Combo + comboValue);
            UpdateHud();
        }

        public void RegisterDamage()
        {
            Score = Mathf.Max(0, Score - damagePenalty);
            Combo = 0;
            UpdateHud();
        }

        private void UpdateHud()
        {
            DebugSummary = $"점수 {Score} | 콤보 x{Combo}";

            if (gameplayHud == null)
            {
                return;
            }

            if (gameplayHud.scoreText != null)
            {
                gameplayHud.scoreText.text = $"점수: {Score}";
            }

            if (gameplayHud.comboText != null)
            {
                gameplayHud.comboText.text = $"콤보: x{Combo}";
            }
        }
    }
}
