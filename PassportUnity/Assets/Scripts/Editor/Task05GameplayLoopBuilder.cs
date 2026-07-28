#if UNITY_EDITOR
using RhythmPassport.Runtime;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RhythmPassport.Editor
{
    public static class Task05GameplayLoopBuilder
    {
        private const string ScenePath = "Assets/Scenes/SampleScene.unity";
        private const string BatchAutoBuildArgument = "RhythmPassportTask05AutoBuild";

        [InitializeOnLoadMethod]
        private static void AutoBuildAfterReloadInBatch()
        {
            if (!Application.isBatchMode)
            {
                return;
            }

            var args = System.Environment.GetCommandLineArgs();
            foreach (var arg in args)
            {
                if (arg != BatchAutoBuildArgument)
                {
                    continue;
                }

                EditorApplication.delayCall += BuildTask05GameplayLoop;
                break;
            }
        }

        [MenuItem("Rhythm Passport/Build Task 05 Gameplay Loop")]
        public static void BuildTask05GameplayLoop()
        {
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

            var canvas = FindRequiredRoot(scene, "Canvas");
            var managers = FindRequiredRoot(scene, "Managers");
            var character = FindRequiredRoot(scene, "Character");
            var environment = FindRequiredRoot(scene, "Environment");

            var gameplayUiRoot = FindRequiredChild(canvas.transform, "Gameplay UI");
            var scoreManagerObject = FindRequiredChild(managers.transform, "ScoreManager");
            var healthManagerObject = FindRequiredChild(managers.transform, "HealthManager");
            var collisionManagerObject = FindRequiredChild(managers.transform, "CollisionManager");
            var spawnManagerObject = FindRequiredChild(managers.transform, "SpawnManager");
            var characterControllerObject = FindRequiredChild(managers.transform, "CharacterLaneController");

            var sceneReferences = GetRequiredComponent<SceneFoundationReferences>(managers);
            var gameplayHud = ConfigureGameplayUi(gameplayUiRoot);
            var runner = GetRequiredComponent<CharacterLaneRunner>(characterControllerObject);
            var spawnRoot = FindOrCreateChild(environment.transform, "Spawned Objects");

            ConfigureManagers(
                sceneReferences,
                gameplayHud,
                runner,
                scoreManagerObject,
                healthManagerObject,
                collisionManagerObject,
                spawnManagerObject,
                spawnRoot.transform);
            ConfigureCharacterCollision(character, collisionManagerObject);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();

            Debug.Log("Task 05 gameplay loop build completed.");
        }

        private static GameplayHudReferences ConfigureGameplayUi(GameObject gameplayUiRoot)
        {
            var hud = gameplayUiRoot.GetComponent<GameplayHudReferences>();
            if (hud == null)
            {
                hud = gameplayUiRoot.AddComponent<GameplayHudReferences>();
            }

            hud.scoreText = ConfigureText(gameplayUiRoot.transform, "Score Text", new Vector2(0.1f, 0.92f), new Vector2(220f, 44f), 28, "점수: 0");
            hud.comboText = ConfigureText(gameplayUiRoot.transform, "Combo Text", new Vector2(0.1f, 0.87f), new Vector2(220f, 40f), 24, "콤보: x0");
            hud.healthText = ConfigureText(gameplayUiRoot.transform, "Health Text", new Vector2(0.1f, 0.82f), new Vector2(260f, 40f), 24, "체력 5 / 5");
            hud.spawnStatusText = ConfigureText(gameplayUiRoot.transform, "Spawn Status Text", new Vector2(0.14f, 0.12f), new Vector2(360f, 64f), 18, "생성 대기 중", TextAnchor.UpperLeft);
            return hud;
        }

        private static void ConfigureCharacterCollision(GameObject characterRoot, GameObject collisionManagerObject)
        {
            var collider = characterRoot.GetComponent<CapsuleCollider>();
            if (collider == null)
            {
                collider = characterRoot.AddComponent<CapsuleCollider>();
            }

            collider.center = new Vector3(0f, 1f, 0f);
            collider.height = 2f;
            collider.radius = 0.45f;
            collider.isTrigger = false;

            var rigidbody = characterRoot.GetComponent<Rigidbody>();
            if (rigidbody == null)
            {
                rigidbody = characterRoot.AddComponent<Rigidbody>();
            }

            rigidbody.isKinematic = true;
            rigidbody.useGravity = false;
            rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

            var relay = characterRoot.GetComponent<RunnerCollisionRelay>();
            if (relay == null)
            {
                relay = characterRoot.AddComponent<RunnerCollisionRelay>();
            }

            relay.collisionManager = GetRequiredComponent<CollisionManager>(collisionManagerObject);
        }

        private static void ConfigureManagers(
            SceneFoundationReferences sceneReferences,
            GameplayHudReferences gameplayHud,
            CharacterLaneRunner runner,
            GameObject scoreManagerObject,
            GameObject healthManagerObject,
            GameObject collisionManagerObject,
            GameObject spawnManagerObject,
            Transform spawnRoot)
        {
            var scoreManager = GetOrAddComponent<ScoreManager>(scoreManagerObject);
            scoreManager.gameplayHud = gameplayHud;
            scoreManager.avoidObstacleScore = 10;
            scoreManager.damagePenalty = 5;

            var healthManager = GetOrAddComponent<HealthManager>(healthManagerObject);
            healthManager.gameplayHud = gameplayHud;
            healthManager.runner = runner;
            healthManager.maxHealth = 5;

            var collisionManager = GetOrAddComponent<CollisionManager>(collisionManagerObject);
            collisionManager.runner = runner;
            collisionManager.scoreManager = scoreManager;
            collisionManager.healthManager = healthManager;

            var spawnManager = GetOrAddComponent<SpawnManager>(spawnManagerObject);
            spawnManager.sceneReferences = sceneReferences;
            spawnManager.runner = runner;
            spawnManager.scoreManager = scoreManager;
            spawnManager.gameplayHud = gameplayHud;
            spawnManager.spawnedObjectsRoot = spawnRoot;
            spawnManager.leftSpawnPoint = sceneReferences.environmentRoot.Find("Spawn Points/Left Spawn Point");
            spawnManager.centerSpawnPoint = sceneReferences.environmentRoot.Find("Spawn Points/Center Spawn Point");
            spawnManager.rightSpawnPoint = sceneReferences.environmentRoot.Find("Spawn Points/Right Spawn Point");
            spawnManager.despawnPoint = sceneReferences.environmentRoot.Find("Despawn Point");
            spawnManager.firstSpawnDelay = 5f;
            spawnManager.minSpawnInterval = 1.8f;
            spawnManager.maxSpawnInterval = 2.8f;
            spawnManager.spawnAheadDistance = 28f;
            spawnManager.despawnBehindDistance = 18f;

            collisionManager.spawnManager = spawnManager;
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
