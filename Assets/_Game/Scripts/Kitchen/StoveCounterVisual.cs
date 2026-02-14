using UnityEngine;

namespace Bady.Kitchen
{
    /// <summary>
    /// Client-side visual for the stove. Shows/hides sizzle particles and stove glow
    /// based on the StoveCounter's cooking state. Not a NetworkBehaviour — reads state
    /// from events on the StoveCounter component.
    /// </summary>
    public sealed class StoveCounterVisual : MonoBehaviour
    {
        [SerializeField] private StoveCounter _stoveCounter;
        [SerializeField] private GameObject _sizzlingParticles;
        [SerializeField] private GameObject _stoveOnVisual;

        private void Start()
        {
            _stoveCounter.OnStateChanged += StoveCounter_OnStateChanged;

            // Sync to current state (handles late-join and undefined Start() ordering)
            UpdateVisual(_stoveCounter.CurrentState);
        }

        private void OnDestroy()
        {
            _stoveCounter.OnStateChanged -= StoveCounter_OnStateChanged;
        }

        private void StoveCounter_OnStateChanged(object sender, StoveCounter.OnStateChangedEventArgs e)
        {
            UpdateVisual(e.State);
        }

        private void UpdateVisual(StoveCounter.State state)
        {
            bool isActive = state is StoveCounter.State.Frying or StoveCounter.State.Burning;
            _sizzlingParticles.SetActive(isActive);
            _stoveOnVisual.SetActive(isActive);
        }
    }
}
