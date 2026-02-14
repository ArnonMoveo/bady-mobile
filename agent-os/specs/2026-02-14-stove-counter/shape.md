# Stove Counter — Shape

## Scope

Add the StoveCounter — the third stage of the core loop: Spawn > Chop > **Cook** > Plate > Deliver. Players place a cuttable/raw item on the stove and it automatically begins frying. After frying completes, the item transforms to its cooked variant. If left too long, it burns.

## Decisions

- **Auto-cook on placement (Overcooked-style):** No InteractAlternate needed. Cooking starts when a valid item is placed.
- **Two-stage transformation:** Raw > Cooked > Burned. The burn phase creates time pressure — core Overcooked tension.
- **Two separate SOs (FryingRecipeSO + BurningRecipeSO):** Matches the CuttingRecipeSO pattern. Clean separation of frying and burning data.
- **Server-only timer in Update():** Server ticks `Time.deltaTime`, clients read via NetworkVariable OnValueChanged.
- **No SetKitchenObject/ClearKitchenObject overrides:** All state managed explicitly in InteractServerRpc and Update(). Avoids reentrant issues during DestroySelf/SpawnKitchenObject transition.
- **Reject non-cookable items:** Stove only accepts items with a matching recipe SO.
- **Resume burning on re-placement:** A cooked item placed back on the stove resumes the burning phase.

## Context

- Counter system (BaseCounter, ClearCounter, CuttingCounter, KitchenObject, IKitchenObjectParent) is implemented and compiling.
- CuttingCounter provides the template for events (OnProgressChanged, OnAnyCut) and recipe caching.
- Unity 6 (6000.3.8f1), Netcode for GameObjects 2.8.0, `[Rpc(SendTo.Server)]` syntax.
