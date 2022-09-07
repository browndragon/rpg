using BDUtil;
using UnityEngine;

namespace BDRPG.Cameos
{
    [CreateAssetMenu(menuName = "BDRPG/Cameo")]
    public class ScriptableObject : UnityEngine.ScriptableObject, IAm
    {
        [field: SerializeField] public Sprite Portrait { get; protected set; }
        [field: SerializeField] protected string OverrideDisplayName { get; set; }
        public string DisplayName => OverrideDisplayName.IsEmpty() ? name : OverrideDisplayName;
        [field: SerializeField] public string Description { get; }
    }
}
