using System.Collections.Generic;
using BDUtil.Serialization;
using UnityEngine;

namespace BDRPG.Slots
{
    [AddComponentMenu("BDRPG/Slots/SlotOwner")]
    [Tooltip("A gameobject holding slots.")]
    public class Owner : MonoBehaviour
    {
        readonly Dictionary<Key, IEquip> equips = new();
        readonly Dictionary<Key, object> values = new();

        protected internal IDictionary<Key, IEquip> Equips => equips;
        protected internal IDictionary<Key, object> Values => values;

        public ICollection<Key> Keys => Equips.Keys;
        public Slot this[Key key] => new(this, key);

        protected virtual bool CanDon(Key key, IEquip equip) => true;
        protected virtual bool CanDoff(Key key, IEquip equip, bool force) => true;
        protected virtual void AbortDon(Key key, IEquip equip) => Values.Remove(key);
        protected virtual void DidDon(Key key, IEquip equip) => Equips[key] = equip;
        protected virtual void DidDoff(Key key, IEquip equip)
        {
            Values.Remove(key);
            Equips.Remove(key);
        }
        internal bool Don(Key key, IEquip bind)
        {
            Slot slot = this[key];
            if (Equips.TryGetValue(key, out IEquip had)) return had.DoRebind(slot, bind);
            if (!CanDon(key, bind)) return false;
            if (!bind.DoBind(slot))
            {
                AbortDon(key, bind);
                return false;
            }
            DidDon(key, bind);
            return true;
        }
        internal bool Doff(Key key, bool force)
        {
            Slot slot = this[key];
            if (!Equips.TryGetValue(key, out IEquip had)) return false;
            if (!CanDoff(key, had, force)) return false;
            if (!had.DoUnbind(slot, force)) return false;
            DidDoff(key, had);
            return true;
        }
    }
}