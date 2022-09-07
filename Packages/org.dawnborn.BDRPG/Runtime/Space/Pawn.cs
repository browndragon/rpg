using System.Collections.Generic;
using BDUtil;
using UnityEngine;

namespace BDRPG.Space
{
    /// An object which takes up space.
    public class Pawn : MonoBehaviour
    {
        public bool isTrigger;
        public Vector2 offset;

        public Vector2Int position;
        public int size;
        public RectInt bounds => new(position, size * Vector2Int.one);

        public void SyncToTransform() => transform.position = (Space.main.Metric.GetWorld(position) - offset).WithZ(transform.position.z);
        public void SyncFromTransform() => position = Space.main.Metric.GetGrid(transform.position.AsTwo() + offset);

        /// You can have as many listeners as you'd like; all get notified when states begin/end.
        public interface IListener
        {
            /// Called after two things come into contact.
            void OnCollisionStart(Pawn other);
            /// Called after two things are no longer in contact.
            void OnCollisionEnd(Pawn other);
        }
        IListener[] listeners;

        /// You get at most one controller; it gets to mediate OnCollision.
        [DisallowMultipleComponent]
        public abstract class Controller : MonoBehaviour
        {
            public abstract bool AdjustCollisionStart(Pawn other);
            public abstract bool AdjustCollisionEnd(Pawn other);
        }
        Controller controller;

        void Awake()
        {
            listeners = GetComponentsInChildren<IListener>();
            controller = GetComponent<Controller>();
        }


    }
}
