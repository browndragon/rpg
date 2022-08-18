using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace BDRPG.Flates
{
    [CreateAssetMenu(menuName = "BDRPG/Deflated")]
    [Tooltip("A flatpacked description of a type of game object ('a sword' or 'a goblin')")]
    public class Deflated : Cameos.ScriptableObject
    {
        /// The stateless game object that this Deflate can be turned into.
        public Inflated Inflated;

        [SuppressMessage("IDE", "IDE0051")]
        void OnValidate()
        {
            if (Inflated == null) return;
            var roundTrip = Inflated.Deflated;
            if (roundTrip == null) return;
            if (roundTrip == this) return;
            Debug.LogWarning($"Mismatch: Round trip through {Inflated} is {roundTrip} not this", this);
        }

        public virtual Inflated Acquire() => Instantiate(Inflated);
        public virtual void Release(Inflated inflate)
        {
            if (inflate == null) return;
            Destroy(inflate.gameObject);
        }
    }
}
