using System;
using UnityEngine;

namespace BDRPG.Slots
{
    /// A game object which (at least in theory) supports round-tripping through a deflate.
    [CreateAssetMenu(menuName = "BDRPG/Slots/SlotKey")]
    [Tooltip("Identifies slots in the context of an Owner. Dictionary key-able. ")]
    public class Key : ScriptableObject, IEquatable<Key>
    {
        public virtual bool Equals(Key other) => this == other;
        public override bool Equals(object obj) => obj is Key other && Equals(other);
        public override int GetHashCode() => base.GetHashCode();
    }
}