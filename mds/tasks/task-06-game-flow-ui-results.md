# Task 06: Game Flow, UI, And Results

## Goal

Finish the full playable loop from recognition check to result panel.

## Scope

- start guide
- ready check
- countdown
- timer
- pause
- camera failure handling
- result panel
- high score save

## Checklist

- [ ] Build the pre-start posture guidance flow
- [ ] Confirm recognition-ready state before starting
- [ ] Add start confirmation and auto-start fallback
- [ ] Implement countdown and play-state transitions
- [ ] Show score, high score, timer, combo, and judge feedback
- [ ] Implement pause and resume via hands-together motion
- [ ] Handle short, medium, and long camera recognition failure
- [ ] End the game on success, health failure, timeout, or unrecovered camera loss
- [ ] Show result panel inside the same `Game Scene`
- [ ] Save and reload high score with `PlayerPrefs`

## Deliverables

- A complete scene-level player loop
- UI for gameplay, camera state, pause, and results
- Success and failure handling

## Inspector Notes

- UI text references
- Countdown timing
- Match timer values
- Camera failure thresholds
- Result panel button bindings

## Test Checklist

- [ ] Ready state only passes when required landmarks are stable
- [ ] Countdown appears before gameplay begins
- [ ] Pause and resume return to a valid playing state
- [ ] Camera-loss warning escalates correctly
- [ ] Result panel shows the correct outcome and score
- [ ] Retry restarts the run cleanly
- [ ] High score persists between sessions

## Failure Thresholds

- Recognition ready hold: 2 seconds
- Short warning: under 1 second
- Auto-pause threshold: about 3 seconds
- Forced end threshold: about 15 seconds
- Hard time limit: 70 to 90 seconds

## Progress Log

- Status: Not started
- Owner: Main agent / sub-agents as assigned
