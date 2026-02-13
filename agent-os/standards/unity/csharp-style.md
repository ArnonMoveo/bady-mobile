# C# Style — Unity

## Naming

- **Classes / Structs / Enums:** `PascalCase` — `PlayerController`, `RecipeData`
- **Public methods / properties:** `PascalCase` — `GetIngredient()`, `IsCooked`
- **Private fields:** `_camelCase` with underscore prefix — `_moveSpeed`, `_currentRecipe`
- **Serialized private fields:** `_camelCase` with `[SerializeField]` — never make fields public just for the Inspector
- **Local variables / parameters:** `camelCase` — `spawnPoint`, `playerIndex`
- **Constants:** `PascalCase` — `MaxPlayers`, `DefaultCookTime`
- **Interfaces:** `I` prefix — `IInteractable`, `IPickupable`

## Serialization

- Always use `[SerializeField]` on private fields instead of making them public
- Group serialized fields with `[Header("Section Name")]`
- Use `[Tooltip("...")]` for non-obvious fields
- Use `[Range(min, max)]` for numeric fields with known bounds

```csharp
[Header("Movement")]
[SerializeField] private float _moveSpeed = 5f;
[SerializeField] private float _rotationSpeed = 720f;

[Header("Interaction")]
[SerializeField] private float _interactRadius = 1.5f;
[SerializeField] private LayerMask _interactableLayer;
```

## Namespaces

- All scripts must be wrapped in the root namespace `Bady`
- Sub-namespaces follow folder structure when needed later (`Bady.Core`, `Bady.Kitchen`, etc.)

```csharp
namespace Bady
{
    public sealed class PlayerController : MonoBehaviour
    {
        // ...
    }
}
```

## Documentation

- Use `/// <summary>` on public members where the name alone doesn't fully convey behavior or contract
- Always document interfaces and methods with side effects
- Skip for self-documenting properties, simple getters, and Unity lifecycle methods

```csharp
// Good — non-obvious behavior
/// <summary>
/// Attempts delivery and awards score. Destroys the plate on success.
/// </summary>
public bool TryDeliverOrder(RecipeData recipe) { ... }

// Skip — self-documenting
public float CookProgress => _cookProgress.Value;
```

## File Organization

- One MonoBehaviour per file, filename matches class name
- File order within a class:
  1. Serialized fields (grouped by `[Header]`)
  2. Private fields
  3. Public properties
  4. Unity lifecycle (`Awake`, `OnEnable`, `Start`, `Update`, `FixedUpdate`, `OnDisable`, `OnDestroy`)
  5. Public methods
  6. Private methods

## Encapsulation

- Default to `private` — only expose what other classes need
- Use properties for read access: `public float MoveSpeed => _moveSpeed;`
- Avoid `public` fields — use `[SerializeField] private` for Inspector, properties for code access
- Mark classes `sealed` unless designed for inheritance

## General

- Use `TryGetComponent` over `GetComponent` when the component may not exist
- Cache component references in `Awake()`, not in `Update()`
- Use `CompareTag("Tag")` instead of `gameObject.tag == "Tag"`
- Prefer `string.IsNullOrEmpty()` over `== ""` or `== null`
- **Never use the `Resources/` folder** — use direct references or Addressables
