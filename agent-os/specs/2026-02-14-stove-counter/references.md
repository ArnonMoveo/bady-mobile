# Stove Counter — References

## Foundation: CuttingCounter

CuttingCounter provides the closest template for StoveCounter's event and recipe patterns.

### Key patterns carried forward:
- `Interact()` fires an `[Rpc(SendTo.Server)]` — no logic on client
- ServerRpc resolves calling player via `TryGetPlayerParent()` helper
- Events: `OnProgressChanged` (instance, for UI) and `OnAnyCut` (static, for SFX)
- Recipe caching to avoid repeated array lookups
- `sealed` class extending `BaseCounter`
- Reusable EventArgs instance to avoid per-fire allocations

### What StoveCounter changes:
- Auto-cook on placement (no InteractAlternate)
- Server-side `Update()` timer instead of per-press increments
- Two-stage transformation (Frying > Burning) with two recipe SO types
- `NetworkVariable<float>` for timers instead of `NetworkVariable<int>` for progress
- `NetworkVariable<State>` enum for state machine sync
- No SetKitchenObject/ClearKitchenObject overrides — explicit state management
- StoveCounterVisual as separate MonoBehaviour for particles/glow

## Key Files
- `Assets/_Game/Scripts/Kitchen/BaseCounter.cs` — abstract base with IKitchenObjectParent, TryGetPlayerParent, TransferKitchenObject
- `Assets/_Game/Scripts/Kitchen/CuttingCounter.cs` — event and recipe caching template
- `Assets/_Game/Scripts/Kitchen/CuttingRecipeSO.cs` — SO pattern template
- `Assets/_Game/Scripts/Kitchen/KitchenObject.cs` — SetKitchenObjectParent(), DestroySelf(), SpawnKitchenObject()
- `Assets/_Game/Scripts/Core/IKitchenObjectParent.cs` — shared interface
