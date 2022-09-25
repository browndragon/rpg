using System;
using System.Collections;
using System.Collections.ObjectModel;
using BDUtil;
using BDUtil.Math;
using UnityEngine;

namespace BDRPG
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Shadowwalk))]
    public class Damagewalk : MonoBehaviour
    {
        public float Damage = .5f;
        public Timer Debounce = .15f;
        public float AttackDegrees = 30f;
        new Rigidbody2D rigidbody;

        void Awake()
        {
            rigidbody = GetComponent<Rigidbody2D>();
            Debounce.Halt();
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (Debounce.IsRunning) return;
            if (collision.rigidbody == null || collision.otherRigidbody == null) return;
            StatusHaver haver = collision.collider.GetComponent<StatusHaver>();
            if (haver == null) return;
            /// If we're not moving into them, then this isn't an attack.
            float angle = Vector2.Angle(collision.rigidbody.position - collision.otherRigidbody.position, collision.otherRigidbody.velocity);
            if (angle > AttackDegrees) return;
            float len = Mathf.Sqrt((collision.otherRigidbody.velocity - collision.rigidbody.velocity).magnitude * collision.otherRigidbody.mass / collision.rigidbody.mass);
            len *= Damage;
            collision.rigidbody.AddForce(len * collision.otherRigidbody.velocity);
            haver.HP -= len;
            SoloDamageTopic.main.OrThrow().Value = new(
                (Vector3)(collision.rigidbody.position + collision.otherRigidbody.position) / 2,
                len
            );
            Debounce.Reset();
        }
    }
}
