#if UNITY_EDITOR
using RhythmPassport.Runtime;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RhythmPassport.Editor
{
    public static class Task03MotionRecognitionBuilder
    {
        private const string ScenePath = "Assets/Scenes/SampleScene.unity";

        [MenuItem("Rhythm Passport/Build Task 03 Motion Recognition")]
        public static void BuildTask03MotionRecognition()
        {
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

            var canvas = FindRequiredRoot(scene, "Canvas");
            var managers = FindRequiredRoot(scene, "Managers");
            var cameraUiRoot = FindRequiredChild(canvas.transform, "Camera UI");
            var poseManagerObject = FindRequiredChild(managers.transform, "PoseDetectionManager");
            var motionManagerObject = FindRequiredChild(managers.transform, "MotionManager");

            var webcamUi = ConfigureCameraUi(cameraUiRoot);
            ConfigureMotionManager(motionManagerObject, poseManagerObject, webcamUi);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();

            Debug.Log("Task 03 motion recognition build completed.");
        }

        private static WebcamUiReferences ConfigureCameraUi(GameObject cameraUiRoot)
        {
            var references = cameraUiRoot.GetComponent<WebcamUiReferences>();
            if (references == null)
            {
                references = cameraUiRoot.AddComponent<WebcamUiReferences>();
            }

            var gestureText = ConfigureText(
                cameraUiRoot.transform,
                "Gesture Status Text",
                new Vector2(0.84f, 0.43f),
                new Vector2(320f, 120f),
                18,
                "제스처 대기 중",
                TextAnchor.UpperLeft);

            references.gestureStatusText = gestureText;
            return references;
        }

        private static void ConfigureMotionManager(GameObject motionManagerObject, GameObject poseManagerObject, WebcamUiReferences webcamUi)
        {
            var motionRecognitionManager = motionManagerObject.GetComponent<MotionRecognitionManager>();
            if (motionRecognitionManager == null)
            {
                motionRecognitionManager = motionManagerObject.AddComponent<MotionRecognitionManager>();
            }

            var poseDetectionManager = poseManagerObject.GetComponent<PoseDetectionManager>();

            motionRecognitionManager.poseDetectionManager = poseDetectionManager;
            motionRecognitionManager.webcamUi = webcamUi;
            motionRecognitionManager.handRaiseShoulderOffset = 0.08f;
            motionRecognitionManager.handsTogetherDistanceThreshold = 0.12f;
            motionRecognitionManager.neutralWristDropOffset = 0.04f;
            motionRecognitionManager.handsTogetherHoldDuration = 0.7f;
            motionRecognitionManager.sideGestureCooldown = 0.25f;
            motionRecognitionManager.jumpCooldown = 0.6f;
            motionRecognitionManager.pauseCooldown = 1f;
        }

        private static Text ConfigureText(
            Transform parent,
            string name,
            Vector2 anchorCenter,
            Vector2 size,
            int fontSize,
            string text,
            TextAnchor alignment)
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
