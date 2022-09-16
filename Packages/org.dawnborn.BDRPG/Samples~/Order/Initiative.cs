using System;
using System.Collections;
using BDUtil;
using BDUtil.Pubsub;
using UnityEngine;

namespace BDRPG
{
    /// There's a head/deque of "who's next", maintained in sorted order (so to reorder, you do have to dequeue/reenqueue explicitly!).
    /// Initiative (as a system) processes each of the members in initiative order.
    /// It enters the processing state when it pops an element off; it enters the
    public class Initiative : SingletonAsset<Initiative>
    {
        public interface IActor : ObjsHead.ISortComponent
        {
            bool IsReady { get; }
            /// Called with this object removed from Head, present in Actor.
            /// You _must_ Delay++ if you wish to block the next person from going (such as a human controlled unit)!
            /// This obliges you to Delay-- when you're done.
            /// You _must_ self-reenqueue if you wish to execute again!
            void Act();
        }
        public ObjsHead Head;
        public IActor Actor;
        public Lock Delay;
        public float Time => UnityEngine.Time.time;
        readonly Disposes.All unsubscribe = new();
        protected override void OnEnableSubsystem()
        {
            base.OnEnableSubsystem();
            if (Head) unsubscribe.Add(Head.Subscribe(OnPop));
            unsubscribe.Add(Funcs.MakeSetter(out Func<bool> canceled));
            Coroutines.StartCoroutine(Consume(canceled));
        }
        protected override void OnDisableSubsystem()
        {
            unsubscribe.Dispose();
            base.OnDisableSubsystem();
        }
        void OnPop(GameObject popped) => (Actor = popped.GetComponent<IActor>()).OrThrowInternal().Act();
        IEnumerator Consume(Func<bool> isCanceled)
        {
            bool ReadyToPop() => !isCanceled() && (Head.Peek?.GetComponent<IActor>()?.IsReady ?? false);
            WaitUntil waitUntil = new(() => isCanceled() || ReadyToPop());
            while (true)
            {
                while (ReadyToPop() && Head.Pop()) { }
                if (isCanceled()) yield break;
                yield return waitUntil;
            }
        }
    }
}
