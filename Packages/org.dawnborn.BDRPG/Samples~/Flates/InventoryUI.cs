using BDUtil;
using BDUtil.Serialization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BDRPG
{
    [RequireComponent(typeof(RectTransform))]
    public class InventoryUI : MonoBehaviour
    {
        public Ref<InventoryTile> InventoryTileProto;
        public WorldClick WorldClick;
        public Transform InventoryTransform;

        void Awake() => InventoryTransform = GetComponentInChildren<LayoutGroup>()?.transform ?? transform;
        public void AddClone(Cameos.MonoBehaviour cameo, int count = 1)
        {
            foreach (InventoryTile tile in GetComponentsInChildren<InventoryTile>())
            {
                if (tile.Cameo.GetComponent<Clone>().PrefabRef != cameo.GetComponent<Clone>().PrefabRef) continue;
                tile.Count += count;
                if (tile.Count <= 0) Destroy(tile.gameObject);
                return;
            }
            (count > 0).OrThrow();
            InventoryTile @new = Clone.Acquire(InventoryTileProto);
            @new.transform.SetParent(InventoryTransform);
            @new.SetCameo(cameo, count);
        }
        public void OnPointerDown(InventoryTile _, PointerEventData _0) { }
        public void OnPointerUp(InventoryTile tile, PointerEventData _)
        {
            tile.Count -= 1;
            if (tile.Count <= 0) Destroy(tile.gameObject);
            WorldClick.Inflating = Clone.Acquire((Ref<Clone>)tile.Cameo.GetComponent<Clone>())?.GetComponent<Cameos.MonoBehaviour>();
        }
    }
}
