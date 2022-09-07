// using System;
// using System.Collections;
// using System.Collections.Generic;
// using BDRPG.Space;
// using BDUtil;
// using BDUtil.Bind;
// using BDUtil.Channels;
// using UnityEngine;
// using UnityEngine.Pool;
// using UnityEngine.SceneManagement;

// namespace BDRPG.Turn
// {
//     [AddComponentMenu("BDRPG/Agent")]
//     [Tooltip("Subscribes to phases (or a whole round) & forwards their events.")]
//     public class Agent : MonoBehaviour
//     {
//         internal readonly Disposes.All unsubscribe = new();
//         void OnEnable()
//         {
//             Order.main.Subscribe(this);
//         }
//         void OnDisable() => unsubscribe.Dispose();
//         internal void Offer(Phase phase)
//         {

//         }
//     }
// }
