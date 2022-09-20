using System;
using System.Collections;
using BDUtil;
using BDUtil.Pubsub;
using UnityEngine;

namespace BDRPG.Space
{
    [Tooltip("Settings & configs for determining the size of a space")]
    public class Metric : SingletonAsset<Metric>
    {
        [SerializeField] Vector3 size = Vector3.one;
        public Vector3 Size
        {
            get => size;
            set { size = value; isDirty = true; }
        }
        [SerializeField] Vector3 offset = Vector3.zero;
        public Vector3 Offset
        {
            get => offset;
            set { offset = value; isDirty = true; }
        }
        [SerializeField] Vector3 rotation = Vector3.zero;
        public Vector3 Rotation
        {
            get => rotation;
            set { rotation = value; isDirty = true; }
        }
        void OnValidate() => isDirty = true;
        bool isDirty;
        Matrix4x4 matrix = Matrix4x4.identity;
        Matrix4x4 inv = Matrix4x4.identity;
        public Matrix4x4 Matrix
        {
            get => isDirty ? RecalculateMatrix() : matrix;
            set
            {
                offset = value.GetPosition();
                size = value.lossyScale;
                rotation = value.rotation.eulerAngles;
                isDirty = false;
                matrix = value;
            }
        }
        Matrix4x4 RecalculateMatrix()
        {
            matrix = default;
            matrix.SetTRS(offset, Quaternion.Euler(rotation), size);
            inv = matrix.inverse;
            isDirty = false;
            return matrix;
        }

        public Vector3 GetGridInterpolatedFromWorld(Vector3 world)
        => matrix * world;
        public Vector3Int GetGridFromWorld(Vector3 world)
        => Vector3Int.RoundToInt(GetGridInterpolatedFromWorld(world));
        public Vector3 GetWorldFromGrid(Vector3 grid)
        => inv * grid;
        public Vector3 GetWorldFromGridInt(Vector3Int grid)
        => inv * (Vector3)grid;
    }
}
