using System.Collections.Generic;
using UnityEngine;

namespace BDRPG.Slots
{
    [AddComponentMenu("BDRPG/Slots/Owner")]
    [Tooltip("A gameobject holding slots.")]
    public class Owner : MonoBehaviour
    {
        public bool Destroying = false;
        readonly Dictionary<Key, IEquip> equips = new();
        readonly Dictionary<Key, object> values = new();

        protected internal IDictionary<Key, IEquip> Equips => equips;
        protected internal IDictionary<Key, object> Values => values;

        public ICollection<Key> Keys => Equips.Keys;
        public Slot this[Key key] => new(this, key);

        protected virtual bool CanDon(Key key, IEquip equip) => !Destroying && enabled;
        protected virtual bool CanDoff(Key key, IEquip equip, bool force) => true;
        protected virtual void AbortDon(Key key, IEquip equip) => Values.Remove(key);
        protected virtual void DidDon(Key key, IEquip equip) => Equips[key] = equip;
        protected virtual void DidDoff(Key key, IEquip equip)
        {
            Values.Remove(key);
            Equips.Remove(key);
        }
        internal bool Don(Key key, IEquip equip)
        {
            Slot slot = this[key];
            if (Equips.TryGetValue(key, out IEquip had)) return had.DoRebind(slot, equip);
            if (!CanDon(key, equip)) return false;
            if (!equip.DoBind(slot))
            {
                AbortDon(key, equip);
                return false;
            }
            DidDon(key, equip);
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
        public void DoffAll()
        { foreach (Key key in new List<Key>(Keys)) Doff(key, force: true); }
        void PreDestroy()
        {
            Destroying = true;
            DoffAll();
        }
    }
}