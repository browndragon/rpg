using System;
using System.Diagnostics.CodeAnalysis;
using BDRPG.Slots;
using BDUtil;
using UnityEngine;

namespace BDRPG
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class TestEquip : MonoBehaviour, IEquip
    {
        public float ZRot = 45f;
        public float Veloc = 12f;
        Vector3 Offset;
        SpriteRenderer Target;
        public bool IsBound => Target != null;
        new SpriteRenderer renderer;
        public bool IsReplace => true;

        [SuppressMessage("IDE", "IDE0051")]
        void Awake() => renderer = GetComponent<SpriteRenderer>();
        [SuppressMessage("IDE", "IDE0051")]
        void LateUpdate()
        {
            if (!IsBound) return;
            Offset = Quaternion.Euler(0f, 0f, Time.deltaTime * ZRot) * Offset;
            Vector3 offset = Target.transform.position + Offset;
            offset -= transform.position;
            Vector3.ClampMagnitude(offset, Time.deltaTime * Veloc);
            transform.position += offset;
        }

        public bool DoBind(Slot slot)
        {
            if (Target != null)
            {
                Debug.Log($"Already bound to {Target}", this);
                return false;
            }
            Debug.Log($"DoBind {slot}", this);
            Target = slot.Owner.GetComponent<SpriteRenderer>();
            Offset = Quaternion.Euler(0f, 0f, Time.time * ZRot) * Vector3.right;
            (renderer.color, Target.color) = (Target.color, renderer.color);
            transform.localScale = .25f * Vector3.one;
            return true;
        }

        public bool DoUnbind(Slot slot, bool force = false)
        {
            Debug.Log($"DoUnbind{(force ? "(force)" : "")} {slot}", this);
            transform.position += transform.position - Target.transform.position;
            transform.localScale = 1f * Vector3.one;
            (renderer.color, Target.color) = (Target.color, renderer.color);

            Target = null;
            return true;
        }
        public bool DoRebind(Slot slot, IEquip @new)
        {
            Debug.Log($"DoRebind {slot}: {this}->{@new}", this);
            slot.Doff(force: true).OrThrowInternal();
            return slot.Don(@new);
        }
    }
}
