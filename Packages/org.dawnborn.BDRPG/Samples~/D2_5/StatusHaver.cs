using System.Collections;
using BDUtil;
using BDUtil.Library;
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
        Player hurtPlayer;
        Player HurtPlayer
        {
            get
            {
                if (hurtPlayer != null) return hurtPlayer;
                foreach (Player p in GetComponentsInChildren<Player>())
                {
                    if (p.Category == "Hurt") { hurtPlayer = p; break; }
                }
                return hurtPlayer;
            }
        }
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
                HurtPlayer?.PlayCurrentCategory();
                if (hP <= 0 && !wasDying) StartCoroutine(Die());
            }
        }
        IEnumerator Die()
        {
            // Seems to be a good idea? Without this, we're stuck trying to rescue from certain death.
            gameObject.BroadcastMessage("PreDestroy", SendMessageOptions.DontRequireReceiver);

            SpriteRenderer renderer = SpriteRenderer;
            Color init = renderer.color;
            Color target = renderer.color.WithA(0f);
            foreach (var timer in new Timer(.5f))
            {
                renderer.transform.eulerAngles = renderer.transform.eulerAngles.WithZ(timer.Elapsed * 4 * 360f);
                renderer.color = Color.Lerp(init, target, timer);
                yield return null;
            }

            Destroy(gameObject);
        }
        public Transform RendererTransform => SpriteRenderer.transform;
    }
}
