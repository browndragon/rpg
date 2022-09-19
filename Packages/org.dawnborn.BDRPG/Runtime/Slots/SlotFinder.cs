using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using BDUtil;
using UnityEngine;

namespace BDRPG.Slots
{
    [AddComponentMenu("BDRPG/EquipSlotFinder")]
    [RequireComponent(typeof(IEquip))]
    public class SlotFinder : MonoBehaviour
    {
        [Flags]
        public enum BindEvents
        {
            None = default,
            // Reserve0 = 1 << 0,
            // Reserve1 = 1 << 1,
            Collide2D = 1 << 2,
            Collide3D = 1 << 3
        }
        [Tooltip("Equip on event(s)")]
        public BindEvents DonBindEvent = BindEvents.Collide2D | BindEvents.Collide3D;
        [Tooltip("Unequip on event(s); since equips often follow target, this isn't always sensible.")]
        public BindEvents DoffBindEvent = default;
        [Tooltip("Attempt to bind to each key in order")]
        public List<Key> Keys;

        IEquip Equip;

        [SuppressMessage("IDE", "IDE0051")]
        void Awake() => Equip = GetComponent<IEquip>();
        /// Bind `this` into `slot`; return true if it did or false to abort.
        [SuppressMessage("IDE", "IDE0051")]
        void OnTriggerEnter2D(Collider2D other) => OnBindableEvent(BindEvents.Collide2D, other?.GetComponent<Owner>());
        [SuppressMessage("IDE", "IDE0051")]
        void OnTriggerEnter(Collider other) => OnBindableEvent(BindEvents.Collide3D, other?.GetComponent<Owner>());
        void OnBindableEvent(BindEvents @event, Owner owner)
        {
            if (!DonBindEvent.HasFlag(@event)) return;
            if (owner == null) return;
            foreach (Key key in Keys)
            {
                Slot slot = owner[key];
                if (!slot) continue;
                if (slot.Don(Equip)) return;
            }
        }
    }
}
