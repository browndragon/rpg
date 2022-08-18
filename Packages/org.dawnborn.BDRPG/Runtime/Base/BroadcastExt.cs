using System;
using UnityEngine;

namespace BDRPG
{
    /// TODO: move to util/.../MonoBehaviours.cs or similar
    public static class BroadcastExt
    {
        public static void Broadcast<T>(this Component thiz, Action<T> action, Predicate<T> only = default)
        {
            only ??= t => true;
            foreach (T t in thiz.GetComponents<T>())
            {
                if (!only(t)) continue;
                action(t);
            }
        }
        public static void BroadcastChildren<T>(this Component thiz, Action<T> action, Predicate<T> only = default)
        {
            only ??= t => true;
            foreach (T t in thiz.GetComponentsInChildren<T>())
            {
                if (!only(t)) continue;
                action(t);
            }
        }
        public static void BroadcastParents<T>(this Component thiz, Action<T> action, Predicate<T> only = default)
        {
            only ??= t => true;
            foreach (T t in thiz.GetComponentsInParent<T>())
            {
                if (!only(t)) continue;
                action(t);
            }
        }
    }
}