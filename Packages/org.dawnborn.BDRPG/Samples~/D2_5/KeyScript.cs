using System.Collections;
using System.Collections.Generic;
using BDRPG.Slots;
using BDUtil.Math;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace BDRPG
{
    public class KeyScript : MonoBehaviour, IEquip
    {
        public List<TileBase> KillTiles = new();
        public bool CanBind = true;
        public bool IsBound => transform.parent != null;
        public bool DoBind(Slot slot)
        {
            if (IsBound) return false;
            Transform newParent = slot.Owner.transform;
            if (newParent == null) return false;
            transform.SetParent(newParent);
            StartCoroutine(MoveToHead(adjust: new(0f, 0f, -1f)));
            return true;
        }
        public bool DoUnbind(Slot slot, bool force = false)
        {
            if (!IsBound) return false;
            transform.SetParent(null, true);
            StartCoroutine(BlinkIFrames(Time.time + 1f));
            return true;
        }
        public bool DoRebind(Slot slot, IEquip @new)
        {
            slot.Doff(force: true);
            slot.Don(@new);
            return true;
        }

        void OnTileEnter(TileCollider2D.TileCollision collision)
        {
            foreach (Vector3Int cell in collision.Enters)
            {
                TileBase atCell = collision.tilemap.GetTile(cell);
                if (KillTiles.Contains(atCell))
                {
                    collision.tilemap.SetTile(cell, null);
                    collision.tilemapCollider.ProcessTilemapChanges();
                }
            }
        }

        IEnumerator MoveToHead(float delay = .25f, Vector3 adjust = default)
        {
            Vector3 startPos = transform.position;
            for (float start = Time.time, elapsed = 0f; elapsed < delay; elapsed = Time.time - start)
            {
                if (this == null) yield break;

                transform.position = Vector3.Lerp(startPos, transform.parent.position + adjust, elapsed / delay);
                yield return null;
            }
            while (this != null && transform.parent != null)
            {
                transform.position = transform.parent.position + adjust;
                yield return null;
            }
        }
        IEnumerator BlinkIFrames(float delay = 1f, float period = .25f, float min = .25f, float max = 1f)
        {
            SpriteRenderer renderer = GetComponentInChildren<SpriteRenderer>();
            if (renderer == null) yield break;
            float initAlpha = renderer.color.a;
            try
            {
                CanBind = false;
                for (float start = Time.time, elapsed = 0f; IsBound && elapsed < delay; elapsed = Time.time - start)
                {
                    if (this == null) yield break;
                    float tween = Mathf.Abs(Mathf.Sin(elapsed * period * Mathf.PI));
                    renderer.color = renderer.color.WithA(Mathf.Lerp(min, max, tween));
                    transform.position = transform.position.WithZ(Mathf.Min(0, transform.position.z + elapsed * elapsed * 9.8f));
                    yield return null;
                }
            }
            finally
            {
                if (renderer != null) renderer.color = renderer.color.WithA(initAlpha);
                if (this != null) CanBind = true;
            }
        }
    }
}