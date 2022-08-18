using System.Diagnostics.CodeAnalysis;
using BDUtil.Raw;
using UnityEngine;

namespace BDRPG.Slots
{
    /// A game object which (at least in theory) supports round-tripping through a deflate.
    [AddComponentMenu("BDRPG/Binder")]
    [Tooltip("A gameobject holding slots")]
    public class Fop : MonoBehaviour, Slot.IOwner
    {
        public readonly Slot.Frippery Frippery = new();
        public virtual object CanDon(Slot slot, IEquip equip) => this;
        public virtual object CanDoff(Slot slot, IEquip equip, bool force) => this;
        public virtual void DidDon(Slot slot, IEquip equip, object couldBind) { }
        public virtual void DidDoff(Slot slot, IEquip equip, object couldUnbind) { }

        public T GetT<T>() => (T)(object)GetComponent(typeof(T));
        [SuppressMessage("IDE", "IDE0051")]
        void Awake() => Frippery.SetOwner(this);
    }
}