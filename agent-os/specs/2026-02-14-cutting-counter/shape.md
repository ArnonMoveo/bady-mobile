# Cutting Counter — Shape

## Scope

Add the CuttingCounter — the second stage of the core loop: Spawn > **Chop** > Cook > Plate > Deliver. Players place a cuttable item on the counter, then press InteractAlternate repeatedly to chop it. After enough chops, the item is destroyed and replaced with the output item.

## Decisions

- **DRY refactor first:** Extract `TryGetPlayerParent()` and `TransferKitchenObject()` into BaseCounter so every counter type can reuse them.
- **CuttingRecipeSO as separate SO:** Not all items are cuttable. A dedicated SO cleanly separates cutting data from KitchenObjectSO.
- **Per-press cutting (Overcooked-style):** Each InteractAlternate press increments a counter. No hold-to-chop.
- **NetworkVariable<int> for progress:** All clients see cut progress without extra RPCs.
- **Recipe caching on placement:** CuttingCounter caches the matching recipe when an item is placed, avoiding repeated lookups on every cut.
- **Virtual SetKitchenObject/ClearKitchenObject:** Required for CuttingCounter's recipe cache to stay in sync via interface dispatch.

## Context

- Counter system (BaseCounter, ClearCounter, KitchenObject, IKitchenObjectParent) is implemented and compiling.
- ClearCounter provides the template for the pickup/drop RPC pattern.
- Unity 6 (6000.3.8f1), Netcode for GameObjects 2.8.0, `[Rpc(SendTo.Server)]` syntax.
