using System;
using Bady.Core;
using Bady.Player;
using Unity.Netcode;
using UnityEngine;

namespace Bady.Kitchen
{
    /// <summary>
    /// Abstract base for all counter types. Implements IInteractable for player interaction
    /// and IKitchenObjectParent for holding kitchen items. Subclasses override Interact()
    /// and/or InteractAlternate() to define counter-specific behavior.
    /// </summary>
    public abstract class BaseCounter : NetworkBehaviour, IInteractable, IKitchenObjectParent
    {
        [SerializeField] private Transform _counterTopPoint;

        private KitchenObject _kitchenObject;

        /// <summary>
        /// Fires on any counter when an item is placed on it. Used for sound/VFX.
        /// </summary>
        public static event EventHandler OnAnyObjectPlacedHere;

        /// <summary>
        /// Clears static event subscribers. Call on scene transitions to prevent leaked references.
        /// </summary>
        public static void ResetStaticData()
        {
            OnAnyObjectPlacedHere = null;
        }

        public virtual void Interact(PlayerController player)
        {
        }

        public virtual void InteractAlternate(PlayerController player)
        {
        }

        public Transform GetKitchenObjectFollowTransform()
        {
            return _counterTopPoint;
        }

        public virtual void SetKitchenObject(KitchenObject kitchenObject)
        {
            _kitchenObject = kitchenObject;

            if (kitchenObject != null)
            {
                OnAnyObjectPlacedHere?.Invoke(this, EventArgs.Empty);
            }
        }

        public KitchenObject GetKitchenObject()
        {
            return _kitchenObject;
        }

        public bool HasKitchenObject()
        {
            return _kitchenObject != null;
        }

        public virtual void ClearKitchenObject()
        {
            _kitchenObject = null;
        }

        /// <summary>
        /// Resolves the calling player's IKitchenObjectParent from an RPC sender.
        /// Returns false if the client is invalid or missing a player object.
        /// </summary>
        protected bool TryGetPlayerParent(RpcParams rpcParams, out IKitchenObjectParent playerParent)
        {
            playerParent = null;
            ulong clientId = rpcParams.Receive.SenderClientId;

            if (!NetworkManager.ConnectedClients.TryGetValue(clientId, out NetworkClient networkClient))
                return false;

            if (networkClient.PlayerObject == null)
                return false;

            playerParent = networkClient.PlayerObject.GetComponent<IKitchenObjectParent>();
            return playerParent != null;
        }

        /// <summary>
        /// Transfers a kitchen object between this counter and the player.
        /// If counter has item and player is empty, gives item to player.
        /// If player has item and counter is empty, places item on counter.
        /// </summary>
        protected void TransferKitchenObject(IKitchenObjectParent playerParent)
        {
            if (HasKitchenObject() && !playerParent.HasKitchenObject())
            {
                GetKitchenObject().SetKitchenObjectParent(playerParent);
            }
            else if (!HasKitchenObject() && playerParent.HasKitchenObject())
            {
                playerParent.GetKitchenObject().SetKitchenObjectParent(this);
            }
        }
    }
}
