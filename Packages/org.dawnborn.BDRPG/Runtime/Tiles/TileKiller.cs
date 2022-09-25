using System.Collections.Generic;
using BDUtil;
using BDUtil.Math;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace BDRPG
{
    /// On tile collision (with a restricted set of tiles), destroys 'em!
    [AddComponentMenu("BDRPG/TileKiller")]
    [RequireComponent(typeof(Collider2D))]
    public class TileKiller : MonoBehaviour
    {
        public List<TileBase> KillTiles = new();

        void OnTileEnter(TileCollider2D.TileCollision collision)
        {
            bool killedAny = false;
            foreach (Vector3Int cell in collision.Enters)
            {
                TileBase atCell = collision.tilemap.GetTile(cell);
                if (KillTiles.Contains(atCell))
                {
                    collision.tilemap.SetTile(cell, null);
                    killedAny = true;
                }
            }
            if (killedAny)
            {
                collision.tilemapCollider.ProcessTilemapChanges();
                SendMessage("OnTileKilled", SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}
