#if UNITY_EDITOR
using RhythmPassport.Runtime;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RhythmPassport.Editor
{
    public static class Task06GameFlowBuilder
    {
        private const string ScenePath = "Assets/Scenes/SampleScene.unity";

        [MenuItem("Rhythm Passport/Build Task 06 Game Flow")]
        public static void BuildTask06GameFlow()
        {
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

            var canvas = FindRequiredRoot(scene, "Canvas");
            var managers = FindRequiredRoot(scene, "Managers");
            var startGuidePanel = FindRequiredChild(canvas.transform, "Start Guide Panel");
            var countdownPanel = FindRequiredChild(canvas.transform, "Countdown Panel");
            var pausePanel = FindRequiredChild(canvas.transform, "Pause Panel");
            var cameraErrorPanel = FindRequiredChild(canvas.transform, "Camera Error Panel");
            var resultPanel = FindRequiredChild(canvas.transform, "Result Panel");
            var gameplayUiRoot = FindRequiredChild(canvas.transform, "Gameplay UI");
            var gameManagerObject = FindRequiredChild(managers.transform, "GameManager");
            var uiManagerObject = FindRequiredChild(managers.transform, "UIManager");
            var motionManagerObject = FindRequiredChild(managers.transform, "MotionManager");
            var poseManagerObject = FindRequiredChild(managers.transform, "PoseDetectionManager");
            var scoreManagerObject = FindRequiredChild(managers.transform, "ScoreManager");
            var healthManagerObject = FindRequiredChild(managers.transform, "HealthManager");
            var characterRunnerObject = FindRequiredChild(managers.transform, "CharacterLaneController");

            var sceneReferences = GetRequiredComponent<SceneFoundationReferences>(managers);
            var gameplayHud = ConfigureGameplayHud(gameplayUiRoot);
            var flowUi = ConfigureFlowPanels(startGuidePanel, countdownPanel, pausePanel, cameraErrorPanel, resultPanel, uiManagerObject);
            ConfigureFlowManager(
                gameManagerObject,
                motionManagerObject,
                poseManagerObject,
                scoreManagerObject,
                healthManagerObject,
                characterRunnerObject,
                sceneReferences,
                gameplayHud,
                flowUi);
            ConfigureFinishTrigger(sceneReferences.finishTrigger, gameManagerObject);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();

            Debug.Log("Task 06 game flow build completed.");
        }

        private static GameplayHudReferences ConfigureGameplayHud(GameObject gameplayUiRoot)
        {
            var hud = GetOrAddComponent<GameplayHudReferences>(gameplayUiRoot);
            hud.scoreText = ConfigureText(gameplayUiRoot.transform, "Score Text", new Vector2(0.1f, 0.92f), new Vector2(220f, 44f), 28, "점수: 0");
            hud.comboText = ConfigureText(gameplayUiRoot.transform, "Combo Text", new Vector2(0.1f, 0.87f), new Vector2(220f, 40f), 24, "콤보: x0");
            hud.healthText = ConfigureText(gameplayUiRoot.transform, "Health Text", new Vector2(0.1f, 0.82f), new Vector2(260f, 40f), 24, "체력 5 / 5");
            hud.timerText = ConfigureText(gameplayUiRoot.transform, "Timer Text", new Vector2(0.9f, 0.92f), new Vector2(250f, 40f), 24, "남은 시간: 75.0", TextAnchor.MiddleRight);
            hud.bestScoreText = ConfigureText(gameplayUiRoot.transform, "Best Score Text", new Vector2(0.9f, 0.87f), new Vector2(250f, 40f), 22, "최고 점수: 0", TextAnchor.MiddleRight);
            hud.judgmentText = ConfigureText(gameplayUiRoot.transform, "Judgment Text", new Vector2(0.5f, 0.92f), new Vector2(260f, 40f), 22, "판정: 준비 대기", TextAnchor.MiddleCenter);
            hud.spawnStatusText = ConfigureText(gameplayUiRoot.transform, "Spawn Status Text", new Vector2(0.14f, 0.12f), new Vector2(360f, 64f), 18, "생성 대기 중", TextAnchor.UpperLeft);
            return hud;
        }

        private static GameFlowUiReferences ConfigureFlowPanels(
            GameObject startGuidePanel,
            GameObject countdownPanel,
            GameObject pausePanel,
            GameObject cameraErrorPanel,
            GameObject resultPanel,
            GameObject uiManagerObject)
        {
            var uiActions = GetOrAddComponent<GameplayFlowUiActions>(uiManagerObject);
            var references = GetOrAddComponent<GameFlowUiReferences>(uiManagerObject);

            references.startGuidePanel = startGuidePanel;
            references.countdownPanel = countdownPanel;
            references.pausePanel = pausePanel;
            references.cameraErrorPanel = cameraErrorPanel;
            references.resultPanel = resultPanel;

            references.startGuideText = ConfigurePanelText(startGuidePanel.transform, "Start Guide Text", new Vector2(0.5f, 0.5f), new Vector2(760f, 140f), 30, "어깨와 손목이 잘 보이도록 자세를 맞춰주세요.");
            references.countdownText = ConfigurePanelText(countdownPanel.transform, "Countdown Text", new Vector2(0.5f, 0.5f), new Vector2(320f, 180f), 72, "3", TextAnchor.MiddleCenter);
            references.pauseText = ConfigurePanelText(pausePanel.transform, "Pause Text", new Vector2(0.5f, 0.5f), new Vector2(700f, 140f), 30, "일시정지됨", TextAnchor.MiddleCenter);
            references.cameraErrorText = ConfigurePanelText(cameraErrorPanel.transform, "Camera Error Text", new Vector2(0.5f, 0.5f), new Vector2(720f, 120f), 28, "카메라 인식이 불안정합니다.", TextAnchor.MiddleCenter);

            references.resultTitleText = ConfigurePanelText(resultPanel.transform, "Result Title Text", new Vector2(0.5f, 0.75f), new Vector2(560f, 70f), 38, "플레이 종료", TextAnchor.MiddleCenter);
            references.resultSummaryText = ConfigurePanelText(resultPanel.transform, "Result Summary Text", new Vector2(0.5f, 0.56f), new Vector2(560f, 120f), 24, "요약", TextAnchor.MiddleCenter);
            references.resultScoreText = ConfigurePanelText(resultPanel.transform, "Result Score Text", new Vector2(0.5f, 0.40f), new Vector2(420f, 44f), 24, "최종 점수: 0", TextAnchor.MiddleCenter);
            references.resultBestScoreText = ConfigurePanelText(resultPanel.transform, "Result Best Score Text", new Vector2(0.5f, 0.33f), new Vector2(420f, 44f), 24, "최고 점수: 0", TextAnchor.MiddleCenter);

            references.retryButton = ConfigureButton(resultPanel.transform, "Retry Button", new Vector2(0.42f, 0.18f), "다시 하기", uiActions, nameof(GameplayFlowUiActions.RestartScene));
            references.homeButton = ConfigureButton(resultPanel.transform, "Home Button", new Vector2(0.58f, 0.18f), "처음으로", uiActions, nameof(GameplayFlowUiActions.ReturnToStart));

            EnsurePanelBackground(startGuidePanel.transform);
            EnsurePanelBackground(countdownPanel.transform);
            EnsurePanelBackground(pausePanel.transform);
            EnsurePanelBackground(cameraErrorPanel.transform);
            EnsurePanelBackground(resultPanel.transform);

            startGuidePanel.SetActive(false);
            countdownPanel.SetActive(false);
            pausePanel.SetActive(false);
            cameraErrorPanel.SetActive(false);
            resultPanel.SetActive(false);

            return references;
        }

        private static void ConfigureFlowManager(
            GameObject gameManagerObject,
            GameObject motionManagerObject,
            GameObject poseManagerObject,
            GameObject scoreManagerObject,
            GameObject healthManagerObject,
            GameObject characterRunnerObject,
            SceneFoundationReferences sceneReferences,
            GameplayHudReferences gameplayHud,
            GameFlowUiReferences flowUi)
        {
            var flowManager = GetOrAddComponent<GameplayFlowManager>(gameManagerObject);
            flowManager.sceneReferences = sceneReferences;
            flowManager.poseDetectionManager = GetRequiredComponent<PoseDetectionManager>(poseManagerObject);
            flowManager.motionRecognitionManager = GetRequiredComponent<MotionRecognitionManager>(motionManagerObject);
            flowManager.runner = GetRequiredComponent<CharacterLaneRunner>(characterRunnerObject);
            flowManager.scoreManager = GetRequiredComponent<ScoreManager>(scoreManagerObject);
            flowManager.healthManager = GetRequiredComponent<HealthManager>(healthManagerObject);
            flowManager.gameplayHud = gameplayHud;
            flowManager.gameFlowUi = flowUi;
            flowManager.readyAutoStartDelay = 0.75f;
            flowManager.countdownDuration = 3f;
            flowManager.maxRunDuration = 75f;
            flowManager.cameraWarningThreshold = 1f;
            flowManager.cameraAutoPauseThreshold = 3f;
            flowManager.cameraFailThreshold = 15f;

            flowManager.runner.autoStart = false;
        }

        private static void ConfigureFinishTrigger(BoxCollider finishTrigger, GameObject gameManagerObject)
        {
            if (finishTrigger == null)
            {
                return;
            }

            var relay = GetOrAddComponent<FinishTriggerRelay>(finishTrigger.gameObject);
            relay.gameplayFlowManager = GetRequiredComponent<GameplayFlowManager>(gameManagerObject);
            finishTrigger.isTrigger = true;
        }

        private static void EnsurePanelBackground(Transform panel)
        {
            var background = FindOrCreateUiObject(panel, "Panel Background", new Vector2(0.5f, 0.5f), new Vector2(1920f, 1080f));
            background.transform.SetAsFirstSibling();

            var image = GetOrAddComponent<Image>(background);
            image.color = new Color(0f, 0f, 0f, 0.6f);

            var rect = background.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;
            }
        }

        private static Button ConfigureButton(Transform parent, string name, Vector2 anchorCenter, string label, GameplayFlowUiActions actions, string methodName)
        {
            var buttonObject = FindOrCreateUiObject(parent, name, anchorCenter, new Vector2(180f, 52f));
            var image = GetOrAddComponent<Image>(buttonObject);
            image.color = new Color(1f, 1f, 1f, 0.92f);

            var button = GetOrAddComponent<Button>(buttonObject);
            button.onClick.RemoveAllListeners();

            switch (methodName)
            {
                case nameof(GameplayFlowUiActions.RestartScene):
                    UnityEventTools.AddPersistentListener(button.onClick, actions.RestartScene);
                    break;
                case nameof(GameplayFlowUiActions.ReturnToStart):
                    UnityEventTools.AddPersistentListener(button.onClick, actions.ReturnToStart);
                    break;
            }

            var text = ConfigurePanelText(buttonObject.transform, $"{name} Label", new Vector2(0.5f, 0.5f), new Vector2(180f, 52f), 24, label, TextAnchor.MiddleCenter);
            text.color = Color.black;
            return button;
        }

        private static Text ConfigurePanelText(Transform parent, string name, Vector2 anchorCenter, Vector2 size, int fontSize, string text, TextAnchor alignment = TextAnchor.MiddleCenter)
        {
            return ConfigureText(parent, name, anchorCenter, size, fontSize, text, alignment);
        }

        private static Text ConfigureText(
            Transform parent,
            string name,
            Vector2 anchorCenter,
            Vector2 size,
            int fontSize,
            string text,
            TextAnchor alignment = TextAnchor.MiddleLeft)
        {
            var gameObject = FindOrCreateUiObject(parent, name, anchorCenter, size);
            var textComponent = GetOrAddComponent<Text>(gameObject);
            textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            textComponent.fontSize = fontSize;
            textComponent.alignment = alignment;
            textComponent.horizontalOverflow = HorizontalWrapMode.Wrap;
            textComponent.verticalOverflow = VerticalWrapMode.Overflow;
            textComponent.color = Color.white;
            textComponent.text = text;
            return textComponent;
        }

        private static GameObject FindOrCreateUiObject(Transform parent, string name, Vector2 anchorCenter, Vector2 size)
        {
            var child = parent.Find(name);
            var gameObject = child != null ? child.gameObject : new GameObject(name, typeof(RectTransform));
            gameObject.transform.SetParent(parent, false);

            var rect = gameObject.GetComponent<RectTransform>();
            rect.anchorMin = anchorCenter;
            rect.anchorMax = anchorCenter;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = size;
            rect.anchoredPosition = Vector2.zero;
            rect.localScale = Vector3.one;
            return gameObject;
        }

        private static GameObject FindRequiredRoot(Scene scene, string name)
        {
            foreach (var root in scene.GetRootGameObjects())
            {
                if (root.name == name)
                {
                    return root;
                }
            }

            throw new MissingReferenceException($"필수 루트 오브젝트를 찾을 수 없습니다: {name}");
        }

        private static GameObject FindRequiredChild(Transform parent, string name)
        {
            var child = parent.Find(name);
            if (child == null)
            {
                throw new MissingReferenceException($"필수 자식 오브젝트를 찾을 수 없습니다: {parent.name}/{name}");
            }

            return child.gameObject;
        }

        private static T GetRequiredComponent<T>(GameObject gameObject) where T : Component
        {
            var component = gameObject.GetComponent<T>();
            if (component == null)
            {
                throw new MissingReferenceException($"{gameObject.name}에서 {typeof(T).Name} 컴포넌트를 찾을 수 없습니다.");
            }

            return component;
        }

        private static T GetOrAddComponent<T>(GameObject gameObject) where T : Component
        {
            var component = gameObject.GetComponent<T>();
            if (component == null)
            {
                component = gameObject.AddComponent<T>();
            }

            return component;
        }
    }
}
#endif
