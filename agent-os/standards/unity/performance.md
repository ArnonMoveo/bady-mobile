# Performance — Mobile Unity

## Garbage Collection

- **Zero-alloc in hot paths** — `Update()`, `FixedUpdate()`, input handlers must not allocate
- Cache collections — declare `List<T>` as a field, `.Clear()` and reuse
- Use `NonAlloc` physics variants: `Physics.OverlapSphereNonAlloc()`, `Physics.RaycastNonAlloc()`
- Avoid `foreach` on non-array collections in hot paths (use `for` with index)
- Avoid string concatenation in hot paths — use `StringBuilder` or `TMP_Text.SetText()` with format args
- Avoid LINQ in runtime code — OK in editor scripts and one-time setup

## Update Loops

- Minimize `Update()` usage — prefer event-driven patterns
- Use coroutines or timers for periodic checks instead of per-frame polling
- Move physics logic to `FixedUpdate()`
- Use `Time.deltaTime` in `Update()`, `Time.fixedDeltaTime` in `FixedUpdate()`

## Object Pooling

- Pool anything spawned/destroyed frequently: ingredients, VFX, UI popups, projectiles
- Use `ObjectPool<T>` from `UnityEngine.Pool` (built-in since 2021)
- Pool pattern: `Get()` to spawn, `Release()` to despawn — never `Destroy()`
- Pre-warm pools during loading screens, not during gameplay

```csharp
private ObjectPool<GameObject> _ingredientPool;

private void Awake()
{
    _ingredientPool = new ObjectPool<GameObject>(
        createFunc: () => Instantiate(_prefab),
        actionOnGet: obj => obj.SetActive(true),
        actionOnRelease: obj => obj.SetActive(false),
        defaultCapacity: 20
    );
}
```

## Rendering (Mobile)

- Target **60fps everywhere** (use `Application.targetFrameRate = 60`) — 30fps menus feel broken on modern devices
- Use URP with a single URP Asset configured for mobile (no HDR, no MSAA, or 2x max)
- Limit real-time lights — bake where possible, max 1-2 real-time lights per scene
- Use GPU instancing for repeated objects (ingredients, kitchen tiles)
- Atlas textures — reduce draw calls by combining small textures

## String Hashing

- **Never use string literals** in `Update()` for Animator or Shader properties
- Cache hashes in `static readonly` fields using `Animator.StringToHash()` and `Shader.PropertyToID()`

```csharp
private static readonly int RunHash = Animator.StringToHash("Run");
private static readonly int CookProgressId = Shader.PropertyToID("_CookProgress");

private void Update()
{
    _animator.SetBool(RunHash, _isMoving);       // Good
    // _animator.SetBool("Run", _isMoving);      // Bad — hashes string every frame
}
```

## UI / Canvas

- **Split dynamic and static UI onto separate Canvases** — any change to a Canvas element triggers a full mesh rebuild of that Canvas
- Frequently updating elements (timers, progress bars, score) go on a dedicated dynamic Canvas
- Static elements (backgrounds, labels, frames) go on a separate Canvas
- This prevents a timer ticking every frame from rebuilding 50 static elements

## General

- Profile on-device, not in editor — editor perf is misleading
- Use `Unity Profiler` and `Frame Debugger` before optimizing
- Set `IL2CPP` as scripting backend for builds (not Mono)
- Disable `Development Build` for performance testing
