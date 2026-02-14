# Plan: StoveCounter — Cook Mechanic (Fry + Burn)

## Context

The Counter System (BaseCounter, ClearCounter, CuttingCounter) is implemented and compiling. The next MVP core loop stage is **Cook** — the third stage of Spawn > Chop > **Cook** > Plate > Deliver. The roadmap specifies Fry/Boil cooking with 3 recipes (Classic Burger, Spicy Ramen, Towering Pizza).

## Key Decisions

- **Auto-cook on placement** — Cooking starts automatically when a cookable item is placed (Overcooked-style). No InteractAlternate needed.
- **Two-stage transformation** — Raw > Cooked (frying) > Burned (if left too long). Core Overcooked tension mechanic.
- **Two separate SOs** — `FryingRecipeSO` (input>output + fryTime) and `BurningRecipeSO` (input>burnedOutput + burnTime). Matches CuttingRecipeSO pattern.
- **Server-only timer in Update()** — Server ticks timer each frame. Clients read progress via `NetworkVariable<float>` OnValueChanged callbacks.
- **No SetKitchenObject/ClearKitchenObject overrides** — All state managed explicitly in InteractServerRpc and Update(). Avoids reentrant callback issues during DestroySelf/SpawnKitchenObject transition.
- **Reject non-cookable items** — Stove only accepts items with a matching FryingRecipeSO or BurningRecipeSO.
- **NetworkVariable for timer max** — `_fryingTimerMax` and `_burningTimerMax` synced to clients for normalized progress.

## State Machine

```
enum State { Idle, Frying, Fried, Burning, Burned }
```

- `Idle` — No item on stove, or just reset after pickup.
- `Frying` — Raw item placed, timer ticking toward cooked.
- `Fried` — Cooked item spawned. Transient (>Burning) if BurningRecipeSO exists, terminal if not.
- `Burning` — Cooked item on stove, burn timer ticking.
- `Burned` — Burned item spawned. Terminal, player can still pick up.

## Files Created

| File | Purpose |
|------|---------|
| `Scripts/Kitchen/FryingRecipeSO.cs` | SO: input > cooked output + fry time |
| `Scripts/Kitchen/BurningRecipeSO.cs` | SO: cooked input > burned output + burn time |
| `Scripts/Kitchen/StoveCounter.cs` | Sealed counter with state machine, timers, RPCs |
| `Scripts/Kitchen/StoveCounterVisual.cs` | Client-side visual for sizzle particles + stove glow |

## Dependency Order

```
Task 1 (Specs)              -- no deps
Task 2 (FryingRecipeSO)     -- no deps
Task 3 (BurningRecipeSO)    -- no deps
Task 4 (StoveCounter)       -- depends on Tasks 2, 3
Task 5 (StoveCounterVisual) -- depends on Task 4
```
