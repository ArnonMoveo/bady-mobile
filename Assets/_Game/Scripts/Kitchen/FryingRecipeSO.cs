using UnityEngine;

namespace Bady.Kitchen
{
    /// <summary>
    /// Data definition for a frying recipe. Maps an input KitchenObjectSO (raw item) to an output
    /// KitchenObjectSO (cooked item) with a frying duration. Not all items are fryable —
    /// only items with a matching FryingRecipeSO can be processed on a StoveCounter.
    /// </summary>
    [CreateAssetMenu(fileName = "New FryingRecipeSO", menuName = "BADY/Frying Recipe")]
    public sealed class FryingRecipeSO : ScriptableObject
    {
        [SerializeField] private KitchenObjectSO _input;
        [SerializeField] private KitchenObjectSO _output;
        [SerializeField] private float _fryingTimerMax = 5f;

        public KitchenObjectSO Input => _input;
        public KitchenObjectSO Output => _output;
        public float FryingTimerMax => _fryingTimerMax;
    }
}
