using System;
using BDUtil.Math;
using BDUtil.Pubsub;
using UnityEngine;

namespace BDRPG
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(SpriteRenderer))]
    public class Actor : MonoBehaviour, Initiative.IActor
    {
        public Initiative Initiative;
        public float DelayDither = 5f;
        public int NumActs = 0;  // Sort of a round counter. Not sure. Let's see.
        public float NextAct = float.NaN;
        public float ForceMag = 5f;
        new Rigidbody2D rigidbody;
        SpriteRenderer sprite;
        TMPro.TMP_Text text;

        public bool IsReady => NextAct <= Initiative.Time;

        void OnEnable()
        {
            rigidbody = GetComponent<Rigidbody2D>();
            sprite = GetComponent<SpriteRenderer>();
            text = GetComponentInChildren<TMPro.TMP_Text>();
            text.text = gameObject.name;
            NumActs = 0;
            NextAct = Initiative.Time + UnityEngine.Random.Range(0, DelayDither);
            Initiative.Head.Push(gameObject);
        }
        void OnDisable()
        {
            NumActs = 0;
            NextAct = float.NaN;
        }
        public int CompareTo(ObjsHead.ISortComponent other)
        => other switch
        {
            null => -1,
            Actor i => Chain.Cmp || NumActs.CompareTo(i.NumActs) || NextAct.CompareTo(i.NextAct),
            _ => throw new NotSupportedException($"Can't compare {this} vs {other}"),
        };

        void OnMouseOver()
        {
            // Remove from deque directly so we don't corrupt the data.
            Initiative.Head.Deque.Collection.Remove(gameObject);
            // But since our comparator is on numActs THEN nextAct, this still won't cut the line.
            NextAct = float.NegativeInfinity;
            // Reenqueue with new value.
            Initiative.Head.Push(gameObject);
        }

        public void Act()
        {
            rigidbody.AddForce(ForceMag * UnityEngine.Random.insideUnitCircle, ForceMode2D.Impulse);
            // This assumes we *know* that if we're acting, we've been popped from the list.
            NumActs++;
            NextAct = Time.time + UnityEngine.Random.Range(0, DelayDither);
            Initiative.Head.Push(gameObject);
        }

        public void Update()
        {
            sprite.color = Color.Lerp(Color.magenta, Color.red, NextAct - Initiative.Time);
        }
    }
}
