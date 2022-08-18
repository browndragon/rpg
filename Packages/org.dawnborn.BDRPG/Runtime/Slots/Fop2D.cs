using UnityEngine;

namespace BDRPG.Slots
{
    /// Causes this object to bind to any bindables it runs over.
    [AddComponentMenu("BDRPG/Binder2D")]
    [Tooltip("A gameobject holding slots that autoequips anything it walks on")]
    [RequireComponent(typeof(Fop), typeof(Collider2D))]
    public class Fop2D : MonoBehaviour
    {
        Fop Fop;
        [SuppressMessage("IDE", "IDE0051")]
        void Awake() => Fop = GetComponent<Fop>();
        /// Bind `this` into `slot`; return true if it did or false to abort.
        void OnTriggerEnter2D(Collider2D other)
        {
            Slot.IFinder finder = other.GetComponent<Slot.IFinder>();
            IEquip equip = other.GetComponent<IEquip>();
            if (finder == null) return;
            foreach (Slot slot in finder.GetSlots(Fop.Frippery))
            {
                if (slot.Don(equip)) return;
            }
        }
    }
}