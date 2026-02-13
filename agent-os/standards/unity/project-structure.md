# Project Structure — Unity

## Root Layout

```
Assets/
├── _Game/          All game-specific assets (prefixed to sort to top)
├── Plugins/        Third-party native plugins
└── StreamingAssets/ Runtime-loaded files
```

- **Everything game-related goes under `_Game/`** — keeps project assets separate from Unity packages and third-party imports
- Never put game files directly in `Assets/` root

## _Game Subdirectories

| Directory | Contents |
|---|---|
| `Animations/` | Animator controllers, animation clips |
| `Art/Materials/` | Materials (.mat) |
| `Art/Models/` | 3D models (.fbx, .obj) |
| `Art/Shaders/` | Shader graphs, custom shaders |
| `Art/Textures/` | Textures, sprites, atlases |
| `Audio/Music/` | Background music tracks |
| `Audio/SFX/` | Sound effects |
| `Prefabs/Characters/` | Player and NPC prefabs |
| `Prefabs/Kitchen/` | Stations, counters, appliances |
| `Prefabs/Ingredients/` | Ingredient prefabs (raw, chopped, cooked) |
| `Prefabs/UI/` | UI element prefabs |
| `Scenes/` | All game scenes |
| `ScriptableObjects/Recipes/` | RecipeData assets |
| `ScriptableObjects/LevelData/` | Level configuration assets |
| `ScriptableObjects/AudioRefs/` | Audio reference assets |
| `Scripts/` | All C# scripts (see below) |
| `Settings/` | URP pipeline assets, Input Action assets |
| `UI/Fonts/` | Font assets (TMP fonts) |

## Scripts Organization

```
Scripts/
├── Bady.asmdef     Root assembly definition
├── Core/           GameManager, ServiceLocator, FSM, shared utilities
├── Kitchen/        Cooking logic, ingredients, delivery, stations
├── Input/          Input actions, virtual joystick, touch handling
├── Network/        NetworkManager wrapper, lobby, state sync
├── Player/         PlayerController, player states, interaction
├── UI/             HUD, menus, progress bars, popups
└── Audio/          SoundManager, audio helpers
```

- **One assembly definition** (`Bady.asmdef`) at `Scripts/` root — covers all subfolders
- One MonoBehaviour per file, filename matches class name
- Place interfaces and enums in the folder of their primary consumer

## Naming Conventions

No prefixes or suffixes on most assets — the folder hierarchy provides type context.

- **Scenes:** `PascalCase` — `MainMenu`, `Lobby`, `Kitchen_Level01`
- **Prefabs:** `PascalCase` matching primary component — `PlayerCharacter`, `CuttingStation`
- **ScriptableObject assets:** `PascalCase_SO` suffix — `BurgerRecipe_SO`, `Level01Data_SO` (distinguishes data assets from classes in global search)
- **ScriptableObject classes:** `PascalCase` with no suffix — `RecipeData`, `LevelData` (the class name stays clean)
- **Materials:** `PascalCase` — `WoodCounter`, `TomatoRed`
- **Folders:** `PascalCase` — never spaces, never lowercase

## Rules

- **Never use the `Resources/` folder** — use direct references or Addressables. Resources bypasses dependency tracking, bloats builds, and can't be unloaded cleanly
- New asset types get a subfolder — don't dump files in parent directories
- Prefabs reference ScriptableObjects for data, not hardcoded values
- Keep scene hierarchies flat — use empty GameObjects as organizers only when grouping 5+ objects
