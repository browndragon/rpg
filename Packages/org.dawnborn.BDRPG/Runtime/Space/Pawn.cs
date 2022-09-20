using BDUtil;
using UnityEngine;

namespace BDRPG.Space
{
    /// An object which takes up space.
    public class Pawn : MonoBehaviour
    {
        [SerializeField] Vector3 positionInterpolated;
        public Vector3Int Position
        {
            get => Vector3Int.RoundToInt(positionInterpolated);
            set => positionInterpolated = (Vector3)value.Let(isDirty = true);
        }
        public Vector3 PositionInterpolated
        {
            get => positionInterpolated;
            set => positionInterpolated = value.Let(isDirty = true);
        }
        bool isDirty;
    }
}
