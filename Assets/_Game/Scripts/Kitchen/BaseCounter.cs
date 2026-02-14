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

        public void SetKitchenObject(KitchenObject kitchenObject)
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

        public void ClearKitchenObject()
        {
            _kitchenObject = null;
        }
    }
}
