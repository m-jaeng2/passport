# 작업 변경 기록

이 문서는 프로젝트를 진행하면서 제가 만든 주요 변경 사항을 기록한다.
의미 있는 작업이 생길 때마다 변경 요약, 커밋 메시지, 수정 파일을 여기에 남긴다.

## 기록 형식

### YYYY-MM-DD HH:MM
- 변경 요약:
- 커밋 메시지:
- 변경 파일:
- 메모:

## 기록 내역

### 2026-07-28 15:20
- 변경 요약: Unity 프로젝트 업로드 준비를 위해 기본 `.gitignore`와 변경 기록 문서를 추가했다.
- 커밋 메시지: `chore: add Unity gitignore and agent change log`
- 변경 파일: `.gitignore`, `agent.md`
- 메모: 현재 환경에는 로컬 `git` 실행 파일이 없어 GitHub 커넥터로 원격 반영을 진행했다.

### 2026-07-28 15:35
- 변경 요약: 현재 Unity 프로젝트 구조를 기준으로 서브에이전트 작업 규칙 문서를 추가했다.
- 커밋 메시지: `docs: define sub-agent working rules`
- 변경 파일: `mds/0. AGENTS요청.md`, `agent.md`
- 메모: 추적 대상 파일, 생성 파일 제외 규칙, 기록 방식, 커밋 메시지 형식을 정리했다.

### 2026-07-28 16:25
- 변경 요약: 프로젝트 기획서를 실행용 문서로 나누어 루트 `AGENTS.md`와 단계별 Task 문서 초안을 만들었다.
- 커밋 메시지: `docs: add project agents guide and phased task files`
- 변경 파일: `AGENTS.md`, `mds/tasks/task-01-game-scene-foundation.md`, `mds/tasks/task-02-webcam-and-pose-pipeline.md`, `mds/tasks/task-03-motion-detection.md`, `mds/tasks/task-04-character-runner-core.md`, `mds/tasks/task-05-spawn-score-health.md`, `mds/tasks/task-06-game-flow-ui-results.md`, `agent.md`
- 메모: 체크리스트, Inspector 메모, 테스트 항목, 미해결 이슈까지 바로 업데이트할 수 있게 구성했다.

### 2026-07-28 16:30
- 변경 요약: 게임 규칙 해석이 갈리지 않도록 월드 고정 오브젝트와 성공 조건 기준을 `AGENTS.md`에 명시했다.
- 커밋 메시지: `docs: clarify gameplay rules in agents guide`
- 변경 파일: `AGENTS.md`, `agent.md`
- 메모: 월드 좌표 고정, `Finish Trigger`, 시간 제한, 콤보 규칙 후속 정리를 기준으로 잡았다.

### 2026-07-28 16:40
- 변경 요약: Task 문서를 루트 `Task` 폴더로 옮기고, 제가 작성한 운영 문서를 전부 한국어로 통일했다.
- 커밋 메시지: `docs: move task docs and translate project guides to Korean`
- 변경 파일: `AGENTS.md`, `agent.md`, `mds/0. AGENTS요청.md`, `Task/01-게임씬-기초구성.md`, `Task/02-웹캠-포즈-파이프라인.md`, `Task/03-동작-인식.md`, `Task/04-캐릭터-러너-코어.md`, `Task/05-스폰-점수-체력.md`, `Task/06-게임흐름-UI-결과.md`
- 메모: 이후 작업 기준 문서는 모두 한국어 기준으로 유지한다.

### 2026-07-28 16:55
- 변경 요약: 1단계 씬 기초 구성을 자동화하기 위해 씬 빌더와 카메라 추적, 씬 참조 스크립트를 추가했다.
- 커밋 메시지: `feat: add task 01 scene foundation builder`
- 변경 파일: `PassportUnity/Assets/Scripts/Editor/Task01SceneBuilder.cs`, `PassportUnity/Assets/Scripts/Runtime/ThirdPersonFollowCamera.cs`, `PassportUnity/Assets/Scripts/Runtime/SceneFoundationReferences.cs`, `Task/01-게임씬-기초구성.md`, `agent.md`
- 메모: 현재 Unity 프로젝트가 이미 열려 있어 별도 배치 실행은 차단되었고, 열린 에디터에서 스크립트 리컴파일 또는 메뉴 실행 후 실제 씬 반영을 확인해야 한다.

### 2026-07-28 16:58
- 변경 요약: 배치 실행으로 `SampleScene`에 1단계 씬 골격을 실제 반영하고 Task 01을 완료 처리했다.
- 커밋 메시지: `feat: build task 01 sample scene foundation`
- 변경 파일: `PassportUnity/Assets/Scenes/SampleScene.unity`, `Task/01-게임씬-기초구성.md`, `agent.md`
- 메모: `Environment`, `Character`, `Managers`, `Canvas`, `EventSystem`, 레인 기준점, 임시 랜드마크, `Finish Trigger`, 카메라 추적 기준점이 씬에 생성된 것을 확인했다.

### 2026-07-28 17:05
- 변경 요약: 하위 Unity 프로젝트 구조에 맞게 `.gitignore`를 보강해 `Library`, `Logs`, `UserSettings`, `csproj`, 배치 로그가 다시 추적되지 않도록 정리했다.
- 커밋 메시지: `chore: update gitignore for nested Unity project`
- 변경 파일: `.gitignore`, `agent.md`
- 메모: 현재 저장소는 `PassportUnity/`가 하위 폴더이므로 루트 기준 무시 규칙만으로는 생성물이 충분히 제외되지 않았다.

### 2026-07-28 17:12
- 변경 요약: 2단계 웹캠 포즈 파이프라인의 기초 구현으로 웹캠 프리뷰, 카메라 UI, 포즈 데이터 구조, 포즈 공급자 추상화, 디버그 랜드마크 공급자를 추가했다.
- 커밋 메시지: `feat: add task 02 webcam pose pipeline scaffold`
- 변경 파일: `PassportUnity/Assets/Scripts/Editor/Task02WebcamPoseBuilder.cs`, `PassportUnity/Assets/Scripts/Runtime/WebcamUiReferences.cs`, `PassportUnity/Assets/Scripts/Runtime/WebcamManager.cs`, `PassportUnity/Assets/Scripts/Runtime/PoseLandmark.cs`, `PassportUnity/Assets/Scripts/Runtime/PoseFrame.cs`, `PassportUnity/Assets/Scripts/Runtime/PoseProviderBehaviour.cs`, `PassportUnity/Assets/Scripts/Runtime/DebugPoseProvider.cs`, `PassportUnity/Assets/Scripts/Runtime/PoseDetectionManager.cs`, `PassportUnity/Assets/Scenes/SampleScene.unity`, `Task/02-웹캠-포즈-파이프라인.md`, `agent.md`
- 메모: 실제 외부 포즈 엔진 연동 전까지는 디버그 포즈 공급자가 얼굴, 어깨, 손목 정규화 좌표를 제공하며, 웹캠 프리뷰와 인식 준비 상태 UI는 실동작한다.

### 2026-07-28 17:35
- 변경 요약: MediaPipe Unity Plugin을 로컬 tarball 패키지로 연결하고, 디버그 공급자를 실제 `MediaPipePoseProvider`로 교체해 `SampleScene`에 반영했다.
- 커밋 메시지: `feat: connect MediaPipe pose provider`
- 변경 파일: `.gitignore`, `PassportUnity/Packages/manifest.json`, `PassportUnity/Packages/packages-lock.json`, `PassportUnity/Assets/Scripts/Runtime/MediaPipePoseProvider.cs`, `PassportUnity/Assets/Scripts/Runtime/WebcamManager.cs`, `PassportUnity/Assets/Scripts/Runtime/PoseDetectionManager.cs`, `PassportUnity/Assets/Scripts/Runtime/PoseProviderBehaviour.cs`, `PassportUnity/Assets/Scripts/Runtime/DebugPoseProvider.cs`, `PassportUnity/Assets/Scripts/Editor/Task02WebcamPoseBuilder.cs`, `PassportUnity/Assets/Scenes/SampleScene.unity`, `Task/02-웹캠-포즈-파이프라인.md`, `agent.md`
- 메모: Unity는 외부 tarball URL을 직접 패키지 버전으로 받지 못해 `vendor/com.github.homuler.mediapipe-0.16.3.tgz`를 로컬 tarball 의존성으로 연결했다. 이 파일은 용량이 커서 `.gitignore`로 제외했고, 저장소에는 `manifest.json`과 `packages-lock.json` 기준 설정만 남긴다. Unity 배치 실행으로 `MediaPipePoseProvider`가 `SampleScene`에 붙은 것까지 확인했다.

### 2026-07-28 17:50
- 변경 요약: 랜드마크를 게임 입력으로 변환하는 `Task 03` 동작 인식 파이프라인을 추가하고 `MotionManager`에 제스처 판정기를 연결했다.
- 커밋 메시지: `feat: add motion gesture recognition pipeline`
- 변경 파일: `PassportUnity/Assets/Scripts/Runtime/MotionGesture.cs`, `PassportUnity/Assets/Scripts/Runtime/MotionRecognitionManager.cs`, `PassportUnity/Assets/Scripts/Runtime/WebcamUiReferences.cs`, `PassportUnity/Assets/Scripts/Editor/Task03MotionRecognitionBuilder.cs`, `PassportUnity/Assets/Scenes/SampleScene.unity`, `Task/03-동작-인식.md`, `agent.md`
- 메모: 기본 자세 복귀 전 입력 잠금, 양손 모으기 홀드 시간, 좌우 제스처/점프/일시정지 재사용 대기시간을 모두 `MotionRecognitionManager`에서 처리한다. Unity 배치 실행으로 `Gesture Status Text`와 `MotionRecognitionManager`가 `SampleScene`에 반영된 것을 확인했다.

### 2026-07-28 18:15
- 변경 요약: `Task 04` 러너 코어를 추가해 제스처 입력이 자동 전진, 3레인 이동, 점프, 일시정지로 이어지도록 연결했다.
- 커밋 메시지: `feat: add runner movement core`
- 변경 파일: `.gitignore`, `PassportUnity/Assets/Scripts/Runtime/RunnerRunState.cs`, `PassportUnity/Assets/Scripts/Runtime/CharacterLaneRunner.cs`, `PassportUnity/Assets/Scripts/Editor/Task04RunnerCoreBuilder.cs`, `PassportUnity/Assets/Scenes/SampleScene.unity`, `Task/04-캐릭터-러너-코어.md`, `agent.md`
- 메모: 러너 이동은 `Transform` 기반 보간으로 구현해 전진, 레인 변경, 점프가 동시에 유지되도록 맞췄다. `HandsTogether`는 일시정지 토글로 연결했고, `Task04RunnerCoreBuilder`로 `CharacterLaneController`와 카메라 추적 참조를 일관되게 세팅할 수 있게 정리했다.

### 2026-07-28 19:05
- 변경 요약: `Task 05` 게임플레이 루프를 추가해 레인 기반 장애물·아이템 생성, 충돌, 점수·콤보·체력, 풀링 구조를 연결했다.
- 커밋 메시지: `feat: add spawn score and health loop`
- 변경 파일: `.gitignore`, `PassportUnity/Assets/Scripts/Runtime/CharacterLaneRunner.cs`, `PassportUnity/Assets/Scripts/Runtime/GameplayHudReferences.cs`, `PassportUnity/Assets/Scripts/Runtime/TrackObjectType.cs`, `PassportUnity/Assets/Scripts/Runtime/SpawnedTrackObject.cs`, `PassportUnity/Assets/Scripts/Runtime/RunnerCollisionRelay.cs`, `PassportUnity/Assets/Scripts/Runtime/ScoreManager.cs`, `PassportUnity/Assets/Scripts/Runtime/HealthManager.cs`, `PassportUnity/Assets/Scripts/Runtime/CollisionManager.cs`, `PassportUnity/Assets/Scripts/Runtime/SpawnManager.cs`, `PassportUnity/Assets/Scripts/Editor/Task05GameplayLoopBuilder.cs`, `PassportUnity/Assets/Scenes/SampleScene.unity`, `Task/05-스폰-점수-체력.md`, `agent.md`
- 메모: `SpawnManager`는 항상 한 개 이상의 안전 레인을 남기는 웨이브 생성 규칙과 타입별 오브젝트 풀을 사용한다. `RunnerCollisionRelay`와 `CollisionManager`가 장애물 피해, 점프 회피, 아이템 획득을 처리하고, `ScoreManager`와 `HealthManager`는 `Gameplay UI`에 점수, 콤보, 체력을 바로 반영한다.

### 2026-07-28 19:35
- 변경 요약: `Task 06` 게임 흐름과 UI를 추가해 자세 준비, 카운트다운, 일시정지, 카메라 오류, 결과 패널, 최고 점수 저장까지 한 씬에서 마무리했다.
- 커밋 메시지: `feat: add full game flow ui loop`
- 변경 파일: `PassportUnity/Assets/Scripts/Runtime/CharacterLaneRunner.cs`, `PassportUnity/Assets/Scripts/Runtime/GameplayHudReferences.cs`, `PassportUnity/Assets/Scripts/Runtime/GameResultType.cs`, `PassportUnity/Assets/Scripts/Runtime/GameFlowUiReferences.cs`, `PassportUnity/Assets/Scripts/Runtime/FinishTriggerRelay.cs`, `PassportUnity/Assets/Scripts/Runtime/GameplayFlowUiActions.cs`, `PassportUnity/Assets/Scripts/Runtime/GameplayFlowManager.cs`, `PassportUnity/Assets/Scripts/Editor/Task06GameFlowBuilder.cs`, `PassportUnity/Assets/Scenes/SampleScene.unity`, `Task/06-게임흐름-UI-결과.md`, `agent.md`
- 메모: `GameplayFlowManager`는 포즈 준비 완료 후 자동 시작, 3초 카운트다운, 타이머 종료, 체력 소진 종료, 결승선 도착 종료, 카메라 장시간 인식 실패 종료를 모두 처리한다. `GameFlowUiReferences`와 `GameplayFlowUiActions`로 결과 패널 버튼과 각 상태 패널을 연결하고, 최고 점수는 `PlayerPrefs`에 저장한다.
