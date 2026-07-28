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
