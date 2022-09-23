using System.Collections;
using System.Collections.Generic;
using BDUtil;
using UnityEngine;

namespace BDRPG
{
    public class SpawnInWorld : MonoBehaviour
    {
        public TMPro.TMP_Text Proto;
        new Camera camera;
        void Awake() => camera = Camera.main;
        public void Spawn(BDRPG.Damage damage) => Coroutines.StartCoroutine(Fade(damage));
        IEnumerator Fade(BDRPG.Damage damage)
        {
            var text = Instantiate(Proto, transform);
            text.text = $"-{damage.Amount:0.#}";
            text.transform.position = camera.WorldToScreenPoint(damage.World);
            Vector3 end = damage.World + Vector3.back;
            Color begin = text.color;
            foreach (var timer in new Timer(.75f))
            {
                float t = BDUtil.Math.Easings.Impl.OutSine(timer);
                text.transform.position = camera.WorldToScreenPoint(Vector3.Lerp(damage.World, end, t));
                text.color = Color.Lerp(begin, Color.clear, t);
                yield return null;
            }
            Destroy(text.gameObject);
        }
    }
}
