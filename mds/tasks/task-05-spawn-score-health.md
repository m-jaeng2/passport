# Task 05: Spawn, Score, And Health

## Goal

Add obstacles, items, and the systems that make the run meaningful.

## Scope

- obstacle spawn
- item spawn
- collision
- score
- combo
- health
- cleanup and pooling

## Checklist

- [ ] Create lane-based obstacle spawn rules
- [ ] Create lane-based item spawn rules
- [ ] Prevent impossible obstacle patterns
- [ ] Implement `Fence`, `Pedestrian`, `Barrier`, and `RedSignalZone`
- [ ] Implement `Heart`, `Snack`, and `GoldenPassport`
- [ ] Apply obstacle damage and item rewards correctly
- [ ] Track score, combo, and health
- [ ] Remove or recycle passed objects
- [ ] Add object pooling for reusable gameplay objects

## Deliverables

- Spawn manager with safe pattern rules
- Obstacle and item behaviors
- Working score, combo, and health loop

## Inspector Notes

- Spawn interval ranges
- Spawn distance ahead of character
- Despawn distance behind character
- Obstacle and item data assets

## Test Checklist

- [ ] Obstacles appear only in valid lanes
- [ ] At least one avoidable path remains
- [ ] Item pickups apply the correct effect
- [ ] Damage values match the design
- [ ] Combo rises and resets in the intended cases
- [ ] Old spawned objects are cleaned up correctly

## Balancing Targets

- First obstacle delay: 5 seconds
- Base play duration target: about 60 seconds
- Golden passport frequency: very low

## Unresolved Issues

- Exact spawn pacing curve over the match
- Whether score feedback timing needs `Perfect/Good/Miss` grading rules now or later

## Progress Log

- Status: Not started
- Owner: Main agent / sub-agents as assigned
