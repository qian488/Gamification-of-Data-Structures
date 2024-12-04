using UnityEngine;
using System.Collections.Generic;

namespace Sokoban
{
    public class TileManager : MonoBehaviour
    {
        private Dictionary<Vector2Int, GameObject> tiles = new Dictionary<Vector2Int, GameObject>();
        private HashSet<Vector2Int> glowingTiles = new HashSet<Vector2Int>();

        private Material normalMaterial;
        private Material glowMaterial;

        private void Awake()
        {
            // 创建材质
            normalMaterial = new Material(Shader.Find("Standard"));
            normalMaterial.color = new Color(0.3f, 0.3f, 0.3f);

            glowMaterial = new Material(Shader.Find("Standard"));
            glowMaterial.color = new Color(0.5f, 0.8f, 1f);
            glowMaterial.EnableKeyword("_EMISSION");
            glowMaterial.SetColor("_EmissionColor", new Color(0.5f, 0.8f, 1f) * 0.5f);
        }

        public void ResetTiles()
        {
            // 清除所有发光效果
            ClearAllGlow();
            // 清除所有瓦片
            foreach (var tile in tiles.Values)
            {
                if (tile != null)
                {
                    Destroy(tile);
                }
            }
            tiles.Clear();
            glowingTiles.Clear();
        }

        public void CreateFloor(int width, int height)
        {
            // 创建新地板前先清理旧的
            ResetTiles();

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    Vector3 pos = new Vector3(x, -0.5f, z);
                    GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    tile.transform.SetParent(transform);
                    tile.transform.position = pos;
                    tile.transform.rotation = Quaternion.Euler(90, 0, 0);
                    tile.transform.localScale = new Vector3(0.95f, 0.95f, 1);
                    tiles[new Vector2Int(x, z)] = tile;
                    tile.GetComponent<MeshRenderer>().material = normalMaterial;
                }
            }
        }

        public void SetTileGlow(Vector2Int position, bool glow)
        {
            // 确保位置有效
            if (!tiles.ContainsKey(position))
            {
                return;
            }

            if (tiles.TryGetValue(position, out GameObject tile))
            {
                tile.GetComponent<MeshRenderer>().material = glow ? glowMaterial : normalMaterial;
                if (glow)
                    glowingTiles.Add(position);
                else
                    glowingTiles.Remove(position);
            }
        }

        public void ClearAllGlow()
        {
            foreach (var pos in glowingTiles)
            {
                if (tiles.TryGetValue(pos, out GameObject tile))
                {
                    tile.GetComponent<MeshRenderer>().material = normalMaterial;
                }
            }
            glowingTiles.Clear();
        }

        private void OnDestroy()
        {
            // 清理材质
            if (normalMaterial != null) Destroy(normalMaterial);
            if (glowMaterial != null) Destroy(glowMaterial);
        }
    }
} 