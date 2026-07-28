using UnityEngine;

namespace RhythmPassport.Runtime
{
    public sealed class CharacterLaneRunner : MonoBehaviour
    {
        [Header("Dependencies")]
        public SceneFoundationReferences sceneReferences;
        public MotionRecognitionManager motionRecognitionManager;

        [Header("Movement")]
        [Min(0f)] public float forwardSpeed = 4.5f;
        [Min(0.1f)] public float laneSpacing = 3f;
        [Min(0.01f)] public float laneChangeDuration = 0.18f;

        [Header("Jump")]
        [Min(0f)] public float jumpHeight = 1.4f;
        [Min(0.05f)] public float jumpDuration = 0.65f;

        private readonly float[] laneOffsets = new float[3];
        private Vector3 startPosition;
        private float forwardDistance;
        private float currentLaneX;
        private float laneChangeVelocity;
        private float jumpTimer;
        private int currentLaneIndex = 1;
        private bool isJumping;

        public RunnerRunState RunState { get; private set; } = RunnerRunState.Ready;
        public bool IsPaused => RunState == RunnerRunState.Paused;
        public bool IsFinished => RunState == RunnerRunState.Finished;
        public int CurrentLaneIndex => currentLaneIndex;
        public float CurrentForwardDistance => forwardDistance;
        public float CurrentJumpOffset { get; private set; }
        public bool IsAirborne => CurrentJumpOffset > 0.1f;
        public string DebugStatus { get; private set; } = "러너 준비 중";

        private void Awake()
        {
            if (sceneReferences == null)
            {
                sceneReferences = FindAnyObjectByType<SceneFoundationReferences>();
            }

            if (motionRecognitionManager == null)
            {
                motionRecognitionManager = FindAnyObjectByType<MotionRecognitionManager>();
            }
        }

        private void OnEnable()
        {
            if (motionRecognitionManager != null)
            {
                motionRecognitionManager.GestureTriggered += HandleGestureTriggered;
            }
        }

        private void OnDisable()
        {
            if (motionRecognitionManager != null)
            {
                motionRecognitionManager.GestureTriggered -= HandleGestureTriggered;
            }
        }

        private void Start()
        {
            InitializeRunner();
            BeginRun();
        }

        private void Update()
        {
            if (sceneReferences == null || sceneReferences.characterRoot == null)
            {
                DebugStatus = "캐릭터 참조 없음";
                return;
            }

            if (RunState != RunnerRunState.Running)
            {
                ApplyPose();
                return;
            }

            forwardDistance += forwardSpeed * Time.deltaTime;
            currentLaneX = Mathf.SmoothDamp(
                currentLaneX,
                laneOffsets[currentLaneIndex],
                ref laneChangeVelocity,
                laneChangeDuration);

            UpdateJump();
            ApplyPose();
            DebugStatus = BuildDebugStatus();
        }

        public void BeginRun()
        {
            if (sceneReferences == null || sceneReferences.characterRoot == null)
            {
                return;
            }

            RunState = RunnerRunState.Running;
            DebugStatus = "자동 전진 시작";
        }

        public void TogglePause()
        {
            if (RunState == RunnerRunState.Finished)
            {
                return;
            }

            RunState = RunState == RunnerRunState.Paused
                ? RunnerRunState.Running
                : RunnerRunState.Paused;

            DebugStatus = RunState == RunnerRunState.Paused
                ? "일시정지"
                : "다시 시작";
        }

        public void FinishRun()
        {
            RunState = RunnerRunState.Finished;
            DebugStatus = "주행 종료";
        }

        private void InitializeRunner()
        {
            if (sceneReferences == null || sceneReferences.characterRoot == null)
            {
                return;
            }

            startPosition = sceneReferences.playerStart != null
                ? sceneReferences.playerStart.position
                : sceneReferences.characterRoot.position;

            laneOffsets[0] = ResolveLaneOffset(sceneReferences.lanePointLeft, -laneSpacing);
            laneOffsets[1] = ResolveLaneOffset(sceneReferences.lanePointCenter, 0f);
            laneOffsets[2] = ResolveLaneOffset(sceneReferences.lanePointRight, laneSpacing);
            currentLaneIndex = 1;
            currentLaneX = laneOffsets[currentLaneIndex];
            forwardDistance = 0f;
            jumpTimer = 0f;
            isJumping = false;
            ApplyPose();
        }

        private float ResolveLaneOffset(Transform lanePoint, float fallback)
        {
            if (lanePoint == null)
            {
                return fallback;
            }

            return lanePoint.position.x - startPosition.x;
        }

        private void HandleGestureTriggered(MotionGesture gesture)
        {
            switch (gesture)
            {
                case MotionGesture.LeftHandUp:
                    MoveLane(-1);
                    break;

                case MotionGesture.RightHandUp:
                    MoveLane(1);
                    break;

                case MotionGesture.BothHandsUp:
                    StartJump();
                    break;

                case MotionGesture.HandsTogether:
                    TogglePause();
                    break;
            }
        }

        private void MoveLane(int direction)
        {
            if (RunState != RunnerRunState.Running)
            {
                return;
            }

            currentLaneIndex = Mathf.Clamp(currentLaneIndex + direction, 0, laneOffsets.Length - 1);
        }

        private void StartJump()
        {
            if (RunState != RunnerRunState.Running || isJumping)
            {
                return;
            }

            isJumping = true;
            jumpTimer = 0f;
        }

        private void UpdateJump()
        {
            if (!isJumping)
            {
                return;
            }

            jumpTimer += Time.deltaTime;
            if (jumpTimer >= jumpDuration)
            {
                jumpTimer = jumpDuration;
                isJumping = false;
            }
        }

        private void ApplyPose()
        {
            if (sceneReferences == null || sceneReferences.characterRoot == null)
            {
                return;
            }

            var jumpOffset = 0f;
            if (jumpDuration > 0f && jumpTimer > 0f)
            {
                var normalizedTime = Mathf.Clamp01(jumpTimer / jumpDuration);
                jumpOffset = Mathf.Sin(normalizedTime * Mathf.PI) * jumpHeight;

                if (!isJumping && normalizedTime >= 1f)
                {
                    jumpTimer = 0f;
                }
            }

            CurrentJumpOffset = jumpOffset;

            sceneReferences.characterRoot.position = new Vector3(
                startPosition.x + currentLaneX,
                startPosition.y + jumpOffset,
                startPosition.z + forwardDistance);
        }

        private string BuildDebugStatus()
        {
            var laneText = currentLaneIndex switch
            {
                0 => "왼쪽 레인",
                1 => "가운데 레인",
                2 => "오른쪽 레인",
                _ => "알 수 없는 레인",
            };

            var jumpText = isJumping ? "점프 중" : "지상 이동";
            return $"주행 상태: {laneText}\n전진 거리: {forwardDistance:0.0}\n점프 상태: {jumpText}";
        }
    }
}
