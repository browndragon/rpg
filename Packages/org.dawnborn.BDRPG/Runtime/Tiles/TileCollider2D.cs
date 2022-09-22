using System.Collections.Generic;
using BDUtil;
using BDUtil.Math;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace BDRPG
{
    /// forwards tilemap collisions into tile collisions.
    /// Not actually itself a collider, but requires a tilemapcollider, and internally stores the collision info.
    [AddComponentMenu("BDRPG/TileCollider2D")]
    [RequireComponent(typeof(TilemapCollider2D), typeof(Tilemap))]
    public class TileCollider2D : MonoBehaviour
    {
        Tilemap tilemap;
        Grid grid;
        TilemapCollider2D tilemapCollider2D;
        readonly Dictionary<Collider2D, List<Vector3Int>> enters = new();
        readonly Dictionary<Collider2D, List<Vector3Int>> stays = new();
        readonly Dictionary<Collider2D, List<Vector3Int>> exits = new();

        void Awake()
        {
            tilemap = GetComponent<Tilemap>();
            grid = tilemap.layoutGrid;
            tilemapCollider2D = GetComponent<TilemapCollider2D>();
            stays.Clear();
            enters.Clear();
            exits.Clear();
        }

        // Supports two tilecollider2ds BOTH colliding.
        // True if we're the only TTC2D OR the other one has a higher instance id, so we're _better_.
        bool SymmetricWinner(Collider2D other)
        {
            var collider = other.GetComponent<TileCollider2D>();
            return collider == null || GetInstanceID() < collider.GetInstanceID();
        }

        public struct TileCollision
        {
            public TileCollision(bool isCollision, TileCollider2D tilemapTileCollider2D, Collider2D intersectingCollider)
            {
                this.isCollision = isCollision;
                this.tilemapTileCollider2D = tilemapTileCollider2D;
                this.intersectingCollider = intersectingCollider;
                thisIsTilemap = false;
            }
            /// Whether this was a collision or a trigger.
            public bool isCollision;
            /// tilemap of the only (or "winning") tilemap involved.
            public TileCollider2D tilemapTileCollider2D;
            public Tilemap tilemap => tilemapTileCollider2D.tilemap;  // Attached to the tilemapCollider.
            public Grid layoutGrid => tilemapTileCollider2D.grid;  // The space for the individual Contacts.
            public TilemapCollider2D tilemapCollider => tilemapTileCollider2D.tilemapCollider2D;  // One of collider/otherCollider.
            public Rigidbody2D tilemapRigidbody => tilemapCollider.attachedRigidbody;

            public Collider2D intersectingCollider;
            public Rigidbody2D intersectingRigidbody => intersectingCollider.attachedRigidbody;

            public IReadOnlyList<Vector3Int> Enters => tilemapTileCollider2D.enters[intersectingCollider];
            public IReadOnlyList<Vector3Int> Stays => tilemapTileCollider2D.stays[intersectingCollider];
            public IReadOnlyList<Vector3Int> Exits => tilemapTileCollider2D.exits[intersectingCollider];

            public bool thisIsTilemap;
            public Collider2D collider => thisIsTilemap ? tilemapCollider : intersectingCollider;
            public Collider2D otherCollider => thisIsTilemap ? intersectingCollider : tilemapCollider;
            public Rigidbody2D rigidbody => thisIsTilemap ? tilemapRigidbody : intersectingRigidbody;
            public Rigidbody2D otherRigidbody => thisIsTilemap ? intersectingRigidbody : tilemapRigidbody;

            public override string ToString() => $"{tilemap}!{intersectingCollider}+{Enters.Summarize()}~{Stays.Summarize()}-{Exits.Summarize()}";
        }

        readonly List<Vector3Int> scratch = new();
        void UpdateSets(bool isCollision, Collider2D other, List<Vector3Int> enters, List<Vector3Int> stays, List<Vector3Int> exits)
        {
            stays.AddRange(enters);
            enters.Clear();
            exits.Clear();

            BoundsInt bounds = default;
            bounds.SetMinMax(
                Vector3Int.FloorToInt(other.bounds.min),
                Vector3Int.CeilToInt(other.bounds.max.WithZ(other.bounds.max.z > 0f ? other.bounds.max.z : 1f))
            );
            foreach (Vector3Int cell in bounds.allPositionsWithin)
            {
                if (!tilemap.HasTile(cell)) continue;
                var cellWorldCenter = grid.CellToWorld(cell);
                var otherClosestPoint = other.ClosestPoint(cellWorldCenter);
                var otherClosestCell = grid.WorldToCell(otherClosestPoint);
                if (otherClosestCell != cell) continue;

                if (!stays.Contains(cell)) enters.Add(cell);
                else scratch.Add(cell);  // Since it's already in stays, note we need to keep it.
            }
            for (int i = stays.Count - 1; i >= 0; --i)
            {
                if (enters.Contains(stays[i])) continue;
                if (scratch.Contains(stays[i])) continue;
                exits.Add(stays[i]);
                stays.RemoveAt(i);
            }
            exits.Reverse();
            scratch.Clear();

            TileCollision collision = new(isCollision, this, other);
            if (enters.Count > 0)
            {
                collision.thisIsTilemap = false;
                other.SendMessage("OnTileEnter", collision, SendMessageOptions.DontRequireReceiver);
                collision.thisIsTilemap = true;
                SendMessage("OnTileEnter", collision, SendMessageOptions.DontRequireReceiver);
            }
            if (stays.Count > 0)
            {
                collision.thisIsTilemap = false;
                other.SendMessage("OnTileStay", collision, SendMessageOptions.DontRequireReceiver);
                collision.thisIsTilemap = true;
                SendMessage("OnTileStay", collision, SendMessageOptions.DontRequireReceiver);
            }
            if (exits.Count > 0)
            {
                collision.thisIsTilemap = false;
                other.SendMessage("OnTileExit", collision, SendMessageOptions.DontRequireReceiver);
                collision.thisIsTilemap = true;
                SendMessage("OnTileExit", collision, SendMessageOptions.DontRequireReceiver);
            }
        }

        void OnTileEnterImpl(bool isCollision, Collider2D collider)
        {
            if (!SymmetricWinner(collider.OrThrow())) return;
            UpdateSets(isCollision, collider, enters[collider] = new(), stays[collider] = new(), exits[collider] = new());
        }
        void OnTileStayImpl(bool isCollision, Collider2D collider)
        {
            if (!SymmetricWinner(collider.OrThrow())) return;
            /// It seems that sometimes we can get an OnTileStay without enter.
            /// Theorizing that those represent some kind of mixup wrt Enters & Exits same turn, let's suppress such events.
            if (!enters.ContainsKey(collider)) return;
            UpdateSets(isCollision, collider, enters[collider], stays[collider], exits[collider]);
        }
        void OnTileExitImpl(bool isCollision, Collider2D collider)
        {
            if (!SymmetricWinner(collider.OrThrow())) return;
            /// It seems that sometimes we can get an OnTileExit without enter.
            /// Theorizing that those represent some kind of mixup wrt Enters & Exits same turn, let's suppress such events.
            if (!enters.ContainsKey(collider)) return;

            exits[collider].Clear();
            exits[collider].AddRange(enters[collider]);
            enters.Clear();
            exits[collider].AddRange(stays[collider]);
            stays.Clear();

            TileCollision collision = new(isCollision, this, collider);
            collider.SendMessage("OnTileExit", collision, SendMessageOptions.DontRequireReceiver);
            collision.thisIsTilemap = true;
            SendMessage("OnTileExit", collision, SendMessageOptions.DontRequireReceiver);

            enters.Remove(collider);
            stays.Remove(collider);
            exits.Remove(collider);
        }

        void OnCollisionEnter2D(Collision2D other) => OnTileEnterImpl(true,
            other.collider == this.tilemapCollider2D ?
            other.otherCollider : other.collider
        );
        void OnCollisionStay2D(Collision2D other) => OnTileStayImpl(true,
            other.collider == this.tilemapCollider2D ?
            other.otherCollider : other.collider
        );
        void OnCollisionExit2D(Collision2D other) => OnTileExitImpl(true,
            other.collider == this.tilemapCollider2D ?
            other.otherCollider : other.collider
        );

        void OnTriggerEnter2D(Collider2D other) => OnTileEnterImpl(false, other);
        void OnTriggerStay2D(Collider2D other) => OnTileStayImpl(false, other);
        void OnTriggerExit2D(Collider2D other) => OnTileExitImpl(false, other);
    }
}
