#if UNITY_EDITOR
using RhythmPassport.Runtime;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RhythmPassport.Editor
{
    public static class Task02WebcamPoseBuilder
    {
        private const string ScenePath = "Assets/Scenes/SampleScene.unity";

        [MenuItem("Rhythm Passport/Build Task 02 Webcam Pose Pipeline")]
        public static void BuildTask02WebcamPosePipeline()
        {
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

            var canvas = FindRequiredRoot(scene, "Canvas");
            var managers = FindRequiredRoot(scene, "Managers");
            var cameraUiRoot = FindRequiredChild(canvas.transform, "Camera UI");
            var cameraManagerObject = FindRequiredChild(managers.transform, "CameraManager");
            var poseManagerObject = FindRequiredChild(managers.transform, "PoseDetectionManager");

            var webcamUi = ConfigureCameraUi(cameraUiRoot);
            ConfigureCameraManager(cameraManagerObject, webcamUi);
            ConfigurePoseManager(poseManagerObject, cameraManagerObject, webcamUi);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();

            Debug.Log("Task 02 webcam pose pipeline build completed.");
        }

        private static WebcamUiReferences ConfigureCameraUi(GameObject cameraUiRoot)
        {
            var references = cameraUiRoot.GetComponent<WebcamUiReferences>();
            if (references == null)
            {
                references = cameraUiRoot.AddComponent<WebcamUiReferences>();
            }

            var previewRoot = FindOrCreateUiObject(cameraUiRoot.transform, "Webcam Preview Root", new Vector2(0.84f, 0.5f), new Vector2(0.28f, 0.58f));
            var previewImage = GetOrAddComponent<RawImage>(previewRoot);
            previewImage.color = new Color(1f, 1f, 1f, 0.15f);
            var fitter = GetOrAddComponent<AspectRatioFitter>(previewRoot);
            fitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
            fitter.aspectRatio = 16f / 9f;

            var statusText = ConfigureText(cameraUiRoot.transform, "Device Status Text", new Vector2(0.84f, 0.84f), new Vector2(320f, 36f), 20, "웹캠 준비 중");
            var recognitionText = ConfigureText(cameraUiRoot.transform, "Recognition Status Text", new Vector2(0.84f, 0.16f), new Vector2(320f, 40f), 24, "플레이어 인식 대기");
            var landmarkText = ConfigureText(cameraUiRoot.transform, "Landmark Status Text", new Vector2(0.84f, 0.74f), new Vector2(320f, 140f), 18, "랜드마크 없음", TextAnchor.UpperLeft);

            references.webcamPreviewImage = previewImage;
            references.webcamPreviewFitter = fitter;
            references.deviceStatusText = statusText;
            references.recognitionStatusText = recognitionText;
            references.landmarkStatusText = landmarkText;
            return references;
        }

        private static void ConfigureCameraManager(GameObject cameraManagerObject, WebcamUiReferences webcamUi)
        {
            var webcamManager = cameraManagerObject.GetComponent<WebcamManager>();
            if (webcamManager == null)
            {
                webcamManager = cameraManagerObject.AddComponent<WebcamManager>();
            }

            webcamManager.webcamUi = webcamUi;
            webcamManager.playOnStart = true;
            webcamManager.mirrorHorizontally = true;
        }

        private static void ConfigurePoseManager(GameObject poseManagerObject, GameObject cameraManagerObject, WebcamUiReferences webcamUi)
        {
            var webcamManager = cameraManagerObject.GetComponent<WebcamManager>();
            var poseManager = poseManagerObject.GetComponent<PoseDetectionManager>();
            if (poseManager == null)
            {
                poseManager = poseManagerObject.AddComponent<PoseDetectionManager>();
            }

            var debugProvider = poseManagerObject.GetComponent<DebugPoseProvider>();
            if (debugProvider == null)
            {
                debugProvider = poseManagerObject.AddComponent<DebugPoseProvider>();
            }

            poseManager.webcamManager = webcamManager;
            poseManager.webcamUi = webcamUi;
            poseManager.poseProvider = debugProvider;
            poseManager.minimumConfidence = 0.6f;
            poseManager.readyHoldDuration = 2f;
        }

        private static Text ConfigureText(
            Transform parent,
            string name,
            Vector2 anchorCenter,
            Vector2 size,
            int fontSize,
            string text,
            TextAnchor alignment = TextAnchor.MiddleCenter)
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
