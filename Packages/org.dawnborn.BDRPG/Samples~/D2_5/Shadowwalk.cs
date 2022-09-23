using System;
using System.Collections;
using BDUtil;
using BDUtil.Math;
using BDUtil.Raw;
using UnityEngine;

namespace BDRPG
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Shadowwalk : MonoBehaviour
    {
        public float PartThresh = 1f;
        new Rigidbody2D rigidbody;
        new SpriteRenderer renderer;
        readonly Deque<SpriteRenderer> trail = new();

        void Awake()
        {
            rigidbody = GetComponent<Rigidbody2D>();
            renderer = GetComponentInChildren<SpriteRenderer>();
        }
        IEnumerator Start()
        {
            float accum = 0f;
            while (true)
            {
                float speed = rigidbody.velocity.magnitude;
                accum += Time.deltaTime * speed;
                if (accum > PartThresh)
                {
                    accum = 0;
                    SpriteRenderer footprint = trail.PopBack() ?? new GameObject("Trail").AddComponent<SpriteRenderer>();
                    footprint.gameObject.SetActive(true);
                    footprint.sprite = renderer.sprite;
                    footprint.color = Color.Lerp(Color.gray * renderer.color, Color.blue, speed / 30f).WithA(.5f);
                    footprint.transform.rotation = renderer.transform.rotation;
                    footprint.transform.position = transform.position;
                    // In case we die, we want our clones to go *later*...
                    Coroutines.StartCoroutine(FadeFootprint(footprint));
                }
                yield return null;
            }
        }
        IEnumerator FadeFootprint(SpriteRenderer footprint)
        {
            Color init = footprint.color;
            Color target = new(1f, 0f, 0f, 0f);
            foreach (var timer in new Timer(.5f))
            {
                if (footprint == null) yield break;
                footprint.color = Color.Lerp(init, target, timer);
                yield return null;
            }
            if (footprint == null) yield break;
            if (this == null) yield break;
            footprint.gameObject.SetActive(false);
            trail.Add(footprint);
        }
    }
}
