# Task 03: Motion Detection

## Goal

Convert landmark data into reliable gameplay motion commands.

## Scope

- neutral pose
- left hand up
- right hand up
- both hands up
- hands together
- input lock and reset logic

## Checklist

- [ ] Define neutral pose conditions
- [ ] Implement `LeftHandUp` detection
- [ ] Implement `RightHandUp` detection
- [ ] Implement `BothHandsUp` detection
- [ ] Implement `HandsTogether` detection
- [ ] Add single-fire input locking
- [ ] Require neutral-pose reset before the next command
- [ ] Add cooldowns for jump and pause actions

## Deliverables

- Motion state enum and detection pipeline
- Reusable thresholds for posture decisions
- Debug-friendly output of current detected action

## Inspector Notes

- Shoulder-relative thresholds
- Wrist distance thresholds
- Hold duration for pause gesture
- Cooldown timings

## Test Checklist

- [ ] One gesture produces one action event
- [ ] Repeated held poses do not spam input
- [ ] Wrong-hand movement does not trigger the opposite action
- [ ] Jump gesture works without requiring a full overhead reach
- [ ] Pause gesture only triggers after the hold duration

## Threshold Tuning

- Left/right hand raise relative to shoulder height: TBD
- Outward hand distance threshold: TBD
- Hands together hold duration: 0.7 seconds
- Pause input lock after trigger: 1 second

## Unresolved Issues

- False positives caused by resting-arm asymmetry
- Behavior when one wrist momentarily disappears

## Progress Log

- Status: Not started
- Owner: Main agent / sub-agents as assigned
