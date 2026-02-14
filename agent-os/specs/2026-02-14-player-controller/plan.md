# Plan: Player Controller — GameInput + PlayerController

## Files to Create

| File | Location | Purpose |
|------|----------|---------|
| `PlayerInputActions.inputactions` | `Assets/_Game/Settings/` | Input Actions asset (Movement Vector2 + Interact + InteractAlternate buttons) |
| `GameInput.cs` | `Assets/_Game/Scripts/Input/` | Singleton wrapper for input; exposes movement, interact, and interact-alternate events |
| `PlayerController.cs` | `Assets/_Game/Scripts/Player/` | NetworkBehaviour: movement, rotation, interaction detection, procedural tilt |
| `IInteractable.cs` | `Assets/_Game/Scripts/Core/` | Stub interface for interactable objects |

## Task Breakdown

### Task 1: Input Actions Asset
- Create `PlayerInputActions.inputactions` Unity Input System JSON
- Action Map: `Player`
- Actions: Move (Value/Vector2), Interact (Button), InteractAlternate (Button)
- Bindings: WASD composite + Gamepad left stick, E + Gamepad south, F + Gamepad west

### Task 2: GameInput Singleton
- Namespace: `Bady.Input`
- Singleton with DontDestroyOnLoad
- Wraps PlayerInputActions generated class
- Public: `GetMovementVectorNormalized()`, interact events
- Cleanup in OnDestroy

### Task 3: IInteractable Interface
- Namespace: `Bady.Core`
- Methods: `Interact(PlayerController)`, `InteractAlternate(PlayerController)`
- XML docs on interface and methods

### Task 4: PlayerController
- Namespace: `Bady.Player`
- NetworkBehaviour with IsOwner guard
- Rigidbody movement in FixedUpdate via MovePosition
- Rotation in Update via Quaternion.Slerp
- Interaction detection via BoxCastNonAlloc in Update
- Procedural tilt on child visual transform
- Pre-allocated arrays, zero-alloc hot paths

## Verification Checklist
- [ ] All scripts compile with Bady.asmdef references
- [ ] `[SerializeField] private` on all inspector fields
- [ ] `_camelCase` private, `PascalCase` public
- [ ] XML docs on non-obvious public members
- [ ] `IsOwner` guard on input processing
- [ ] Zero-alloc Update (NonAlloc physics, pre-allocated arrays)
- [ ] Classes marked `sealed`
- [ ] Correct namespaces matching folder structure
- [ ] File organization follows standard order
