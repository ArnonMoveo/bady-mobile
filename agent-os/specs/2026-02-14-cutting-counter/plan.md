# Plan: CuttingCounter — Chopping Mechanic (with Architectural Refactors)

## Context

The Counter System (BaseCounter, ClearCounter, KitchenObject, IKitchenObjectParent) is implemented and compiling in Unity 6. The next step in the MVP core loop is **Chop** — the second stage of Spawn > **Chop** > Cook > Plate > Deliver.

Before building CuttingCounter, we refactor BaseCounter to extract shared logic that would otherwise be duplicated across every counter type.

## Key Decisions

- **DRY: Extract shared counter helpers into BaseCounter** — The RPC player-resolution boilerplate and pickup/drop swap logic repeat in every counter. Extract `TryGetPlayerParent()` and `TransferKitchenObject()` as `protected` methods on BaseCounter.
- **Helpers are called from ServerRpc, not Interact()** — Swap logic must run on the server. `Interact()` only fires the RPC; the helpers are used inside each counter's ServerRpc.
- **Cache recipe on placement** — CuttingCounter caches the matching `CuttingRecipeSO` when an item is placed, avoiding repeated array lookups on every cut press. Cache is cleared when item is removed.
- **CuttingRecipeSO** (separate SO) — cleaner separation, not all items are cuttable
- **Per-press cutting** (Overcooked-style) — each InteractAlternate press increments cut count
- **NetworkVariable<int> for cut progress** — all clients see progress without extra RPCs

## Files Modified/Created

| File | Action | Purpose |
|------|--------|---------|
| `Scripts/Kitchen/BaseCounter.cs` | Modified | Added `TryGetPlayerParent()` + `TransferKitchenObject()` helpers, made `SetKitchenObject`/`ClearKitchenObject` virtual |
| `Scripts/Kitchen/ClearCounter.cs` | Modified | Simplified ServerRpc to use base helpers |
| `Scripts/Kitchen/CuttingRecipeSO.cs` | Created | SO mapping input -> output + cut count |
| `Scripts/Kitchen/CuttingCounter.cs` | Created | Sealed counter with cutting, recipe caching |

## Dependency Order

```
Task 1 (Specs)          -- no deps
Task 2 (BaseCounter)    -- no deps
Task 3 (ClearCounter)   -- depends on Task 2
Task 4 (CuttingRecipeSO) -- no deps
Task 5 (CuttingCounter) -- depends on Tasks 2, 4
```
