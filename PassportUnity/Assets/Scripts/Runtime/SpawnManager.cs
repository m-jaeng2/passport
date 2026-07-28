using System.Collections.Generic;
using UnityEngine;

namespace RhythmPassport.Runtime
{
    public sealed class SpawnManager : MonoBehaviour
    {
        [Header("Dependencies")]
        public SceneFoundationReferences sceneReferences;
        public CharacterLaneRunner runner;
        public ScoreManager scoreManager;
        public GameplayHudReferences gameplayHud;
        public Transform spawnedObjectsRoot;
        public Transform leftSpawnPoint;
        public Transform centerSpawnPoint;
        public Transform rightSpawnPoint;
        public Transform despawnPoint;

        [Header("Spawn Timing")]
        [Min(0f)] public float firstSpawnDelay = 5f;
        [Min(0.5f)] public float minSpawnInterval = 1.8f;
        [Min(0.5f)] public float maxSpawnInterval = 2.8f;
        [Min(5f)] public float spawnAheadDistance = 28f;
        [Min(5f)] public float despawnBehindDistance = 18f;

        private readonly Dictionary<TrackObjectType, Stack<SpawnedTrackObject>> pooledObjects = new();
        private readonly List<SpawnedTrackObject> activeObjects = new();

        private float nextSpawnTime;
        private string debugSummary = "생성 대기 중";

        private void Awake()
        {
            if (sceneReferences == null)
            {
                sceneReferences = FindAnyObjectByType<SceneFoundationReferences>();
            }

            if (runner == null)
            {
                runner = FindAnyObjectByType<CharacterLaneRunner>();
            }
        }

        private void Start()
        {
            nextSpawnTime = Time.time + firstSpawnDelay;
            UpdateHud();
        }

        private void Update()
        {
            RecyclePassedObjects();

            if (runner == null || runner.IsFinished || runner.IsPaused)
            {
                return;
            }

            if (Time.time < nextSpawnTime)
            {
                return;
            }

            SpawnWave();
            nextSpawnTime = Time.time + Random.Range(minSpawnInterval, maxSpawnInterval);
            UpdateHud();
        }

        public void RecycleTrackObject(SpawnedTrackObject trackObject)
        {
            if (trackObject == null)
            {
                return;
            }

            activeObjects.Remove(trackObject);
            trackObject.gameObject.SetActive(false);

            if (!pooledObjects.TryGetValue(trackObject.trackObjectType, out var pool))
            {
                pool = new Stack<SpawnedTrackObject>();
                pooledObjects.Add(trackObject.trackObjectType, pool);
            }

            pool.Push(trackObject);
        }

        private void SpawnWave()
        {
            var safeLane = Random.Range(0, 3);
            var obstacleCount = Random.value < 0.55f ? 1 : 2;
            var spawnZ = runner.transform.position.z + spawnAheadDistance;

            var blockedLanes = new List<int>(2);
            for (var lane = 0; lane < 3; lane++)
            {
                if (lane != safeLane)
                {
                    blockedLanes.Add(lane);
                }
            }

            Shuffle(blockedLanes);

            for (var index = 0; index < obstacleCount && index < blockedLanes.Count; index++)
            {
                SpawnTrackObject(PickObstacleType(), blockedLanes[index], spawnZ);
            }

            if (Random.value < 0.8f)
            {
                SpawnTrackObject(PickItemType(), safeLane, spawnZ + 2.5f);
            }

            debugSummary = $"생성 완료 | 안전 레인: {LaneLabel(safeLane)} | 활성 오브젝트: {activeObjects.Count}";
        }

        private void RecyclePassedObjects()
        {
            if (runner == null)
            {
                return;
            }

            var playerZ = runner.transform.position.z;
            for (var index = activeObjects.Count - 1; index >= 0; index--)
            {
                var trackObject = activeObjects[index];
                if (trackObject == null)
                {
                    activeObjects.RemoveAt(index);
                    continue;
                }

                if (!trackObject.wasResolved
                    && trackObject.isObstacle
                    && playerZ - trackObject.transform.position.z >= 0.5f)
                {
                    trackObject.MarkResolved();
                    scoreManager?.RegisterAvoidObstacle();
                }

                if (playerZ - trackObject.transform.position.z >= despawnBehindDistance)
                {
                    RecycleTrackObject(trackObject);
                }
            }
        }

        private void SpawnTrackObject(TrackObjectType type, int laneIndex, float spawnZ)
        {
            var trackObject = AcquireTrackObject(type);
            var lanePosition = ResolveLanePosition(laneIndex, spawnZ);
            trackObject.transform.SetPositionAndRotation(lanePosition, Quaternion.identity);
            trackObject.transform.SetParent(spawnedObjectsRoot, true);
            ConfigureTrackObject(trackObject, type, laneIndex, spawnZ);
            activeObjects.Add(trackObject);
        }

        private SpawnedTrackObject AcquireTrackObject(TrackObjectType type)
        {
            if (pooledObjects.TryGetValue(type, out var pool) && pool.Count > 0)
            {
                return pool.Pop();
            }

            return CreateTrackObject(type);
        }

        private SpawnedTrackObject CreateTrackObject(TrackObjectType type)
        {
            var primitiveType = ResolvePrimitive(type);
            var gameObject = GameObject.CreatePrimitive(primitiveType);
            gameObject.name = $"{type} Spawn";
            gameObject.layer = 0;

            var collider = gameObject.GetComponent<Collider>();
            if (collider != null)
            {
                collider.isTrigger = true;
            }

            var trackObject = gameObject.AddComponent<SpawnedTrackObject>();
            ApplyVisualStyle(trackObject, type);
            gameObject.SetActive(false);
            return trackObject;
        }

        private void ConfigureTrackObject(SpawnedTrackObject trackObject, TrackObjectType type, int laneIndex, float spawnZ)
        {
            trackObject.gameObject.name = $"{type} Spawn";
            trackObject.Configure(
                type,
                laneIndex,
                spawnZ,
                GetScoreValue(type),
                GetComboValue(type),
                GetDamageValue(type),
                GetHealValue(type),
                IsObstacle(type),
                IsObstacle(type));
            ApplyVisualStyle(trackObject, type);
        }

        private Vector3 ResolveLanePosition(int laneIndex, float spawnZ)
        {
            var lanePoint = laneIndex switch
            {
                0 => leftSpawnPoint,
                1 => centerSpawnPoint,
                2 => rightSpawnPoint,
                _ => centerSpawnPoint,
            };

            if (lanePoint != null)
            {
                return new Vector3(lanePoint.position.x, lanePoint.position.y, spawnZ);
            }

            var x = sceneReferences != null && sceneReferences.lanePointCenter != null
                ? sceneReferences.lanePointCenter.position.x + ((laneIndex - 1) * 3f)
                : (laneIndex - 1) * 3f;
            return new Vector3(x, 0.75f, spawnZ);
        }

        private void ApplyVisualStyle(SpawnedTrackObject trackObject, TrackObjectType type)
        {
            var transformRef = trackObject.transform;
            var renderer = trackObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            }

            switch (type)
            {
                case TrackObjectType.Fence:
                    transformRef.localScale = new Vector3(1.4f, 1f, 0.45f);
                    SetColor(renderer, new Color(0.75f, 0.42f, 0.2f));
                    break;

                case TrackObjectType.Pedestrian:
                    transformRef.localScale = new Vector3(0.9f, 1.8f, 0.9f);
                    SetColor(renderer, new Color(0.25f, 0.6f, 0.95f));
                    break;

                case TrackObjectType.Barrier:
                    transformRef.localScale = new Vector3(1.8f, 1.2f, 0.8f);
                    SetColor(renderer, new Color(1f, 0.45f, 0.15f));
                    break;

                case TrackObjectType.RedSignalZone:
                    transformRef.localScale = new Vector3(2.4f, 0.25f, 2.4f);
                    SetColor(renderer, new Color(0.9f, 0.12f, 0.12f));
                    break;

                case TrackObjectType.Heart:
                    transformRef.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                    SetColor(renderer, new Color(1f, 0.25f, 0.45f));
                    break;

                case TrackObjectType.Snack:
                    transformRef.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                    SetColor(renderer, new Color(1f, 0.85f, 0.25f));
                    break;

                case TrackObjectType.GoldenPassport:
                    transformRef.localScale = new Vector3(0.7f, 1f, 0.2f);
                    SetColor(renderer, new Color(1f, 0.78f, 0.1f));
                    break;
            }
        }

        private void UpdateHud()
        {
            if (gameplayHud != null && gameplayHud.spawnStatusText != null)
            {
                gameplayHud.spawnStatusText.text = debugSummary;
            }
        }

        private static PrimitiveType ResolvePrimitive(TrackObjectType type)
        {
            return type switch
            {
                TrackObjectType.Fence => PrimitiveType.Cube,
                TrackObjectType.Pedestrian => PrimitiveType.Capsule,
                TrackObjectType.Barrier => PrimitiveType.Cube,
                TrackObjectType.RedSignalZone => PrimitiveType.Cube,
                TrackObjectType.Heart => PrimitiveType.Sphere,
                TrackObjectType.Snack => PrimitiveType.Cube,
                TrackObjectType.GoldenPassport => PrimitiveType.Cube,
                _ => PrimitiveType.Cube,
            };
        }

        private static TrackObjectType PickObstacleType()
        {
            var value = Random.Range(0, 4);
            return (TrackObjectType)value;
        }

        private static TrackObjectType PickItemType()
        {
            var value = Random.Range(4, 7);
            return (TrackObjectType)value;
        }

        private static bool IsObstacle(TrackObjectType type)
        {
            return type <= TrackObjectType.RedSignalZone;
        }

        private static int GetScoreValue(TrackObjectType type)
        {
            return type switch
            {
                TrackObjectType.Heart => 10,
                TrackObjectType.Snack => 20,
                TrackObjectType.GoldenPassport => 100,
                _ => 0,
            };
        }

        private static int GetComboValue(TrackObjectType type)
        {
            return type switch
            {
                TrackObjectType.Heart => 1,
                TrackObjectType.Snack => 1,
                TrackObjectType.GoldenPassport => 2,
                _ => 0,
            };
        }

        private static int GetDamageValue(TrackObjectType type)
        {
            return type switch
            {
                TrackObjectType.Fence => 1,
                TrackObjectType.Pedestrian => 1,
                TrackObjectType.Barrier => 2,
                TrackObjectType.RedSignalZone => 1,
                _ => 0,
            };
        }

        private static int GetHealValue(TrackObjectType type)
        {
            return type == TrackObjectType.Heart ? 1 : 0;
        }

        private static string LaneLabel(int laneIndex)
        {
            return laneIndex switch
            {
                0 => "왼쪽",
                1 => "가운데",
                2 => "오른쪽",
                _ => "알 수 없음",
            };
        }

        private static void Shuffle(List<int> values)
        {
            for (var index = values.Count - 1; index > 0; index--)
            {
                var swapIndex = Random.Range(0, index + 1);
                (values[index], values[swapIndex]) = (values[swapIndex], values[index]);
            }
        }

        private static void SetColor(Renderer renderer, Color color)
        {
            if (renderer == null)
            {
                return;
            }

            renderer.sharedMaterial.color = color;
        }
    }
}
