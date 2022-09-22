using System;
using System.Collections;
using BDUtil;
using BDUtil.Math;
using UnityEngine;

namespace BDRPG
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Shadowwalk : MonoBehaviour
    {
        public float PartThresh = 1f;
        new Rigidbody2D rigidbody;
        new SpriteRenderer renderer;

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
                    GameObject trailPart = new();
                    SpriteRenderer footprint = trailPart.AddComponent<SpriteRenderer>();
                    footprint.sprite = renderer.sprite;

                    footprint.color = Color.Lerp(Color.gray * renderer.color, Color.blue, speed / 30f).WithA(.5f);
                    footprint.transform.rotation = renderer.transform.rotation;
                    trailPart.transform.position = transform.position;
                    // In case we die, we want our clones to go, so put this on a different object.
                    Coroutines.StartCoroutine(FadeFootprint(footprint));
                }
                yield return null;
            }
        }
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
    }
}
