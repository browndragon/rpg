using BDUtil;
using UnityEngine;
using UnityEngine.Events;

namespace BDRPG
{
    /// TODO: move to util/.../Channels/Channel.cs -- this doesn't seem to cause overload problems.
    public static class ButtonExt
    {
        public static void Subscribe(this UnityEvent thiz, UnityAction action, Disposes.All unsubscribe)
        {
            thiz.AddListener(action);
            unsubscribe.Add(() => thiz.RemoveListener(action));
        }
    }
}