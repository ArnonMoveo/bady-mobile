# BADY Mobile — Unity Project Setup

## Context

BADY is a mobile co-op cooking game (Overcooked-style). This is the initial project scaffolding — no game logic, just the Unity project structure, required packages, and assembly definitions. The repo lives at `/Users/arnon.meltser/bady-mobile`.

## Directory Structure

```
bady-mobile/
├── Assets/
│   └── _Game/
│       ├── Animations/
│       ├── Art/
│       │   ├── Materials/
│       │   ├── Models/
│       │   ├── Shaders/
│       │   └── Textures/
│       ├── Audio/
│       │   ├── Music/
│       │   └── SFX/
│       ├── Prefabs/
│       │   ├── Characters/
│       │   ├── Kitchen/
│       │   ├── Ingredients/
│       │   └── UI/
│       ├── Scenes/
│       ├── ScriptableObjects/
│       │   ├── Recipes/
│       │   ├── LevelData/
│       │   └── AudioRefs/
│       ├── Scripts/
│       │   ├── Core/        (GameManager, ServiceLocator, FSM)
│       │   ├── Kitchen/     (Cooking, Ingredients, Delivery)
│       │   ├── Input/       (Input actions, virtual joystick)
│       │   ├── Network/     (NetworkManager, lobby, sync)
│       │   ├── Player/      (PlayerController, PlayerStates)
│       │   ├── UI/          (HUD, Menus, Progress bars)
│       │   └── Audio/       (SoundManager)
│       ├── Settings/        (URP settings, Input Action assets)
│       └── UI/
│           └── Fonts/
│   ├── Plugins/
│   └── StreamingAssets/
├── Packages/
│   └── manifest.json
└── ProjectSettings/        (empty — Unity Hub generates these on first open)
```

---

## Task 1: Save Spec Documentation

Create `agent-os/specs/2026-02-13-unity-project-setup/` with:
- **plan.md** — This full plan
- **shape.md** — Shaping notes (scope, decisions, context)
- **references.md** — No external references (fresh project)

## Task 2: Create Folder Hierarchy

Create all directories under `bady-mobile/Assets/_Game/` with `.gitkeep` files to ensure empty folders are tracked by git:
- `Animations/`, `Art/{Materials,Models,Shaders,Textures}`, `Audio/{Music,SFX}`
- `Prefabs/{Characters,Kitchen,Ingredients,UI}`, `Scenes/`
- `ScriptableObjects/{Recipes,LevelData,AudioRefs}`
- `Scripts/{Core,Kitchen,Input,Network,Player,UI,Audio}`
- `Settings/`, `UI/Fonts/`
- `Assets/Plugins/`, `Assets/StreamingAssets/`

## Task 3: Create Packages/manifest.json

Create `bady-mobile/Packages/manifest.json` with required packages:
- `com.unity.render-pipelines.universal` (URP)
- `com.unity.netcode.gameobjects` (NGO)
- `com.unity.inputsystem` (New Input System)
- `com.unity.transport` (Unity Transport)
- Standard Unity defaults (TextMeshPro, etc.)

Use versions compatible with Unity 6 / 2022.3 LTS.

## Task 4: Create ProjectSettings Folder

Create an empty `bady-mobile/ProjectSettings/` directory. **Do NOT generate .asset files manually** — Unity Hub will populate this folder with correct default files on first project open.

## Task 5: Create Assembly Definitions

Create `.asmdef` files for script organization:
- `Assets/_Game/Scripts/Bady.asmdef` — Root assembly for all game scripts
  - References: `Unity.Netcode.Runtime`, `Unity.InputSystem`, `Unity.TextMeshPro`
- Optionally, per-subfolder asmdefs if you prefer strict isolation (Core, Network, etc.)

**Decision:** A single root `.asmdef` is simpler for MVP. Per-folder asmdefs can be added later if compile times become an issue.

## Task 6: Create .gitignore

Add a Unity-standard `.gitignore` to `bady-mobile/` to exclude:
- `Library/`, `Temp/`, `Obj/`, `Build/`, `Builds/`, `Logs/`
- `.vs/`, `*.csproj`, `*.sln`, `UserSettings/`

---

## Verification

1. Open the `bady-mobile/` folder in Unity Hub — it should recognize it as a valid project
2. Unity should auto-download packages from manifest.json on first open
3. Folder structure should appear under `Assets/_Game/`
4. Assembly definition should compile without errors (no scripts yet, so no compilation issues)
5. URP pipeline asset can be created in-editor via `Assets > Create > Rendering > URP Asset`
