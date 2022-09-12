using System.Diagnostics.CodeAnalysis;
using BDUtil.Serialization;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BDRPG
{
    [RequireComponent(typeof(Camera))]
    public class WorldClick : MonoBehaviour
    {
        public InventoryUI InventoryClick;
        public Cameos.MonoBehaviour Inflating;

        new Camera camera;
        EventSystem eventSystem;
        [SuppressMessage("IDE", "IDE0051")]
        void OnEnable()
        {
            camera = GetComponent<Camera>();
            eventSystem = EventSystem.current;
        }
        [SuppressMessage("IDE", "IDE0051")]
        void Update()
        {
            Vector2 worldPoint = camera.ScreenToWorldPoint(Input.mousePosition);
            if (Inflating != null) Inflating.transform.position = worldPoint;
            if (!Input.GetMouseButtonUp(0)) return;
            if (eventSystem.IsPointerOverGameObject()) return;
            if (Inflating != null)
            {
                // Also tell it it's inflated; whatever, this is just a test.
                Inflating = null;
                return;
            }
            foreach (Collider2D collider in Physics2D.OverlapPointAll(worldPoint))
            {
                var clone = collider.GetComponent<Clone>();
                if (clone == null) continue;
                InventoryClick.AddClone(clone.GetComponent<Cameos.MonoBehaviour>(), 1);
                Clone.Release(clone);
            }
        }
    }
}
