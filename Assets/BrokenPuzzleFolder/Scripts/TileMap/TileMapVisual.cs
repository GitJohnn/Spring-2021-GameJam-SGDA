using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMapVisual : MonoBehaviour
{
    private Grid<TileMap.TileMapObject> grid;
    private Mesh mesh;
    private bool updateMesh;

    private void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    public void SetGrid(Grid<TileMap.TileMapObject> grid)
    {
        this.grid = grid;
        UpdateTileMapVisual();

        grid.OnGridValueChanged += Grid_OnGridValueChanged;
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
                Vector2 gridValueUV;
                if(tilemapSprite == TileMap.TileMapObject.TileMapSprite.None)
                {
                    gridValueUV = Vector2.zero;
                }
                else
                {
                    gridValueUV = Vector2.one;
                }
                MeshUtils.AddToMeshArrays(vertices, uv, triangles, index, grid.GetWorldPosition(x, y) + quadSize * 0.5f, 0f, quadSize, Vector2.zero, Vector2.zero);
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }

}
