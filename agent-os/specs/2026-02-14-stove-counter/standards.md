# Stove Counter — Relevant Standards

## C# Style (csharp-style.md)
- `_camelCase` private fields with `[SerializeField]`
- `PascalCase` public methods/properties
- `sealed` unless designed for inheritance
- `/// <summary>` on public members where name alone doesn't convey behavior
- File order: serialized fields, private fields, properties, lifecycle, public methods, private methods
- Group serialized fields with `[Header("Section Name")]`

## Architecture (architecture.md)
- ScriptableObjects for data definitions only (FryingRecipeSO, BurningRecipeSO)
- `[CreateAssetMenu]` for Inspector workflow
- Wire dependencies via `[SerializeField]`
- State machine with explicit enum states

## Netcode (netcode.md)
- Server-authoritative: clients request via `[Rpc(SendTo.Server)]`, server validates and executes
- `NetworkVariable<float>` for persistent state (timers) — server write, everyone read
- `NetworkVariable<State>` for cooking state — server write, everyone read
- Subscribe to `OnValueChanged` for client-side reactions (UI, VFX)
- Never pass GameObject/Transform in RPCs
- Don't forget `base.OnNetworkSpawn()` when overriding

## Performance (performance.md)
- No LINQ at runtime — `for` loops for recipe lookup
- Zero allocations in Update() — only primitive operations in hot path
- Cache references (recipe caching pattern)
- Reuse EventArgs instances to avoid allocation per event fire
