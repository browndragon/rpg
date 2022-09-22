using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BDUtil;
using BDUtil.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BDRPG
{
    [RequireComponent(typeof(TMP_Text))]
    public class OpenLink : MonoBehaviour, IPointerClickHandler
    {
        public StoreMap<string, string> Urls = new();
        TMP_Text text;
        void Awake() => text = GetComponent<TMP_Text>();

        public void OnPointerClick(PointerEventData @event)
        {
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(text, @event.position, @event.pressEventCamera);
            if (linkIndex == -1) return;
            TMP_LinkInfo link = text.textInfo.linkInfo[linkIndex];

            string url = link.GetLinkID();
            if (url.IsEmpty()) url = link.GetLinkText();
            if (!Fix(ref url))
            {
                url = Urls.Collection.GetValueOrDefault(url);
                if (!Fix(ref url)) return;
            }
            Application.OpenURL(url);
        }
        static bool Fix(ref string url)
        {
            if (url.IsEmpty()) return false;
            // if it looks like a url, coerce it to a url.
            if (url.Contains(".com") || url.Contains(".net") || url.Contains(".org"))
            {
                if (!url.StartsWith("http")) url = $"http://{url}";
                return true;
            }
            return false;
        }
    }
}
