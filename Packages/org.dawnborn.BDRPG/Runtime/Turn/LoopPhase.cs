// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.Analytics;

// namespace BDRPG.Turn
// {
//     [CreateAssetMenu(menuName = "BDRPG/LoopPhase")]
//     [Tooltip("Notify self, then each other phase in order")]
//     public class LoopPhase : Phase
//     {
//         [Tooltip("Whether to loop at each call to begin")]
//         public bool Loop;

//         [Tooltip("In-code whether the current call to Begin should continue")]
//         public bool Continue { get; set; }

//         public override IEnumerator Begin()
//         {
//             Continue = Loop;
//             while (Continue) { yield return base.Begin(); }
//         }
//     }
// }