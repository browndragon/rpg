// using UnityEngine;

// namespace BDRPG.Turn
// {
//     [Tooltip("Settings & configs for the order of play")]
//     public class Order : Phase
//     {
//         public static Order main => SingletonAsset<Order>.Instance;

//         [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
// #if UNITY_EDITOR
//         [UnityEditor.InitializeOnLoadMethod]
// #endif
//         static void Init() => _ = main;
//         protected override void OnEnable() { base.OnEnable(); SingletonAsset<Order>.SetIfUnset(this); }
//     }
// }
