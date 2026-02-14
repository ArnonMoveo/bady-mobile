# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

BADY Mobile is a Unity 6 mobile co-op cooking game (Overcooked-style) for iOS and Android. 2-4 players connect via local Wi-Fi in a client-hosted topology. Currently in MVP scaffolding phase.

## Opening the Project

Open the `bady-mobile/` folder in Unity Hub. Unity auto-downloads packages from `Packages/manifest.json` on first open. ProjectSettings are auto-generated.

## Key Dependencies

- Unity 6 (6000.3.8f1) with URP (17.3.0)
- Netcode for GameObjects (2.8.0) — multiplayer framework
- Unity Input System (1.18.0) — touch controls
- Unity Transport (2.6.0) — network transport
- TextMeshPro — bundled in UGUI 2.0.0 (no separate package)

## Repository Structure

- `Assets/_Game/` — all game assets and scripts (prefixed `_` to sort to top)
- `Assets/_Game/Scripts/` — C# code, organized by domain (Core, Kitchen, Input, Network, Player, UI, Audio)
- `Assets/_Game/Scripts/Bady.asmdef` — root assembly definition referencing Netcode, InputSystem, TextMeshPro
- `agent-os/product/` — mission, tech stack, and roadmap docs
- `agent-os/standards/unity/` — 5 coding standards (must follow these)
- `agent-os/specs/` — timestamped feature specs with shaping notes

## Coding Standards

**Read and follow** `agent-os/standards/unity/` before writing any code:

- `csharp-style.md` — naming (`_camelCase` private fields, `PascalCase` public), serialization, file organization
- `architecture.md` — singletons for managers, Bootstrap scene pattern, FSM for player states, ScriptableObjects for data only
- `project-structure.md` — folder layout, asset naming conventions (`_SO` suffix for SO assets)
- `netcode.md` — server-authoritative model, RPC patterns, NetworkVariable usage, no ownership transfer on interactables
- `performance.md` — zero-alloc hot paths, object pooling, canvas splitting, string hash caching

## Architecture Quick Reference

- **Namespace root:** `Bady` with sub-namespaces matching folders (`Bady.Core`, `Bady.Kitchen`, etc.)
- **Bootstrap scene:** `_Bootstrap` loads all persistent managers (`DontDestroyOnLoad`), then transitions to game scenes
- **Scene flow:** `_Bootstrap → MainMenu → Lobby → Kitchen_Level01`
- **State machines:** Player states (`Idle`, `Moving`, `Carrying`, `Cooking`, `Interacting`) use `IState` with `Enter()/Execute()/Exit()`
- **Netcode:** Server-authoritative. Clients send `ServerRpc` requests, server validates and responds via `ClientRpc` or `NetworkVariable`. Use `NetworkObjectReference` in RPCs, never `GameObject`/`Transform`
- **Spawning:** Server only, via `NetworkObject.Spawn()` — never `Instantiate()` for networked objects
- **Dependencies:** Wire via `[SerializeField]` or `Awake()` lookups. Access managers via singleton `Instance`. Avoid `FindObjectOfType`

## Critical Performance Rules

- Target 60fps. Zero allocations in `Update()`/`FixedUpdate()`
- Use `NonAlloc` physics, cache collections, avoid LINQ at runtime
- Pool frequently spawned objects with `ObjectPool<T>`
- Cache animator/shader hashes in `static readonly` fields
- Split dynamic UI (timers, scores) onto separate Canvas from static UI
- Build with IL2CPP, profile on-device not in editor
