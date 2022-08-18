using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using BDUtil;
using UnityEngine;

namespace BDRPG.Slots
{
    [RequireComponent(typeof(IEquip))]
    public class SlotFinder : MonoBehaviour, Slot.IFinder
    {
        IEquip Equip;
        IEquip Slot.IFinder.Equip => Equip;
        public List<string> Slots;
        public bool EmptyOnly = false;

        [SuppressMessage("IDE", "IDE0051")]
        void Awake() => Equip = GetComponent<IEquip>();
        IEnumerable<Slot> Slot.IFinder.GetSlots(Slot.Frippery frippery)
        {
            foreach (string name in Slots)
            {
                Slot slot = frippery.GetSlot(name);
                if (!slot) continue;
                if (EmptyOnly && slot.GetEquip(out var _)) continue;
                yield return slot;
            }
        }
    }
}
