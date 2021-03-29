using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteTileMapVisual : MonoBehaviour
{
    [SerializeField] private List<SpriteTileMap.SpriteTileMapObject> tilesetObjectArray;
    private Grid<SpriteTileMap.SpriteTileMapObject> grid;
    private bool updateMesh;
    private Dictionary<SpriteTileMap.SpriteTileMapObject.GroundTileMapSprite, Sprite> tileDictionary = new Dictionary<SpriteTileMap.SpriteTileMapObject.GroundTileMapSprite, Sprite>();

    private void Awake()
    {
        foreach (SpriteTileMap.SpriteTileMapObject tilemapSprite in tilesetObjectArray)
        {
            tileDictionary[tilemapSprite.GetTileMapSprite()] = tilemapSprite.GetSprite();
        }
    }

    public void SetGrid(SpriteTileMap tilemap, Grid<SpriteTileMap.SpriteTileMapObject> grid)
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

    private void Grid_OnGridValueChanged(object sender, Grid<SpriteTileMap.SpriteTileMapObject>.OnGridValueChangedEventArgs e)
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
        //MeshUtils.CreateEmptyMeshArrays(grid.GetWidth() * grid.GetHeight(), out Vector3[] vertices, out Vector2[] uv, out int[] triangles);
        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                //int index = x * grid.GetHeight() + y;
                
                //Vector3 quadSize = new Vector3(1, 1) * grid.GetCellSize();

                SpriteTileMap.SpriteTileMapObject gridObject = grid.GetGridObject(x, y);
                SpriteTileMap.SpriteTileMapObject.GroundTileMapSprite tilemapSprite = gridObject.GetTileMapSprite();
                gridObject.SetGroundSprite(tileDictionary[tilemapSprite]);
                if (gridObject.GetObstacleObject() != null)
                {
                    gridObject.SetObstacleSprite(tileDictionary[SpriteTileMap.SpriteTileMapObject.GroundTileMapSprite.Obstacle]);
                }                
                ////Vector2 gridValueUV00, gridValueUV11;
                //foreach (SpriteTileMap.SpriteTileMapObject tileObject in tilesetObjectArray)
                //{
                //    //set the ground sprite
                //    if (tilemapSprite == tileObject.GetTileMapSprite())
                //    {
                //        gridObject.SetGroundSprite(tileObject.GetGroundSprite());
                //        

                //        if (gridObject.GetObstacleObject() != null)
                //        {
                //            //gridObject.SetObstacleSprite()
                //        }
                //    }
                //}
            }
        }
    }

}
