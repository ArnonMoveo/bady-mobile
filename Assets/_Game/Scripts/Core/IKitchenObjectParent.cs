using Unity.Netcode;
using UnityEngine;

namespace Bady.Core
{
    /// <summary>
    /// Shared interface for anything that can hold a KitchenObject (counters and players).
    /// Provides the contract for item parenting, enabling KitchenObject.SetKitchenObjectParent()
    /// to work uniformly regardless of whether the parent is a counter or a player.
    /// </summary>
    public interface IKitchenObjectParent
    {
        /// <summary>
        /// Returns the Transform where a held KitchenObject should snap to (counter top point or player hold point).
        /// </summary>
        Transform GetKitchenObjectFollowTransform();

        /// <summary>
        /// Assigns a KitchenObject to this parent. Called by KitchenObject.SetKitchenObjectParent().
        /// </summary>
        void SetKitchenObject(Kitchen.KitchenObject kitchenObject);

        /// <summary>
        /// Returns the currently held KitchenObject, or null if empty.
        /// </summary>
        Kitchen.KitchenObject GetKitchenObject();

        /// <summary>
        /// Returns true if this parent is currently holding a KitchenObject.
        /// </summary>
        bool HasKitchenObject();

        /// <summary>
        /// Clears the held KitchenObject reference without destroying it.
        /// Called by KitchenObject.SetKitchenObjectParent() when an item leaves this parent.
        /// </summary>
        void ClearKitchenObject();

        /// <summary>
        /// The NetworkObject on this parent, used by KitchenObject for TrySetParent() reparenting.
        /// </summary>
        NetworkObject NetworkObject { get; }
    }
}
