using System.Collections;
using System.Collections.Generic;

namespace BDRPG.Slots
{
    /// This is ambivalent over whether the same object can be bound to multiple slots
    /// (or indeed, people).
    /// ScriptableObject equips or other _stateless_ equips can be (especially fire or poison...).
    /// MonoBehaviours can't (but since they're stateful, they can then store internal values; see Merge...)
    public interface IEquip
    {
        /// Bind `this` into `slot`; return true if it did or false to abort.
        bool DoBind(Slot slot);
        /// Unbind `this` from `slot`; return true if it did (and if `force`, it must!)
        bool DoUnbind(Slot slot, bool force = false);
    }
}