# Standards Applied: Player Controller

## csharp-style.md
- `_camelCase` private fields with `[SerializeField]`
- `PascalCase` public methods and properties
- `[Header]` grouping for serialized fields
- `sealed` classes unless designed for inheritance
- `/// <summary>` on non-obvious public members
- `Bady` root namespace with sub-namespaces matching folders

## architecture.md
- Singleton pattern for `GameInput` (Instance + null-check + DontDestroyOnLoad)
- `[SerializeField]` or `Awake()` for dependency wiring
- Access managers via singleton `Instance`

## netcode.md
- `NetworkBehaviour` base class for `PlayerController`
- `IsOwner` guard before processing input
- Owner-authoritative movement via `NetworkTransform`
- No ownership transfer on interactables

## performance.md
- Zero-alloc `Update()` and `FixedUpdate()`
- `Physics.BoxCastNonAlloc` for interaction detection
- Pre-allocated `RaycastHit[]` array as field
- No LINQ in runtime code
- Cache component references in `Awake()`

## project-structure.md
- Files in correct Script subfolders (Input/, Player/, Core/)
- Input Actions asset in Settings/
- One MonoBehaviour per file, filename matches class name
