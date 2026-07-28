#if UNITY_EDITOR
using RhythmPassport.Runtime;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RhythmPassport.Editor
{
    public static class Task01SceneBuilder
    {
        private const string ScenePath = "Assets/Scenes/SampleScene.unity";
        private const string AutoBuildSessionKey = "RhythmPassport.Task01Foundation.AutoBuild";

        [InitializeOnLoadMethod]
        private static void AutoBuildOnceAfterReload()
        {
            if (Application.isBatchMode)
            {
                return;
            }

            if (SessionState.GetBool(AutoBuildSessionKey, false))
            {
                return;
            }

            SessionState.SetBool(AutoBuildSessionKey, true);
            EditorApplication.delayCall += () =>
            {
                if (EditorApplication.isCompiling || EditorApplication.isUpdating)
                {
                    SessionState.SetBool(AutoBuildSessionKey, false);
                    return;
                }

                BuildTask01Foundation();
            };
        }

        [MenuItem("Rhythm Passport/Build Task 01 Foundation")]
        public static void BuildFromMenu()
        {
            BuildTask01Foundation();
        }

        public static void BuildTask01Foundation()
        {
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

            var environment = FindOrCreateRoot(scene, "Environment");
            var character = FindOrCreateRoot(scene, "Character");
            var managers = FindOrCreateRoot(scene, "Managers");
            var canvasRoot = FindOrCreateRoot(scene, "Canvas");

            ConfigureEnvironment(environment);
            var characterSetup = ConfigureCharacter(character);
            ConfigureCanvas(canvasRoot);
            ConfigureManagers(managers, environment, characterSetup, canvasRoot);
            ConfigureCamera(characterSetup.CameraTarget);
            EnsureEventSystem(scene);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();

            Debug.Log("Task 01 scene foundation build completed.");
        }

        private static void ConfigureEnvironment(GameObject environment)
        {
            var road = FindOrCreateChild(environment.transform, "Road");
            road.transform.SetPositionAndRotation(new Vector3(0f, 0f, 145f), Quaternion.identity);
            road.transform.localScale = new Vector3(1.2f, 1f, 30f);
            EnsurePrimitiveMesh(road, PrimitiveType.Plane);

            var lanePoints = FindOrCreateChild(environment.transform, "Lane Points");
            CreateMarker(lanePoints.transform, "Left Lane Point", new Vector3(-3f, 0.1f, 0f), new Vector3(0.35f, 0.35f, 0.35f), Color.blue);
            CreateMarker(lanePoints.transform, "Center Lane Point", new Vector3(0f, 0.1f, 0f), new Vector3(0.35f, 0.35f, 0.35f), Color.green);
            CreateMarker(lanePoints.transform, "Right Lane Point", new Vector3(3f, 0.1f, 0f), new Vector3(0.35f, 0.35f, 0.35f), Color.red);

            var spawnPoints = FindOrCreateChild(environment.transform, "Spawn Points");
            CreateEmptyPoint(spawnPoints.transform, "Left Spawn Point", new Vector3(-3f, 0.5f, 25f));
            CreateEmptyPoint(spawnPoints.transform, "Center Spawn Point", new Vector3(0f, 0.5f, 25f));
            CreateEmptyPoint(spawnPoints.transform, "Right Spawn Point", new Vector3(3f, 0.5f, 25f));

            var destination = FindOrCreateChild(environment.transform, "Destination Landmark");
            destination.transform.SetPositionAndRotation(new Vector3(0f, 4f, 290f), Quaternion.identity);
            destination.transform.localScale = new Vector3(10f, 8f, 4f);
            EnsurePrimitiveMesh(destination, PrimitiveType.Cube);
            ApplyColor(destination, new Color(0.7f, 0.8f, 1f));

            var finishTriggerObject = FindOrCreateChild(environment.transform, "Finish Trigger");
            finishTriggerObject.transform.SetPositionAndRotation(new Vector3(0f, 2f, 284f), Quaternion.identity);
            finishTriggerObject.transform.localScale = new Vector3(12f, 4f, 4f);
            var finishCollider = finishTriggerObject.GetComponent<BoxCollider>();
            if (finishCollider == null)
            {
                finishCollider = finishTriggerObject.AddComponent<BoxCollider>();
            }

            finishCollider.isTrigger = true;
            finishCollider.size = Vector3.one;

            var despawnPoint = FindOrCreateChild(environment.transform, "Despawn Point");
            despawnPoint.transform.localPosition = new Vector3(0f, 0.5f, -15f);
        }

        private static CharacterSetup ConfigureCharacter(GameObject character)
        {
            var playerStart = FindOrCreateChild(character.transform, "Player Start");
            playerStart.transform.localPosition = Vector3.zero;

            var playerVisual = FindOrCreateChild(character.transform, "Travel Runner");
            playerVisual.transform.localPosition = new Vector3(0f, 1f, 0f);
            playerVisual.transform.localRotation = Quaternion.identity;
            playerVisual.transform.localScale = new Vector3(1f, 2f, 1f);
            EnsurePrimitiveMesh(playerVisual, PrimitiveType.Capsule);
            ApplyColor(playerVisual, new Color(1f, 0.8f, 0.25f));

            var cameraTarget = FindOrCreateChild(character.transform, "Camera Target");
            cameraTarget.transform.localPosition = new Vector3(0f, 1.5f, 0f);

            var healthBarRoot = FindOrCreateChild(character.transform, "Health Bar World Canvas");
            healthBarRoot.transform.localPosition = new Vector3(0f, 2.7f, 0f);

            return new CharacterSetup(playerStart.transform, playerVisual.transform, cameraTarget.transform);
        }

        private static void ConfigureManagers(
            GameObject managers,
            GameObject environment,
            CharacterSetup characterSetup,
            GameObject canvasRoot)
        {
            var managerNames = new[]
            {
                "GameManager",
                "CameraManager",
                "PoseDetectionManager",
                "MotionManager",
                "CharacterLaneController",
                "SpawnManager",
                "CollisionManager",
                "ScoreManager",
                "HealthManager",
                "AudioManager",
                "UIManager",
            };

            foreach (var managerName in managerNames)
            {
                FindOrCreateChild(managers.transform, managerName);
            }

            var references = managers.GetComponent<SceneFoundationReferences>();
            if (references == null)
            {
                references = managers.AddComponent<SceneFoundationReferences>();
            }

            references.environmentRoot = environment.transform;
            references.road = environment.transform.Find("Road");
            references.lanePointLeft = environment.transform.Find("Lane Points/Left Lane Point");
            references.lanePointCenter = environment.transform.Find("Lane Points/Center Lane Point");
            references.lanePointRight = environment.transform.Find("Lane Points/Right Lane Point");
            references.destinationLandmark = environment.transform.Find("Destination Landmark");
            references.finishTrigger = environment.transform.Find("Finish Trigger").GetComponent<BoxCollider>();
            references.characterRoot = characterSetup.PlayerStart.parent;
            references.playerStart = characterSetup.PlayerStart;
            references.playerVisual = characterSetup.PlayerVisual;
            references.cameraTarget = characterSetup.CameraTarget;
            references.uiCanvas = canvasRoot.GetComponent<Canvas>();
            references.mainCamera = Camera.main;
        }

        private static void ConfigureCanvas(GameObject canvasRoot)
        {
            var canvas = canvasRoot.GetComponent<Canvas>();
            if (canvas == null)
            {
                canvas = canvasRoot.AddComponent<Canvas>();
            }

            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            if (canvasRoot.GetComponent<CanvasScaler>() == null)
            {
                var scaler = canvasRoot.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920f, 1080f);
                scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                scaler.matchWidthOrHeight = 0.5f;
            }

            if (canvasRoot.GetComponent<GraphicRaycaster>() == null)
            {
                canvasRoot.AddComponent<GraphicRaycaster>();
            }

            var uiNames = new[]
            {
                "Top UI",
                "Gameplay UI",
                "Camera UI",
                "Start Guide Panel",
                "Countdown Panel",
                "Pause Panel",
                "Camera Error Panel",
                "Result Panel",
            };

            foreach (var uiName in uiNames)
            {
                FindOrCreateChild(canvasRoot.transform, uiName);
            }
        }

        private static void ConfigureCamera(Transform cameraTarget)
        {
            var mainCamera = Camera.main;
            if (mainCamera == null)
            {
                return;
            }

            mainCamera.transform.SetPositionAndRotation(
                new Vector3(0f, 5.5f, -8f),
                Quaternion.Euler(18f, 0f, 0f));

            var follow = mainCamera.GetComponent<ThirdPersonFollowCamera>();
            if (follow == null)
            {
                follow = mainCamera.gameObject.AddComponent<ThirdPersonFollowCamera>();
            }

            follow.Target = cameraTarget;
        }

        private static void EnsureEventSystem(Scene scene)
        {
            foreach (var root in scene.GetRootGameObjects())
            {
                if (root.GetComponent<EventSystem>() != null)
                {
                    return;
                }
            }

            var eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            SceneManager.MoveGameObjectToScene(eventSystem, scene);
        }

        private static GameObject FindOrCreateRoot(Scene scene, string name)
        {
            foreach (var root in scene.GetRootGameObjects())
            {
                if (root.name == name)
                {
                    return root;
                }
            }

            var gameObject = new GameObject(name);
            SceneManager.MoveGameObjectToScene(gameObject, scene);
            return gameObject;
        }

        private static GameObject FindOrCreateChild(Transform parent, string name)
        {
            var child = parent.Find(name);
            if (child != null)
            {
                return child.gameObject;
            }

            var gameObject = new GameObject(name);
            gameObject.transform.SetParent(parent, false);
            return gameObject;
        }

        private static void CreateEmptyPoint(Transform parent, string name, Vector3 localPosition)
        {
            var point = FindOrCreateChild(parent, name);
            point.transform.localPosition = localPosition;
            point.transform.localRotation = Quaternion.identity;
            point.transform.localScale = Vector3.one;
        }

        private static void CreateMarker(
            Transform parent,
            string name,
            Vector3 localPosition,
            Vector3 localScale,
            Color color)
        {
            var marker = FindOrCreateChild(parent, name);
            marker.transform.localPosition = localPosition;
            marker.transform.localRotation = Quaternion.identity;
            marker.transform.localScale = localScale;
            EnsurePrimitiveMesh(marker, PrimitiveType.Cube);
            ApplyColor(marker, color);
        }

        private static void EnsurePrimitiveMesh(GameObject target, PrimitiveType primitiveType)
        {
            var meshFilter = target.GetComponent<MeshFilter>();
            var meshRenderer = target.GetComponent<MeshRenderer>();

            if (meshFilter != null && meshRenderer != null)
            {
                return;
            }

            var primitive = GameObject.CreatePrimitive(primitiveType);
            var primitiveTransform = primitive.transform;
            primitiveTransform.SetParent(target.transform, false);
            primitiveTransform.localPosition = Vector3.zero;
            primitiveTransform.localRotation = Quaternion.identity;
            primitiveTransform.localScale = Vector3.one;
            primitive.name = "Visual";

            var primitiveCollider = primitive.GetComponent<Collider>();
            if (primitiveCollider != null)
            {
                Object.DestroyImmediate(primitiveCollider);
            }
        }

        private static void ApplyColor(GameObject target, Color color)
        {
            var renderer = target.GetComponentInChildren<MeshRenderer>();
            if (renderer == null)
            {
                return;
            }

            if (renderer.sharedMaterial == null)
            {
                return;
            }

            var material = new Material(renderer.sharedMaterial)
            {
                color = color,
            };
            renderer.sharedMaterial = material;
        }

        private readonly struct CharacterSetup
        {
            public CharacterSetup(Transform playerStart, Transform playerVisual, Transform cameraTarget)
            {
                PlayerStart = playerStart;
                PlayerVisual = playerVisual;
                CameraTarget = cameraTarget;
            }

            public Transform PlayerStart { get; }
            public Transform PlayerVisual { get; }
            public Transform CameraTarget { get; }
        }
    }
}
#endif
