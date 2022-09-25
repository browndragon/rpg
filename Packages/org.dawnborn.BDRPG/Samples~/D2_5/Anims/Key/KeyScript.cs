using System.Collections;
using BDRPG.Slots;
using BDUtil;
using BDUtil.Library;
using BDUtil.Math;
using UnityEngine;

namespace BDRPG
{
    public class KeyScript : MonoBehaviour, IEquip
    {
        Player player;
        public Slot Bound;
        Timer debounce;

        void Awake() => player = GetComponentInChildren<Player>();

        void PreDestroy()
        {
            Debug.Log($"Parent dying; self-unbinding {this}", this);
            Bound.Doff(true).OrThrow();
        }

        public bool DoBind(Slot slot)
        {
            if (this == null) return false;
            // We're shutting down, or have too recently been bound.
            if (debounce.IsRunning) return false;
            // We're already bound (and somebody else bumped into us or something).
            if (Bound) return false;
            // It's not a real slot. Programming error?
            slot.IsValid.OrThrow();
            Bound = slot;
            Transform newParent = slot.Owner.transform;
            if (newParent == null) return false;
            transform.SetParent(newParent);
            player.PlayByCategory("Don");

            Vector3 adjust = new(0f, 0f, -1f);
            IEnumerator MoveToHead()
            {
                foreach (var t in new Timer(.25f))
                {
                    if (!transform || !transform.parent) yield break;
                    transform.position = Vector3.Lerp(transform.position, transform.parent.position + adjust, t);
                    yield return null;
                }
                if (transform && transform.parent) transform.position = transform.parent.position + adjust;
            }
            StartCoroutine(MoveToHead());
            debounce = new(.25f);
            return true;
        }
        public bool DoUnbind(Slot slot, bool force = false)
        {
            if (!force && debounce.IsRunning) return false;
            Bound.OrThrowInternal();
            Bound = default;
            transform.SetParent(null, true);
            player.PlayByCategory("Doff");

            IEnumerator Fall(float acc = 10f)
            {
                float v = 0f;
                while (transform.position.z < 0f)
                {
                    float dT = Time.deltaTime;
                    transform.position = transform.position.WithZ(Mathf.Min(0f, transform.position.z + dT * (v += dT * acc)));
                    yield return null;
                }
            }
            StartCoroutine(Fall());

            debounce = new(1f);
            SpriteRenderer renderer = GetComponentInChildren<SpriteRenderer>();
            if (!renderer) return true;
            IEnumerator BlinkIFrames(float delay = 1f, float frequency = 4f, float min = .25f, float max = 1f)
            {
                foreach (var timer in new Timer(delay))
                {
                    if (this == null) break;
                    float tween = Mathf.Abs(Mathf.Sin(timer.Elapsed * frequency * Mathf.PI));
                    renderer.color = renderer.color.WithA(Mathf.Lerp(min, max, tween));
                    yield return null;
                }
                renderer.color = renderer.color.WithA(1f);
            }
            StartCoroutine(BlinkIFrames());
            return true;
        }
        // TODO: Really?! This is probably wrong arch.
        void OnTileKilled() => player.PlayByCategory("");

        public bool DoRebind(Slot slot, IEquip @new)
        {
            if (!slot.Doff()) return false;
            if (slot.Don(@new)) return true;
            debounce = 0f;
            return slot.Don(this);
        }
    }
}