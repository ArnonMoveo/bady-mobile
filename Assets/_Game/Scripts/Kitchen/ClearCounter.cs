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
            if (!TryGetPlayerParent(rpcParams, out IKitchenObjectParent playerParent))
            {
                return;
            }

            TransferKitchenObject(playerParent);
        }
    }
}
