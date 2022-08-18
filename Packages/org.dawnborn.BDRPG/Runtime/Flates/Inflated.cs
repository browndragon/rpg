using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace BDRPG.Flates
{
    /// A game object which (at least in theory) supports round-tripping through a deflate.
    [AddComponentMenu("BDRPG/Inflated")]
    [Tooltip("A gameobject which supports inflation/deflation back through a flatpack type")]
    public class Inflated : MonoBehaviour, Cameos.IAm
    {
        public Deflated Deflated;
        [SuppressMessage("IDE", "IDE0051")]
        void OnValidate()
        {
            if (Deflated == null) return;
            var roundTrip = Deflated.Inflated;
            if (roundTrip == null) return;
            if (roundTrip == this) return;
            Debug.LogWarning($"Mismatch: Round trip through {Deflated} is {roundTrip} not this", this);
        }

        public Sprite Portrait => Deflated.Portrait;
        public string DisplayName => Deflated.DisplayName;
        public string Description => Deflated.Description;
    }
}
