# Agent Change Log

This file tracks the major project changes I make while we build.
For each meaningful code update, I will log the summary, proposed commit message, and changed files here.

## Log Format

### YYYY-MM-DD HH:MM
- Summary:
- Commit message:
- Changed files:
- Notes:

## Entries

### 2026-07-28 15:20
- Summary: Prepared the repository for Unity project uploads and ongoing change tracking.
- Commit message: `chore: add Unity gitignore and agent change log`
- Changed files: `.gitignore`, `agent.md`
- Notes: Local `git` is not available in this environment, so GitHub updates are done through the GitHub connector.

### 2026-07-28 15:35
- Summary: Added a working guide for sub-agents based on the current Unity project structure.
- Commit message: `docs: define sub-agent working rules`
- Changed files: `mds/0. AGENTS요청.md`, `agent.md`
- Notes: The guidance focuses on tracked Unity files, generated-file exclusions, logging, and commit message format.

### 2026-07-28 16:25
- Summary: Converted the project plan into an execution guide with a root AGENTS file and six step-by-step task trackers.
- Commit message: `docs: add project agents guide and phased task files`
- Changed files: `AGENTS.md`, `mds/tasks/task-01-game-scene-foundation.md`, `mds/tasks/task-02-webcam-and-pose-pipeline.md`, `mds/tasks/task-03-motion-detection.md`, `mds/tasks/task-04-character-runner-core.md`, `mds/tasks/task-05-spawn-score-health.md`, `mds/tasks/task-06-game-flow-ui-results.md`, `agent.md`
- Notes: The task split follows the planning document while keeping checklist, inspector, test, and open-issue sections ready for ongoing updates.

### 2026-07-28 16:30
- Summary: Clarified the gameplay document so future implementation follows world-fixed spawning and a single success-condition interpretation.
- Commit message: `docs: clarify gameplay rules in agents guide`
- Changed files: `AGENTS.md`, `agent.md`
- Notes: Added guardrails for world-space objects, finish-trigger success, time-limit interpretation, and combo-rule follow-up.
