using System;
using UnityEngine;

namespace Bady.Input
{
    /// <summary>
    /// Singleton wrapper for player input. Provides normalized movement vector
    /// and events for interact actions. Persists across scenes via DontDestroyOnLoad.
    /// </summary>
    public sealed class GameInput : MonoBehaviour
    {
        public static GameInput Instance { get; private set; }

        /// <summary>
        /// Fired when the primary interact button is performed (pick up, drop, use).
        /// </summary>
        public event EventHandler OnInteractAction;

        /// <summary>
        /// Fired when the alternate interact button is performed (chop, cut).
        /// </summary>
        public event EventHandler OnInteractAlternateAction;

        private PlayerInputActions _playerInputActions;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            _playerInputActions = new PlayerInputActions();
            _playerInputActions.Player.Enable();

            _playerInputActions.Player.Interact.performed += Interact_performed;
            _playerInputActions.Player.InteractAlternate.performed += InteractAlternate_performed;
        }

        private void OnDestroy()
        {
            if (Instance != this) return;

            _playerInputActions.Player.Interact.performed -= Interact_performed;
            _playerInputActions.Player.InteractAlternate.performed -= InteractAlternate_performed;

            _playerInputActions.Player.Disable();
            _playerInputActions.Dispose();

            Instance = null;
        }

        /// <summary>
        /// Returns the current movement input as a normalized Vector2.
        /// Magnitude is clamped to 1 to prevent diagonal speed boost from WASD
        /// while preserving analog joystick sensitivity.
        /// </summary>
        public Vector2 GetMovementVectorNormalized()
        {
            Vector2 inputVector = _playerInputActions.Player.Move.ReadValue<Vector2>();
            return Vector2.ClampMagnitude(inputVector, 1f);
        }

        private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            OnInteractAction?.Invoke(this, EventArgs.Empty);
        }

        private void InteractAlternate_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            OnInteractAlternateAction?.Invoke(this, EventArgs.Empty);
        }
    }
}
