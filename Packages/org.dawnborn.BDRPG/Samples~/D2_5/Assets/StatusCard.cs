using BDUtil;
using BDUtil.Math;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BDRPG
{
    public class StatusCard : MonoBehaviour
    {
        CanvasGroup canvasGroup;
        new Camera camera;
        TMPro.TMP_Text Name;
        Image Logo;
        public Image HeartPrefab;
        LayoutGroup Hearts;
        Collider2D[] scratch = new Collider2D[16];

        void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            camera = Camera.main;
            Name = GetComponentInChildren<TMP_Text>().OrThrow();
            Logo = GetComponentInChildren<Image>().OrThrow();
            Hearts = GetComponentInChildren<LayoutGroup>().OrThrow();
        }

        void LateUpdate()
        {
            StatusHaver target = null;
            int collided = Physics2D.OverlapPointNonAlloc(camera.ScreenPointToRay(Input.mousePosition).AtZ(), scratch);
            for (int i = 0; i < collided; ++i)
            {
                target = scratch[i].GetComponent<StatusHaver>();
                if (target != null) break;
            }
            if (target == null)
            {
                canvasGroup.alpha -= Time.time;
                return;
            }
            transform.position = camera.WorldToScreenPoint(target.RendererTransform.transform.position);
            transform.position += 1f * Vector3.up;
            canvasGroup.alpha = 1f;

            Name.text = target.DisplayName;
            Logo.sprite = target.Logo;
            bool isNegative = target.HP <= 0;

            if (isNegative) Logo.color = Color.red;
            else Logo.color = Color.white;

            float HP = Mathf.Abs(target.HP);
            int intPart = Mathf.FloorToInt(HP);
            for (int i = 0; i < intPart; ++i)
            {
                var image = FillHeart(target, i);
                image.fillAmount = 1f;
            }
            float floatPart = HP - intPart;
            if (floatPart > 0f)
            {
                var image = FillHeart(target, intPart++);
                image.fillAmount = floatPart;
            }
            for (int i = intPart; i < Hearts.transform.childCount; ++i) Destroy(
                Hearts.transform.GetChild(i).gameObject
            );
        }

        private Image FillHeart(StatusHaver target, int i)
        {
            Transform transform = Hearts.transform;
            Image heart;
            if (i >= Hearts.transform.childCount) heart = Instantiate(HeartPrefab, Hearts.transform);
            else heart = transform.GetChild(i).GetComponent<Image>();
            heart.sprite = target.HPLogo;
            heart.fillMethod = Image.FillMethod.Radial360;
            heart.fillOrigin = (int)Image.Origin360.Top;
            heart.fillClockwise = false;
            return heart;
        }
    }
}
