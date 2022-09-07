// using System;
// using System.Collections;
// using System.Collections.Generic;
// using BDUtil;
// using UnityEngine;

// namespace BDRPG.Turn
// {
//     [CreateAssetMenu(menuName = "BDRPG/Phase")]
//     [Tooltip("Settings & configs for a phase of a round: Untap, Upkeep, etc. Use Await to delay exit from the phase.")]
//     public class Phase : BDUtil.Channels.Channel
//     {
//         [Tooltip("Additional phase(s) to call _before_ `this`.")]
//         public List<Phase> Phases = new();

//         public Phase CurrentPhase { get; private set; }

//         public int Awaiting { get; private set; }

//         protected virtual void OnEnable() => Awaiting = 0;
//         protected virtual void OnDisable() { }

//         public bool IsAwaiting() => Awaiting > 0;
//         public void BeginAwait(int count = 1) => Awaiting += count;
//         public void EndAwait(int count = 1) => Awaiting -= count;

//         public virtual IEnumerator Begin()
//         {
//             foreach (Phase phase in Phases)
//             {
//                 yield return phase.Begin();
//             }
//             CurrentPhase = this;
//             Invoke();
//             yield return new WaitWhile(IsAwaiting);
//         }
//         internal virtual void Subscribe(Agent agent)
//         {
//             foreach (Phase phase in Phases) phase.Subscribe(agent);
//             agent.Offer(this);
//         }
//     }
// }