# Standards Applied: Counter System

## csharp-style.md
- `_camelCase` private fields, `PascalCase` public
- `[SerializeField] private` — never public fields
- `sealed` on leaf classes, `abstract` on BaseCounter
- XML docs on interfaces and non-obvious public API
- File order: serialized fields → private fields → properties → lifecycle → public → private

## netcode.md
- Server-authoritative validation via ServerRpc
- `NetworkObject.TrySetParent()` for reparenting
- `NetworkObject.Spawn()`/`Despawn()` for lifecycle
- Never transfer ownership of interactables
- `RequireOwnership = false` on counter RPCs (counters are server-owned)
- No `GameObject`/`Transform` in RPCs

## performance.md
- Selection visual is event-driven (no Update polling)
- `for` loops, no `foreach` on arrays, no LINQ
- Zero allocations in hot paths

## architecture.md
- `[CreateAssetMenu]` for KitchenObjectSO
- `[SerializeField]` wiring, no `FindObjectOfType`
- Singleton pattern for player LocalInstance

## project-structure.md
- Kitchen scripts in `Scripts/Kitchen/` (namespace `Bady.Kitchen`)
- Shared interfaces in `Scripts/Core/` (namespace `Bady.Core`)
