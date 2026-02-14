using System;
using Bady.Player;
using UnityEngine;

namespace Bady.Kitchen
{
    /// <summary>
    /// Client-side visual that highlights a counter when the local player is looking at it.
    /// Purely local — not a NetworkBehaviour. Event-driven with no Update polling.
    /// </summary>
    public sealed class SelectedCounterVisual : MonoBehaviour
    {
        [SerializeField] private BaseCounter _counter;
        [SerializeField] private GameObject[] _visualObjects;

        private PlayerController _subscribedPlayer;

        private void Start()
        {
            if (PlayerController.LocalInstance != null)
            {
                SubscribeToPlayerEvents(PlayerController.LocalInstance);
            }
            else
            {
                PlayerController.OnLocalInstanceSet += PlayerController_OnLocalInstanceSet;
            }
        }

        private void OnDestroy()
        {
            PlayerController.OnLocalInstanceSet -= PlayerController_OnLocalInstanceSet;

            if (_subscribedPlayer != null)
            {
                _subscribedPlayer.OnSelectedInteractableChanged -= PlayerController_OnSelectedInteractableChanged;
            }
        }

        private void PlayerController_OnLocalInstanceSet(object sender, EventArgs e)
        {
            PlayerController.OnLocalInstanceSet -= PlayerController_OnLocalInstanceSet;
            SubscribeToPlayerEvents(PlayerController.LocalInstance);
        }

        private void SubscribeToPlayerEvents(PlayerController player)
        {
            _subscribedPlayer = player;
            player.OnSelectedInteractableChanged += PlayerController_OnSelectedInteractableChanged;
        }

        private void PlayerController_OnSelectedInteractableChanged(
            object sender,
            PlayerController.OnSelectedInteractableChangedEventArgs e)
        {
            bool isSelected = e.SelectedInteractable == (Core.IInteractable)_counter;

            for (int i = 0; i < _visualObjects.Length; i++)
            {
                _visualObjects[i].SetActive(isSelected);
            }
        }
    }
}
