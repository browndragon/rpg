// using System;
// using System.Collections.Generic;
// using BDRPG.Turn;
// using BDUtil;
// using BDUtil.Channels;
// using UnityEngine;
// using UnityEngine.Events;

// namespace BDRPG.Space
// {
//     [AddComponentMenu("BDRPG/Broker")]
//     [Tooltip("specifically forwards events")]
//     public class Broker : Agent
//     {
//         readonly Disposes.All unsubscribe = new();
//         public Map<Phase, UnityEvent> Phases = new();

//         void OnEnable()
//         {
//             foreach ((Phase phase, UnityEvent @event) in Phases) phase.Subscribe(@event, unsubscribe);
//         }
//         void OnDisable() => unsubscribe.Dispose();
//     }

//     public static class Brokers
//     {
//         public static void Subscribe(this Component thiz, Phase phase, Action action, Disposes.All unsubscribe)
//         {
//             thiz.GetComponent<Broker>().Phases[phase].Subscribe(action.Invoke, unsubscribe);
//         }
//     }
// }