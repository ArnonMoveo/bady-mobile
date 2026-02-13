# BADY Mobile

A mobile co-op cooking game (Overcooked-style) built with Unity.

## Project Structure

```
Assets/_Game/       Game assets (scripts, art, audio, prefabs, scenes)
Packages/           Unity package manifest
ProjectSettings/    Unity project settings (auto-generated on first open)
agent-os/           Specifications, product docs, and standards
```

## Specs & Documentation

All project specifications and product documentation live in [`agent-os/`](agent-os/):

- **[`agent-os/product/`](agent-os/product/)** — Mission, roadmap, tech stack
- **[`agent-os/specs/`](agent-os/specs/)** — Feature specs (timestamped folders with plan, shape, references)
- **[`agent-os/standards/`](agent-os/standards/)** — Reusable coding patterns and conventions

### Current Specs

- [`2026-02-13-unity-project-setup`](agent-os/specs/2026-02-13-unity-project-setup/) — Initial Unity project scaffolding

## Getting Started

1. Open the `bady-mobile/` folder in Unity Hub
2. Unity will auto-generate `ProjectSettings/` and download packages from `Packages/manifest.json`
3. Create a URP pipeline asset via `Assets > Create > Rendering > URP Asset`
