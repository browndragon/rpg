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
        /// This can't be a partial removal (that's some sort of "bind a negative value" to trigger merge behaviour).
        /// It's allowed to return false;
        bool DoUnbind(Slot slot, bool force);

        /// Called on old `this` when `@new` tries to bind to `slot`.
        /// Generally return `false` if the situation shouldn't change (because @new was not equipped),
        /// and `true` if any of:
        /// * it replaced the old equip (changing weapons): `slot.Doff(out var _, true); slot.Don(@new); return true`
        /// * the old equip merged its values (merging poison values): `slot.
        /// * the old equip swallowed the new one (grabbing 2 mushrooms in mario)
        bool DoRebind(Slot slot, IEquip @new);
    }
}