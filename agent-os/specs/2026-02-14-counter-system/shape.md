# Shape: Counter System

## Problem

Players need interactable surfaces to place and pick up kitchen objects. The core loop requires counters as the fundamental work surfaces.

## Appetite

Small batch — foundational system that unblocks all kitchen gameplay.

## Solution

### Counter Architecture
- Abstract `BaseCounter` provides shared behavior (IInteractable, IKitchenObjectParent)
- Concrete counters (ClearCounter, CuttingCounter, StoveCounter) inherit and override
- Single ServerRpc per counter type — server determines action from its own state

### Kitchen Object System
- `KitchenObject` is a NetworkBehaviour that manages its own parenting
- `IKitchenObjectParent` interface unifies counters and players
- `SetKitchenObjectParent()` centralizes reparenting: clear old → assign new → network parent
- **Critical ordering:** Must clear old parent BEFORE reassigning field to avoid duplication

### Selection Visual
- Event-driven (no Update polling) — subscribes to `OnSelectedInteractableChanged`
- Handles spawn timing via `OnLocalInstanceSet` fallback

## Rabbit Holes

- Don't implement recipe validation yet — just pick up / place down
- Don't add ingredient combining — that's PlateKitchenObject scope
- Don't network the selection visual — it's purely local

## No-Gos

- No ownership transfer on interactables (server owns all counters)
- No NetworkObjectReference in RPCs (server checks its own state)
- No Update() polling for selection state
