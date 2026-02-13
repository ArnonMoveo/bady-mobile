# BADY Mobile — Shaping Notes

## What

Initial Unity project scaffolding for a mobile co-op cooking game (Overcooked-style). No game logic — just folder structure, packages, and assembly definitions.

## Scope

- **In scope:** Directory hierarchy, manifest.json, .asmdef, .gitignore
- **Out of scope:** Game logic, scenes, prefabs, art assets, networking implementation

## Key Decisions

1. **Single root assembly definition** (`Bady.asmdef`) rather than per-subfolder asmdefs. Simpler for MVP; can split later if compile times grow.
2. **URP** chosen over built-in or HDRP — best fit for mobile performance.
3. **Netcode for GameObjects (NGO)** for multiplayer — Unity's first-party solution, well-integrated.
4. **New Input System** — required for modern mobile input handling (touch, virtual joystick).
5. **ProjectSettings left empty** — Unity Hub auto-generates correct defaults on first open. Hand-writing .asset files is fragile and error-prone.
6. **Unity 6 / 2022.3 LTS** target — stable, long-term support.

## Context

- Target platform: iOS + Android
- Multiplayer: 2-4 player co-op
- Art style: TBD (stylized/cartoon likely)
- Repo: `/Users/arnon.meltser/bady-mobile`
