# AGENTS

## Project Summary

- Project name: `Rhythm Passport`
- Goal: build a seated webcam-controlled Unity runner game for older adults and players with mobility limitations.
- Current target: complete only the first playable `Game Scene` prototype.
- Core fantasy: travel through Seoul toward Gyeongbokgung while avoiding obstacles and collecting travel items.

## Source Of Truth

- Primary planning document: `Plan/1차 프로젝트 기획서`
- Sub-agent request and role note: `mds/0. AGENTS요청.md`
- Change log: `agent.md`
- Unity project root: `PassportUnity/`

## Working Rules

- Work phase by phase. Do not build the whole system at once.
- Prefer functional prototypes before visual polish.
- Do not add extra scenes or systems unless the user asks for them.
- Keep gameplay input focused on webcam motion; debug keyboard input must stay optional and separate.
- Before adding a new script, check whether an existing script already covers the same responsibility.
- Keep generated Unity files out of tracked work.

## Tracked Areas

- `PassportUnity/Assets`
- `PassportUnity/Packages`
- `PassportUnity/ProjectSettings`
- `mds/`
- `AGENTS.md`
- `agent.md`

## Avoid Tracking

- `PassportUnity/Library`
- `PassportUnity/Temp`
- `PassportUnity/Logs`
- `PassportUnity/UserSettings`
- Generated IDE files such as `*.csproj`, `*.sln`, `*.slnx`

## Delivery Expectations Per Task

After each meaningful implementation step, report:

1. Completed work
2. Created files
3. Updated files
4. Unity Inspector setup
5. Test steps
6. Remaining work
7. Risks or issues

## Active Task Order

1. `mds/tasks/task-01-game-scene-foundation.md`
2. `mds/tasks/task-02-webcam-and-pose-pipeline.md`
3. `mds/tasks/task-03-motion-detection.md`
4. `mds/tasks/task-04-character-runner-core.md`
5. `mds/tasks/task-05-spawn-score-health.md`
6. `mds/tasks/task-06-game-flow-ui-results.md`

## Phase Strategy

- Task 01 sets the playable scene foundation.
- Task 02 establishes webcam input and pose landmarks.
- Task 03 turns pose data into robust gameplay actions.
- Task 04 connects actions to runner movement.
- Task 05 adds core challenge, reward, and game-state systems.
- Task 06 finishes the player loop from start guide to result panel.

## Gameplay Clarifications

- The runner moves forward through the world; obstacles and items stay fixed in world space after spawning.
- Spawning should happen ahead of the character, and cleanup should happen after objects fall behind the character.
- `Finish Trigger` arrival is the primary success condition for the first prototype.
- The base play target is about 60 seconds, with a separate hard fail-safe time limit in the 70 to 90 second range.
- Combo behavior should be implemented with an explicit rule set during Task 05 because the current planning note is ambiguous.
