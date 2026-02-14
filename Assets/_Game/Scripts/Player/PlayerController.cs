using System;
using Bady.Core;
using Bady.Input;
using Bady.Kitchen;
using Unity.Netcode;
using UnityEngine;

namespace Bady.Player
{
    /// <summary>
    /// Networked player controller handling movement, rotation, interaction detection,
    /// and procedural tilt. Owner-authoritative — input is processed only on the owning client.
    /// Requires NetworkObject and NetworkTransform (owner-authoritative) on the prefab.
    /// </summary>
    public sealed class PlayerController : NetworkBehaviour, IKitchenObjectParent
    {
        public sealed class OnSelectedInteractableChangedEventArgs : EventArgs
        {
            public IInteractable SelectedInteractable { get; set; }
        }

        [Header("Movement")]
        [SerializeField] private float _moveSpeed = 7f;
        [SerializeField] private float _rotationSpeed = 10f;

        [Header("Interaction")]
        [SerializeField] private float _interactDistance = 2f;
        [SerializeField] private Vector3 _interactBoxHalfExtents = new Vector3(0.5f, 0.5f, 0.5f);
        [SerializeField] private LayerMask _interactableLayer;

        [Header("Visuals")]
        [SerializeField] private Transform _playerVisual;
        [SerializeField] private float _tiltAngle = 15f;
        [SerializeField] private float _tiltSpeed = 10f;

        [Header("Kitchen Object")]
        [SerializeField] private Transform _kitchenObjectHoldPoint;

        private const int MaxHitResults = 4;

        private Rigidbody _rigidbody;
        private GameInput _gameInput;
        private Vector3 _lastInteractDirection;
        private IInteractable _selectedInteractable;
        private readonly RaycastHit[] _hitResults = new RaycastHit[MaxHitResults];
        private readonly OnSelectedInteractableChangedEventArgs _selectedInteractableEventArgs = new();
        private Vector2 _cachedInput;
        private KitchenObject _kitchenObject;

        /// <summary>
        /// The local player's PlayerController instance. Set on the owning client during OnNetworkSpawn.
        /// </summary>
        public static PlayerController LocalInstance { get; private set; }

        /// <summary>
        /// Fires after LocalInstance is set. Used by systems that need the local player reference
        /// but may initialize before the player spawns (e.g. SelectedCounterVisual).
        /// </summary>
        public static event EventHandler OnLocalInstanceSet;

        /// <summary>
        /// Fires when the currently selected interactable changes (including to/from null).
        /// Used by UI to show/hide interaction highlights.
        /// </summary>
        public event EventHandler<OnSelectedInteractableChangedEventArgs> OnSelectedInteractableChanged;

        public bool IsWalking { get; private set; }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (!IsOwner) return;

            LocalInstance = this;
            OnLocalInstanceSet?.Invoke(this, EventArgs.Empty);

            _gameInput = GameInput.Instance;
            _lastInteractDirection = transform.forward;

            _gameInput.OnInteractAction += GameInput_OnInteractAction;
            _gameInput.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            if (!IsOwner) return;

            LocalInstance = null;

            if (_gameInput != null)
            {
                _gameInput.OnInteractAction -= GameInput_OnInteractAction;
                _gameInput.OnInteractAlternateAction -= GameInput_OnInteractAlternateAction;
            }
        }

        private void Update()
        {
            if (!IsOwner) return;

            _cachedInput = _gameInput.GetMovementVectorNormalized();

            HandleRotation();
            HandleInteractions();
            HandleTilt();
        }

        private void FixedUpdate()
        {
            if (!IsOwner) return;

            HandleMovement();
        }

        public Transform GetKitchenObjectFollowTransform()
        {
            return _kitchenObjectHoldPoint;
        }

        public void SetKitchenObject(KitchenObject kitchenObject)
        {
            _kitchenObject = kitchenObject;
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

        private void HandleMovement()
        {
            Vector2 inputVector = _gameInput.GetMovementVectorNormalized();
            Vector3 moveDirection = new Vector3(inputVector.x, 0f, inputVector.y);

            IsWalking = moveDirection.sqrMagnitude > 0.001f;

            if (!IsWalking) return;

            Vector3 targetPosition = _rigidbody.position + moveDirection * (_moveSpeed * Time.fixedDeltaTime);
            _rigidbody.MovePosition(targetPosition);
        }

        private void HandleRotation()
        {
            Vector3 moveDirection = new Vector3(_cachedInput.x, 0f, _cachedInput.y);

            if (moveDirection.sqrMagnitude < 0.001f) return;

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }

        private void HandleInteractions()
        {
            Vector3 moveDirection = new Vector3(_cachedInput.x, 0f, _cachedInput.y);

            if (moveDirection.sqrMagnitude > 0.001f)
            {
                _lastInteractDirection = moveDirection;
            }

            int hitCount = Physics.BoxCastNonAlloc(
                transform.position,
                _interactBoxHalfExtents,
                _lastInteractDirection,
                _hitResults,
                Quaternion.identity,
                _interactDistance,
                _interactableLayer
            );

            Debug.DrawRay(transform.position, _lastInteractDirection * _interactDistance, Color.green);

            IInteractable newInteractable = null;

            if (hitCount > 0)
            {
                float closestDistance = float.MaxValue;

                for (int i = 0; i < hitCount; i++)
                {
                    if (_hitResults[i].transform.TryGetComponent(out IInteractable interactable))
                    {
                        float distance = _hitResults[i].distance;
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            newInteractable = interactable;
                        }
                    }
                }
            }

            if (newInteractable != _selectedInteractable)
            {
                _selectedInteractable = newInteractable;
                _selectedInteractableEventArgs.SelectedInteractable = _selectedInteractable;
                OnSelectedInteractableChanged?.Invoke(this, _selectedInteractableEventArgs);
            }
        }

        private void HandleTilt()
        {
            if (_playerVisual == null) return;

            Quaternion targetTilt;

            if (_cachedInput.sqrMagnitude > 0.001f)
            {
                float tiltX = -_cachedInput.y * _tiltAngle;
                float tiltZ = _cachedInput.x * _tiltAngle;
                targetTilt = Quaternion.Euler(tiltX, 0f, tiltZ);
            }
            else
            {
                targetTilt = Quaternion.identity;
            }

            _playerVisual.localRotation = Quaternion.Lerp(
                _playerVisual.localRotation,
                targetTilt,
                _tiltSpeed * Time.deltaTime
            );
        }

        private void GameInput_OnInteractAction(object sender, EventArgs e)
        {
            if (!IsOwner) return;

            _selectedInteractable?.Interact(this);
        }

        private void GameInput_OnInteractAlternateAction(object sender, EventArgs e)
        {
            if (!IsOwner) return;

            _selectedInteractable?.InteractAlternate(this);
        }
    }
}
