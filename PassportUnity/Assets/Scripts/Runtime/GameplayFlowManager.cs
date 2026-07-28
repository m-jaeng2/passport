using UnityEngine;

namespace RhythmPassport.Runtime
{
    public sealed class GameplayFlowManager : MonoBehaviour
    {
        private const string BestScoreKey = "RhythmPassport.BestScore";

        [Header("Dependencies")]
        public SceneFoundationReferences sceneReferences;
        public PoseDetectionManager poseDetectionManager;
        public MotionRecognitionManager motionRecognitionManager;
        public CharacterLaneRunner runner;
        public ScoreManager scoreManager;
        public HealthManager healthManager;
        public GameplayHudReferences gameplayHud;
        public GameFlowUiReferences gameFlowUi;

        [Header("Timers")]
        [Min(0.1f)] public float readyAutoStartDelay = 0.75f;
        [Min(1f)] public float countdownDuration = 3f;
        [Min(10f)] public float maxRunDuration = 75f;

        [Header("Camera Failure")]
        [Min(0f)] public float cameraWarningThreshold = 1f;
        [Min(0f)] public float cameraAutoPauseThreshold = 3f;
        [Min(0f)] public float cameraFailThreshold = 15f;

        private FlowState flowState = FlowState.WaitingForReady;
        private float readyTimer;
        private float countdownRemaining;
        private float runRemaining;
        private float missingTrackingTimer;
        private bool wasTrackingPaused;
        private int bestScore;

        private enum FlowState
        {
            WaitingForReady,
            Countdown,
            Running,
            Paused,
            Result,
        }

        private void Awake()
        {
            if (sceneReferences == null)
            {
                sceneReferences = FindAnyObjectByType<SceneFoundationReferences>();
            }

            if (poseDetectionManager == null)
            {
                poseDetectionManager = FindAnyObjectByType<PoseDetectionManager>();
            }

            if (motionRecognitionManager == null)
            {
                motionRecognitionManager = FindAnyObjectByType<MotionRecognitionManager>();
            }

            if (runner == null)
            {
                runner = FindAnyObjectByType<CharacterLaneRunner>();
            }

            if (scoreManager == null)
            {
                scoreManager = FindAnyObjectByType<ScoreManager>();
            }

            if (healthManager == null)
            {
                healthManager = FindAnyObjectByType<HealthManager>();
            }
        }

        private void Start()
        {
            bestScore = PlayerPrefs.GetInt(BestScoreKey, 0);
            runRemaining = maxRunDuration;

            if (runner != null)
            {
                runner.autoStart = false;
                runner.ResetRunner();
            }

            HideAllPanels();
            ShowStartGuide("어깨와 손목이 잘 보이도록 자세를 맞춰주세요.");
            UpdateGameplayHud();
        }

        private void Update()
        {
            SyncPauseState();
            UpdateCameraTrackingState();

            switch (flowState)
            {
                case FlowState.WaitingForReady:
                    UpdateWaitingForReady();
                    break;

                case FlowState.Countdown:
                    UpdateCountdown();
                    break;

                case FlowState.Running:
                    UpdateRunning();
                    break;

                case FlowState.Paused:
                    UpdatePaused();
                    break;
            }

            UpdateGameplayHud();
        }

        public void CompleteRun()
        {
            if (flowState == FlowState.Result)
            {
                return;
            }

            EndRun(GameResultType.Success);
        }

        private void UpdateWaitingForReady()
        {
            if (poseDetectionManager == null)
            {
                ShowStartGuide("포즈 인식 매니저를 찾을 수 없습니다.");
                return;
            }

            if (!poseDetectionManager.IsReady)
            {
                readyTimer = 0f;
                ShowStartGuide("포즈 준비 중입니다. 어깨와 손목을 화면 안에 유지해주세요.");
                return;
            }

            readyTimer += Time.deltaTime;
            var remaining = Mathf.Max(0f, readyAutoStartDelay - readyTimer);
            ShowStartGuide($"준비 완료. {remaining:0.0}초 뒤 자동으로 시작합니다.");

            if (readyTimer >= readyAutoStartDelay)
            {
                BeginCountdown();
            }
        }

        private void UpdateCountdown()
        {
            countdownRemaining -= Time.deltaTime;
            SetPanelState(gameFlowUi != null ? gameFlowUi.countdownPanel : null, true);

            if (gameFlowUi != null && gameFlowUi.countdownText != null)
            {
                var count = Mathf.CeilToInt(Mathf.Max(0f, countdownRemaining));
                gameFlowUi.countdownText.text = count > 0 ? count.ToString() : "시작";
            }

            if (countdownRemaining <= 0f)
            {
                StartGameplay();
            }
        }

        private void UpdateRunning()
        {
            runRemaining -= Time.deltaTime;
            if (runRemaining <= 0f)
            {
                EndRun(GameResultType.Timeout);
                return;
            }

            if (healthManager != null && healthManager.CurrentHealth <= 0)
            {
                EndRun(GameResultType.HealthDepleted);
            }
        }

        private void UpdatePaused()
        {
            if (runner != null && !runner.IsPaused && !wasTrackingPaused)
            {
                HidePanel(gameFlowUi != null ? gameFlowUi.pausePanel : null);
                flowState = FlowState.Running;
            }
        }

        private void SyncPauseState()
        {
            if (runner == null || flowState == FlowState.Result || flowState == FlowState.WaitingForReady || flowState == FlowState.Countdown)
            {
                return;
            }

            if (runner.IsPaused && flowState == FlowState.Running)
            {
                flowState = FlowState.Paused;
                ShowPause("일시정지됨\n양손 모으기로 다시 시작하세요.");
            }
            else if (!runner.IsPaused && flowState == FlowState.Paused && !wasTrackingPaused)
            {
                HidePanel(gameFlowUi != null ? gameFlowUi.pausePanel : null);
                flowState = FlowState.Running;
            }
        }

        private void UpdateCameraTrackingState()
        {
            if (poseDetectionManager == null || flowState == FlowState.Result)
            {
                return;
            }

            var trackingLost = !poseDetectionManager.HasReliableTracking;
            if (!trackingLost)
            {
                missingTrackingTimer = 0f;
                HidePanel(gameFlowUi != null ? gameFlowUi.cameraErrorPanel : null);

                if (wasTrackingPaused && runner != null && runner.IsPaused)
                {
                    wasTrackingPaused = false;
                    runner.Resume();
                    HidePanel(gameFlowUi != null ? gameFlowUi.pausePanel : null);
                    if (flowState == FlowState.Paused)
                    {
                        flowState = FlowState.Running;
                    }
                }

                return;
            }

            missingTrackingTimer += Time.deltaTime;
            if (missingTrackingTimer >= cameraWarningThreshold)
            {
                ShowCameraError($"카메라 인식이 불안정합니다.\n{missingTrackingTimer:0.0}초 동안 포즈가 사라졌습니다.");
            }

            if (missingTrackingTimer >= cameraFailThreshold && flowState != FlowState.WaitingForReady && flowState != FlowState.Result)
            {
                EndRun(GameResultType.CameraLost);
                return;
            }

            if (missingTrackingTimer >= cameraAutoPauseThreshold && flowState == FlowState.Running && runner != null && !runner.IsPaused)
            {
                wasTrackingPaused = true;
                runner.Pause();
                flowState = FlowState.Paused;
                ShowPause("카메라 인식이 끊겨 자동 일시정지되었습니다.");
            }
        }

        private void BeginCountdown()
        {
            flowState = FlowState.Countdown;
            countdownRemaining = countdownDuration;
            HidePanel(gameFlowUi != null ? gameFlowUi.startGuidePanel : null);
            SetPanelState(gameFlowUi != null ? gameFlowUi.countdownPanel : null, true);
        }

        private void StartGameplay()
        {
            flowState = FlowState.Running;
            runRemaining = maxRunDuration;
            wasTrackingPaused = false;
            HidePanel(gameFlowUi != null ? gameFlowUi.countdownPanel : null);
            HidePanel(gameFlowUi != null ? gameFlowUi.cameraErrorPanel : null);
            runner?.BeginRun();
        }

        private void EndRun(GameResultType resultType)
        {
            flowState = FlowState.Result;
            runner?.FinishRun();
            HidePanel(gameFlowUi != null ? gameFlowUi.pausePanel : null);
            HidePanel(gameFlowUi != null ? gameFlowUi.cameraErrorPanel : null);
            HidePanel(gameFlowUi != null ? gameFlowUi.countdownPanel : null);
            HidePanel(gameFlowUi != null ? gameFlowUi.startGuidePanel : null);

            var score = scoreManager != null ? scoreManager.Score : 0;
            if (score > bestScore)
            {
                bestScore = score;
                PlayerPrefs.SetInt(BestScoreKey, bestScore);
                PlayerPrefs.Save();
            }

            var title = resultType switch
            {
                GameResultType.Success => "여행 성공",
                GameResultType.HealthDepleted => "체력이 모두 소진되었습니다",
                GameResultType.Timeout => "시간이 초과되었습니다",
                GameResultType.CameraLost => "카메라 인식이 오래 끊겼습니다",
                _ => "플레이 종료",
            };

            var summary = $"최종 점수: {score}\n최고 점수: {bestScore}\n남은 시간: {Mathf.Max(0f, runRemaining):0.0}초";
            ShowResult(title, summary, score, bestScore);
        }

        private void UpdateGameplayHud()
        {
            if (gameplayHud == null)
            {
                return;
            }

            if (gameplayHud.timerText != null)
            {
                gameplayHud.timerText.text = $"남은 시간: {Mathf.Max(0f, runRemaining):0.0}";
            }

            if (gameplayHud.bestScoreText != null)
            {
                gameplayHud.bestScoreText.text = $"최고 점수: {bestScore}";
            }

            if (gameplayHud.judgmentText != null)
            {
                gameplayHud.judgmentText.text = flowState switch
                {
                    FlowState.WaitingForReady => "판정: 준비 대기",
                    FlowState.Countdown => "판정: 카운트다운",
                    FlowState.Running => "판정: 플레이 중",
                    FlowState.Paused => "판정: 일시정지",
                    FlowState.Result => "판정: 결과 표시",
                    _ => "판정: 대기",
                };
            }
        }

        private void HideAllPanels()
        {
            HidePanel(gameFlowUi != null ? gameFlowUi.startGuidePanel : null);
            HidePanel(gameFlowUi != null ? gameFlowUi.countdownPanel : null);
            HidePanel(gameFlowUi != null ? gameFlowUi.pausePanel : null);
            HidePanel(gameFlowUi != null ? gameFlowUi.cameraErrorPanel : null);
            HidePanel(gameFlowUi != null ? gameFlowUi.resultPanel : null);
        }

        private void ShowStartGuide(string message)
        {
            SetPanelState(gameFlowUi != null ? gameFlowUi.startGuidePanel : null, true);
            if (gameFlowUi != null && gameFlowUi.startGuideText != null)
            {
                gameFlowUi.startGuideText.text = message;
            }
        }

        private void ShowPause(string message)
        {
            SetPanelState(gameFlowUi != null ? gameFlowUi.pausePanel : null, true);
            if (gameFlowUi != null && gameFlowUi.pauseText != null)
            {
                gameFlowUi.pauseText.text = message;
            }
        }

        private void ShowCameraError(string message)
        {
            SetPanelState(gameFlowUi != null ? gameFlowUi.cameraErrorPanel : null, true);
            if (gameFlowUi != null && gameFlowUi.cameraErrorText != null)
            {
                gameFlowUi.cameraErrorText.text = message;
            }
        }

        private void ShowResult(string title, string summary, int score, int recordedBestScore)
        {
            SetPanelState(gameFlowUi != null ? gameFlowUi.resultPanel : null, true);
            if (gameFlowUi == null)
            {
                return;
            }

            if (gameFlowUi.resultTitleText != null)
            {
                gameFlowUi.resultTitleText.text = title;
            }

            if (gameFlowUi.resultSummaryText != null)
            {
                gameFlowUi.resultSummaryText.text = summary;
            }

            if (gameFlowUi.resultScoreText != null)
            {
                gameFlowUi.resultScoreText.text = $"최종 점수: {score}";
            }

            if (gameFlowUi.resultBestScoreText != null)
            {
                gameFlowUi.resultBestScoreText.text = $"최고 점수: {recordedBestScore}";
            }
        }

        private static void SetPanelState(GameObject panel, bool active)
        {
            if (panel != null)
            {
                panel.SetActive(active);
            }
        }

        private static void HidePanel(GameObject panel)
        {
            SetPanelState(panel, false);
        }
    }
}
