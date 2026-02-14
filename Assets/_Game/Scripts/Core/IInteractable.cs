using Bady.Player;

namespace Bady.Core
{
    /// <summary>
    /// Interface for objects the player can interact with (counters, stations, appliances).
    /// Implement on any MonoBehaviour that should respond to player interaction input.
    /// </summary>
    public interface IInteractable
    {
        /// <summary>
        /// Primary interaction (pick up, drop, use).
        /// Called when the player presses the interact button while this object is selected.
        /// </summary>
        void Interact(PlayerController player);

        /// <summary>
        /// Secondary interaction (chop, cut).
        /// Called when the player presses the alternate interact button while this object is selected.
        /// </summary>
        void InteractAlternate(PlayerController player);
    }
}
