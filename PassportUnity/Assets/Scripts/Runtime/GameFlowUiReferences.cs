using UnityEngine;
using UnityEngine.UI;

namespace RhythmPassport.Runtime
{
    public sealed class GameFlowUiReferences : MonoBehaviour
    {
        [Header("Panels")]
        public GameObject startGuidePanel;
        public GameObject countdownPanel;
        public GameObject pausePanel;
        public GameObject cameraErrorPanel;
        public GameObject resultPanel;

        [Header("Status Texts")]
        public Text startGuideText;
        public Text countdownText;
        public Text pauseText;
        public Text cameraErrorText;

        [Header("Result Texts")]
        public Text resultTitleText;
        public Text resultSummaryText;
        public Text resultScoreText;
        public Text resultBestScoreText;

        [Header("Buttons")]
        public Button retryButton;
        public Button homeButton;
    }
}
