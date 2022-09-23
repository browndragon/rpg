using System;
using System.Collections;
using System.Collections.Generic;
using BDUtil;
using BDUtil.Math;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BDRPG
{
    public class VictoryAnimator : MonoBehaviour
    {
        public Color[] Sunset = { new(1f, 1f, .8f, .1f), new(.7f, .8f, .5f, .2f), new(.5f, .5f, .6f, .3f), new(.2f, .1f, .8f, .5f), new(0f, 0f, 0f, 1f) };
        new Camera camera;
        void Awake() => camera = Camera.main;
        void Start()
        {
            foreach (GameObject panel in GameObject.FindGameObjectsWithTag("Finish"))
            {
                switch (panel.name)
                {
                    case "Slide": StartCoroutine(SlidePanel(panel.GetComponent<RectTransform>())); break;
                    case "Scrim": StartCoroutine(FadePanel(panel.GetComponent<Image>())); break;
                    default: throw new NotSupportedException($"Unrecognized {panel}");
                }
            }

            foreach (GameObject celebrant in GameObject.FindGameObjectsWithTag("Player"))
            {
                StartCoroutine(Celebrate(celebrant.GetComponent<SpriteRenderer>()));
            }
        }
        IEnumerator SlidePanel(RectTransform slide)
        {
            Vector2 end = slide.anchoredPosition;
            Vector2 start = end.WithY(end.y - slide.rect.height);
            slide.anchoredPosition = start;
            foreach (var timer in new Timer(.5f))
            {
                slide.anchoredPosition = Vector2.Lerp(start, end, timer);
                yield return null;
            }
        }
        IEnumerator Celebrate(SpriteRenderer celebrant)
        {
            bool isWalker = !celebrant.flipX;  // By coincidence :-D

            // Prepare everyone for the first loop.
            Vector3 offset = new(-1, 0, 0);
            celebrant.flipX = !celebrant.flipX;
            while (celebrant != null)
            {
                offset = new(
                    -Mathf.Sign(offset.x) * UnityEngine.Random.Range(.25f, .5f),
                    isWalker ? 0f : UnityEngine.Random.Range(-.125f, +.125f),
                    0f
                );
                celebrant.flipX = !celebrant.flipX;
                yield return null;

                int steps = UnityEngine.Random.Range(1, 6);

                foreach (var timer in new Timer(UnityEngine.Random.Range(1f, 2f)))
                {
                    celebrant.transform.position += timer.Delta * offset;
                    yield return null;
                }
                if (!isWalker)
                {
                    yield return new WaitForSeconds(UnityEngine.Random.Range(.5f, 1.5f));
                    continue;
                }
                float start = celebrant.transform.position.y;
                yield return new WaitForSeconds(.05f);
                for (int i = 0; i < 6 - steps; ++i)
                {
                    float mid = start + UnityEngine.Random.Range(.9f, 1.1f);
                    Timer jumptime = new((mid - start) / 3f);
                    foreach (var timer in jumptime.Restart())
                    {
                        celebrant.transform.position = celebrant.transform.position.WithY(
                            Mathf.Lerp(start, mid, BDUtil.Math.Easings.Impl.OutQuad(timer))
                        );
                        yield return null;
                    }
                    foreach (var timer in jumptime.Restart())
                    {
                        celebrant.transform.position = celebrant.transform.position.WithY(
                            Mathf.Lerp(mid, start, BDUtil.Math.Easings.Impl.OutQuad(timer))
                        );
                        yield return null;
                    }
                    celebrant.transform.position = celebrant.transform.position.WithY(start);
                    yield return new WaitForSeconds(.1f);
                }
                celebrant.transform.position = celebrant.transform.position.WithY(start);
                yield return new WaitForSeconds(.2f);
            }
        }

        IEnumerator FadePanel(Image panel)
        {
            yield return new WaitForSeconds(5f);
            foreach (Color sunset in Sunset)
            {
                Color had = panel.color;
                foreach (var timer in new Timer(1f))
                {
                    panel.color = Color.Lerp(had, sunset, timer);
                    yield return null;
                }
            }
            SceneManager.LoadScene("Samples/D2_5/Splash");
        }
    }
}