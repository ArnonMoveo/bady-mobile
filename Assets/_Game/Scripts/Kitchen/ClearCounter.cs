using Bady.Core;
using Bady.Player;
using Unity.Netcode;
using UnityEngine;

namespace Bady.Kitchen
{
    /// <summary>
    /// Simple counter that allows picking up and placing down items.
    /// All logic runs on the server via a single ServerRpc.
    /// </summary>
    public sealed class ClearCounter : BaseCounter
    {
        public override void Interact(PlayerController player)
        {
            InteractServerRpc();
        }

        [Rpc(SendTo.Server)]
        private void InteractServerRpc(RpcParams rpcParams = default)
        {
            ulong clientId = rpcParams.Receive.SenderClientId;

            if (!NetworkManager.ConnectedClients.TryGetValue(clientId, out NetworkClient networkClient))
            {
                return;
            }

            if (networkClient.PlayerObject == null)
            {
                return;
            }

            IKitchenObjectParent playerParent =
                networkClient.PlayerObject.GetComponent<IKitchenObjectParent>();

            if (playerParent == null)
            {
                return;
            }

            if (HasKitchenObject() && !playerParent.HasKitchenObject())
            {
                // Counter has item, player is empty — give item to player
                GetKitchenObject().SetKitchenObjectParent(playerParent);
            }
            else if (!HasKitchenObject() && playerParent.HasKitchenObject())
            {
                // Player has item, counter is empty — place item on counter
                playerParent.GetKitchenObject().SetKitchenObjectParent(this);
            }
        }
    }
}
