using System.Collections;
using BDUtil.Math;
using UnityEngine;

namespace BDRPG
{
    public class StatusHaver : MonoBehaviour
    {
        [SerializeField] string displayName;
        public string DisplayName => string.IsNullOrEmpty(displayName) ? gameObject.name : displayName;
        [SerializeField] Sprite logo;
        public Sprite Logo => logo == null ? SpriteRenderer.sprite : logo;
        SpriteRenderer spriteRenderer;
        SpriteRenderer SpriteRenderer => spriteRenderer ??= GetComponentInChildren<SpriteRenderer>();
        [SerializeField] Sprite hPLogo;
        public Sprite HPLogo => hPLogo == null ? Logo : hPLogo;
        [SerializeField] float hP = 3f;
        public float HP
        {
            get => hP;
            set
            {
                bool wasDying = hP <= 0;
                hP = value;
                if (hP <= 0 && !wasDying) StartCoroutine(Die());
            }
        }
        IEnumerator Die()
        {
            SpriteRenderer renderer = SpriteRenderer;
            Color init = renderer.color;
            Color target = renderer.color.WithA(0f);
            for (float start = Time.time, max = .5f, elapsed = 0f; elapsed < max; elapsed = Time.time - start)
            {
                renderer.transform.eulerAngles = renderer.transform.eulerAngles.WithZ(elapsed * 4 * 360f);
                renderer.color = Color.Lerp(init, target, elapsed / max);
                yield return null;
            }
            Destroy(gameObject);
        }
        public Transform RendererTransform => SpriteRenderer.transform;
    }
}
