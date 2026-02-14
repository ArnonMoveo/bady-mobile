using UnityEngine;

namespace Bady.Kitchen
{
    /// <summary>
    /// Data definition for a kitchen item. Holds the visual prefab reference,
    /// display name, and UI icon. Create instances via Assets > Create > BADY > Kitchen Object.
    /// </summary>
    [CreateAssetMenu(fileName = "New KitchenObjectSO", menuName = "BADY/Kitchen Object")]
    public sealed class KitchenObjectSO : ScriptableObject
    {
        [SerializeField] private Transform _prefab;
        [SerializeField] private string _objectName;
        [SerializeField] private Sprite _icon;

        /// <summary>
        /// The prefab to instantiate for this kitchen item. Must have a NetworkObject
        /// and KitchenObject component at the root. Registered in the NetworkManager prefab list.
        /// </summary>
        public Transform Prefab => _prefab;

        public string ObjectName => _objectName;

        public Sprite Icon => _icon;
    }
}
