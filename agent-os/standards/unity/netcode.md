# Netcode — Unity (Netcode for GameObjects)

## Authority Model

- **Server-authoritative** by default — clients request, server validates and executes
- Never trust client state for game logic (recipe completion, scoring, timers)
- Use `IsServer` / `IsClient` / `IsOwner` guards at the top of methods

## Ownership

- **Never transfer ownership of interactables** (ingredients, plates, utensils) to players
- The server retains ownership and uses `NetworkObject.TrySetParent()` to attach items to players
- This avoids race conditions on simultaneous grabs and simplifies item passing between players

## RPCs

- **ServerRpc** — Client → Server. Use for player input/requests
  - Always validate input on server before acting
  - Name suffix: `ServerRpc` — `RequestPickupServerRpc()`
  - Attribute: `[ServerRpc(RequireOwnership = true)]` unless explicitly needed otherwise

- **ClientRpc** — Server → Clients. Use for state sync, VFX, audio cues
  - Name suffix: `ClientRpc` — `PlayCookEffectClientRpc()`
  - Keep payloads small — send IDs/enums, not full objects

- **Never pass `GameObject`, `Transform`, or any Unity object reference in RPCs** — use `NetworkObjectReference` or `ulong` (NetworkObjectId) instead

```csharp
[ServerRpc(RequireOwnership = true)]
private void RequestPickupServerRpc(ulong ingredientNetId)
{
    // Validate: does ingredient exist? Is player close enough? Hands free?
    if (!ValidatePickup(ingredientNetId)) return;

    // Execute on server
    PerformPickup(ingredientNetId);

    // Notify clients
    PickupConfirmedClientRpc(ingredientNetId, OwnerClientId);
}
```

## NetworkVariables

- Use for state that all clients need continuously (health, cook progress, score)
- Prefer `NetworkVariable<T>` over RPCs for persistent state
- Set write permission explicitly: `NetworkVariableWritePermission.Server` (default, preferred)
- Subscribe to `OnValueChanged` for client-side reactions (UI updates, VFX)
- Keep types simple — primitives, enums, `FixedString`. Use `INetworkSerializable` for custom structs
- **`List<T>` is not supported** — use `NetworkList<T>` for synchronized collections

```csharp
private NetworkVariable<float> _cookProgress = new(0f,
    NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Server);
```

## Spawning

- Spawn networked objects only on the server
- Use `NetworkObject.Spawn()` — never `Instantiate()` for networked prefabs
- Register prefabs in the `NetworkManager` prefab list

## Common Pitfalls

- Don't use `Update()` for state sync — use `NetworkVariable` or periodic RPCs
- Don't serialize Unity types (`Vector3`, `Quaternion`) directly in RPCs when a simpler representation works (e.g., send a tile index instead of world position)
- Don't forget `base.OnNetworkSpawn()` when overriding
- Test with simulated latency — `Unity Transport` has built-in latency simulation
