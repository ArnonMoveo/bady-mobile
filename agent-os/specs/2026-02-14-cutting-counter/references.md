# Cutting Counter — References

## Foundation: ClearCounter

ClearCounter is the simplest counter type and serves as the template for CuttingCounter's pickup/drop logic.

### Key patterns carried forward:
- `Interact()` fires an `[Rpc(SendTo.Server)]` — no logic on client
- ServerRpc resolves the calling player via `RpcParams.Receive.SenderClientId`
- Swap logic: counter has item + player empty = give to player; player has item + counter empty = take from player
- `sealed` class extending `BaseCounter`

### What CuttingCounter adds:
- `InteractAlternate()` for cutting (second RPC)
- `NetworkVariable<int>` for cut progress
- Recipe caching via virtual `SetKitchenObject`/`ClearKitchenObject` overrides
- Events for UI progress bar and global SFX

## Key Files
- `Assets/_Game/Scripts/Kitchen/BaseCounter.cs` — abstract base with IKitchenObjectParent
- `Assets/_Game/Scripts/Kitchen/ClearCounter.cs` — pickup/drop template
- `Assets/_Game/Scripts/Kitchen/KitchenObject.cs` — SetKitchenObjectParent() calls interface methods
- `Assets/_Game/Scripts/Core/IKitchenObjectParent.cs` — shared interface
