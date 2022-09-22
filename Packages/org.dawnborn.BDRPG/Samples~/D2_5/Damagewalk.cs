using System;
using System.Collections;
using BDUtil;
using BDUtil.Math;
using UnityEngine;

namespace BDRPG
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Shadowwalk))]
    public class Damagewalk : MonoBehaviour
    {
        public float Damage = .5f;
        public float Debounce = .15f;
        public float AttackDegrees = 30f;
        float DebounceUntil = float.NegativeInfinity;
        new Rigidbody2D rigidbody;

        void Awake() => rigidbody = GetComponent<Rigidbody2D>();

        IEnumerator FadeFootprint(SpriteRenderer footprint)
        {
            Color init = footprint.color;
            Color target = new(1f, 0f, 0f, 0f);
            for (float start = Time.time, elapsed = 0f, max = .5f; elapsed < max; elapsed = Time.time - start)
            {
                footprint.color = Color.Lerp(init, target, elapsed / max);
                yield return null;
            }
            Destroy(footprint.gameObject);
        }
        void OnCollisionEnter2D(Collision2D collision)
        {
            if (Time.time < DebounceUntil) return;
            StatusHaver haver = collision.collider.GetComponent<StatusHaver>();
            if (haver == null) return;
            /// If we're not moving into them, then this isn't an attack.
            float angle = Vector2.Angle(collision.rigidbody.position - collision.otherRigidbody.position, collision.otherRigidbody.velocity);
            if (angle > AttackDegrees)
            {
                Debug.Log($"Missed attack angle {angle} > {AttackDegrees}");
                return;
            }
            float len = Mathf.Sqrt((collision.otherRigidbody.velocity - collision.rigidbody.velocity).magnitude * collision.otherRigidbody.mass / collision.rigidbody.mass);
            len *= Damage;
            Debug.Log($"Dealing {len} damage {collision.otherCollider}->{collision.collider}");
            collision.rigidbody.AddForce(len * collision.otherRigidbody.velocity);
            haver.HP -= len;
            DebounceUntil = Time.time + Debounce;
        }
    }
}
