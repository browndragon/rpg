using System;
using System.Collections;
using System.Collections.Generic;
using BDUtil;

namespace BDRPG.Slots
{
    /// NOT serializable; too recursive -- has a reference to the full Slot.Binder.
    public readonly struct Slot
    {
        readonly IFop Root;
        public readonly string Name;

        Slot(IFop root, string name) : this()
        {
            Root = root;
            Name = name;
        }

        public Slot WithName(string name) => new(Root, name);

        public bool IsValid => Root != null;
        public static implicit operator bool(Slot s) => s.IsValid;

        public bool GetEquip(out IEquip bound) => Root.GetEquip(Name, out bound);
        public bool Don(IEquip bound) => Root.Don(Name, bound);
        public bool Doff(out IEquip bound, bool force) => Root.Doff(Name, out bound, force);
        public T GetT<T>() => Root.Owner.GetT<T>();

        /// A thing which can own slots, being actually bound or unbound.
        /// Almost invariably a MonoBehaviour.
        public interface IOwner
        {
            /// Get "this", other components, etc. To be used from the Bound to find out context.
            T GetT<T>();

            /// Assumes the slot is empty (if it isn't, maybe they can merge, but they can't bind).
            /// Called before IBound.DoBind; you don't need to (& shouldn't) call it.
            /// If you return non-null, binding will continue (but might abort in IBound!); false, it will abort now.
            object CanDon(Slot slot, IEquip equip);
            /// Gets the slot, bound, and retval of CanBind after checking with the IBound that it's legal.
            void DidDon(Slot slot, IEquip equip, object couldDon);

            /// As CanBind, but in the other direction.
            /// Some things can't be unbound in the current circumstances (like a cursed weapon); force means "even so, take it off" (like curse removal).
            object CanDoff(Slot slot, IEquip equip, bool force);
            /// Warning: You get back whatever you passed to CanUnbind...
            void DidDoff(Slot slot, IEquip equip, object couldDoff);
        }
        /// Given a binder, finds slots meeting our own criteria
        public interface IFinder
        {
            IEnumerable<Slot> GetSlots(Frippery binder);
            IEquip Equip { get; }
        }
        /// Special handling for sharing a slot after equipping.
        public interface IMerge
        {
            /// If true, unequip old and, if that succeeds, equip new.
            /// If it fails, nothing happens (not even merge).
            bool IsReplace { get; }
            /// If !IsReplace, this gets called on Old with the New value to be merged.
            bool DoMerge(Slot slot, IMerge other);
        }
        /// Internal interface: a slot's view of the binder, access to the stringly-typed methods.
        interface IFop
        {
            IOwner Owner { get; }
            bool GetEquip(string name, out IEquip bound);
            bool Don(string name, IEquip bound);
            bool Doff(string name, out IEquip bound, bool force);
        }

        /// A root-slot which owns leaf sub-slots (which can be bound to)
        /// This *doesn't* support things like inventory; it's specifically here to support equipping items & status effects.
        /// Inventory just uses Flates (which a Bound might also be).
        [Serializable]
        public class Frippery : IFop, IReadOnlyCollection<KeyValuePair<Slot, IEquip>>
        {
            public IOwner Owner { get; private set; }
            public Frippery SetOwner(IOwner thiz) { Owner = thiz; return this; }

            readonly Dictionary<string, IEquip> equips = new();
            public int Count => equips.Count;
            public IEnumerator<KeyValuePair<Slot, IEquip>> GetEnumerator()
            {
                foreach ((string slot, IEquip equip) in equips) yield return new(new(this, slot), equip);
            }
            public Slot GetSlot(string name) => new(this, name);
            public Slot GetEmptySlot(string name) => equips.ContainsKey(name) ? default : GetSlot(name);
            public void DoffAll()
            {
                foreach ((string name, IEquip binding) in equips)
                {
                    Slot slot = GetSlot(name);
                    object couldUnbind = Owner.CanDoff(slot, binding, true).OrThrow();
                    binding.DoUnbind(slot, true).OrThrow();
                    Owner.DidDoff(slot, binding, couldUnbind);
                }
                equips.Clear();
            }

            bool IFop.GetEquip(string name, out IEquip bound) => equips.TryGetValue(name, out bound);
            bool IFop.Don(string name, IEquip bind)
            {
                Slot slot = GetSlot(name);
                if (equips.TryGetValue(name, out IEquip had))
                {
                    if (had is not IMerge hadMerge) return false;
                    if (!hadMerge.IsReplace) return hadMerge.DoMerge(slot, (IMerge)bind);
                    IFop thiz = this;
                    thiz.Doff(name, out var _, true).OrThrow();
                    // Fallthrough intentional!
                    // Having unbound the previous, now we're ready to bind the new.
                }
                object couldBind = Owner.CanDon(slot, bind);
                if (couldBind == null) return false;
                if (!bind.DoBind(slot))
                {
                    Owner.DidDoff(slot, bind, Owner.CanDoff(slot, bind, true));
                    return false;
                }
                equips[name] = bind;
                Owner.DidDon(slot, bind, couldBind);
                return true;
            }
            bool IFop.Doff(string name, out IEquip bound, bool force)
            {
                Slot slot = GetSlot(name);
                bound = equips[name];
                object couldUnbind = Owner.CanDoff(slot, bound, force);
                if (couldUnbind == null && !force) return false;
                if (!bound.DoUnbind(slot, force) && !force) return false;
                equips.Remove(name).OrThrow();
                Owner.DidDoff(slot, bound, couldUnbind);
                return true;
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}