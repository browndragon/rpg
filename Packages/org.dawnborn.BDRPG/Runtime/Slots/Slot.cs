namespace BDRPG.Slots
{
    /// NOT serializable; too recursive -- has a reference to the full Slot.Binder.
    public readonly struct Slot
    {
        public readonly Owner Owner;
        public readonly Key Key;

        internal Slot(Owner owner, Key key) : this()
        {
            Owner = owner;
            Key = key;
        }
        public bool IsValid => Owner != null;
        public static implicit operator bool(Slot s) => s.IsValid;

        public IEquip Equip => Owner.Equips.TryGetValue(Key, out var v) ? v : default;
        public object Value
        {
            get => Owner.Values.TryGetValue(Key, out var v) ? v : default;
            set => Owner.Values[Key] = value;
        }
        public bool Don(IEquip bound) => Owner.Don(Key, bound);
        public bool Doff(bool force = false) => Owner.Doff(Key, force);
        public override string ToString() => $"{Owner}[{Key}]";
    }
}