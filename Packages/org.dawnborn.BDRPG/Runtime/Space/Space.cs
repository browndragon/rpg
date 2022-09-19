// using System;
// using BDUtil;
// using UnityEngine;

// namespace BDRPG.Space
// {
//     [Tooltip("Settings & configs for a singleton space instance")]
//     public class Space : SingletonAsset<Space>
//     {
//         [SerializeReference, Subtype]
//         public IMetric Metric = new GridMetric(Vector2.one, Vector2.zero);
//         public interface IMetric
//         {
//             public Vector2Int GetGrid(Vector2 point);
//             public Vector2 GetWorld(Vector2Int grid);
//         }
//         public static Vector2Int GetGrid(Vector2 point) => main.Metric.GetGrid(point);
//         public static Vector2 GetWorld(Vector2Int grid) => main.Metric.GetWorld(grid);


//         [Serializable]
//         public struct GridMetric : IMetric
//         {
//             public Vector2 Resolution;
//             Vector2 InvResolution;
//             public Vector2 Offset;
//             public GridMetric(Vector2 resolution, Vector2 offset)
//             {
//                 Resolution = resolution;
//                 InvResolution = new(1f / resolution.x, 1f / resolution.y);
//                 Offset = offset;
//             }
//             public Vector2Int GetGrid(Vector2 point)
//             {
//                 point -= Offset;
//                 point.Scale(Resolution);
//                 return point.AsInt();
//             }
//             public Vector2 GetWorld(Vector2Int grid)
//             {
//                 Vector2 world = grid;
//                 world.Scale(InvResolution);
//                 world += Offset;
//                 return world;
//             }
//         }
//     }
// }
