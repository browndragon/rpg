using System.Collections;
using BDUtil;
using BDUtil.Pubsub;
using UnityEngine;

namespace BDRPG
{
    public class RunOrder : SingletonAsset<RunOrder>
    {
        public ObjsHead Head;
        public bool Continue;
        readonly Disposes.All unsubscribe = new();
        protected override void OnEnableSubsystem()
        {
            base.OnEnableSubsystem();
            if (Head) unsubscribe.Add(Head.Subscribe(OnPop));
            if (!Continue) Coroutines.StartCoroutine(Consume());
        }
        protected override void OnDisableSubsystem()
        {
            unsubscribe.Dispose();
            Continue = false;
            base.OnDisableSubsystem();
        }
        void OnPop(GameObject popped)
        {
            popped.GetComponent<Initiative>().Act();
        }
        bool MRE()
        => !Continue
        || (Head?.Peek?.GetComponent<Initiative>().NextAct ?? float.PositiveInfinity) <= Time.time;
        IEnumerator Consume()
        {
            Continue = true;
            while (Continue)
            {
                while (MRE() && Head.Pop()) { }
                yield return new WaitUntil(MRE);
            }
        }

    }
}
