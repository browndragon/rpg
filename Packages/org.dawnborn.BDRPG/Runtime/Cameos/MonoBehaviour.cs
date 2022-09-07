using BDUtil;
using UnityEngine;

namespace BDRPG.Cameos
{
    [AddComponentMenu("BDRPG/Cameo")]
    public class MonoBehaviour : UnityEngine.MonoBehaviour, IAm
    {
        [field: SerializeField] public Sprite Portrait { get; protected set; }
        [field: SerializeField] protected string OverrideDisplayName { get; set; }
        public string DisplayName => OverrideDisplayName.IsEmpty() ? name : OverrideDisplayName;
        [field: SerializeField] public string Description { get; }
    }
}
