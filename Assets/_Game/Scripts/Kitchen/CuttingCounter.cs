using System;
using Bady.Core;
using Bady.Player;
using Unity.Netcode;
using UnityEngine;

namespace Bady.Kitchen
{
    /// <summary>
    /// Counter that chops items into their cut variants. Players place a cuttable item,
    /// then press InteractAlternate repeatedly to cut. After reaching the recipe's required
    /// cut count, the input item is destroyed and replaced with the output item.
    /// </summary>
    public sealed class CuttingCounter : BaseCounter
    {
        public sealed class OnProgressChangedEventArgs : EventArgs
        {
            public float ProgressNormalized { get; set; }
        }

        [SerializeField] private CuttingRecipeSO[] _cuttingRecipes;

        private readonly NetworkVariable<int> _cuttingProgress = new(0,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);

        private CuttingRecipeSO _cachedRecipe;
        private readonly OnProgressChangedEventArgs _progressChangedEventArgs = new();

        /// <summary>
        /// Fires on any CuttingCounter when a cut happens. Used for global SFX.
        /// </summary>
        public static event EventHandler OnAnyCut;

        /// <summary>
        /// Fires on this instance when cut progress changes. Used for progress bar UI.
        /// </summary>
        public event EventHandler<OnProgressChangedEventArgs> OnProgressChanged;

        /// <summary>
        /// Clears static event subscribers. Call on scene transitions to prevent leaked references.
        /// </summary>
        public new static void ResetStaticData()
        {
            OnAnyCut = null;
        }

        public override void SetKitchenObject(KitchenObject kitchenObject)
        {
            base.SetKitchenObject(kitchenObject);
            _cachedRecipe = kitchenObject != null
                ? GetCuttingRecipeForInput(kitchenObject.KitchenObjectSO)
                : null;
        }

        public override void ClearKitchenObject()
        {
            base.ClearKitchenObject();
            _cachedRecipe = null;
        }

        public override void Interact(PlayerController player)
        {
            InteractServerRpc();
        }

        public override void InteractAlternate(PlayerController player)
        {
            CutServerRpc();
        }

        [Rpc(SendTo.Server)]
        private void InteractServerRpc(RpcParams rpcParams = default)
        {
            if (!TryGetPlayerParent(rpcParams, out IKitchenObjectParent playerParent))
            {
                return;
            }

            TransferKitchenObject(playerParent);
            _cuttingProgress.Value = 0;
            OnProgressResetClientRpc();
        }

        [Rpc(SendTo.Server)]
        private void CutServerRpc(RpcParams rpcParams = default)
        {
            if (!HasKitchenObject()) return;
            if (_cachedRecipe == null) return;

            _cuttingProgress.Value++;

            float progressNormalized = (float)_cuttingProgress.Value / _cachedRecipe.CuttingProgressMax;
            OnCutClientRpc(progressNormalized);

            if (_cuttingProgress.Value >= _cachedRecipe.CuttingProgressMax)
            {
                KitchenObjectSO outputSO = _cachedRecipe.Output;
                GetKitchenObject().DestroySelf();
                KitchenObject.SpawnKitchenObject(outputSO, this);
                _cuttingProgress.Value = 0;
            }
        }

        [Rpc(SendTo.Everyone)]
        private void OnCutClientRpc(float progressNormalized)
        {
            _progressChangedEventArgs.ProgressNormalized = progressNormalized;
            OnProgressChanged?.Invoke(this, _progressChangedEventArgs);
            OnAnyCut?.Invoke(this, EventArgs.Empty);
        }

        [Rpc(SendTo.Everyone)]
        private void OnProgressResetClientRpc()
        {
            _progressChangedEventArgs.ProgressNormalized = 0f;
            OnProgressChanged?.Invoke(this, _progressChangedEventArgs);
        }

        private CuttingRecipeSO GetCuttingRecipeForInput(KitchenObjectSO input)
        {
            for (int i = 0; i < _cuttingRecipes.Length; i++)
            {
                if (_cuttingRecipes[i].Input == input)
                {
                    return _cuttingRecipes[i];
                }
            }

            return null;
        }
    }
}
