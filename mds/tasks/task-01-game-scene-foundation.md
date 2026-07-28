# Task 01: Game Scene Foundation

## Goal

Build the single playable `Game Scene` foundation for the first prototype.

## Scope

- Scene structure
- World layout
- lane anchors
- camera follow baseline
- temporary finish area

## Checklist

- [ ] Create or confirm a single main `Game Scene`
- [ ] Define `Environment`, `Character`, `Managers`, and `Canvas` root structure
- [ ] Set up a 3-lane road layout with left, center, and right lane points
- [ ] Place a temporary destination landmark and `Finish Trigger`
- [ ] Define forward direction and world-scale assumptions
- [ ] Add a basic third-person follow camera baseline
- [ ] Document required Inspector references

## Deliverables

- A stable scene hierarchy for the prototype
- Clear lane reference points
- A testable forward path from start to finish

## Inspector Notes

- Lane transforms
- Finish trigger collider
- Camera follow target
- Character start transform

## Test Checklist

- [ ] Scene opens without missing references
- [ ] Character start position is centered
- [ ] Lane markers match visible road positions
- [ ] Finish trigger is reachable in the scene
- [ ] Camera points in the correct forward direction

## Open Questions

- Exact road length for the first playable pass
- Whether placeholder art will be primitive-based or imported early

## Progress Log

- Status: Not started
- Owner: Main agent / sub-agents as assigned
