namespace BDRPG.Slots
{
    /// This is ambivalent over whether the same object can be bound to multiple slots
    /// (or indeed, owners).
    /// ScriptableObject equips or other _stateless_ equips can be (especially fire or poison...).
    /// MonoBehaviours can't (but since they're stateful, they can then store internal values; see Merge...)
    public interface IEquip
    {
        /// Bind `this` into `slot`; return non-null if it did or null to abort.
        bool DoBind(Slot slot);
        /// Clear `this` from `slot`.
        /// This can't be a partial removal (that's some sort of "bind a negative value" to trigger DoRebind).
        /// It's allowed to return false if force is false; if so, it *didn't* unbind.
        bool DoUnbind(Slot slot, bool force);

        /// Called on old `this` whenever `@new` tries to bind to `slot`.
        /// Generally return `false` if the situation shouldn't change (because @new was not equipped),
        /// and `true` if any of:
        /// * the new equip replaces the old equip (changing weapons): `return slot.Doff(force:true) && slot.Don(@new)`
        /// * the old equip merges the new values (merging poison counters): `return (slot.Value += @new.Value) <= 0 ? slot.Doff(force:true) : true;`
        ///   * This can be arbitrarily complex.
        /// * Special case, the old equip suppresses the new equip (grabbing 2 mushrooms in mario): `return true`.
        bool DoRebind(Slot slot, IEquip @new);
    }
}