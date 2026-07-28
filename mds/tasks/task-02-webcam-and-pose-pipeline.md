# Task 02: Webcam And Pose Pipeline

## Goal

Connect webcam input and expose the body landmarks needed for gameplay.

## Scope

- webcam connection
- preview rendering
- pose landmark acquisition
- recognition state reporting

## Checklist

- [ ] Start and stop the webcam safely
- [ ] Display webcam feed in the camera UI area
- [ ] Handle mirror mode and preview orientation correctly
- [ ] Acquire face center or nose, both shoulders, and both wrists
- [ ] Expose normalized landmark coordinates for gameplay logic
- [ ] Surface confidence and recognition-ready state
- [ ] Add clear handling for missing camera or unavailable input

## Deliverables

- Webcam preview in Unity
- Stable landmark data feed for core joints
- Recognition state usable by gameplay systems

## Inspector Notes

- Camera device selection
- Preview texture target
- Pose pipeline component references

## Test Checklist

- [ ] Webcam image appears in play mode
- [ ] Left and right landmarks are mapped consistently
- [ ] Face, shoulders, and wrists update live
- [ ] Temporary camera disconnect is handled gracefully
- [ ] Preview works at the expected seated distance

## Detection Thresholds

- Landmark confidence minimum: TBD
- Recognition ready duration: 2 seconds
- Camera loss short warning: under 1 second
- Camera loss pause threshold: around 3 seconds

## Open Questions

- Exact MediaPipe integration approach inside Unity
- Whether pose smoothing is handled in provider or game layer

## Progress Log

- Status: Not started
- Owner: Main agent / sub-agents as assigned
