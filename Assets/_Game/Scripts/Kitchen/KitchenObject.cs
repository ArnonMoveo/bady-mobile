using Bady.Core;
using Unity.Netcode;
using UnityEngine;

namespace Bady.Kitchen
{
    /// <summary>
    /// Networked kitchen item that moves between IKitchenObjectParent holders (counters and players).
    /// Manages its own parenting via SetKitchenObjectParent(). Not sealed — future types
    /// (e.g. PlateKitchenObject) will inherit.
    /// </summary>
    public class KitchenObject : NetworkBehaviour
    {
        [SerializeField] private KitchenObjectSO _kitchenObjectSO;

        private IKitchenObjectParent _kitchenObjectParent;

        public KitchenObjectSO KitchenObjectSO => _kitchenObjectSO;

        public IKitchenObjectParent KitchenObjectParent => _kitchenObjectParent;

        /// <summary>
        /// Moves this item to a new parent. Order is critical:
        /// 1. Clear old parent FIRST (before reassigning the field)
        /// 2. Reassign the field
        /// 3. Tell the new parent about us
        /// 4. Reparent in the network hierarchy
        /// If steps 1-2 are swapped, ClearKitchenObject() hits the NEW parent causing duplication.
        /// </summary>
        public void SetKitchenObjectParent(IKitchenObjectParent newParent)
        {
            _kitchenObjectParent?.ClearKitchenObject();
            _kitchenObjectParent = newParent;
            newParent.SetKitchenObject(this);

            NetworkObject.TrySetParent(newParent.NetworkObject);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        /// <summary>
        /// Clears parent reference and despawns this object. Server only.
        /// </summary>
        public void DestroySelf()
        {
            if (!IsServer) return;

            _kitchenObjectParent?.ClearKitchenObject();
            NetworkObject.Despawn();
        }

        /// <summary>
        /// Server-only factory: instantiates, spawns on the network, and parents a new KitchenObject.
        /// The kitchenObjectSO's prefab must be a NetworkObject registered in the NetworkManager prefab list.
        /// </summary>
        public static void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent parent)
        {
            if (!NetworkManager.Singleton.IsServer) return;

            Transform kitchenObjectTransform = Object.Instantiate(kitchenObjectSO.Prefab);
            NetworkObject networkObject = kitchenObjectTransform.GetComponent<NetworkObject>();
            networkObject.Spawn();

            KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();
            kitchenObject.SetKitchenObjectParent(parent);
        }
    }
}
