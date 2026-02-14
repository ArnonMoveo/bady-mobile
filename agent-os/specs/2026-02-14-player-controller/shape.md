# Shape: Player Controller (GameInput + PlayerController)

## Scope

First feature implementation for BADY Mobile MVP. Creates the core player controller with input handling, physics-based movement, interaction detection, and procedural tilt.

## Key Decisions

1. **Owner-authoritative movement** — Prioritize snappy feel over anti-cheat for local Wi-Fi co-op MVP. Players move on their own client; NetworkTransform syncs to others.
2. **Include interaction detection** — `Physics.BoxCast` with `Debug.DrawRay` visualization. Defer visual highlight until Counters are built.
3. **Defer FSM** — Keep PlayerController simple for now. Add state machine when more player states are needed (Carrying, Cooking, etc.).
4. **Procedural tilt** — Lean character visual in movement direction for juice. Simple lerp-based, no animation needed.
5. **Defer squash animation** — Needs tweening library (DoTween/PrimeTween) and character models. Not in scope for this spec.
6. **Physics-based movement** — Use Rigidbody + MovePosition instead of CharacterController. Enables future physics interactions (conveyor belts, ice, push mechanics).

## Context

- Project is in MVP scaffolding phase — zero C# code exists
- Assembly definition, packages, and folder structure are all in place
- This is the foundational feature that all subsequent features build on
- Touch input (virtual joystick) is deferred — using WASD + gamepad for editor testing
