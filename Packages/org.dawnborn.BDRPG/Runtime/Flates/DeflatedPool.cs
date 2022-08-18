using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace BDRPG.Flates
{
    [CreateAssetMenu(menuName = "BDRPG/DeflatedPool")]
    [Tooltip("A flatpacked game object which supports instance pooling & recycling")]
    public class DeflatedPool : Deflated
    {
        [Tooltip("Number of instances to preallocate & maintain queue size")]
        public int Prealloc = 3;
        [Tooltip("Retain elements beyond prealloc to ensure cache of BurstSize")]
        public int BurstSize = 1;
        [Tooltip("Allocate new elements when cache empty up to MaxOutstanding")]
        public int MaxOutstanding = 10;

        int Outstanding = 0;
        readonly Stack<Inflated> Inflates = new();

        [SuppressMessage("IDE", "IDE0051")]
        void OnEnable()
        {
            if (!Application.isPlaying) return;
            while (Inflates.Count < Prealloc)
            {
                var inflate = base.Acquire();
                inflate.gameObject.SetActive(false);
                Inflates.Push(inflate);
            }
        }
        [SuppressMessage("IDE", "IDE0051")]
        void OnDisable()
        {
            foreach (var inflate in Inflates) base.Release(inflate);
            Inflates.Clear();
            Outstanding = 0;
        }
        public override Inflated Acquire()
        {
            if (!Inflates.TryPop(out var inflate))
            {
                if (Outstanding >= MaxOutstanding) return null;
                inflate = base.Acquire();
            }
            Outstanding++;
            inflate.gameObject.SetActive(true);
            return inflate;
        }
        public override void Release(Inflated inflate)
        {
            if (inflate == null) return;
            inflate.gameObject.SetActive(false);
            Outstanding--;
            if (Inflates.Count + Outstanding < Prealloc) Inflates.Push(inflate);
            else if (Inflates.Count < BurstSize) Inflates.Push(inflate);
            else base.Release(inflate);
        }
    }
}
