# Architecture — Unity

## Singletons

- Singletons are accepted for core managers: `GameManager`, `SoundManager`, `NetworkManager`
- All singletons must use the Instance null-check pattern to guard against duplicates
- Mark with `DontDestroyOnLoad` — initialization happens once in the Bootstrap scene

```csharp
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
```

- Keep singletons limited to true managers — not every shared class needs one

## Bootstrap Scene

- All persistent managers live in a `_Bootstrap` scene
- The game must always start from `_Bootstrap` — it initializes managers, then loads the first real scene
- Managers are marked `DontDestroyOnLoad` once and never re-initialized in gameplay scenes
- This guarantees initialization order and prevents duplicate managers when switching scenes

```
_Bootstrap → loads → MainMenu → Lobby → Kitchen_Level01
     ↑ managers persist across all scenes via DontDestroyOnLoad
```

## ScriptableObjects

- Use ScriptableObjects for **data definitions** — recipes, level configs, audio refs, ingredient properties
- ScriptableObjects are assets, not runtime state — don't mutate them at runtime
- Create via `[CreateAssetMenu]` attribute for easy Inspector workflow

```csharp
[CreateAssetMenu(fileName = "New Recipe", menuName = "BADY/Recipe")]
public class RecipeData : ScriptableObject
{
    [SerializeField] private string _recipeName;
    [SerializeField] private IngredientType[] _ingredients;
    [SerializeField] private float _cookTime;
    [SerializeField] private int _scoreValue;

    public string RecipeName => _recipeName;
    public IReadOnlyList<IngredientType> Ingredients => _ingredients;
    public float CookTime => _cookTime;
    public int ScoreValue => _scoreValue;
}
```

- For runtime state, use plain C# classes or structs owned by a MonoBehaviour
- Group SOs in `ScriptableObjects/{Recipes,LevelData,AudioRefs}/`

## State Machines

- Use an explicit FSM for player states (`Idle`, `Moving`, `Carrying`, `Cooking`, `Interacting`)
- States are plain C# classes implementing an `IState` interface
- Transitions managed by context class (e.g., `PlayerController`), not by states themselves

```csharp
public interface IState
{
    void Enter();
    void Execute();  // Called per-frame
    void Exit();
}
```

- Keep states small and focused — one responsibility per state
- FSM can be added later for game flow too (`MainMenu`, `Lobby`, `Playing`, `RoundEnd`, `ScoreScreen`)

## Scene Management

- One scene per major game state: `MainMenu`, `Lobby`, `Kitchen_Level01`, etc.
- Use additive loading for shared UI or overlay screens
- Load scenes via `SceneManager.LoadSceneAsync()` — never synchronous loads on mobile
- Always start from `_Bootstrap` — use a `[RuntimeInitializeOnLoadMethod]` guard in dev to redirect if needed

## Dependencies

- Use `[SerializeField]` references or `Awake()` lookups for wiring dependencies
- Access managers via their singleton `Instance` property
- Wire scene-local dependencies in the hierarchy or via ScriptableObject references — avoid runtime searches
- Avoid `FindObjectOfType` — it's slow and fragile
