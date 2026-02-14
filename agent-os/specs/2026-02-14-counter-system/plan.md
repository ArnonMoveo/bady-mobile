# Plan: Counter System — BaseCounter + ClearCounter + KitchenObject

## Context

BADY Mobile's Player Controller is implemented (GameInput, PlayerController, IInteractable). The next step in the MVP core loop (Spawn → Chop → Cook → Plate → Deliver) is the Counter System — the interactable surfaces players work with.

## Scope

- BaseCounter (abstract) with IInteractable + IKitchenObjectParent
- ClearCounter (concrete) — pick up / place down items
- KitchenObject — NetworkBehaviour item that moves between parents
- KitchenObjectSO — ScriptableObject data definition
- IKitchenObjectParent — shared interface for counters and players
- SelectedCounterVisual — client-side highlight
- PlayerController modifications — IKitchenObjectParent + LocalInstance

## Key Decisions

- Client checks `HasKitchenObject()` locally before sending ServerRpc; server validates anyway
- BaseCounter is abstract, designed for inheritance
- KitchenObject manages its own parenting via `SetKitchenObjectParent()`
- IKitchenObjectParent unifies counters and players
- Single InteractServerRpc per counter, no NetworkObjectReference in RPCs
- Counter ServerRpcs use `RequireOwnership = false`
- Atomic parenting: clear old parent BEFORE reassigning `_kitchenObjectParent`

## Files

| File | Action |
|------|--------|
| `Scripts/Kitchen/KitchenObjectSO.cs` | Create |
| `Scripts/Core/IKitchenObjectParent.cs` | Create |
| `Scripts/Kitchen/KitchenObject.cs` | Create |
| `Scripts/Kitchen/BaseCounter.cs` | Create |
| `Scripts/Kitchen/ClearCounter.cs` | Create |
| `Scripts/Kitchen/SelectedCounterVisual.cs` | Create |
| `Scripts/Player/PlayerController.cs` | Modify |
