# References: Counter System

## Foundation Code
- `Assets/_Game/Scripts/Player/PlayerController.cs` — Player controller with interaction detection
- `Assets/_Game/Scripts/Core/IInteractable.cs` — Interface for interactable objects
- `Assets/_Game/Scripts/Input/GameInput.cs` — Input system wrapper

## Design Reference
- Overcooked-style gameplay: counters are work surfaces where items are placed/picked up
- Players interact by facing a counter and pressing the interact button
- Items snap to counter top points and player hold points

## Netcode Pattern
- Server-authoritative: clients send requests, server validates state and executes
- Items are server-owned, attached to parents via `TrySetParent()`
- No ownership transfer — avoids race conditions on simultaneous grabs
