# Cutting Counter — Relevant Standards

## C# Style (csharp-style.md)
- `_camelCase` private fields with `[SerializeField]`
- `PascalCase` public methods/properties
- `sealed` unless designed for inheritance
- `/// <summary>` on public members where name alone doesn't convey behavior
- File order: serialized fields, private fields, properties, lifecycle, public methods, private methods

## Architecture (architecture.md)
- ScriptableObjects for data definitions only (CuttingRecipeSO)
- `[CreateAssetMenu]` for Inspector workflow
- Wire dependencies via `[SerializeField]`

## Netcode (netcode.md)
- Server-authoritative: clients request via `[Rpc(SendTo.Server)]`, server validates and executes
- `NetworkVariable<int>` for persistent state (cut progress) — server write, everyone read
- `[Rpc(SendTo.Everyone)]` for VFX/audio cues
- Never pass GameObject/Transform in RPCs

## Performance (performance.md)
- No LINQ at runtime — `for` loops for recipe lookup
- No allocations in Update — event-driven progress
- Cache references (recipe caching pattern)
