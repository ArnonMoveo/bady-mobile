# Tech Stack: BADY

## Core Engine

* **Engine:** Unity 6 (or 2022.3 LTS)
* **Language:** C# (Standard .NET compliance)
* **Render Pipeline:** URP (Universal Render Pipeline) - Optimized for Mobile

## Architecture

* **Pattern:** Service Locator / Singleton for Managers (GameManager, SoundManager)
* **State Machine:** Finite State Machine (FSM) for Game Loop and Player States
* **Input:** New Unity Input System package

## Multiplayer / Networking

* **Framework:** Unity Netcode for GameObjects (NGO) 1.x
* **Transport:** Unity Transport (UTP)
* **Topology:** Client-Hosted (One mobile device acts as Host)
* **Discovery:** Local Network Discovery (for MVP), Unity Relay (for Phase 2)

## Art & Visuals

* **3D Style:** Cartoon / Low Poly
* **Shaders:** URP Shader Graph (Custom Toon Shader with Outlines)
* **Animation:** Unity Animator (Mecanim) + Tweening Library (DoTween or PrimeTween) for UI and "Juice"

## Data Management

* **Config:** ScriptableObjects (Recipes, LevelData, AudioRefs)
* **Save System:** PlayerPrefs (for MVP), JSON/BinaryFormatter (for Post-Launch)
