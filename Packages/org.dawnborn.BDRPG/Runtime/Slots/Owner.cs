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
            if (Equips.TryGetValue(key, out IEquip had))
            {
                Debug.Log($"Doing rebind instead of bind", this);
                return had.DoRebind(slot, equip);
            }
            if (!CanDon(key, equip))
            {
                Debug.Log($"Aborting bind because we don't want to", this);
                return false;
            }
            if (!equip.DoBind(slot))
            {
                Debug.Log($"Aborting bind _at their request_", this);
                AbortDon(key, equip);
                return false;
            }
            DidDon(key, equip);
            return true;
        }
        internal bool Doff(Key key, bool force)
        {
            Slot slot = this[key];
            if (!Equips.TryGetValue(key, out IEquip had))
            {
                Debug.Log($"Can't doff missing {key}", this);
                return false;
            }
            if (!CanDoff(key, had, force))
            {
                Debug.Log($"!{this}.CanDoff({key},{had},{force}), my choice", this);
                return false;
            }
            if (!had.DoUnbind(slot, force))
            {
                Debug.Log($"!{key}->{had}.DoUnbind({slot},{force}), their choice");
                return false;
            }
            Debug.Log($"{this}.DidDoff({key},{had}", this);
            DidDoff(key, had);
            return true;
        }
        public void DoffAll()
        {
            foreach (Key key in new List<Key>(Keys)) Doff(key, force: true);
        }
        void PreDestroy()
        {
            Destroying = true;
            DoffAll();
        }
    }
}