using UnityEngine;

namespace Bady.Kitchen
{
    /// <summary>
    /// Data definition for a cutting recipe. Maps an input KitchenObjectSO to an output
    /// KitchenObjectSO with a required number of cuts. Not all items are cuttable —
    /// only items with a matching CuttingRecipeSO can be processed on a CuttingCounter.
    /// </summary>
    [CreateAssetMenu(fileName = "New CuttingRecipeSO", menuName = "BADY/Cutting Recipe")]
    public sealed class CuttingRecipeSO : ScriptableObject
    {
        [SerializeField] private KitchenObjectSO _input;
        [SerializeField] private KitchenObjectSO _output;
        [SerializeField] private int _cuttingProgressMax = 3;

        public KitchenObjectSO Input => _input;
        public KitchenObjectSO Output => _output;
        public int CuttingProgressMax => _cuttingProgressMax;
    }
}
