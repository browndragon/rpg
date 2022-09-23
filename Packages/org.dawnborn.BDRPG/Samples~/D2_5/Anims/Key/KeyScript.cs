using System.Collections;
using BDRPG.Slots;
using BDUtil;
using BDUtil.Math;
using UnityEngine;

namespace BDRPG
{
    public class KeyScript : MonoBehaviour, IEquip
    {
        public bool CanBind = true;
        public Slot Bound;

        void PreDestroy()
        {
            Debug.Log($"Parent dying; self-unbinding {this}", this);
            Bound.Doff(true);
        }

        public bool DoBind(Slot slot)
        {
            if (Bound) return false;
            Bound = slot;
            Transform newParent = slot.Owner.transform;
            if (newParent == null) return false;
            transform.SetParent(newParent);
            StartCoroutine(MoveToHead(adjust: new(0f, 0f, -1f)));
            return true;
        }
        public bool DoUnbind(Slot slot, bool force = false)
        {
            if (!Bound) return false;
            Bound = default;
            transform.SetParent(null, true);
            StartCoroutine(BlinkIFrames(1f));
            return true;
        }
        public bool DoRebind(Slot slot, IEquip @new)
        {
            slot.Doff(force: true);
            slot.Don(@new);
            return true;
        }

        IEnumerator MoveToHead(float speed = 16f, Vector3 adjust = default)
        {
            Transform transform = this.transform;
            while (transform != null && transform.parent != null)
            {
                transform.position = Vector3.MoveTowards(transform.position, transform.parent.position + adjust, Time.deltaTime * speed);
                yield return null;
            }
        }
        IEnumerator BlinkIFrames(float delay = 1f, float frequency = 4f, float min = .25f, float max = 1f)
        {
            SpriteRenderer renderer = GetComponentInChildren<SpriteRenderer>();
            if (renderer == null) yield break;
            float initAlpha = renderer.color.a;
            try
            {
                CanBind = false;
                foreach (var timer in new Timer(delay))
                {
                    if (this == null) yield break;
                    float tween = Mathf.Abs(Mathf.Sin(timer.Elapsed * frequency * Mathf.PI));
                    renderer.color = renderer.color.WithA(Mathf.Lerp(min, max, tween));
                    transform.position = transform.position.WithZ(Mathf.Min(0, transform.position.z + Mathf.Pow(timer.Elapsed, 2f) * 9.8f));
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