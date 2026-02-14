using UnityEngine;

namespace Bady.Kitchen
{
    /// <summary>
    /// Data definition for a burning recipe. Maps an input KitchenObjectSO (cooked item) to an output
    /// KitchenObjectSO (burned item) with a burning duration. The input matches the output of a
    /// FryingRecipeSO. Not all cooked items burn — only items with a matching BurningRecipeSO.
    /// </summary>
    [CreateAssetMenu(fileName = "New BurningRecipeSO", menuName = "BADY/Burning Recipe")]
    public sealed class BurningRecipeSO : ScriptableObject
    {
        [SerializeField] private KitchenObjectSO _input;
        [SerializeField] private KitchenObjectSO _output;
        [SerializeField] private float _burningTimerMax = 8f;

        public KitchenObjectSO Input => _input;
        public KitchenObjectSO Output => _output;
        public float BurningTimerMax => _burningTimerMax;
    }
}
