using System;
using Bady.Core;
using Bady.Player;
using Unity.Netcode;
using UnityEngine;

namespace Bady.Kitchen
{
    /// <summary>
    /// Counter that automatically fries items placed on it. Two-stage transformation:
    /// Raw → Cooked (frying timer) → Burned (burning timer, if left too long).
    /// Server ticks timers in Update(), clients read state via NetworkVariable callbacks.
    /// </summary>
    public sealed class StoveCounter : BaseCounter
    {
        public enum State
        {
            Idle,
            Frying,
            Fried,
            Burning,
            Burned
        }

        public sealed class OnProgressChangedEventArgs : EventArgs
        {
            public float ProgressNormalized { get; set; }
        }

        public sealed class OnStateChangedEventArgs : EventArgs
        {
            public State State { get; set; }
        }

        [Header("Recipes")]
        [SerializeField] private FryingRecipeSO[] _fryingRecipes;
        [SerializeField] private BurningRecipeSO[] _burningRecipes;

        private readonly NetworkVariable<State> _state = new(State.Idle,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);

        private readonly NetworkVariable<float> _fryingTimer = new(0f,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);

        private readonly NetworkVariable<float> _burningTimer = new(0f,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);

        private readonly NetworkVariable<float> _fryingTimerMax = new(0f,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);

        private readonly NetworkVariable<float> _burningTimerMax = new(0f,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);

        private FryingRecipeSO _cachedFryingRecipe;
        private BurningRecipeSO _cachedBurningRecipe;

        private readonly OnProgressChangedEventArgs _progressChangedEventArgs = new();
        private readonly OnStateChangedEventArgs _stateChangedEventArgs = new();

        /// <summary>
        /// Fires on any StoveCounter when cooking state changes. Used for global SFX (sizzle).
        /// </summary>
        public static event EventHandler OnAnyStoveStateChanged;

        /// <summary>
        /// Fires on this instance when cooking progress changes. Used for progress bar UI.
        /// </summary>
        public event EventHandler<OnProgressChangedEventArgs> OnProgressChanged;

        /// <summary>
        /// Fires on this instance when cooking state changes. Used for visual state indicators.
        /// </summary>
        public event EventHandler<OnStateChangedEventArgs> OnStateChanged;

        public State CurrentState => _state.Value;

        /// <summary>
        /// Clears static event subscribers. Call on scene transitions to prevent leaked references.
        /// </summary>
        public new static void ResetStaticData()
        {
            OnAnyStoveStateChanged = null;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            _state.OnValueChanged += State_OnValueChanged;
            _fryingTimer.OnValueChanged += FryingTimer_OnValueChanged;
            _burningTimer.OnValueChanged += BurningTimer_OnValueChanged;

            // Late-join sync: fire initial state and progress if not idle
            if (_state.Value != State.Idle)
            {
                _stateChangedEventArgs.State = _state.Value;
                OnStateChanged?.Invoke(this, _stateChangedEventArgs);
                OnAnyStoveStateChanged?.Invoke(this, EventArgs.Empty);

                if (_state.Value == State.Frying && _fryingTimerMax.Value > 0f)
                {
                    _progressChangedEventArgs.ProgressNormalized =
                        _fryingTimer.Value / _fryingTimerMax.Value;
                    OnProgressChanged?.Invoke(this, _progressChangedEventArgs);
                }
                else if (_state.Value == State.Burning && _burningTimerMax.Value > 0f)
                {
                    _progressChangedEventArgs.ProgressNormalized =
                        _burningTimer.Value / _burningTimerMax.Value;
                    OnProgressChanged?.Invoke(this, _progressChangedEventArgs);
                }
            }
        }

        public override void OnNetworkDespawn()
        {
            _state.OnValueChanged -= State_OnValueChanged;
            _fryingTimer.OnValueChanged -= FryingTimer_OnValueChanged;
            _burningTimer.OnValueChanged -= BurningTimer_OnValueChanged;

            base.OnNetworkDespawn();
        }

        private void Update()
        {
            if (!IsServer) return;

            switch (_state.Value)
            {
                case State.Frying:
                    if (!HasKitchenObject() || _cachedFryingRecipe == null)
                    {
                        _state.Value = State.Idle;
                        break;
                    }
                    _fryingTimer.Value += Time.deltaTime;
                    if (_fryingTimer.Value >= _cachedFryingRecipe.FryingTimerMax)
                    {
                        KitchenObjectSO outputSO = _cachedFryingRecipe.Output;
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(outputSO, this);

                        _cachedBurningRecipe = GetBurningRecipeForInput(outputSO);
                        if (_cachedBurningRecipe != null)
                        {
                            _burningTimerMax.Value = _cachedBurningRecipe.BurningTimerMax;
                            _burningTimer.Value = 0f;
                            _state.Value = State.Burning;
                        }
                        else
                        {
                            _state.Value = State.Fried;
                        }
                    }
                    break;

                case State.Burning:
                    if (!HasKitchenObject() || _cachedBurningRecipe == null)
                    {
                        _state.Value = State.Idle;
                        break;
                    }
                    _burningTimer.Value += Time.deltaTime;
                    if (_burningTimer.Value >= _cachedBurningRecipe.BurningTimerMax)
                    {
                        KitchenObjectSO burnedSO = _cachedBurningRecipe.Output;
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(burnedSO, this);

                        _state.Value = State.Burned;
                    }
                    break;
            }
        }

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

            if (!HasKitchenObject() && playerParent.HasKitchenObject())
            {
                // Player placing item on empty stove
                KitchenObjectSO inputSO = playerParent.GetKitchenObject().KitchenObjectSO;

                FryingRecipeSO fryingRecipe = GetFryingRecipeForInput(inputSO);
                if (fryingRecipe != null)
                {
                    playerParent.GetKitchenObject().SetKitchenObjectParent(this);

                    _cachedFryingRecipe = fryingRecipe;
                    _cachedBurningRecipe = null;
                    _fryingTimerMax.Value = fryingRecipe.FryingTimerMax;
                    _fryingTimer.Value = 0f;
                    _burningTimer.Value = 0f;
                    _burningTimerMax.Value = 0f;
                    _state.Value = State.Frying;
                    return;
                }

                BurningRecipeSO burningRecipe = GetBurningRecipeForInput(inputSO);
                if (burningRecipe != null)
                {
                    playerParent.GetKitchenObject().SetKitchenObjectParent(this);

                    _cachedBurningRecipe = burningRecipe;
                    _cachedFryingRecipe = null;
                    _burningTimerMax.Value = burningRecipe.BurningTimerMax;
                    _burningTimer.Value = 0f;
                    _fryingTimer.Value = 0f;
                    _fryingTimerMax.Value = 0f;
                    _state.Value = State.Burning;
                }

                // No matching recipe — reject (do nothing)
            }
            else if (HasKitchenObject() && !playerParent.HasKitchenObject())
            {
                // Player picking up item from stove
                GetKitchenObject().SetKitchenObjectParent(playerParent);

                _cachedFryingRecipe = null;
                _cachedBurningRecipe = null;
                _fryingTimer.Value = 0f;
                _burningTimer.Value = 0f;
                _fryingTimerMax.Value = 0f;
                _burningTimerMax.Value = 0f;
                _state.Value = State.Idle;
            }

            // Both have items or both empty — no-op
        }

        private void State_OnValueChanged(State previousValue, State newValue)
        {
            _stateChangedEventArgs.State = newValue;
            OnStateChanged?.Invoke(this, _stateChangedEventArgs);
            OnAnyStoveStateChanged?.Invoke(this, EventArgs.Empty);

            if (newValue is State.Idle or State.Fried or State.Burned)
            {
                _progressChangedEventArgs.ProgressNormalized = 0f;
                OnProgressChanged?.Invoke(this, _progressChangedEventArgs);
            }
        }

        private void FryingTimer_OnValueChanged(float previousValue, float newValue)
        {
            if (_state.Value != State.Frying) return;
            if (_fryingTimerMax.Value <= 0f) return;

            _progressChangedEventArgs.ProgressNormalized = newValue / _fryingTimerMax.Value;
            OnProgressChanged?.Invoke(this, _progressChangedEventArgs);
        }

        private void BurningTimer_OnValueChanged(float previousValue, float newValue)
        {
            if (_state.Value != State.Burning) return;
            if (_burningTimerMax.Value <= 0f) return;

            _progressChangedEventArgs.ProgressNormalized = newValue / _burningTimerMax.Value;
            OnProgressChanged?.Invoke(this, _progressChangedEventArgs);
        }

        private FryingRecipeSO GetFryingRecipeForInput(KitchenObjectSO input)
        {
            for (int i = 0; i < _fryingRecipes.Length; i++)
            {
                if (_fryingRecipes[i].Input == input)
                {
                    return _fryingRecipes[i];
                }
            }

            return null;
        }

        private BurningRecipeSO GetBurningRecipeForInput(KitchenObjectSO input)
        {
            for (int i = 0; i < _burningRecipes.Length; i++)
            {
                if (_burningRecipes[i].Input == input)
                {
                    return _burningRecipes[i];
                }
            }

            return null;
        }
    }
}
