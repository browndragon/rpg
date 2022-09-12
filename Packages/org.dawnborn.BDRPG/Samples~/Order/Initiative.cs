using System;
using BDUtil.Pubsub;
using UnityEngine;

namespace BDRPG
{
    [RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
    public class Initiative : MonoBehaviour, ObjsHead.ISortComponent
    {
        public ObjsHead Head;
        public float DelayDither = 5f;
        public float NextAct = float.NaN;
        public float ForceMag = 5f;
        new Rigidbody2D rigidbody;
        SpriteRenderer sprite;
        TMPro.TMP_Text text;
        void OnEnable()
        {
            rigidbody = GetComponent<Rigidbody2D>();
            sprite = GetComponent<SpriteRenderer>();
            text = GetComponentInChildren<TMPro.TMP_Text>();
            text.text = gameObject.name;
            NextAct = Time.time + UnityEngine.Random.Range(0, DelayDither);
            Head.Push(gameObject);
        }
        void OnDisable()
        {
            NextAct = float.NaN;
        }

        public int CompareTo(ObjsHead.ISortComponent other)
        => other switch
        {
            null => -1,
            Initiative i => NextAct.CompareTo(i.NextAct),
            _ => throw new NotSupportedException($"Can't compare {this} vs {other}"),
        };

        public void Act()
        {
            rigidbody.AddForce(ForceMag * UnityEngine.Random.insideUnitCircle, ForceMode2D.Impulse);
            NextAct = Time.time + UnityEngine.Random.Range(0, DelayDither);
            Head.Push(gameObject);
        }

        public void Update()
        {
            sprite.color = Color.Lerp(Color.magenta, Color.red, NextAct - Time.time);
        }
    }
}
