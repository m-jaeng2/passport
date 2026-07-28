using UnityEngine;

namespace RhythmPassport.Runtime
{
    public sealed class HealthManager : MonoBehaviour
    {
        [Header("Dependencies")]
        public GameplayHudReferences gameplayHud;
        public CharacterLaneRunner runner;

        [Header("Health")]
        [Min(1)] public int maxHealth = 5;

        public int CurrentHealth { get; private set; }
        public string DebugSummary { get; private set; } = "체력 5 / 5";

        private void Start()
        {
            CurrentHealth = maxHealth;
            UpdateHud();
        }

        public void ApplyDamage(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
            UpdateHud();

            if (CurrentHealth == 0 && runner != null)
            {
                runner.FinishRun();
            }
        }

        public void Heal(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
            UpdateHud();
        }

        private void UpdateHud()
        {
            DebugSummary = $"체력 {CurrentHealth} / {maxHealth}";

            if (gameplayHud != null && gameplayHud.healthText != null)
            {
                gameplayHud.healthText.text = DebugSummary;
            }
        }
    }
}
