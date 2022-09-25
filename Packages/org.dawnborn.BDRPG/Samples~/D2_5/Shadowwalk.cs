using System.Collections;
using BDUtil;
using BDUtil.Math;
using BDUtil.Raw;
using UnityEngine;

namespace BDRPG
{
    public class Shadowwalk : MonoBehaviour
    {
        public float SpeedScale = .5f;
        public float SpeedDecay = .5f;
        public float PartThresh = 2.5f;
        Vector3 LastFrame;

        float Speed;
        float Accum;
        new SpriteRenderer renderer;
        Animator animator;
        readonly Deque<SpriteRenderer> trail = new();


        void Awake()
        {
            renderer = GetComponentInChildren<SpriteRenderer>();
            animator = renderer.GetComponent<Animator>();
        }
        void Start()
        {
            LastFrame = transform.position;
            Speed = 0f;
        }
        void Update()
        {
            var speed = ((transform.position - LastFrame) / Time.deltaTime).magnitude;
            LastFrame = transform.position;
            if (speed > 1e-06f) Speed = speed;
            else Speed *= SpeedDecay;
            animator.SetFloat("Speed", Speed * SpeedScale);
            if (Speed < PartThresh) { Accum = 0; return; }
            Accum += Time.deltaTime * Speed;
            if (Accum < PartThresh) return;
            Accum = 0;
            SpriteRenderer footprint = trail.PopBack() ?? new GameObject("Trail").AddComponent<SpriteRenderer>();
            footprint.gameObject.SetActive(true);
            footprint.sprite = renderer.sprite;
            footprint.color = Color.Lerp(Color.gray * renderer.color, Color.blue, Speed / 5f).WithA(.25f);
            footprint.transform.rotation = renderer.transform.rotation;
            footprint.transform.position = transform.position + .0001f * new Vector3(1f, 1f, 0f);
            // In case we die, we want our clones to go *later*...
            Coroutines.StartCoroutine(FadeFootprint(footprint));
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
