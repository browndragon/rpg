using System.Collections;
using BDUtil.Math;
using BDUtil.Raw;
using UnityEngine;

namespace BDRPG
{
    public class SpawnInWorld : MonoBehaviour
    {
        public Vector3 Float = new(.5f, .5f, -2f);
        readonly Deque<TMPro.TMP_Text> Cached = new();
        public TMPro.TMP_Text Proto;
        public void Spawn(BDRPG.Damage damage) => StartCoroutine(Fade(damage));
        IEnumerator Fade(BDRPG.Damage damage)
        {
            var text = Cached.PopBack();
            if (text == null)
            {
                text = Instantiate(Proto, transform);
            }
            text.gameObject.SetActive(true);
            text.text = $"-{damage.Amount:0.#}";
            text.transform.position = damage.World;
            Vector3 random = (Vector3)Vector2.Scale(Float, UnityEngine.Random.insideUnitCircle) + Float.z * Vector3.forward;
            Vector3 end = damage.World + random;
            Color begin = Color.red;
            foreach (var timer in new Timer(.75f))
            {
                float sin = BDUtil.Math.Easings.Impl.OutSine(timer);
                text.transform.position = Vector3.Lerp(damage.World, end, sin);
                text.color = Color.Lerp(begin, Color.clear, sin);
                yield return null;
            }
            text.gameObject.SetActive(false);
            Cached.PushBack(text);
        }
    }
}
