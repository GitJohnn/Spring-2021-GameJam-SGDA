using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMapVisual : MonoBehaviour
{
    [System.Serializable]
    public struct TilemapSpriteUV
    {
        public TileMap.TileMapObject.TileMapSprite tilemapSprite;
        public Vector2Int uv00Pixels;
        public Vector2Int uv11Pixels;
    }

    private struct UVcoords
    {
        public Vector2 uv00;
        public Vector2 uv11;
    }

    [SerializeField] private TilemapSpriteUV[] tilemapSpriteUVArray;
    private Grid<TileMap.TileMapObject> grid;
    private Mesh mesh;
    private bool updateMesh;
    private Dictionary<TileMap.TileMapObject.TileMapSprite, UVcoords> uvCoordsDictionary;

    private void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        Texture texture = GetComponent<MeshRenderer>().material.mainTexture;
        float textureWidth = texture.width;
        float textureHeight = texture.height;

        uvCoordsDictionary = new Dictionary<TileMap.TileMapObject.TileMapSprite, UVcoords>();

        foreach(TilemapSpriteUV tilemapSpriteUV in tilemapSpriteUVArray)
        {
            uvCoordsDictionary[tilemapSpriteUV.tilemapSprite] = new UVcoords
            {
                uv00 = new Vector2(tilemapSpriteUV.uv00Pixels.x / textureWidth, tilemapSpriteUV.uv00Pixels.y / textureHeight),
                uv11 = new Vector2(tilemapSpriteUV.uv11Pixels.x / textureWidth, tilemapSpriteUV.uv11Pixels.y / textureHeight),
            };  
        }
    }

    public void SetGrid(TileMap tilemap, Grid<TileMap.TileMapObject> grid)
    {
        this.grid = grid;
        UpdateTileMapVisual();

        grid.OnGridValueChanged += Grid_OnGridValueChanged;
        tilemap.OnLoaded += TileMap_OnLoaded;
    }

    private void TileMap_OnLoaded(object sender, System.EventArgs e)
    {
        updateMesh = true;
    }

    private void Grid_OnGridValueChanged(object sender, Grid<TileMap.TileMapObject>.OnGridValueChangedEventArgs e)
    {
        updateMesh = true;
    }

    private void LateUpdate()
    {
        if (updateMesh)
        {
            updateMesh = false;
            UpdateTileMapVisual();
        }
    }

    private void UpdateTileMapVisual()
    {
        MeshUtils.CreateEmptyMeshArrays(grid.GetWidth() * grid.GetHeight(), out Vector3[] vertices, out Vector2[] uv, out int[] triangles);

        for(int x = 0; x < grid.GetWidth(); x++)
        {
            for(int y = 0; y < grid.GetHeight(); y++)
            {
                int index = x * grid.GetHeight() + y;
                Vector3 quadSize = new Vector3(1, 1) * grid.GetCellSize();

                TileMap.TileMapObject gridObject = grid.GetGridObject(x, y);
                TileMap.TileMapObject.TileMapSprite tilemapSprite = gridObject.GetTileMapSprite();
                Vector2 gridValueUV00, gridValueUV11;
                if(tilemapSprite == TileMap.TileMapObject.TileMapSprite.None)
                {
                    gridValueUV00 = Vector2.zero;
                    gridValueUV11 = Vector2.zero;
                    quadSize = Vector3.zero;
                }
                else
                {
                    UVcoords uvCoords = uvCoordsDictionary[tilemapSprite];
                    gridValueUV00 = uvCoords.uv00;
                    gridValueUV11 = uvCoords.uv11;
                }
                MeshUtils.AddToMeshArrays(vertices, uv, triangles, index, grid.GetWorldPosition(x, y) + quadSize * 0.5f, 0f, quadSize, gridValueUV00, gridValueUV11);
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }

}
