# Task 04: Character Runner Core

## Goal

Turn motion commands into an actual 3-lane auto-runner character.

## Scope

- forward movement
- lane changes
- jump
- follow camera
- pause interaction

## Checklist

- [ ] Implement constant forward movement along the world `Z` axis
- [ ] Define the three lane positions
- [ ] Move left and right one lane at a time
- [ ] Prevent movement outside the lane bounds
- [ ] Implement jump with a short cooldown
- [ ] Keep forward motion active during lane changes and jump
- [ ] Stop motion during pause and end states
- [ ] Smoothly follow the runner with the main camera

## Deliverables

- Playable runner movement core
- Camera follow behavior
- Motion-command to character-action wiring

## Inspector Notes

- Forward speed
- Lane spacing
- Lane change duration
- Jump height and jump duration
- Camera offset and smoothing

## Test Checklist

- [ ] Character starts in the center lane
- [ ] Left and right move exactly one lane
- [ ] Character never leaves the 3-lane bounds
- [ ] Jump can clear low obstacles once they exist
- [ ] Pause freezes movement cleanly
- [ ] Camera remains readable during jump and lane change

## Open Questions

- Whether movement uses transform lerp, controller, or rigidbody
- Desired animation placeholders before final assets

## Progress Log

- Status: Not started
- Owner: Main agent / sub-agents as assigned
