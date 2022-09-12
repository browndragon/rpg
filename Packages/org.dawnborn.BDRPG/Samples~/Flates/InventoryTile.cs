using System.Diagnostics.CodeAnalysis;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BDRPG
{
    [RequireComponent(typeof(Image))]
    public class InventoryTile : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        Image Image;
        TMP_Text LabelText;
        Image CountBack;
        TMP_Text CountText;

        [SerializeField] int count;
        public int Count
        {
            get => count;
            set
            {
                if (value < 0)
                {
                    CountBack.enabled = false;
                    CountText.enabled = false;
                    return;
                }
                count = value;
                CountBack.enabled = true;
                CountText.enabled = true;
                CountText.text = value.ToString();
            }
        }
        [SerializeField] Cameos.MonoBehaviour cameo;
        public Cameos.MonoBehaviour Cameo
        {
            get => cameo;
            set
            {
                cameo = value;
                Image.sprite = cameo?.Portrait;
                LabelText.text = cameo?.DisplayName;
            }
        }

        public void SetCameo(Cameos.MonoBehaviour cameo, int count)
        {
            Cameo = cameo;
            Count = count;
        }

        void Awake()
        {
            Image = GetComponent<Image>();
            LabelText = transform.Find("Label").GetComponent<TMP_Text>();
            CountBack = transform.Find("CountBack").GetComponent<Image>();
            CountText = transform.Find("Count").GetComponent<TMP_Text>();
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        => GetComponentInParent<InventoryUI>().OnPointerDown(this, eventData);

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        => GetComponentInParent<InventoryUI>().OnPointerUp(this, eventData);
    }
}
