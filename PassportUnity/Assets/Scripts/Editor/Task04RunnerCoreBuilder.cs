#if UNITY_EDITOR
using RhythmPassport.Runtime;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RhythmPassport.Editor
{
    public static class Task04RunnerCoreBuilder
    {
        private const string ScenePath = "Assets/Scenes/SampleScene.unity";

        [MenuItem("Rhythm Passport/Build Task 04 Runner Core")]
        public static void BuildTask04RunnerCore()
        {
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

            var managers = FindRequiredRoot(scene, "Managers");
            var characterRunnerObject = FindRequiredChild(managers.transform, "CharacterLaneController");
            var motionManagerObject = FindRequiredChild(managers.transform, "MotionManager");

            ConfigureRunner(managers, characterRunnerObject, motionManagerObject);
            ConfigureFollowCamera(managers);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();

            Debug.Log("Task 04 runner core build completed.");
        }

        private static void ConfigureRunner(GameObject managers, GameObject characterRunnerObject, GameObject motionManagerObject)
        {
            var sceneReferences = managers.GetComponent<SceneFoundationReferences>();
            if (sceneReferences == null)
            {
                throw new MissingReferenceException("SceneFoundationReferences를 찾을 수 없습니다.");
            }

            var motionRecognitionManager = motionManagerObject.GetComponent<MotionRecognitionManager>();
            if (motionRecognitionManager == null)
            {
                throw new MissingReferenceException("MotionRecognitionManager를 찾을 수 없습니다.");
            }

            var runner = characterRunnerObject.GetComponent<CharacterLaneRunner>();
            if (runner == null)
            {
                runner = characterRunnerObject.AddComponent<CharacterLaneRunner>();
            }

            runner.sceneReferences = sceneReferences;
            runner.motionRecognitionManager = motionRecognitionManager;
            runner.forwardSpeed = 4.5f;
            runner.laneSpacing = 3f;
            runner.laneChangeDuration = 0.18f;
            runner.jumpHeight = 1.4f;
            runner.jumpDuration = 0.65f;
        }

        private static void ConfigureFollowCamera(GameObject managers)
        {
            var sceneReferences = managers.GetComponent<SceneFoundationReferences>();
            if (sceneReferences == null || sceneReferences.mainCamera == null)
            {
                return;
            }

            var followCamera = sceneReferences.mainCamera.GetComponent<ThirdPersonFollowCamera>();
            if (followCamera == null)
            {
                followCamera = sceneReferences.mainCamera.gameObject.AddComponent<ThirdPersonFollowCamera>();
            }

            followCamera.Target = sceneReferences.cameraTarget;
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
    }
}
#endif
